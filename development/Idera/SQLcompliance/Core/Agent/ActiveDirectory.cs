using System;
using System.Collections;
using System.DirectoryServices;

namespace Idera.SQLcompliance.Core.Agent
{
   public class MemberAttr
   {
      public string name;
      public string domain;
      public string type;
      public string path;
   }

   internal class ActiveDirectory
   {

      //-----------------------------------------------------------------------
      // GetDomainController - get the address of the domain controller
      //-----------------------------------------------------------------------
      internal Hashtable 
         GetADGroupUsers( 
            string domain,
            string groupName 
             )
      {
         Hashtable members = new Hashtable();
         string dc = NetManagement.GetDomainController( domain );

         try
         {
            dc = dc.Substring(2);
            DirectoryEntry de = new DirectoryEntry( "LDAP://"+dc );
            SearchResult result;
            DirectorySearcher search = new DirectorySearcher();
            search.SearchRoot = de;
            search.Filter = String.Format( "(CN={0})", groupName);
            search.PropertiesToLoad.Add("member");
            result = search.FindOne();

            ArrayList users = new ArrayList();

            if (result == null)
               return members;

            if( result.Properties["member"] == null )
               return members;

            for (int counter = 0; counter < result.Properties["member"].Count; counter++)
            {
               string user = (string)result.Properties["member"][counter];
               string domainName = null;
               string flatDomainName = null;
               string domainController = null;
               int start = 0;
               int end;
               while( start >= 0 )
               {
                  start = user.IndexOf( "DC=", start );
                  if( start == -1 )
                     break;
                  start+=3;
                  end = user.IndexOf( ",", start );
                  if( domainName!= null )
                     domainName += ".";
                  if( end == -1 )
                     domainName = domainName + user.Substring( start );
                  else
                     domainName = domainName + user.Substring( start, end - start );
               }

               GetDomainInfo( domainName, out flatDomainName, out domainController );

               DirectoryEntry tmpDe = new DirectoryEntry();
               tmpDe.Path = String.Format( "LDAP://{0}/{1}", 
                                           domainController, 
                                           user);
               string tmpUser = (string)tmpDe.Properties["sAMAccountName"][0];
               MemberAttr attr = new MemberAttr();
               attr.name = tmpUser;
               attr.type = tmpDe.SchemaClassName;
               attr.domain = flatDomainName ;
               attr.path = tmpDe.Path;
               try
               {
                  members.Add( tmpUser, attr );
               }
               catch{}

            }  

         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred querying group members.",
                                     e,
                                     true );
         }
         return members;
      }


      //-----------------------------------------------------------------------
      // GetADGroups 
      //-----------------------------------------------------------------------
      internal string [] 
         GetADGroups( 
            string domain 
         )
      {
         ArrayList groups = new ArrayList();
         string dc = NetManagement.GetDomainController( domain );

         try
         {
            dc = dc.Substring(2);
            DirectoryEntry de = new DirectoryEntry( "LDAP://"+dc );

            SearchResultCollection result;
            DirectorySearcher search = new DirectorySearcher();
            search.SearchRoot = de;
            search.Filter = String.Format("(objectClass=group)");
            search.PropertiesToLoad.Add("name");
            result = search.FindAll();

            if (result != null)
            {
               for( int i = 0; i < result.Count; i++ )
               {
                  SearchResult tmp = result[i];
                  for (int counter = 0; counter < tmp.Properties["name"].Count; counter++)
                  {
                     groups.Add((string)tmp.Properties["name"][counter]);
                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( "An error occurred querying domain groups.",
                                     e,
                                     true );
         }

         return (string [])groups.ToArray( typeof(string) );
      }

      //-----------------------------------------------------------------------
      // GetDomainInfo
      //-----------------------------------------------------------------------
      private void
         GetDomainInfo(
            string dnsDomain,
            out string flatDomain,
            out string dc
         )
      {
         flatDomain = null;
         dc = NetManagement.ADGetDomainController( dnsDomain, true, out flatDomain );
      }

   }

      



}
