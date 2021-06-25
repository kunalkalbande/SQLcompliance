using System;
using System.Collections;
using System.DirectoryServices ;
using System.Runtime.InteropServices;
using System.Text;
using ActiveDs ;

namespace Idera.SQLcompliance.Core.Agent
{
	enum DomainType
	{
		Server,
		SAM,
		ActiveDirectory,
		Unknown
	} ;


	class Account
	{
		private string m_ntDomain ;
		private string m_samName ;
		private string m_path ;
		private bool m_isGroup = false ;
      private static string builtinGroupNameString = GroupEnumerator.GetBuiltinGroupName();

		public string NtDomain
		{
			get { return m_ntDomain ; }
			set 
			{ 
				if(value != null && String.Compare( builtinGroupNameString, value, true) == 0)
					m_ntDomain = System.Environment.MachineName ;
				else
					m_ntDomain = value ; 
			}
		}

		public string SAMName
		{
			get { return m_samName ; }
			set { m_samName = value ; }
		}

		public string Path
		{
			get { return m_path ; }
			set { m_path = value ; }
		}

		public bool IsGroup
		{
			get { return m_isGroup ; }
			set { m_isGroup = value ; }
		}


		public override string ToString()
		{
			return String.Format("Full: [{0}]\r\nShort: [{1}]\r\nIsGroup: [{2}]\r\n", 
				m_path, ShortPath(), m_isGroup.ToString()) ;
		}

		public string ShortPath()
		{
			return m_ntDomain + "\\" + m_samName ;
		}

	}

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class GroupEnumerator
	{
		private Hashtable m_domainSids = new Hashtable() ;
		private Hashtable m_domainTypeCache = new Hashtable();


		public void ClearCache()
		{
			m_domainSids.Clear() ;
			m_domainTypeCache.Clear() ;
		}

		private string ExtractDomainId(byte[] sid)
		{
			if(sid == null || (sid.Length != 24 && sid.Length != 28) || !NetInterop.IsValidSid(sid))
				return null ;

			StringBuilder retVal = new StringBuilder() ;

			for(int i = 12; i < 24 ; i++)
			{
				retVal.Append(sid[i].ToString("D3")) ;
			}

			return retVal.ToString() ;
		}

		protected string GetDomainFromSid(byte[] sid)
		{
			StringBuilder name, domainName ;
			uint nameLength = 257, domainNameLength = 257 ;
			NetInterop.SID_NAME_USE sidType ;
			string domainSid = ExtractDomainId(sid) ;

			if(domainSid != null && domainSid.Length > 0 && m_domainSids.ContainsKey(domainSid))
			{
				return m_domainSids[domainSid] as string ;
			}

			name = new StringBuilder(257) ;
			domainName = new StringBuilder(257) ;
			NetInterop.LookupAccountSid(null, sid, name, ref nameLength, domainName, ref domainNameLength, out sidType) ;
			if(domainName.Length > 0)
			{
				m_domainSids[domainSid] = domainName.ToString() ;
			}

			return domainName.ToString() ;
		}

		public ArrayList ExpandGroup(Account theAccount)
		{
			return ExpandGroup(theAccount, false) ;
		}

		public ArrayList ExpandGroup(Account theAccount, bool expandSubs)
		{
			Hashtable results = new Hashtable() ;
			InternalExpandGroup(theAccount, expandSubs, new ArrayList(), results) ;

			return new ArrayList(results.Values) ;
		}

		/// <summary>
		/// This is a recursive function that takes the supplied account and enumerates all of the
		/// user and group members in the accuont.  If the expandSubs option is set to true, the functino
		/// will recurse on subgroups to finally return a list of only user that are children of the supplied
		/// group.  Duplicates are not placed in the resulting list and cycles are checked for.
		/// </summary>
		/// <param name="theAccount">The container account to expand</param>
		/// <param name="expandSubs">True to expand subgroups recursively until only users are in the resultant list</param>
		/// <param name="visitedGroups">Used to detect cycles in the group expansion</param>
		/// <param name="results">The final resultset that is returned.  This is passed along to avoid duplicate entries during recursion</param>
		private void InternalExpandGroup(Account theAccount, bool expandSubs, ArrayList visitedGroups, Hashtable results)
		{
			if(theAccount == null)
				return ;
			theAccount.Path = GetPath(theAccount) ;

         try
         {
            // Directory.Exists can throw exceptions, so we catch them and return if it fails
            if(theAccount.Path == null || theAccount.Path.Trim().Length == 0 || !DirectoryEntry.Exists(theAccount.Path))
               return ;
         }
         catch
         {
            return ;
         }

			using(DirectoryEntry group = new DirectoryEntry(theAccount.Path))
			{
				object members ;

				try
				{
					members = group.Invoke("Members", null) ;
					theAccount.IsGroup = true ;
					if(!visitedGroups.Contains(theAccount.ShortPath().ToUpper()))
						visitedGroups.Add(theAccount.ShortPath().ToUpper()) ;
				}
				catch(Exception)
				{
					// Not a group
					return; 
				}
				foreach(object member in (IEnumerable)members)
				{
					using(DirectoryEntry x = new DirectoryEntry(member))
					{
						Account myAccount = new Account() ;

						myAccount.Path = x.Path ;
						if(myAccount.Path.StartsWith(@"WinNT://"))
						{
							String[] tokens = myAccount.Path.Split(new char[] {'/'}) ;
							if(tokens.Length < 2)
							{
								// Error - Invalid path format.
								ErrorLog.Instance.Write(String.Format("Invalid WinNT path: {0}", x.Path), ErrorLog.Severity.Warning) ;
								continue ;
							}
							myAccount.SAMName = tokens[tokens.Length - 1] ;
							myAccount.NtDomain = tokens[tokens.Length - 2] ;
                     ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug,
                        String.Format( "InternalExpandGroup().\n\tAccount path = {0}\n\tSAM Name = {1}\n\tNT Domain = {2}",
                        myAccount.Path, myAccount.SAMName, myAccount.NtDomain ));
						}
						else
						{
							if(String.Compare(x.SchemaClassName, "foreignsecurityprincipal", true) == 0)
							{
								try
								{
									using(DirectorySearcher searcher = new DirectorySearcher())
									{
                              string s = (string)x.Properties["name"][0];
                              searcher.Filter = String.Format("(objectSid={0})", s);

                              SearchResult result = searcher.FindOne();

                              if (result != null)
                              {
                                 myAccount.Path = (string)result.Properties["adspath"][0];
                                 myAccount.NtDomain = GetDomainFromSid((byte[])result.Properties["objectSid"][0]);                                 
                                 myAccount.SAMName = (string)result.Properties["sAMAccountName"][0];
                              }
									}
								}
								catch(Exception)
								{
									// Error - Unable to process foreignsecurityprincipal
									ErrorLog.Instance.Write(String.Format("Unable to process foreignSecurityPrincipal member: {0}", x.Path), ErrorLog.Severity.Error) ;
									continue ;
								}
							}
							else if(String.Compare(x.SchemaClassName, "user", true) == 0)
							{
								try
								{
									myAccount.NtDomain = GetDomainFromSid((byte[])x.Properties["objectSid"][0]) ;
									myAccount.SAMName = (string)x.Properties["sAMAccountName"][0] ;
								}
								catch(Exception)
								{
									// Error - Unable to process user
									ErrorLog.Instance.Write(String.Format("Unable to process user member: {0}", x.Path), ErrorLog.Severity.Error) ;
									continue ;
								}
							}
							else if(String.Compare(x.SchemaClassName, "group", true) == 0)
							{
								try
								{
									myAccount.NtDomain = GetDomainFromSid((byte[])x.Properties["objectSid"][0]) ;
									myAccount.SAMName = (string)x.Properties["sAMAccountName"][0] ;
								}
								catch(Exception)
								{
									// Error - Unable to process group
									ErrorLog.Instance.Write(String.Format("Unable to process group member: {0}", x.Path), ErrorLog.Severity.Error) ;
									continue ;
								}
							}
							else if(String.Compare(x.SchemaClassName, "msDS-GroupManagedServiceAccount", true) == 0)
							{
								try
								{
									myAccount.NtDomain = GetDomainFromSid((byte[])x.Properties["objectSid"][0]) ;
									myAccount.SAMName = (string)x.Properties["sAMAccountName"][0] ;
								}
								catch(Exception)
								{
									// Error - Unable to process GMSA
									ErrorLog.Instance.Write(String.Format("Unable to process GMSA group member: {0}", x.Path), ErrorLog.Severity.Error) ;
									continue ;
								}
							}
						}
						if(String.Compare(x.SchemaClassName, "group", true) == 0)
							myAccount.IsGroup = true ;
                  // 
                  // This is special code for NT AUTHORITY\SYSTEM.  This account
                  //  shows up as a group, however, we need to treat it as a user.
                  //
                  if((String.Compare("NT AUTHORITY", myAccount.NtDomain, true) == 0) &&
                     (String.Compare("SYSTEM", myAccount.SAMName, true) == 0))
                  {
                     myAccount.IsGroup = false ;
                  }
                  if(myAccount.IsGroup && expandSubs)
						{
							if(!visitedGroups.Contains(myAccount.ShortPath().ToUpper()))
							{
								visitedGroups.Add(myAccount.ShortPath().ToUpper()) ;
								InternalExpandGroup(myAccount, true, visitedGroups, results) ;
							}
						}
						else
						{
							if(!results.Contains(myAccount.ShortPath().ToUpper()))
								results.Add(myAccount.ShortPath().ToUpper(), myAccount) ;
						}
					}
				}
			}
		}


		/// <summary>
		/// Returns an AD path for the specified account object.  The domain and account name
		/// are used to determine the type of path to construct
		/// </summary>
		/// <param name="myAccount"></param>
		/// <returns></returns>
		private string GetPath(Account myAccount)
		{
			DomainType domType = GetDomainType(myAccount.NtDomain) ;
			switch(domType)
			{
				case DomainType.ActiveDirectory:
					return GetADPath(myAccount.NtDomain, myAccount.SAMName) ;
				case DomainType.SAM:
					return GetSAMPath(myAccount.NtDomain, myAccount.SAMName) ;
				case DomainType.Server:
					return GetSAMPath(myAccount.NtDomain, myAccount.SAMName) ;
				default:
					return null ;
			}
		}

		/// <summary>
		/// Get the SAM-style path for the specified domain and account
		/// </summary>
		/// <param name="domainIn">short form windows domain name</param>
		/// <param name="accountIn">SAM account name</param>
		/// <returns></returns>
		private string GetSAMPath(string domainIn, string accountIn)
		{
			StringBuilder retVal = new StringBuilder("WinNT://") ;
			retVal.Append(domainIn) ;
			retVal.Append("/") ;
			retVal.Append(accountIn) ;
			return retVal.ToString() ;
		}

		/// <summary>
		/// This function returns the ADSpath for the specified domain and account.  This is
		/// done by translating the NT4 style address into 1779 format.
		/// </summary>
		/// <param name="domainIn">short form windows domain name</param>
		/// <param name="accountIn">SAM account name</param>
		/// <returns></returns>
		private string GetADPath(string domainIn, string accountIn)
		{
			try
			{
				NameTranslateClass translator = new NameTranslateClass() ;

				translator.Init((int)ADS_NAME_INITTYPE_ENUM.ADS_NAME_INITTYPE_DOMAIN, domainIn) ;
				translator.Set((int)ADS_NAME_TYPE_ENUM.ADS_NAME_TYPE_NT4, String.Format("{0}\\{1}", domainIn, accountIn)) ;
				string retVal = translator.Get((int)ADS_NAME_TYPE_ENUM.ADS_NAME_TYPE_1779) ;

				return String.Format("LDAP://{0}", retVal) ;
			}
			catch(Exception)
			{
			}
			return null ;
		}

		/// <summary>
		/// Get the type of domain.  This function first checks in the cache for the domain type.
		/// If the domain is not in the cache, the type is deduced and added to the cache unless
		/// the type is Unknown.
		/// </summary>
		/// <param name="domainIn">short form windows domain name</param>
		/// <returns></returns>
		private DomainType GetDomainType(string domainIn)
		{
			if(m_domainTypeCache.Contains(domainIn))
				return (DomainType)m_domainTypeCache[domainIn] ;

			string computerName = System.Environment.MachineName ;

			if(String.Compare(computerName, domainIn, true) == 0)
			{
				m_domainTypeCache.Add(domainIn, DomainType.Server) ;
				return DomainType.Server ; 
			}
			else
			{
				IntPtr pDcInfo = IntPtr.Zero ;

				try
				{
					NetInterop.DOMAIN_CONTROLLER_INFO dcInfo ;

					if(NetInterop.DsGetDcName(String.Empty, domainIn, null, String.Empty, 
						NetInterop.DS_IS_FLAT_NAME | NetInterop.DS_RETURN_FLAT_NAME, out pDcInfo) == 0)
					{
						dcInfo = (NetInterop.DOMAIN_CONTROLLER_INFO)Marshal.PtrToStructure(pDcInfo, typeof(NetInterop.DOMAIN_CONTROLLER_INFO)) ;
						bool isDomainDNS = ((dcInfo.Flags & NetInterop.DS_DNS_DOMAIN_FLAG) == NetInterop.DS_DNS_DOMAIN_FLAG) && 
							dcInfo.DomainName != null && dcInfo.DomainName.Length > 0 ;
						bool isForestDNS = ((dcInfo.Flags & NetInterop.DS_DNS_FOREST_FLAG) == NetInterop.DS_DNS_FOREST_FLAG) &&
							dcInfo.DnsForestName != null && dcInfo.DnsForestName.Length > 0 ;
						if(isDomainDNS || isForestDNS)
						{
							m_domainTypeCache.Add(domainIn, DomainType.ActiveDirectory) ;
							return DomainType.ActiveDirectory ;
						}
						else
						{
							m_domainTypeCache.Add(domainIn, DomainType.SAM) ;
							return DomainType.SAM ;
						}
					}
				}
				finally
				{
					NetInterop.NetApiBufferFree(pDcInfo) ;
				}
			}
			return DomainType.Unknown ;
		}

      internal static string GetBuiltinGroupName()
      {
         string builtinString = "builtin";
         IntPtr pSid = IntPtr.Zero;

         try
         {
            byte [] sidAuthority = new byte[6];

            // Byte array for SECURITY_NT_AUTHORITY {0,0,0,0,0,5}
            sidAuthority[0] = 0;
            sidAuthority[1] = 0;
            sidAuthority[2] = 0;
            sidAuthority[3] = 0;
            sidAuthority[4] = 0;
            sidAuthority[5] = 5; 

            // Create an SID for the BUILTIN\Administrators group.
            if( SecurityInterop.AllocateAndInitializeSid(  sidAuthority, 
                                                      2,
                                                      SecurityInterop.SECURITY_BUILTIN_DOMAIN_RID,
                                                      SecurityInterop.DOMAIN_ALIAS_RID_ADMINS,
                                                      0, 
                                                      0, 
                                                      0,
                                                      0, 
                                                      0, 
                                                      0,
                                                      ref pSid ) == 0 ) 
            {
                  string msg = String.Format( "Error creating SID for well known group 'BUILTIN\\ADMINISTRATORS'. Error code = {0}.", NativeMethods.GetLastError() );
                  ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
                  return builtinString;
            }
         }
         catch( Exception e)
         {
               ErrorLog.Instance.Write( e, true );
               throw e;
         }

			StringBuilder name, domainName ;
			uint nameLength = 257, domainNameLength = 257 ;
			int sidType ;

			name = new StringBuilder(257) ;
			domainName = new StringBuilder(257) ;
         try
         {
			   if( SecurityInterop.LookupAccountSid( null, 
                                       pSid, 
                                       name, 
                                       ref nameLength, 
                                       domainName, 
                                       ref domainNameLength, 
                                       out sidType ) &&
			        domainName.Length > 0 )
			   {
				   builtinString = domainName.ToString() ;
			   }
            else
            {
               string msg = String.Format( "Error looking up name for well known group 'BUILTIN\\ADMINISTRATORS'. Error code = {0}.", NativeMethods.GetLastError() );
               ErrorLog.Instance.Write( msg, ErrorLog.Severity.Error );
            }
         }
         catch( Exception e )
         {
               ErrorLog.Instance.Write( "Error looking up name for well known group 'BUILTIN\\ADMINISTRATORS'. ",
                                        e, 
                                        ErrorLog.Severity.Warning,
                                        true );
               throw e;
         }
         finally
         {
            if( pSid !=IntPtr.Zero )
               SecurityInterop.FreeSid( pSid );
         }

         ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug,
                                  String.Format( "GroupEnumerator: BUILTIN group name string = {0}", builtinString ) );


         return builtinString;
      }
	}

	class NetInterop
	{
      internal enum WELL_KNOWN_SID_TYPE
      {
         WinNullSid = 0, 
         WinWorldSid = 1, 
         WinLocalSid = 2, 
         WinCreatorOwnerSid = 3, 
         WinCreatorGroupSid = 4, 
         WinCreatorOwnerServerSid = 5, 
         WinCreatorGroupServerSid = 6, 
         WinNtAuthoritySid = 7, 
         WinDialupSid = 8, 
         WinNetworkSid = 9, 
         WinBatchSid = 10, 
         WinInteractiveSid = 11, 
         WinServiceSid = 12, 
         WinAnonymousSid = 13, 
         WinProxySid = 14, 
         WinEnterpriseControllersSid = 15, 
         WinSelfSid = 16, 
         WinAuthenticatedUserSid = 17, 
         WinRestrictedCodeSid = 18, 
         WinTerminalServerSid = 19, 
         WinRemoteLogonIdSid = 20, 
         WinLogonIdsSid = 21, 
         WinLocalSystemSid = 22, 
         WinLocalServiceSid = 23, 
         WinNetworkServiceSid = 24, 
         WinBuiltinDomainSid = 25, 
         WinBuiltinAdministratorsSid = 26, 
         WinBuiltinUsersSid = 27, 
         WinBuiltinGuestsSid = 28, 
         WinBuiltinPowerUsersSid = 29, 
         WinBuiltinAccountOperatorsSid = 30, 
         WinBuiltinSystemOperatorsSid = 31, 
         WinBuiltinPrintOperatorsSid = 32, 
         WinBuiltinBackupOperatorsSid = 33, 
         WinBuiltinReplicatorSid = 34, 
         WinBuiltinPreWindows2000CompatibleAccessSid = 35, 
         WinBuiltinRemoteDesktopUsersSid = 36, 
         WinBuiltinNetworkConfigurationOperatorsSid = 37, 
         WinAccountAdministratorSid = 38, 
         WinAccountGuestSid = 39, 
         WinAccountKrbtgtSid = 40, 
         WinAccountDomainAdminsSid = 41, 
         WinAccountDomainUsersSid = 42, 
         WinAccountDomainGuestsSid = 43, 
         WinAccountComputersSid = 44, 
         WinAccountControllersSid = 45, 
         WinAccountCertAdminsSid = 46, 
         WinAccountSchemaAdminsSid = 47, 
         WinAccountEnterpriseAdminsSid = 48, 
         WinAccountPolicyAdminsSid = 49, 
         WinAccountRasAndIasServersSid = 50, 
         WinNTLMAuthenticationSid = 51, 
         WinDigestAuthenticationSid = 52, 
         WinSChannelAuthenticationSid = 53, 
         WinThisOrganizationSid = 54, 
         WinOtherOrganizationSid = 55, 
         WinBuiltinIncomingForestTrustBuildersSid = 56, 
         WinBuiltinPerfMonitoringUsersSid = 57, 
         WinBuiltinPerfLoggingUsersSid = 58, 
         WinBuiltinAuthorizationAccessSid = 59, 
         WinBuiltinTerminalServerLicenseServersSid = 60
      } ;

		public const uint DS_IS_FLAT_NAME = 0x00010000 ;
		public const uint DS_RETURN_FLAT_NAME = 0x80000000 ;
		public const uint DS_DNS_DOMAIN_FLAG = 0x40000000 ;
		public const uint DS_DNS_FOREST_FLAG = 0x80000000 ;
      internal const uint SECURITY_MAX_SID_SIZE = 100;
      internal const uint MAX_WIN_NAME_LENGTH = 40;


		[DllImport("Netapi32.dll", SetLastError=true)]
		public static extern int NetApiBufferFree(IntPtr Buffer);
		
		[DllImport("Netapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern uint DsGetDcName(
			[MarshalAs(UnmanagedType.LPTStr)]
			string ComputerName,
			[MarshalAs(UnmanagedType.LPTStr)]
			string DomainName,
			[In] GuidClass DomainGuid,
			[MarshalAs(UnmanagedType.LPTStr)]
			string SiteName,
			uint Flags,
			out IntPtr pDOMAIN_CONTROLLER_INFO
			);

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Unicode)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Unicode)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
#endif
			public struct DOMAIN_CONTROLLER_INFO
		{
			[MarshalAs(UnmanagedType.LPTStr)]
			public string    DomainControllerName;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string    DomainControllerAddress;
			public uint    DomainControllerAddressType;
			public Guid    DomainGuid;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string    DomainName;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string    DnsForestName;
			public uint        Flags;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string    DcSiteName;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string    ClientSiteName;
		}

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8)]
#else
      [StructLayout(LayoutKind.Sequential)]
#endif
      public class GuidClass
		{
			public Guid TheGuid = Guid.Empty ;
		}

		public enum SID_NAME_USE 
		{
			SidTypeUser = 1,
			SidTypeGroup,
			SidTypeDomain,
			SidTypeAlias,
			SidTypeWellKnownGroup,
			SidTypeDeletedAccount,
			SidTypeInvalid,
			SidTypeUnknown,
			SidTypeComputer
		} ;


		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError = true)]
		public static extern bool LookupAccountSid(
			string lpSystemName,
			[MarshalAs(UnmanagedType.LPArray)] byte[] Sid,
			System.Text.StringBuilder lpName,
			ref uint cchName,
			System.Text.StringBuilder ReferencedDomainName,
			ref uint cchReferencedDomainName,
			out SID_NAME_USE peUse);       

		[DllImport("advapi32", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern bool IsValidSid([MarshalAs(UnmanagedType.LPArray)] byte [] pSID) ;

      [DllImport("Advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern bool CreateWellKnownSid(
         WELL_KNOWN_SID_TYPE      wellKnownSidType,
         [MarshalAs(UnmanagedType.LPArray)] byte [] domainSid,
         [MarshalAs(UnmanagedType.LPArray)] byte [] sid,
         ref uint                 nSid );

	}

   internal class SecurityInterop
   {
      internal const int    SECURITY_BUILTIN_DOMAIN_RID =    0x00000020;
      internal const int    DOMAIN_ALIAS_RID_ADMINS     =   0x00000220;

      [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError = true)]
		internal static extern int AllocateAndInitializeSid(
			      [In,MarshalAs(UnmanagedType.LPArray)]byte [] sidSecurityIdentifier,
			      byte subAuthorityCount,
			      int sub0,
			      int sub1,
			      int sub2,
			      int sub3,
			      int sub4,
			      int sub5,
			      int sub6,
			      int sub7,
			      ref IntPtr Sid );       

      [DllImport("advapi32", CharSet=CharSet.Auto, SetLastError=true)]
		internal static extern void FreeSid(IntPtr pSID) ;


		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError = true)]
		internal static extern bool LookupAccountSid(
			string lpSystemName,
			IntPtr Sid,
			System.Text.StringBuilder lpName,
			ref uint cchName,
			System.Text.StringBuilder ReferencedDomainName,
			ref uint cchReferencedDomainName,
			out int peUse);  
     
   }





}
