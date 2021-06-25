using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Rules
{
	/// <summary>
	/// Summary description for RulesDal.
	/// </summary>
	public class RulesDal
	{
      private const string SELECT_EventTypesExcludable = "SELECT evtypeid,evcatid,name,category,isExcludable FROM {0}..{1} WHERE isExcludable=1" ;
      private const string SELECT_EventTypesAll = "SELECT evtypeid,evcatid,name,category,isExcludable FROM {0}..{1}";

      private const string SELECT_Servers = "SELECT instance FROM {0}..{1} ORDER BY instance" ;
      private const string SELECT_AuditedServers = "SELECT instance FROM {0}..{1} WHERE isAuditedServer=1" ;
      private const string SELECT_TargetInstances = "SELECT DISTINCT targetInstances FROM {0}..{1} WHERE enabled=1" ;

      
      /// <summary>
      /// Verifies that the connection object is not null and that the connection is
      /// open.
      /// </summary>
      /// <param name="connection"></param>
      /// <exception cref="ArgumentNullException">Thrown when connection is null</exception>
      /// <exception cref="Exception">Thrown when the connection is not open.</exception>
      private static void ValidateConnection(SqlConnection connection)
      {
         if(connection == null)
            throw new ArgumentNullException("connection") ;
         if(connection.State != ConnectionState.Open)
            throw new Exception("SqlConnection object is not opened.") ;
      }

      #region Instance Servers

      public static string[] SelectRegisteredInstances(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectRegisteredInstances(connection) ;
         }
      }

      public static string[] SelectRegisteredInstances(SqlConnection connection)
      {
         ArrayList list = new ArrayList();
         string cmdStr = String.Format(SELECT_Servers, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable) ;
         ValidateConnection(connection);
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  list.Add(reader.GetString(0)) ;
               }
               string[] retVal = new string[list.Count] ;
               for(int i = 0 ; i < list.Count ; i ++)
                  retVal[i] = (string)list[i] ;
               return retVal ;
            }
         }
      }

      public static string[] SelectAuditedInstances(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectAuditedInstances(connection) ;
         }
      }

      public static string[] SelectAuditedInstances(SqlConnection connection)
      {
         ArrayList list = new ArrayList();
         string cmdStr = String.Format(SELECT_AuditedServers, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable) ;
         ValidateConnection(connection);
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  list.Add(reader.GetString(0)) ;
               }
               string[] retVal = new string[list.Count] ;
               for(int i = 0 ; i < list.Count ; i ++)
                  retVal[i] = (string)list[i] ;
               return retVal ;
            }
         }
      }

      public static string[] SelectTargetInstances(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectTargetInstances(connection) ;
         }
      }

      public static string[] SelectTargetInstances(SqlConnection connection)
      {
         ArrayList list = new ArrayList();
         string cmdStr = String.Format(SELECT_TargetInstances, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryAlertRulesTable) ;
         ValidateConnection(connection);
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  string s = reader.GetString(0) ;
                  if(String.Compare("<ALL>", s, true) == 0)
                     return new string[] {"<ALL>"} ;
                  list.Add(s) ;
               }
               string[] retVal = new string[list.Count] ;
               for(int i = 0 ; i < list.Count ; i ++)
                  retVal[i] = (string)list[i] ;
               return retVal ;
            }
         }
      }

      public static Dictionary<int, CMEventCategory> SelectEventCategories(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectEventCategories(connection) ;
         }
      }

      public static Dictionary<int, CMEventCategory> SelectAllEventCategories(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectAllEventCategories(connection) ;
         }
      }

      public static Dictionary<int, CMEventCategory> SelectEventCategories(SqlConnection connection)
      {
         return SelectEventCategories(connection, false) ;
      }

      public static Dictionary<int, CMEventCategory> SelectAllEventCategories(SqlConnection connection)
      {
         return SelectEventCategories(connection, true) ;
      }

      private static Dictionary<int, CMEventCategory> SelectEventCategories(SqlConnection connection, bool includeAll)
      {
         Dictionary<int, CMEventCategory> retVal = new Dictionary<int, CMEventCategory>();
         // Sql Server Event Types
         string cmdStr ;
         
         if(includeAll)
            cmdStr = String.Format(SELECT_EventTypesAll, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventTypesTable) ;
         else
            cmdStr = String.Format(SELECT_EventTypesExcludable, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventTypesTable) ;


         ValidateConnection(connection);
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            CMEventCategory category ;
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  int catId = -1, typeId ;
                  string sName = "", sCategory = "" ;
                  bool excludable = true;

                  typeId = reader.GetInt32(0) ;
                  if(!reader.IsDBNull(1))
                     catId = reader.GetInt32(1) ;
                  if(!reader.IsDBNull(2))
                     sName = reader.GetString(2) ;
                  if(!reader.IsDBNull(3))
                     sCategory = reader.GetString(3) ;
                  excludable = SQLHelpers.ByteToBool(reader, 4) ;

                  if(!retVal.ContainsKey(catId))
                  {
                     category = new CMEventCategory(sCategory, catId);
                     retVal.Add(catId, category);
                  }
                  else
                  {
                     category = retVal[catId] ;
                  }
                  CMEventType eventType = new CMEventType(sName, typeId);
                  eventType.Excludable = excludable;
                  category.AddEventType(eventType);
               }
            }
         }
         return retVal ;
      }

      #endregion
	}
}
