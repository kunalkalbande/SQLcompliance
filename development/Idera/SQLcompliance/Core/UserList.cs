using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Linq;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core
{
   public enum UserType
   {
      User = 1,
      GlobalGroup,
      Domain,
      Alias,
      WellKnownGroup,
      Deleted,
      Invalid,
      Unknown,
      Computer
   }

   [Serializable]
   public struct ServerRole : ISerializable
   {
      internal int  structVersion;
      public string FullName;
      public string Name;
      public  int    Id;

      #region Constructor
      public ServerRole (
         string inName,
         string inFullName,
         int    inId )
      {
         structVersion = CoreConstants.SerializationVersion;
         Name = inName;
         FullName = inFullName;
         Id = inId;
      }

      // Deserialization constructor
      public ServerRole (
         SerializationInfo    info,
         StreamingContext     context )
      {
         structVersion = CoreConstants.SerializationVersion;
         FullName = null;
         Name = null;
         Id = 0;

         try
         {
            try
            {
               structVersion = info.GetInt32( "structVersion" );
            }
            catch
            {
               // There is no target version prior to V 2.0.  
               // Assume it is created by 1.1 and 1.2 assembly.
               structVersion = 0;
            }
            FullName = info.GetString( "FullName" );
            Name     = info.GetString("Name");
            Id       = info.GetInt32( "Id" );
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }
      }
      #endregion

      #region ISerializable Members
      // Required ISerializable member
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "structVersion", structVersion );
            info.AddValue( "FullName", FullName );
            info.AddValue( "Name", Name );
            info.AddValue( "Id", Id );
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }

      }

        #endregion

        /// <summary>
        /// Compares the server role considering default properties
        /// </summary>
        /// <param name="otherServerRole">server role compared against</param>
        /// <returns>true if roles are equal</returns>
        public bool CompareName(ServerRole otherServerRole)
        {
            var isFirstDefault = string.Equals(this.Name, this.FullName, StringComparison.OrdinalIgnoreCase);
            var isOtherDefault = string.Equals(otherServerRole.Name, otherServerRole.FullName, StringComparison.OrdinalIgnoreCase);
            if (isFirstDefault || isOtherDefault)
            {
                return string.Equals(this.FullName, otherServerRole.FullName, StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(this.Name, otherServerRole.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares the server role considering default properties
        /// </summary>
        /// <param name="otherServerRole">server role compared against</param>
        /// <returns>true if roles are equal</returns>
        public bool CompareName(RawRoleObject otherServerRole)
        {
            var isFirstDefault = string.Equals(this.Name, this.FullName, StringComparison.OrdinalIgnoreCase);
            if (isFirstDefault)
            {
                return string.Equals(this.FullName, otherServerRole.fullName, StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(this.Name, otherServerRole.name, StringComparison.OrdinalIgnoreCase);
        }
    }


   [Serializable]
   public struct Login
   {
      internal int    structVersion;
      public string   Name;
      public byte []  Sid;

      #region Constructor
      public Login (
         string inName,
         byte [] inSid )
      {
         structVersion = CoreConstants.SerializationVersion;
         Name = inName;
         Sid = inSid;
      }

      // Deserialization constructor
      public Login (
         SerializationInfo    info,
         StreamingContext     context )
      {
         structVersion = CoreConstants.SerializationVersion;
         Name = null;
         Sid  = null;

         try
         {
            try
            {
               structVersion = info.GetInt32( "structVersion" );
            }
            catch
            {
               structVersion = 0;
            }

            Name  = info.GetString("Name");
            Sid   = info.GetValue( "Sid", typeof(byte []) ) as byte [];
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowDeserializationException( e, this.GetType());
         }

      }

      #endregion

      #region ISerializable Members

      // Required ISerializable member
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         try
         {
            info.AddValue( "Name", Name );
            info.AddValue( "Sid", Sid );
         }
         catch( Exception e )
         {
            SerializationHelper.ThrowSerializationException( e, this.GetType());
         }
      }

      #endregion

   }

   public struct UserInfo
   {
      public string Name;
      public UserType type;
   }

    /// <summary>
    /// Summary description for UserList.
    /// </summary>
    public class UserList
    {

        #region Private fields

        private Hashtable serverRoleList;
        private Hashtable loginList;

        #endregion

        #region Constructors

        public UserList() : this(null)
        {
        }

        //-------------------------------------------------------------------
        // Initialize the instance using an user list string
        //-------------------------------------------------------------------
        public UserList(string inUserList)
        {
            serverRoleList = new Hashtable();
            loginList = new Hashtable();

            if (inUserList != null && inUserList != "")
            {
                LoadFromString(inUserList);
            }
        }

        //-------------------------------------------------------------------
        // Initialzie the instance using a RemoteUserList object
        //-------------------------------------------------------------------
        public UserList(RemoteUserList list)
        {
            serverRoleList = new Hashtable();
            loginList = new Hashtable();

            this.ServerRoles = list.ServerRoles;
            this.Logins = list.Logins;
        }

        public static bool Match(string userList1, string userList2)
        {
            UserList ul1 = new UserList(userList1);
            UserList ul2 = new UserList(userList2);

            return ul1.ToString() == ul2.ToString();
        }

        #endregion

        #region Properties

        //-------------------------------------------------------------------
        // Logins
        //-------------------------------------------------------------------
        public Login[] Logins
        {
            get
            {
                Login[] loginArry = new Login[loginList.Count];
                try
                {
                    IDictionaryEnumerator enumerator = loginList.GetEnumerator();
                    int i = 0;
                    while (enumerator.MoveNext())
                        loginArry[i++] = (Login)enumerator.Value;
                }
                catch
                {
                }
                return loginArry;
            }

            set
            {
                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++)
                        AddLogin(value[i]);
                }
                else
                    loginList.Clear();
            }
        }

        //-------------------------------------------------------------------
        // UserNames
        //-------------------------------------------------------------------
        public string[] UserNames
        {
            get
            {
                string[] names = new string[loginList.Count];
                try
                {
                    IDictionaryEnumerator enumerator = loginList.GetEnumerator();
                    int i = 0;
                    while (enumerator.MoveNext())
                        names[i++] = ((Login)enumerator.Value).Name;
                }
                catch
                {
                }
                return names;
            }
        }


        //-------------------------------------------------------------------
        // ServerRoles
        //-------------------------------------------------------------------
        public ServerRole[] ServerRoles
        {
            get
            {
                ServerRole[] roles = new ServerRole[serverRoleList.Count];
                try
                {
                    IDictionaryEnumerator enumerator = serverRoleList.GetEnumerator();
                    int i = 0;
                    while (enumerator.MoveNext())
                        roles[i++] = (ServerRole)enumerator.Value;
                }
                catch
                {
                }
                return roles;
            }

            set
            {
                if (value != null && value.Length > 0)
                {
                    for (int i = 0; i < value.Length; i++)
                        AddServerRole(value[i]);
                }
                else
                    serverRoleList.Clear();
            }
        }

        //-------------------------------------------------------------------
        // ServerRoleIdList
        //-------------------------------------------------------------------
        public int[] ServerRoleIdList
        {
            get
            {
                int[] idList = new int[serverRoleList.Count];
                try
                {
                    IDictionaryEnumerator enumerator = serverRoleList.GetEnumerator();
                    int i = 0;
                    while (enumerator.MoveNext())
                        idList[i++] = ((ServerRole)enumerator.Value).Id;
                }
                catch
                {
                }
                return idList;
            }
        }

        #endregion

        #region Add/Remove/Clear Public Methods

        //-------------------------------------------------------------------
        // AddServerRole
        //-------------------------------------------------------------------
        public void
           AddServerRole(
              string roleName,
              string fullName,
              int id
           )
        {
            ServerRole newRole = new ServerRole(roleName, fullName, id);
            AddServerRole(newRole);
        }

        ///<summary>
        /// AddServerRole from <see cref="RawRoleObject"/>
        ///</summary>
        ///<remarks>
        /// SQLCM-5849: Handle case-insensitive trusted/privilege user for default settings
        ///</remarks>
        public void AddServerRole(
              RawRoleObject rawRole
           )
        {
            ServerRole newRole = new ServerRole(rawRole.name, rawRole.fullName, rawRole.roleid);
            AddServerRole(newRole);
        }

        //-------------------------------------------------------------------
        // AddServerRole
        //-------------------------------------------------------------------
        public void
           AddServerRole(
              ServerRole newRole
           )
        {
            if (newRole.Name != null && !serverRoleList.ContainsKey(newRole.Name))
                serverRoleList.Add(newRole.Name, newRole);
        }

        //-------------------------------------------------------------------
        // RemoveServerRole
        //-------------------------------------------------------------------
        public void
           RemoveServerRole(
              string roleName
           )
        {
            try
            {
                serverRoleList.Remove(roleName);
            }
            catch { }
        }

        //-------------------------------------------------------------------
        // ClearServerRoles
        //-------------------------------------------------------------------
        public void
           ClearServerRoles()
        {
            serverRoleList.Clear();
        }


        //-------------------------------------------------------------------
        // AddLogin
        //-------------------------------------------------------------------
        public void
           AddLogin(
              string name,
              byte[] sid
           )
        {
            try
            {
                Login newLogin = new Login(name, sid);
                AddLogin(newLogin);
            }
            catch { }  // ignore duplicates and nulls
        }

        ///<summary>
        /// AddLogin from <see cref="RawLoginObject"/>
        ///</summary>
        ///<remarks>
        /// SQLCM-5849: Handle case-insensitive trusted/privilege user for default settings
        ///</remarks>
        public void
           AddLogin(
              RawLoginObject rawLogin
           )
        {
            try
            {
                Login newLogin = new Login(rawLogin.name, rawLogin.sid);
                AddLogin(newLogin);
            }
            catch { }  // ignore duplicates and nulls
        }

        //-------------------------------------------------------------------
        // AddLogin
        //-------------------------------------------------------------------
        public void
           AddLogin(
              Login newLogin
           )
        {
            if (newLogin.Name != null && !loginList.ContainsKey(newLogin.Name))
                loginList.Add(newLogin.Name, newLogin);
        }

        //-------------------------------------------------------------------
        // RemoveLogin
        //-------------------------------------------------------------------
        public void
           RemoveLogin(
              string name
           )
        {
            try
            {
                loginList.Remove(name);
            }
            catch { }
        }

        //-------------------------------------------------------------------
        // ClearLogins
        //-------------------------------------------------------------------
        public void
           ClearLogins()
        {
            loginList.Clear();
        }

        #endregion

        #region String Conversion
        //-------------------------------------------------------------------
        // ToXMLString
        //-------------------------------------------------------------------
        public override string
           ToString()
        {
            return ToString(this);
        }

        //-------------------------------------------------------------------
        // ToXMLString
        //-------------------------------------------------------------------
        public static string
           ToString(
              UserList userList
           )
        {
            RemoteUserList remoteList = new RemoteUserList(userList.ServerRoles,
                                                            userList.Logins);
            MemoryStream stream = null;
            IFormatter formatter;
            string outputString = null;

            try
            {
                formatter = new BinaryFormatter();
                stream = new MemoryStream();

                formatter.Serialize(stream, remoteList);
                outputString = Convert.ToBase64String(stream.ToArray());

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Error writing user list to string.",
                                         e,
                                         true);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return outputString;
        }

        //-------------------------------------------------------------------
        // LoadFromString
        //-------------------------------------------------------------------
        public void
           LoadFromString(
              string userList
           )
        {
            IFormatter formatter;
            MemoryStream stream = null;

            if (String.IsNullOrWhiteSpace(userList))
            {
                return;
            }

            try
            {
                stream = new MemoryStream(Convert.FromBase64String(userList));
                formatter = new BinaryFormatter();

                RemoteUserList remoteList = (RemoteUserList)formatter.Deserialize(stream);

                this.ServerRoles = remoteList.ServerRoles;
                this.Logins = remoteList.Logins;

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Error reading user list from string",
                                         e,
                                         true);

            }
            finally
            {
                if (stream != null)
                    stream.Close();

            }
        }

        #endregion

        #region User Group Helpers
        //-------------------------------------------------------------------
        // GetSQLServerUserGroups
        //-------------------------------------------------------------------
        public static Hashtable
           GetSQLServerUserGroups(
              string instance
           )
        {
            string query = "SELECT loginname FROM syslogins WHERE isntgroup = 1 AND hasaccess = 1";
            string groupName;
            Hashtable groups = new Hashtable();

            if (SQLcomplianceAgent.Instance == null)
                return groups;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instance))
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                while (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                    {
                                        groupName = reader.GetString(0);
                                        groups.Add(groupName.ToUpper(), groupName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred retrieving user groups needed to build privileged user list.  Stmt: {0}.",
                                                        query),
                                         e,
                                         true);
            }

            return groups;
        }

        //-------------------------------------------------------------------
        // ExplodeGroupsToUserList
        //-------------------------------------------------------------------
        public static string[]
           ExplodeGroupsToUserList(
              string instanceName,
              string[] groups,
              UserCache userCache
           )
        {
            UserCache cache = userCache == null ? new UserCache(instanceName) : userCache;
            string[] userList = null;

            try
            {
                userList = GetUserLIst(instanceName, groups, cache);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred expanding user groups into a list of users to create the list of privileged users.",
                                         e,
                                         true);

            }

            if (groups != null && groups.Length > 0)
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "ExplodeGroupsToUserList: Complete");

            return userList;
        }

        /*
              //-------------------------------------------------------------------
              // ExplodeUserGroups
              //-------------------------------------------------------------------
              public static Hashtable
                    ExplodeUserGroups(
                    string    instanceName,
                    string [] users)
                {
                    Hashtable userList = new Hashtable();
                    GroupEnumerator enumerator = new GroupEnumerator() ;

                    if( users != null )
                    {
                        string domainName;
                        string groupName;

                        foreach( string userName in users )
                        {

                            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                String.Format( "Retrieving user information for {0}",
                                userName ));
                            domainName = ParseDomainFromFullGroupName( userName );
                            groupName = ParseGroupNameFromFullGroupName( userName );

                            if( domainName == "" )
                            {
                                try
                                {
                                    userList.Add( userName.ToUpper(), userName );
                                    ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        String.Format( "SQL Server login {0} added",
                                        userName ));
                                }
                                catch{}
                            }
                            else
                            {
                                Account baseAccount = new Account() ;
                                string shortPath ;
                                baseAccount.NtDomain = domainName ;
                                baseAccount.SAMName = groupName ;
                          ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug,
                                String.Format( "ExplodeUserGroups().\n\tFull User Name = {0}\n\tSAM Name = {1}\n\tNT Domain = {2}",
                                userName, groupName, domainName ));
                                ArrayList accountList = enumerator.ExpandGroup(baseAccount, true) ;

                                if(accountList.Count == 0 && !baseAccount.IsGroup)
                                {
                                    // This is a user, just add it.
                                    shortPath = baseAccount.ShortPath() ;
                                    if(!userList.Contains(shortPath.ToUpper()))
                                    {
                                        userList.Add(shortPath.ToUpper(), shortPath) ;
                                        ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                            String.Format( "Windows login {0} added",
                                            shortPath ));
                                    }
                                }
                                else
                                {
                                    // This was a group, add the subitems
                                    foreach(Account subAccount in accountList)
                                    {
                                        shortPath = subAccount.ShortPath() ;
                                        if(!userList.Contains(shortPath.ToUpper()) &&
                                   IsValidLogin( shortPath ) )
                                        {
                                            userList.Add(shortPath.ToUpper(), shortPath) ;
                                            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                String.Format( "Group subitem: Windows login {0} added",
                                                shortPath ));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return userList;

                }
         */

        //-------------------------------------------------------------------
        // GetUserLIst
        //-------------------------------------------------------------------
        public static string[]
           GetUserLIst(
           string instanceName,
           string[] users,
           UserCache cache)
        {
            GroupEnumerator enumerator = new GroupEnumerator();
            Dictionary<string, string> userList = new Dictionary<string, string>();

            if (users != null)
            {
                string domainName;
                string groupName;

                foreach (string userName in users)
                {

                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format("Retrieving user information for {0}",
                                            userName));
                    domainName = ParseDomainFromFullGroupName(userName);
                    groupName = ParseGroupNameFromFullGroupName(userName);

                    if (domainName == "")
                    {
                        try
                        {
                            cache.Add(userName);
                            userList.Add(userName.ToUpper(), userName);
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                               String.Format("SQL Server login {0} added",
                               userName));
                        }
                        catch { }
                    }
                    else
                    {
                        // Windows login or group
                        Account baseAccount = new Account();
                        string shortPath;
                        baseAccount.NtDomain = domainName;
                        baseAccount.SAMName = groupName;
                        ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                              String.Format("ExplodeUserGroups().\n\tFull User Name = {0}\n\tSAM Name = {1}\n\tNT Domain = {2}",
                              userName, groupName, domainName));
                        ArrayList accountList = enumerator.ExpandGroup(baseAccount, true);

                        if (accountList.Count == 0 && !baseAccount.IsGroup)
                        {
                            // This is a user, just add it.
                            shortPath = baseAccount.ShortPath();
                            if (!userList.ContainsKey(shortPath.ToUpper()))
                            {
                                cache.Add(shortPath);
                                userList.Add(shortPath.ToUpper(), shortPath);
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                   String.Format("Windows login {0} added",
                                   shortPath));
                            }
                        }
                        else
                        {
                            // This was a group, add the subitems
                            foreach (Account subAccount in accountList)
                            {
                                shortPath = subAccount.ShortPath();
                                cache.Add(userName, shortPath);
                                if (!userList.ContainsKey(shortPath.ToUpper()) &&
                                   IsValidLogin(shortPath))
                                {
                                    userList.Add(shortPath.ToUpper(), shortPath);
                                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                       String.Format("Group subitem: Windows login {0} added",
                                       shortPath));
                                }
                            }
                        }
                    }
                }
            }

            List<string> theList = new List<string>(userList.Count);
            theList.AddRange(userList.Values);
            return theList.ToArray();

        }

        /*
        //-------------------------------------------------------------------
        // ExplodeGroup
        //-------------------------------------------------------------------
        public static string []
           ExplodeGroup(
              string domain,
              string group
           )
        {
           Hashtable members;
           Hashtable tmpUsers = new Hashtable();
           MemberAttr member;
           ActiveDirectory ad = new ActiveDirectory();
           string name;

           members = ad.GetADGroupUsers( domain, group );
           IDictionaryEnumerator enumerator = members.GetEnumerator();
           while( enumerator.MoveNext() )
           {
              member = (MemberAttr)enumerator.Value;
              if( member.type == "group" )
              {
                 string [] users = ExplodeGroup( member.domain, member.name );
                 if( users != null )
                 {
                    for( int i = 0; i < users.Length; i++ )
                    {
                       try
                       {
                          tmpUsers.Add( users[i], users[i] );
                       }
                       catch{}
                    }
                 }
              }
              else if( member.type == "user" )
              {
                 name = member.domain + @"\" + member.name;
                 try
                 {
                    tmpUsers.Add( name, name );
                 }
                 catch{}
              }
           }

           IDictionaryEnumerator enum1 = tmpUsers.GetEnumerator();
           string [] finalList = new string[tmpUsers.Count];
           int j = 0;
           while( enum1.MoveNext() )
              finalList[j++] = (string)enum1.Value;
           return finalList;

        }

          //-------------------------------------------------------------------
          // ExplodeUserGroups
          //-------------------------------------------------------------------
        //-------------------------------------------------------------------
        // ExplodeUserGroups
        //-------------------------------------------------------------------
        public static Hashtable
           ExplodeUserGroups(
              string    instanceName,
              string [] users,
              out bool  exploded
           )
        {
           Hashtable userList = new Hashtable();
           exploded = false;

           if( users != null )
           {
              string [] members;
              string domainName;
              string fullName;

              foreach( string userName in users )
              {

                 ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format( "Retrieving user information for {0}",
                                                           userName ));
                 domainName = ParseDomainFromFullGroupName( userName );

                 if( domainName == "" )
                 {
                    try
                    {
                       userList.Add( userName, userName );
                       ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                               String.Format( "SQL Server login {0} added",
                                                                 userName ));
                    }
                    catch{}
                 }
                 else
                 {
                    members = GetGroupUsers( out domainName, userName );
                    if( members == null )
                       continue;
                    exploded = false;

                    for( int i = 0; i < members.Length; i++ )
                    {
                       try
                       {
                          fullName = CreateFullUserName( domainName, members[i] );
                          userList.Add( fullName, fullName );
                          ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                  String.Format( "Windows login {0} added",
                                                                    fullName ));
                       }
                       catch{} // ignore duplicates
                    }
                 }
              }
           }

           return userList;

        }
        //-------------------------------------------------------------------
        // GetGroupUsers
        //-------------------------------------------------------------------
        public static string []
           GetGroupUsers(
              out string domainName,
              string groupName
           )
        {
           string    dc    = null;
           string [] users = null;
           Hashtable ntGroups = new Hashtable();
           string [] groups;
           ArrayList userList = new ArrayList();
           bool      isDomain;
           bool      isLocal = false;
           bool      tryDomainLocal = false;
           string    groupDomain;
           string    tmpDomain;
           string    savedName = groupName;

           ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                    String.Format( "Getting groups users:  Group = {0}",
                                                     groupName == null ? "<null>" : groupName ));

           tmpDomain = NetManagement.GetDomainName( out isDomain );

           groupDomain = ParseDomainFromFullGroupName( groupName );
           groupName = ParseGroupNameFromFullGroupName( groupName );
           domainName = groupDomain;
           if( isDomain )
           {
              if( domainName != tmpDomain )
              {
                 if( domainName.ToUpper() == SQLcomplianceAgent.Instance.AgentServer.ToUpper() )
                    isLocal = true;
              }
           }

           ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                    String.Format( "GetGroupUsers: this machine is {0} in a domain.  Domain name = {1}",
                                                     isDomain ? "" : "not",
                                                     tmpDomain ));
           if( isLocal )
           {
              if( NetManagement.IsUser( null, groupName ) )
              {
                 userList.Add( savedName );
              }
              else
              {
                 userList.AddRange( GetLocalGroupUsers( groupName ) );
              }
              return (string [])userList.ToArray( typeof(string) );
           }

           if( isDomain )
           {
              try
              {
                 dc = NetManagement.GetDomainController( groupDomain );

              }
              catch( Exception e )
              {
                 ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                          "An error occurred getting domain controller for" + groupDomain,
                                          e.Message );
                 dc = null;
                 isDomain = false;

              }

              if( isDomain )
              {

                 try
                 {
                    groups = NetManagement.GetGroups( dc );
                    if( groups != null )
                    {
                       for( int i = 0; i < groups.Length; i++ )
                       {
                          ntGroups.Add( groups[i], groups[i] );
                       }
                       ErrorLog.Instance.Write( ErrorLog.Level.Debug,                             
                                            String.Format( "{0} groups retrieved",
                                            groups.Length ) );
                   }
                 }
                 catch( Exception e )
                 {
                    ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                             "An error occurred getting domain groups",
                                              e.Message );
                 }
              }
              else
              {
                 isDomain = false;
              }
           }


           try
           {
              if( isDomain ) // is in a domain
              {
                 ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                          String.Format( "Retrieving domain group users for {0} from domain controller {1}",
                                                         groupName, dc ));
                 try
                 {
                    domainName = groupDomain;
                    if( ntGroups.Contains( groupName ) )
                    {
                       string [] gUsers = ExplodeGroup( groupDomain, groupName );
                       if( gUsers != null && gUsers.Length > 0 )
                          userList.AddRange( gUsers );
                    }
                    else
                       userList.Add( groupDomain + @"\" + groupName );
                 }
                 catch( Exception e )
                 {
                    if( NetManagement.LastError == NetManagement.ERROR_GROUP_DOES_NOT_EXISTS )
                    {
                       tryDomainLocal = true;
                    }
                    else
                    {
                       ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                "An error occurred getting domain group users.",
                                                e,
                                                true );
                       throw;
                    }
                 }

                 if( tryDomainLocal )
                 {
                    try
                    {
                       // Domain local group
                       string domainLocalGroup = CreateFullUserName( tmpDomain, groupName );
                       ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                               String.Format( "Retrieving domain local group {0} users.",
                                                              domainLocalGroup ));
                       UserInfo [] userInfo;
                       ArrayList tmpUsers = new ArrayList();
                       userInfo = NetManagement.GetLocalGroupUsers( dc, groupName );
                       for( int i = 0; i < userInfo.Length; i++ )
                       {
                          if( userInfo[i].type == UserType.User )
                             tmpUsers.Add( userInfo[i].Name );
                          else if( userInfo[i].type == UserType.GlobalGroup )
                          {
                             string tmp;
                             tmpUsers.AddRange( GetGroupUsers( out tmp, userInfo[i].Name ) );
                          }
                       }
                       users = (string [])tmpUsers.ToArray( typeof(string) );
                    }
                    catch
                    {
                       ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                                String.Format( "Last error for domain local group call: {0}",
                                                NetManagement.LastError ));
                       throw;
                    }

                 }

                 if( users != null )
                 {
                    for( int i = 0; i < users.Length; i++ )
                    {
                       if( ntGroups.Contains( users[i] ) )
                       {
                          string [] groupUsers = GetGroupUsers( out domainName, users[i] );
                          if( groupUsers != null )
                          {
                             for( int j = 0; j < groupUsers.Length; j++ )
                             {
                                userList.Add( CreateFullUserName( domainName, groupUsers[j] ));
                             }
                          }
                       }
                       else
                       {
                          ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                             String.Format( "Adding user {0}",
                             users[i] ));
                          userList.Add( CreateFullUserName( domainName, users[i] ));
                       }
                    }
                 }
              }
              else // not in a domain
              {
                 userList.AddRange( GetLocalGroupUsers( groupName ));
              }

           }
           catch( Exception e )
           {
              ErrorLog.Instance.Write( "An error occurred getting user group members",
                                       e,
                                       true );

           }

           return (string [])userList.ToArray( typeof(string) );
        }

        //-------------------------------------------------------------------
        // GetGroupUsers
        //-------------------------------------------------------------------
        private static string []
           GetLocalGroupUsers(
              string groupName
           )
        {
              ArrayList userList = new ArrayList();

              ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                      "Retrieving local group users for " + groupName );

              try
              {
                 UserInfo [] userInfo;
                 userInfo = NetManagement.GetLocalGroupUsers( groupName );
                 for( int i = 0; i < userInfo.Length; i++ )
                 {
                    if( userInfo[i].type == UserType.User )
                       userList.Add( userInfo[i].Name );
                    else if( userInfo[i].type == UserType.GlobalGroup ||
                             userInfo[i].type == UserType.Alias )
                    {
                       string tmp;
                       userList.AddRange( GetGroupUsers( out tmp, userInfo[i].Name ) );
                    }
                    else if( userInfo[i].type == UserType.WellKnownGroup )
                    {
                       userList.AddRange( GetLocalGroupUsers( userInfo[i].Name ) );
                    }
                 }
              }
              catch( Exception ex )
              {
                 if( NetManagement.LastError == NetManagement.ERROR_LOCAL_GROUP_DOES_NOT_EXIST )
                    userList.Add( groupName );
                 else
                 {
                    ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                            "An error occurred retrieving local group users.",
                                            ex,
                                            true );
                    throw;
                 }
              }
              return (string []) userList.ToArray( typeof(string) );
        }
        */

        //-------------------------------------------------------------------
        // ParseDomainFromFullGroupName
        //-------------------------------------------------------------------
        private static string
           ParseDomainFromFullGroupName(
              string groupName
           )
        {
            int index = groupName.IndexOf(@"\");
            if (index > 0)
            {
                return groupName.Substring(0, index);
            }
            else
            {
                return "";
            }
        }

        //-------------------------------------------------------------------
        // ParseGroupNameFromFullGroupName
        //-------------------------------------------------------------------
        private static string
           ParseGroupNameFromFullGroupName(
              string fullGroupName
           )
        {
            int index = fullGroupName.IndexOf(@"\") + 1;

            if (index > 0)
            {
                return fullGroupName.Substring(index, (fullGroupName.Length - index));
            }
            else
            {
                return fullGroupName;
            }
        }

        //-------------------------------------------------------------------
        // GetNewUserList
        //-------------------------------------------------------------------
        public static string[]
           GetNewUserList(
              string instanceName,
              string[] userGroups,
              int[] serverRoles,
              string[] userList
           )
        {
            Hashtable newUserTable = new Hashtable();
            string[] newUsers = ConfigurationHelper.GetAuditedPrivilegedUsers(instanceName,
                                                                                 userGroups,
                                                                                 serverRoles);
            for (int i = 0; i < newUsers.Length; i++)
                newUserTable.Add(newUsers[i], newUsers[i]);

            bool changed = false;

            if (userList != null && userList.Length == newUserTable.Count)
            {
                for (int i = 0; i < userList.Length; i++)
                {
                    if (!newUserTable.Contains(userList[i]))
                    {
                        changed = true;
                        break;
                    }
                }
            }
            else
                changed = true;

            if (changed)
            {
                return newUsers;
            }
            else
            {
                return null;
            }
        }

        /*
        //-------------------------------------------------------------------
        // CreateFullUserName
        //-------------------------------------------------------------------
        private static string
           CreateFullUserName(
              string domainName,
              string userName
           )
        {
           if( ParseDomainFromFullGroupName( userName ) == "" )
           {
              bool isDomain;
              if( domainName == null )
                 domainName = NetManagement.GetDomainName( out isDomain );

              return domainName + @"\" + userName;
           }
           else
              return userName;
        }
         */

        private static bool
           IsValidLogin(
              string loginName
           )
        {
            int index = loginName.IndexOf(@"\");

            if (index <= 0)
                return false;

            if ((index + 1) == loginName.Length)
                return false;

            return true;
        }
        #endregion



        //v5.6 SQLCM-5373
        #region Trusted user Server Roles
        //-------------------------------------------------------------------------
        // GetAuditedPrivUsers
        //-------------------------------------------------------------------------
        public static string[]
         GetTrustedUsersServerRole(
            string instance,
            int[] roles,
            UserCache userCache
         )
        {
            if (roles == null || roles.Length == 0)
                return null;

            UserCache cache = userCache == null ? new UserCache(instance) : userCache;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instance))
                {
                    int sqlVersion = SQLHelpers.GetSqlVersion(conn);

                    try
                    {
                        if (sqlVersion < 9)
                        {
                            return Get2000PrivUsers(conn, cache, roles, true);
                        }
                        else // SQL 2005 and later
                        {
                            return Get2005PrivUsers(conn, cache, roles, true);
                        }

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                   String.Format("An error occurred retrieving trusted user list for {0}.", instance),
                                                   ex,
                                                   true);
                    }
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                           String.Format("Error connecting to {0} during retrieving trusted user list.", instance),
                                           e,
                                           true);
            }
            // There are errors when querying the SQL instance.  Just return whatever is in cache.
            return cache.GetTrustedUsers(roles, null, false);
        }

        #region Trusted user Server Roles
        //-------------------------------------------------------------------------
        // GetAuditedPrivUsers
        //-------------------------------------------------------------------------
        public static string[]
           GetAuditedServerRoleUsers(
              string instance,
              int[] roles,
              UserCache userCache
           )
        {
            if (roles == null || roles.Length == 0)
                return null;

            UserCache cache = userCache == null ? new UserCache(instance) : userCache;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instance))
                {
                    int sqlVersion = SQLHelpers.GetSqlVersion(conn);

                    try
                    {
                        if (sqlVersion < 9)
                        {
                            return Get2000PrivUsers(conn, cache, roles, false); // added parameter //v5.6 SQLCM-5373
                        }
                        else // SQL 2005 and later
                        {
                            return Get2005PrivUsers(conn, cache, roles, false);// added parameter //v5.6 SQLCM-5373
                        }

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                   String.Format("An error occurred retrieving privileged user list for {0}.", instance),
                                                   ex,
                                                   true);
                    }
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                           String.Format("Error connecting to {0} during retrieving privileged user list.", instance),
                                           e,
                                           true);
            }
            // There are errors when querying the SQL instance.  Just return whatever is in cache.
            return cache.GetAuditedUsers(roles, null, false);
        }


        #region SQL Server 2000 implementation


        // added paramter //v5.6 SQLCM-5373
        private static string[] Get2000PrivUsers(SqlConnection conn, UserCache cache, int[] roles, bool forTrustedUsers)
        {
            string query = BuildGetSQL2000PrivUserQuery(roles);

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                string user = reader.GetString(0);

                                cache.Add(reader.GetInt32(1), user);
                            }
                        }
                    }
                }
            }
            //v5.6 SQLCM-5373
            if (forTrustedUsers)
                return cache.GetTrustedUsers(roles, null, false);
            else
                return cache.GetAuditedUsers(roles, null, false);
        }


        //-------------------------------------------------------------------------
        // GetAuditedPrivUsers
        //-------------------------------------------------------------------------
        private static string
           BuildGetSQL2000PrivUserQuery(
              int[] serverRoles
           )
        {
            StringBuilder builder = new StringBuilder("SELECT DISTINCT l.name, s.number FROM master.dbo.sysxlogins l, master.dbo.spt_values s WHERE ( s.low = 0 AND s.type = 'SRV' AND ( s.number & l.xstatus = s.number ) AND l.srvid IS NULL ) AND s.number in ( ");
            for (int i = 0; i < serverRoles.Length - 1; i++)
                builder.AppendFormat("{0}, ", serverRoles[i]);
            builder.AppendFormat("{0} )", serverRoles[serverRoles.Length - 1]);

            return builder.ToString();
        }

        #endregion

        #region SQL Server 2005 implementation


        //v5.6 SQLCM-5373
        private static string[] Get2005PrivUsers(SqlConnection conn, UserCache cache, int[] roles, bool forTrustedUsers)
        {
            Dictionary<string, int> roleNames = GetSQL2005SvrRoleNames(roles, conn);

            // build query
            StringBuilder query = new StringBuilder("SELECT name");

            if (roleNames != null && roleNames.Count > 0)
            {
                foreach (string name in roleNames.Keys)
                {
                    query.AppendFormat(", {0}", name);
                }

                query.Append(" FROM master..syslogins WHERE ");
                bool first = true;
                foreach (string name in roleNames.Keys)
                {
                    if (first)
                    {
                        query.AppendFormat(" {0} = 1", name);
                        first = false;
                    }
                    else
                        query.AppendFormat(" OR {0} = 1", name);
                }

                //else
                //   return new string[0];

                // get result set
                int[] counters = new int[roleNames.Count];
                using (SqlCommand command = new SqlCommand(query.ToString(), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    string user = reader.GetString(0);

                                    int idx = 1;
                                    foreach (string name in roleNames.Keys) // check which roles this user is in
                                    {
                                        if (reader.GetInt32(idx) == 1)
                                        {
                                            counters[idx - 1]++;
                                            cache.Add(roleNames[name], user);
                                        }
                                        idx++;
                                    }
                                }
                            }
                            int i = 0;
                            foreach (string name in roleNames.Keys)
                            {
                                if (counters[i++] == 0)
                                {
                                    cache.Add(roleNames[name], new List<string>());
                                }
                            }
                        }
                    }
                }
            }
            //fix for SQLCM-6395 start
            if ((roleNames == null && roles.Length > 0) || (roleNames.Count < roles.Length))
            {
                foreach (var r in roles)
                {
                    try
                    {
                        if (roleNames==null || !roleNames.ContainsValue(r))
                        {
                            query = new StringBuilder(String.Format("select  m.name as name from master.sys.server_role_members rm" +
                               " inner join master.sys.server_principals r on r.principal_id = rm.role_principal_id" +
                               " and r.type = 'R'" +
                               " inner join master.sys.server_principals m on m.principal_id = rm.member_principal_id" +
                               " where r.principal_id = {0} ", r));
                            using (SqlCommand command = new SqlCommand(query.ToString(), conn))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader != null)
                                    {
                                        while (reader.Read())
                                        {
                                            if (!reader.IsDBNull(0))
                                            {
                                                string user = reader.GetString(0);
                                                cache.Add(r, user);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
                return new string[0];
            //fix for SQLCM-6395 end
            //v5.6 SQLCM-5373
            if (forTrustedUsers)
            return cache.GetTrustedUsers(roles, null, false);
         else
            return cache.GetAuditedUsers( roles, null, false );
         
      }

	   private static Dictionary<string, int> GetSQL2005SvrRoleNames( int[] roles, SqlConnection conn )
	   {
         Dictionary<string, int> nameMap = new Dictionary<string, int>();

         StringBuilder query = new StringBuilder("SELECT name, number FROM master..spt_values WHERE low = 0 and type = 'SRV' " );
         if( roles != null && roles.Length > 0 )
         {
            query.Append( "AND number IN ( " );
            for( int i = 0; i < roles.Length-1; i++ )
            {
               query.AppendFormat(" {0}, ", roles[i] );
            }
            query.AppendFormat( "{0} )", roles[roles.Length -1]);
         }

         using( SqlCommand cmd = new SqlCommand( query.ToString(), conn ))
         {
            using( SqlDataReader reader = cmd.ExecuteReader() )
            {
               if ( reader != null )
               {
                  while ( reader.Read() )
                  {
                     if ( !reader.IsDBNull( 0 ) )
                     {
                        nameMap.Add( reader.GetString( 0 ), reader.GetInt32( 1 ) );
                     }
                  }
               }
            }
         }

         return nameMap;
	   }

        /*
              private static string BuildGetSQL2005PrivUserQuery( int [] roles, SqlConnection conn )
              {
                 Dictionary<string, int> roleNames = GetSQL2005SvrRoleNames(roles, conn);

                 StringBuilder query = new StringBuilder("SELECT name" );         

                 if( roleNames != null && roleNames.Count > 0 )
                 {
                    foreach( string name in roleNames.Keys )
                    {
                       query.AppendFormat( ", (0)", name );
                    }

                    query.Append( " FROM master..syslogins WHERE " );
                    bool first = true;
                    foreach (string name in roleNames.Keys)
                    {
                       if ( first )
                       {
                          query.AppendFormat( " {0} = 1", name );
                          first = false;
                       }
                       else
                          query.AppendFormat( " OR {0} = 1", name );
                    }
                 }
                 else
                    query.Append(" FROM master..syslogins WHERE  0 = 1 " ); // result in empty result set

                 return query.ToString();
              }
         */


      #endregion
      #endregion
    }
	
	public class UserCache
	{
	   #region Private Fields
	   
	   string _instance;
	   
	   Dictionary<int, List<string>> _srvRoleCache = new Dictionary<int, List<string>>();
	   Dictionary<string, List<string>> _groupCache = new Dictionary<string, List<string>>();
	   Dictionary<string, string> _userCache = new Dictionary<string, string>();

        //v5.6 SQLCM-5373
        Dictionary<int, List<string>> _srvRoleTrustedUsersCache = new Dictionary<int, List<string>>();
       Dictionary<string, List<string>> _trustedusersgroupCache = new Dictionary<string, List<string>>();
       Dictionary<string, string> _trusteduserCache = new Dictionary<string, string>();
	   
	   #endregion
	   
	   #region Properties
	   
	   public string Instance
	   {
	      get
	      {
	         return _instance;
	      }
	   }

      #endregion
      
      #region Constructors
      
      public UserCache( string instance )
      { 
         _instance = instance;
      }

	   #endregion
	   
	   #region Public Methods
	   
	   //
	   // Clear the cached users
	   //
	   public void Clear()
	   {
	      _srvRoleCache.Clear();
	      _groupCache.Clear();
	      _userCache.Clear();
	   }

        //v5.6 SQLCM-5373
        public void ClearTrustedUsers()
       {
           _srvRoleTrustedUsersCache.Clear();
           _trustedusersgroupCache.Clear();
           _trusteduserCache.Clear();
       }


       public string[] GetTrustedUsers(int[] roles, string[] groups)
       {
           return GetTrustedUsers(roles, groups, true);
       }

       //
       // Get a list of trusted users in the server roles and user groups
       //
       public string[] GetTrustedUsers(int[] roles, string[] groups, bool queryServer)
       {
           Dictionary<string, string> users = new Dictionary<string, string>();
           Dictionary<string, string> groupList = new Dictionary<string, string>();

           if (roles != null)
           {
               foreach (int role in roles)
               {
                   if (_srvRoleTrustedUsersCache.ContainsKey(role))
                   {
                       foreach (string member in _srvRoleTrustedUsersCache[role])
                       {
                           if (!groupList.ContainsKey(member))
                           {
                               groupList.Add(member, member);
                           }
                       }
                   }
                   else if (queryServer)
                   {
                       int[] newRole = new int[1];
                       newRole[0] = role;
                       UserList.GetTrustedUsersServerRole(_instance, newRole, this);

                       // now add the members in the server role to the group list
                       if (_srvRoleTrustedUsersCache.ContainsKey(role))
                       {
                           foreach (string member in _srvRoleTrustedUsersCache[role])
                           {
                               if (!groupList.ContainsKey(member))
                               {
                                   groupList.Add(member, member);
                               }
                           }
                       }
                   }
                    //fix for SQLCM-6129 start 
                    if (_srvRoleCache.ContainsKey(role))
                    {
                        foreach (string member in _srvRoleCache[role])
                        {
                            if (!users.ContainsKey(member))
                            {
                                users.Add(member, member);
                            }
                        }
                    }
                    //fix for SQLCM-6129 end
                }
            }

           if (groups != null)
           {
               foreach (string group in groups)
               {
                   if (!groupList.ContainsKey(group))
                   {
                       groupList.Add(group, group);
                   }
               }
           }


           foreach (string group in groupList.Values)
           {
               if (_trustedusersgroupCache.ContainsKey(group))
               {
                   foreach (string member in _trustedusersgroupCache[group])
                   {
                       if (!users.ContainsKey(member))
                       {
                           users.Add(member, member);
                       }
                   }
               }
               else if (queryServer)
               {
                   string[] newGroup = new string[1];
                   newGroup[0] = group;
                   string[] newUsers = UserList.GetUserLIst(_instance, newGroup, this);
                   if (newUsers != null && newUsers.Length > 0)
                   {
                       foreach (string user in newUsers)
                       {
                           if (!users.ContainsKey(user))
                           {
                               users.Add(user, user);
                           }
                       }
                   }
               }
           }

          
           string[] list = new string[users.Count];
           users.Values.CopyTo(list, 0);

           return list;
       }
	   
	   public string [] GetAuditedUsers( int [] roles, string [] groups )
	   {
	      return GetAuditedUsers( roles, groups, true );
	   }

	   //
	   // Get a list of users in the server roles and user groups
	   //
	   public string [] GetAuditedUsers( int [] roles, string [] groups, bool queryServer )
	   {
	      Dictionary<string, string> users = new Dictionary<string, string>();
	      Dictionary<string, string> groupList = new Dictionary<string, string>( );

         if( roles != null )
         {
            foreach ( int role in roles )
            {
               if ( _srvRoleCache.ContainsKey( role ) )
               {
                  foreach ( string member in _srvRoleCache[role] )
                  {
                     if( !groupList.ContainsKey( member ) )
                     {
                           groupList.Add(member, member);
                     }
                  }
               }
               else if( queryServer )
               {
                  int [] newRole = new int[1];
                  newRole[0] = role;
                  UserList.GetAuditedServerRoleUsers( _instance, newRole, this );
                  
                  // now add the members in the server role to the group list
                  if (_srvRoleCache.ContainsKey(role))
                  {
                     foreach (string member in _srvRoleCache[role])
                     {
                        if (!groupList.ContainsKey(member))
                        {
                           groupList.Add(member, member);
                        }
                     }
                  }
               }
            }
         }
         
         if( groups != null )
         {
            foreach ( string group in groups )
            {
               if( !groupList.ContainsKey( group ) )
               {
                  groupList.Add( group, group );
               }
            }
         }


	      foreach ( string group in groupList.Values )
	      {
	         if ( _groupCache.ContainsKey( group ) )
	         {
               foreach ( string member in _groupCache[group] )
               {
                  if( !users.ContainsKey( member ) )
                  {
                     users.Add( member, member );
                  }
	            }
	         }
	         else if( queryServer )
	         {
               string[] newGroup = new string[1];
               newGroup[0] = group;
               string [] newUsers = UserList.GetUserLIst( _instance,newGroup, this );
               if( newUsers != null && newUsers.Length > 0 )
               {
                  foreach ( string user in newUsers )
                  {
                     if( !users.ContainsKey( user ) )
                     {
                        users.Add( user, user );
                     }
                  }
               }
	         }
	      }

	      string[] list = new string[users.Count];
	      users.Values.CopyTo( list, 0 );

	      return list;
	   }
	   
	   internal void Add( int role, List<string> members)
	   {
	      if ( _srvRoleCache.ContainsKey( role ) )
	      {
	         _srvRoleCache[role] = members;
	      }
	      else
	      {
	         _srvRoleCache.Add( role, members );
	      }
	   }
	   
	   public void Add( int role, string member)
	   {
	      if ( _srvRoleCache.ContainsKey( role ) )
	      {
	         _srvRoleCache[role].Add( member );
	      }
	      else
	      {
	         List<string> members = new List<string>();
	         members.Add( member );
	         _srvRoleCache.Add( role, members );
	      }
	      
	      if( !_groupCache.ContainsKey( member ))
	      {
	         string[] g = new string[1];
	         g[0] = member;

	         UserList.GetUserLIst( _instance, g, this );
	      }
	   }

	   public void Add( string group, List<string>members )
	   {
	      if ( _groupCache.ContainsKey( group ) )
	      {
	         _groupCache[group] = members;
	      }
	      else
	      {
	         _groupCache.Add( group, members );
	      }
	   }
	   
	   public void Add( string group, string member )
	   {
	      if ( _groupCache.ContainsKey( group ) )
	      {
            _groupCache[group].Add(member);
	      }
	      else
	      {
	         List<string> members = new List<string>();
	         members.Add( member );
	         _groupCache.Add( group, members );
	      }
	   }


	   public void Add( string user )
	   {
	      Add( user, user );
	   }

        //
        // Rebuild the cache using the server roles and groups
        //v5.6 SQLCM-5373
        //
        public string[] RefreshTrustedUsers(int[] roles, string[] groups)
       {
           this.Clear();
           Dictionary<string, string> auditedUsers = new Dictionary<string, string>();
           string[] users;

           users = UserList.GetTrustedUsersServerRole(_instance, roles, this);
           if (users != null && users.Length > 0)
           {
               foreach (string user in users)
               {
                   try
                   {
                       auditedUsers.Add(user, user);
                   }
                   catch
                   { }
               }
           }

           users = UserList.GetUserLIst(_instance, groups, this);
           if (users != null)
           {
               foreach (string user in users)
               {
                   try
                   {
                       auditedUsers.Add(user, user);
                   }
                   catch
                   { }
               }
           }
           List<string> theList = new List<string>(auditedUsers.Count);
           theList.AddRange(auditedUsers.Values);

           return theList.ToArray();


       }
	   
	   //
	   // Rebuild the cache using the server roles and groups
	   //
	   public string [] Refresh( int [] roles, string [] groups )
	   {
	      this.Clear();
	      Dictionary<string, string>auditedUsers = new Dictionary<string, string>();
         string[] users;
         
         users = UserList.GetAuditedServerRoleUsers( _instance, roles, this );
         if( users != null && users.Length > 0 )
         {
            foreach ( string user in users )
            {
               try
               {
                  auditedUsers.Add( user, user );
               }
               catch
               {}
            }
         }

         users = UserList.GetUserLIst( _instance, groups, this );
         if( users != null )
         {
            foreach ( string user in users )
            {
               try
               {
                  auditedUsers.Add( user, user );
               }
               catch
               {}
            }
         }
	      List<string>theList = new List<string>( auditedUsers.Count );
         theList.AddRange( auditedUsers.Values );
         
         return theList.ToArray();


	   }

	   #endregion
      #endregion
    }
}
