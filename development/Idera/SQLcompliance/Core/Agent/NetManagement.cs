using System;
using System.Data.SqlClient;
using System.Collections;
using System.Runtime.InteropServices;	// for DllImport, MarshalAs, etc

namespace Idera.SQLcompliance.Core.Agent
{

   public class UserGroup
   {
      string name;
      ArrayList members = new ArrayList();

      public UserGroup ( string groupName )
      {
         if( groupName != null )
         {
            name = groupName;
         }
      }

      public string Name
      {
         get { return name; }
         set { name = value; }
      }

      public string [] Members
      {
         get{ return (string []) members.ToArray( typeof(string) ); }
         set
         {
            if( value != null )
            {
               for( int i = 0; i < value.Length; i++ )
               {
                  try
                  {
                     members.Add( value[i] );
                  }
                  catch{}
               }
            }
         }
      }
   }
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
   internal class NetManagement
   {
      #region constants

      // Network Management API return code.
      public const int   ERROR_SUCCESS                 = 0;
      public const int   ERROR_INVALID_LEVEL           = 124;
      public const int   ERROR_MORE_DATA               = 234;
      public const int   ERROR_ACCESS_DENIED           = 994;
      public const int   ERROR_SERVER_UNAVAILABLE      = 1722;
      public const int   ERROR_INVALID_COMPUTER        = 2221;
      public const int   ERROR_GROUP_DOES_NOT_EXISTS   = 2220;
      public const int   ERROR_LOCAL_GROUP_DOES_NOT_EXIST = 1376;
      public const int   ERROR_DC_NOT_FOUND            = 2453;

      // DsGetDcName constants
      public const uint DS_FORCE_REDISCOVERY           = 0x00000001;
      public const uint DS_DIRECTORY_SERVICE_REQUIRED  = 0x00000010;
      public const uint DS_DIRECTORY_SERVICE_PREFERRED = 0x00000020;
      public const uint DS_GC_SERVER_REQUIRED          = 0x00000040;
      public const uint DS_PDC_REQUIRED                = 0x00000080;
      public const uint DS_BACKGROUND_ONLY             = 0x00000100;
      public const uint DS_IP_REQUIRED                 = 0x00000200;
      public const uint DS_KDC_REQUIRED                = 0x00000400;
      public const uint DS_TIMESERV_REQUIRED           = 0x00000800;
      public const uint DS_WRITABLE_REQUIRED           = 0x00001000;
      public const uint DS_GOOD_TIMESERV_PREFERRED     = 0x00002000;
      public const uint DS_AVOID_SELF                  = 0x00004000;
      public const uint DS_ONLY_LDAP_NEEDED            = 0x00008000;
      public const uint DS_IS_FLAT_NAME                = 0x00010000;
      public const uint DS_IS_DNS_NAME                 = 0x00020000;
      public const uint DS_RETURN_DNS_NAME             = 0x40000000;
      public const uint DS_RETURN_FLAT_NAME            = 0x80000000;

      public enum NETSETUP_JOIN_STATUS 
      {
         NetSetupUnknownStatus = 0,
         NetSetupUnjoined,
         NetSetupWorkgroupName,
         NetSetupDomainName
      }

      // TODO: move it to CoreConstants
      public const string EXCEPTION_NetworkManagementAPIError = "An error occurred calling Network Management API.  Error code = {0}.";
      #endregion

      #region Constructor

      private NetManagement(){}

      #endregion

      #region Member Fields
      public static uint    LastError = 0;
      protected static bool initialized = false;

      protected static int LOCALGROUP_MEMBERS_INFO_1_SIZE;
      protected static int LOCALGROUP_MEMBERS_INFO_2_SIZE;
      protected static int LOCALGROUP_INFO_1_SIZE;
      protected static int NET_DISPLAY_GROUP_SIZE;
      protected static int NET_DISPLAY_USER_SIZE;
      protected static int GROUP_USERS_INFO_0_SIZE;
      #endregion

      #region Static Wrapper functions

      //-----------------------------------------------------------------------
      // GetDomainController - get the address of the domain controller
      //-----------------------------------------------------------------------
      internal static string
         GetDomainController(
            string inDomainName 
         )
      {
         uint rc = 0;
         string domainName = null;
         IntPtr outputBuffer = IntPtr.Zero;
         string domainControllerName;

         if( !initialized )
            Init();

         LastError = 0;

         try
         {
            if( inDomainName != null && inDomainName.Length > 0 )
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                       "Getting domain controller for " + inDomainName );
               domainName = inDomainName;
            }

            rc = NetGetDCName(   IntPtr.Zero,
                                 domainName == null ? IntPtr.Zero :Marshal.StringToCoTaskMemAuto( domainName ),
                                 ref outputBuffer );
            SetLastError( rc );

            domainControllerName = Marshal.PtrToStringAuto( outputBuffer );
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "Get domain controller : rc = " + rc + "\n Domain controller = " + domainControllerName);

         }
         catch(Exception e)
         { 
            throw e; 
         }
         finally
         {
            NetApiBufferFree(outputBuffer);
         }

         return domainControllerName;
      }

      //-----------------------------------------------------------------------
      // GetDomainName - returns domain name or workgroup name.
      //-----------------------------------------------------------------------
      internal static string
         GetDomainName(
            out bool isDomain
         )
      {
         return GetDomainName(
            out isDomain,
            null );
      }

      //-----------------------------------------------------------------------
      // GetDomainName - returns domain name or workgroup name.
      //-----------------------------------------------------------------------
      internal static string
         GetDomainName(
            out bool isDomain,
            string   server
         )
      {
         string domainName = null;
         IntPtr outputBuffer = IntPtr.Zero;

         try
         {
            uint   rc = 0;
            uint   type = 0;

            isDomain = false;

            rc = NetGetJoinInformation( server == null ? IntPtr.Zero : Marshal.StringToCoTaskMemAuto( server ),  
                                        ref outputBuffer,
                                        ref type );
            SetLastError( rc );

            domainName = Marshal.PtrToStringAuto( outputBuffer );
            if( type == (uint)NETSETUP_JOIN_STATUS.NetSetupDomainName )
               isDomain = true;
         }
         finally
         {
            NetApiBufferFree(outputBuffer);
         }

         return domainName;
      }

      //-----------------------------------------------------------------------
      // ADGetDomainController - use directory service to get domain controller.
      //-----------------------------------------------------------------------
      internal static string
         GetFlatDomainName(
            string dnsDomainName
         )
      {
         string flatName = null;
         ADGetDomainController( dnsDomainName,
                                true,
                                out flatName );
         return flatName;
      }

      //-----------------------------------------------------------------------
      // ADGetDomainController - use directory service to get domain controller.
      //-----------------------------------------------------------------------
      internal static string
         ADGetDomainController(
            string inDomainName,
            bool       isDnsName,
            out string flatDomainName
         )
      {
         uint rc = 0;
         uint flags = DS_RETURN_FLAT_NAME;
         IntPtr outputBuffer = IntPtr.Zero;
         string domainControllerName = null;

         if( !initialized )
            Init();
         LastError = 0;
         flatDomainName = "";
         if( isDnsName )
            flags |= DS_IS_DNS_NAME;
         else
            flags |= DS_IS_FLAT_NAME;

         try
         {
            rc = DsGetDcName( IntPtr.Zero,
                              Marshal.StringToCoTaskMemAuto( inDomainName ),
                              IntPtr.Zero,
                              IntPtr.Zero,
                              flags,
                              ref outputBuffer );

            SetLastError( rc );

            DOMAIN_CONTROLLER_INFO dcInfo = (DOMAIN_CONTROLLER_INFO)Marshal.PtrToStructure( outputBuffer, 
                                                                                            typeof(DOMAIN_CONTROLLER_INFO));
            domainControllerName = Marshal.PtrToStringAuto(dcInfo.DCName);
            domainControllerName = domainControllerName.Substring(2);
            bool isDomain = true;
            flatDomainName = GetDomainName( out isDomain, domainControllerName );
         }
         catch(Exception e)
         { 
            throw e; 
         }
         finally
         {
            NetApiBufferFree(outputBuffer);
         }

         return domainControllerName;
      }

      //-----------------------------------------------------------------------
      // GetLocalGroups - get local groups on this machine.
      //-----------------------------------------------------------------------
      internal static string []
         GetLocalGroups()
      {
         return GetLocalGroups( null );
      }


      //-----------------------------------------------------------------------
      // GetLocalGroups - get local groups on this machine.
      //-----------------------------------------------------------------------
      internal static string []
         GetLocalGroups(
            string server
         )
      {
         uint level = 1; // group info
         uint preferredMaximumLength = 0xFFFFFFFF;
         uint returnedEntryCount = 0;
         uint totalEntries = 0;
         uint rc = 0;

         IntPtr   GroupInfoPtr = IntPtr.Zero;
         IntPtr   resumeHandle = IntPtr.Zero;

         int       newOffset;
         string    groupName;
         ArrayList groupList = new ArrayList();

         if( !initialized )
            Init();
         LastError = 0;

         try
         {
            do
            {
               rc = NetLocalGroupEnum( server == null ? IntPtr.Zero : Marshal.StringToCoTaskMemAuto(server),
                                       level,
                                       ref GroupInfoPtr,
                                       preferredMaximumLength,
                                       ref returnedEntryCount,
                                       ref totalEntries,
                                       ref resumeHandle);

               SetLastError( rc );

               for(int i = 0; i < totalEntries; i++)
               {
                  newOffset = GroupInfoPtr.ToInt32() + LOCALGROUP_INFO_1_SIZE * i;
                  LOCALGROUP_INFO_1 groupInfo = (LOCALGROUP_INFO_1)Marshal.PtrToStructure(new IntPtr(newOffset), typeof(LOCALGROUP_INFO_1));
                  groupName = Marshal.PtrToStringAuto(groupInfo.lpszGroupName);
                  groupList.Add( groupName );
               }
            } while( rc == ERROR_MORE_DATA );
         }
         catch(Exception e)
         { 
            throw e; 
         }
         finally
         {
            NetApiBufferFree(GroupInfoPtr);
         }

         return (string [])groupList.ToArray( typeof(string) );
      }

      //-----------------------------------------------------------------------
      // GetLocalGroupUsers - get members of a local group.
      //-----------------------------------------------------------------------
      internal static UserInfo []
         GetLocalGroupUsers(
            string server,
            string groupName
         )
      {
         uint preferredMaximumLength = 0xFFFFFFFF;
         uint returnedEntryCount = 0;
         uint totalEntries = 0;
         uint rc = 0;
         uint type;
         UserInfo user;

         IntPtr   UserInfoPtr = IntPtr.Zero;
         IntPtr   resumeHandle = IntPtr.Zero;

         int       newOffset;
         string    userName;
         ArrayList userList = new ArrayList();

         if( !initialized )
            Init();
         LastError = 0;

         try
         {
            do
            {
               rc = NetLocalGroupGetMembers( server == null ? IntPtr.Zero : Marshal.StringToCoTaskMemAuto(server),
                                             Marshal.StringToCoTaskMemAuto(groupName),
                                             2,  // user info
                                             ref UserInfoPtr,
                                             preferredMaximumLength,
                                             ref returnedEntryCount,
                                             ref totalEntries,
                                             ref resumeHandle );

               SetLastError( rc );

               for(int i = 0; i < totalEntries; i++)
               {
                  newOffset = UserInfoPtr.ToInt32() + LOCALGROUP_MEMBERS_INFO_2_SIZE * i;
                  LOCALGROUP_MEMBERS_INFO_2 memberInfo = (LOCALGROUP_MEMBERS_INFO_2)Marshal.PtrToStructure(new IntPtr(newOffset), typeof(LOCALGROUP_MEMBERS_INFO_2));
                  userName = Marshal.PtrToStringAuto(memberInfo.lgrmi2_domainname);
                  type = memberInfo.lgrmi2_sidusage;
                  user = new UserInfo();
                  user.Name = userName;
                  switch( type )
                  {
                     case 1: //user
                        user.type = UserType.User;
                        break;
                     case 2: // group
                        user.type = UserType.GlobalGroup;
                        break;
                     case 31: // domain
                        user.type = UserType.Domain;
                        break;
                     case 4: // alias
                        user.type = UserType.Alias;
                        break;
                     case 5: // well known group
                        user.type = UserType.WellKnownGroup;
                        break;
                     case 6: // deleted account
                        user.type = UserType.Deleted;
                        break;
                     case 7: // invalid
                        user.type = UserType.Invalid;
                        break;
                     case 8: // unknown
                        user.type = UserType.Unknown;
                        break;
                     case 9: // computer
                        user.type = UserType.Computer;
                        break;
                     default:
                        user.type = UserType.Unknown;
                        break;
                  }
                  userList.Add( user );
               }
            } while( rc == ERROR_MORE_DATA );
         }
         catch(Exception e)
         { 
            throw e; 
         }
         finally
         {
            NetApiBufferFree(UserInfoPtr);
         }
         return (UserInfo [])userList.ToArray( typeof(UserInfo) );

      }

      //-----------------------------------------------------------------------
      // GetLocalGroupUsers - get members of a local group.
      //-----------------------------------------------------------------------
      internal static UserInfo []
         GetLocalGroupUsers(
            string groupName
         )
      {
         return GetLocalGroupUsers( null, groupName );
      }

      //-----------------------------------------------------------------------
      // GetGroups - get group names on a server.
      //-----------------------------------------------------------------------
      internal static string []
         GetGroups(
            string serverName
         )
      {
         //IntPtr returnBuffer;
         IntPtr groupInfoPtr = IntPtr.Zero;

         uint preferredMaximumLength = 0xFFFFFFFF;
         uint returnedEntryCount = 0;
         uint rc = 0;
         uint index = 0;

         int       newOffset;
         string    groupName;
         ArrayList groupList = new ArrayList();

         if( !initialized )
            Init();
         LastError = 0;

         try
         {
            serverName = FormatServerName( serverName );
            do
            {
               rc = NetQueryDisplayInformation( Marshal.StringToCoTaskMemAuto(serverName),
                                                3,
                                                index,
                                                1000,
                                                preferredMaximumLength,
                                                ref returnedEntryCount,
                                                ref groupInfoPtr );

               SetLastError( rc );

               if( rc == ERROR_SUCCESS || rc == ERROR_MORE_DATA )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                           String.Format( "{0} domain groups retrieved.", returnedEntryCount ) );
                  for( int i = 0; i < returnedEntryCount; i++ )
                  {
                     newOffset = groupInfoPtr.ToInt32() + NET_DISPLAY_GROUP_SIZE * i;
                     NET_DISPLAY_GROUP groupInfo = (NET_DISPLAY_GROUP)Marshal.PtrToStructure(new IntPtr(newOffset), typeof(NET_DISPLAY_GROUP));
                     groupName = Marshal.PtrToStringAuto( groupInfo.grpi3_name );
                     groupList.Add( groupName );
                  }
               }
               index += returnedEntryCount;

            } while( rc == ERROR_MORE_DATA );
         }
         catch(Exception e)
         { 
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     e,
                                     true );
         }
         finally
         {
            NetApiBufferFree( groupInfoPtr );
         }

         return (string [])groupList.ToArray( typeof(string) );
      }

      //-----------------------------------------------------------------------
      // GetGroupUsers - get members of a group
      //-----------------------------------------------------------------------
      internal static string []
         GetGroupUsers(
            string serverName,
            string groupName
         )
      {
         uint preferredMaximumLength = 0xFFFFFFFF;
         uint returnedEntryCount = 0;
         uint totalEntries = 0;
         uint rc = 0;

         IntPtr   UserInfoPtr = IntPtr.Zero;
         IntPtr   resumeHandle = IntPtr.Zero;

         int       newOffset;
         ArrayList userList = new ArrayList();
         string    userName;

         if( !initialized )
            Init();
         LastError = 0;

         try
         {
            serverName = FormatServerName( serverName );
            do
            {
               rc = NetGroupGetUsers(  Marshal.StringToCoTaskMemAuto(serverName), 
                                       Marshal.StringToCoTaskMemAuto(groupName),
                                       0,
                                       ref UserInfoPtr,
                                       preferredMaximumLength,
                                       ref returnedEntryCount,
                                       ref totalEntries,
                                       ref resumeHandle );

               SetLastError( rc );
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        String.Format( "{0} domain users retrieved for group {1}",
                                                       returnedEntryCount,
                                                       groupName ));

               for(int i = 0; i < returnedEntryCount; i++)
               {
                  newOffset = UserInfoPtr.ToInt32() + GROUP_USERS_INFO_0_SIZE * i;
                  GROUP_USERS_INFO_0 userInfo = (GROUP_USERS_INFO_0)Marshal.PtrToStructure(new IntPtr(newOffset), typeof(GROUP_USERS_INFO_0));
                  userName = Marshal.PtrToStringAuto(userInfo.grui0_name);
                  userList.Add( userName );
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                           String.Format( "Domain user added: {0}",
                                           userName ));
               }
            } while( rc == ERROR_MORE_DATA );
         }
         catch(Exception e)
         { 
               throw e; 
         }
         finally
         {
            NetApiBufferFree(UserInfoPtr);
         }

         return (string [])userList.ToArray( typeof(string) );

      }

      internal static bool
         IsUser(
            string serverName,
            string userName
         )
      {
         uint rc = 0;
         uint level = 0;
         IntPtr outputBuffer = IntPtr.Zero;
         bool isUser = false;

         try
         {
            rc = NetUserGetInfo( serverName == null ? IntPtr.Zero : Marshal.StringToCoTaskMemAuto(serverName), 
                                 Marshal.StringToCoTaskMemAuto(userName),
                                 level,
                                 ref outputBuffer );
            if( rc == ERROR_SUCCESS )
               isUser = true;
         }
         finally
         {
            NetApiBufferFree(outputBuffer);
         }
         return isUser;
      }

      #endregion

      #region Network Management Interfaces

      [DllImport( "netapi32.dll", EntryPoint = "NetApiBufferFree" )]
      private static extern void NetApiBufferFree(IntPtr bufptr);

      #region APIs for Machine Info
      [DllImport( "netapi32.dll", EntryPoint = "NetGetJoinInformation" )]
      private static extern uint NetGetJoinInformation(
         IntPtr lpServer,
         ref IntPtr GroupName,
         ref uint BufferType );


      #endregion

      #region APIs for Local Group

      [DllImport( "netapi32.dll", EntryPoint = "NetLocalGroupGetMembers" )]
      private static extern uint NetLocalGroupGetMembers(
         IntPtr serverName,
         IntPtr grouprName,
         uint level,
         ref IntPtr siPtr,
         uint prefmaxlen,
         ref uint entriesread,
         ref uint totalentries,
         ref IntPtr resumeHandle);

      [DllImport( "netapi32.dll", EntryPoint = "NetLocalGroupEnum" )]
      private static extern uint NetLocalGroupEnum(
         IntPtr ServerName, 
         uint level,
         ref IntPtr siPtr,
         uint prefmaxlen,
         ref uint entriesread,
         ref uint totalentries,
         ref IntPtr resumeHandle);

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct LOCALGROUP_MEMBERS_INFO_1
      { 
         public IntPtr lgrmi1_sid;
         public uint lgrmi1_sidusage;
         public IntPtr lgrmi1_name;

      }

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct LOCALGROUP_MEMBERS_INFO_2
      { 
         public IntPtr lgrmi2_sid;
         public uint lgrmi2_sidusage;
         public IntPtr lgrmi2_domainname;

      }

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct LOCALGROUP_INFO_1 
      { 
         public IntPtr lpszGroupName;
         public IntPtr lpszComment;
      }

      #endregion

      #region APIs for Domain

      [DllImport( "netapi32.dll", EntryPoint = "NetQueryDisplayInformation", CharSet=CharSet.Auto )]
      private static extern uint NetQueryDisplayInformation(
         IntPtr serverName, 
         uint level,
         uint index,
         uint entriesRequested,
         uint preferredMaximumLength,
         ref uint returnedEntryCount,
         ref IntPtr returnBuffer);

      [DllImport( "netapi32.dll", EntryPoint = "NetGroupGetUsers", CharSet=CharSet.Auto )]
      private static extern uint NetGroupGetUsers(
         IntPtr serverName, 
         IntPtr groupname,
         uint level,
         ref IntPtr bufptr,
         uint preferredMaximumLength,
         ref uint returnedEntryCount,
         ref uint totalentries,
         ref IntPtr resumeHandle);

      [DllImport( "netapi32.dll", EntryPoint = "NetUserGetInfo", CharSet=CharSet.Auto )]
      private static extern uint NetUserGetInfo(
         IntPtr servername,
         IntPtr username,
         uint level,
         ref IntPtr bufptr );



#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct NET_DISPLAY_GROUP 
      { 
         public IntPtr grpi3_name;
         public IntPtr grpi3_comment;
         public uint   grpi3_group_id;
         public uint   grpi3_attributes;
         public uint   grpi3_next_index;
      }

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct NET_DISPLAY_USER 
      { 
         public IntPtr usri1_name;
         public IntPtr usri1_comment;
         public uint   usri1_flags;
         public IntPtr usri1_full_name;
         public uint   usri1_user_id;
         public uint   usri1_next_index;
      }

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct GROUP_USERS_INFO_0 
      { 
         public IntPtr grui0_name;
      }

      [DllImport( "netapi32.dll", EntryPoint = "NetGetDCName", CharSet=CharSet.Auto )]
      private static extern uint NetGetDCName(
                                                IntPtr serverName, 
                                                IntPtr domainName,
                                                ref IntPtr outputBuffer );

      // Directory service API
      [DllImport( "netapi32.dll", EntryPoint = "DsGetDcName", CharSet=CharSet.Auto )]
      private static extern uint DsGetDcName(
                                                IntPtr serverName, 
                                                IntPtr domainName,
                                                IntPtr guid,
                                                IntPtr siteName,
                                                uint  flags,
                                                ref IntPtr dcInfo );

#if x86
      [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Auto)]
#elif x64
      [StructLayout(LayoutKind.Sequential, Pack=8, CharSet = CharSet.Auto)]
#else
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
#endif
      private struct DOMAIN_CONTROLLER_INFO  
      { 
         public IntPtr DCName;
         public IntPtr DCAddress;
         public uint  DCAddressType;
         public IntPtr GUID;
         public IntPtr DomainName;
         public IntPtr DnsForestName;
         public uint  Flags;
         public IntPtr DcSiteName;
         public IntPtr ClientSiteName;
      }

      #endregion

      #endregion

      #region Utilities
      //-----------------------------------------------------------------------
      // Init - initializes private variables
      //-----------------------------------------------------------------------
      private static void
         Init()
      {
         unsafe
         {
            LOCALGROUP_INFO_1_SIZE = sizeof(LOCALGROUP_INFO_1);
            LOCALGROUP_MEMBERS_INFO_1_SIZE = sizeof(LOCALGROUP_MEMBERS_INFO_1);
            LOCALGROUP_MEMBERS_INFO_2_SIZE = sizeof(LOCALGROUP_MEMBERS_INFO_2);

            NET_DISPLAY_GROUP_SIZE = sizeof( NET_DISPLAY_GROUP );
            NET_DISPLAY_USER_SIZE = sizeof( NET_DISPLAY_USER );

            GROUP_USERS_INFO_0_SIZE = sizeof( GROUP_USERS_INFO_0 );
         }

         initialized = true;
      }


      //------------------------------------------------------------------------------
      // SetLastError - set LastError field and throw an exception if error occurred.
      //------------------------------------------------------------------------------
      private static void
         SetLastError(
            uint rc
         )
      {
         LastError = rc;

         if( rc != ERROR_SUCCESS && rc != ERROR_MORE_DATA )
         {
            Exception ne = new Exception( String.Format( EXCEPTION_NetworkManagementAPIError, rc ) );
            throw ne;
         }
      }

      //------------------------------------------------------------------------------
      // FormatServerName - format server name so the APIs can recognize it.
      //------------------------------------------------------------------------------
      private static string
         FormatServerName(
            string serverName
         )
      {
         if( serverName == null || serverName.Length == 0 )
            return serverName;
         
         if( !serverName.StartsWith( @"\\" ) )
            serverName = @"\\" + serverName;

         return serverName;
      }


      #endregion

      
   }


}
