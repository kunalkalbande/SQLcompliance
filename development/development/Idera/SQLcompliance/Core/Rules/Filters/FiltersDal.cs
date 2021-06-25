using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Idera.SQLcompliance.Core.Rules.Filters
{
	/// <summary>
	/// Summary description for FiltersDal.
	/// </summary>
	public class FiltersDal
	{
      #region Member Variables

      private const string SELECT_EventFiltersForServer = "SELECT filterId,name,description,eventType,targetInstances,enabled FROM {0}..{1} where targetInstances='<ALL>' or targetInstances='{2}' or targetInstances LIKE '{2},%' or targetInstances LIKE '%,{2}' or targetInstances LIKE '%,{2},%' ";
      private const string SELECT_EventFilter = "SELECT filterId,name,description,eventType,targetInstances,enabled FROM {0}..{1}";
      private const string SELECT_EventFilterId = "SELECT filterId,name,description,eventType,targetInstances,enabled FROM {0}..{1} WHERE filterId={2}" ;
      private const string INSERT_EventFilter = "INSERT INTO {0}..{1} (name,description,eventType,targetInstances,enabled) VALUES({2}, {3}, {4}, {5}, {6}) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]" ;
      private const string DELETE_EventFilter = "DELETE FROM {0}..{1} WHERE filterId={2}" ;
      private const string UPDATE_EventFilter = "UPDATE {0}..{1} SET name={2},description={3},eventType={4},targetInstances={5},enabled={6} WHERE filterId={7}" ;
      private const string UPDATE_EventFilterEnable = "UPDATE {0}..{1} SET enabled={2} WHERE filterId={3}" ;

      private const string SELECT_EventFilterCondition = "SELECT conditionId, fieldId, matchString FROM {0}..{1} WHERE filterId={2}" ;
      private const string INSERT_EventFilterCondition = "INSERT INTO {0}..{1} (filterId, fieldId, matchString) VALUES ({2}, {3}, {4}) SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]" ;
      private const string DELETE_EventFilterCondition = "DELETE FROM {0}..{1} WHERE conditionId={2}" ;
      private const string DELETE_EventFilterConditions = "DELETE FROM {0}..{1} WHERE filterId={2}" ;
      private const string UPDATE_EventFilterCondition = "UPDATE {0}..{1} SET matchString={2} WHERE conditionId={3}" ;

      private static Hashtable _eventFields ;

      #endregion

      #region Properties

      public static Hashtable SqlServerFilterFields
      {
         set { _eventFields = value ; }
         get { return _eventFields ; }
      }

      #endregion

      #region Construction/Destruction

      public FiltersDal()
      {
      }

      #endregion

      #region General

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

      #endregion // General


      #region Event Filters

      public static List<EventFilter> SelectEventFiltersForServer(string connectionString, string serverName)
      {
         if (connectionString == null)
            throw new ArgumentNullException("connectionString");

         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return SelectEventFiltersForServer(connection, serverName);
         }
      }

      public static List<EventFilter> SelectEventFiltersForServer(SqlConnection connection, string serverName)
      {
         List<EventFilter> retVal = new List<EventFilter>();
         int column = 0;

         ValidateConnection(connection);

         // Get the Rule objects
         string cmdStr = String.Format(SELECT_EventFiltersForServer, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryEventFiltersTable, serverName);
         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  EventFilter filter = new EventFilter();
                  column = 0;

                  filter.Id = reader.GetInt32(column++);
                  filter.Name = reader.GetString(column++);
                  filter.Description = reader.GetString(column++);
                  filter.RuleType = (EventType)reader.GetInt32(column++);
                  filter.TargetInstanceList = reader.GetString(column++);
                  filter.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                  // We only return filters that are enabled
                  if(filter.Enabled)
                     retVal.Add(filter);
               }
            }
         }

         foreach (EventFilter filter in retVal)
         {
            // Get the associated conditions
            ArrayList conditions = SelectEventFilterConditions(filter, connection);
            foreach (EventCondition condition in conditions)
               filter.AddCondition(condition);
         }
         return retVal;
      }
      
      public static ArrayList SelectEventFilters(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectEventFilters(connection) ;
         }
      }
      public static ArrayList SelectEventFilters(string connectionString,string request)
      {
          if (connectionString == null)
              throw new ArgumentNullException("connectionString");

          using (SqlConnection connection = new SqlConnection(connectionString))
          {
              connection.Open();
              return SelectEventFilters(connection, request);
          }
      }
      public static ArrayList SelectEventFilters(SqlConnection connection)
      {
         ArrayList retVal = new ArrayList() ;
         int column = 0 ;

         ValidateConnection(connection) ;

         // Get the Rule objects
         string cmdStr = String.Format(SELECT_EventFilter, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryEventFiltersTable) ;
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  EventFilter filter = new EventFilter() ;
                  column = 0 ;

                  filter.Id = reader.GetInt32(column++) ;
                  filter.Name = reader.GetString(column++) ;
                  filter.Description = reader.GetString(column++) ;
                  filter.RuleType = (EventType)reader.GetInt32(column++) ;
                  filter.TargetInstanceList = reader.GetString(column++) ;
                  filter.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;

                  retVal.Add(filter) ;
               }
            }
         }

         foreach(EventFilter filter in retVal)
         {
            // Get the associated conditions
            ArrayList conditions = SelectEventFilterConditions(filter, connection) ;
            foreach(EventCondition condition in conditions)
               filter.AddCondition(condition) ;
         }
         return retVal ;
      }

      public static ArrayList SelectEventFilters(SqlConnection connection,string request)
      {
          ArrayList retVal = new ArrayList();
          int column = 0;

          ValidateConnection(connection);

          // Get the Rule objects
          string cmdStr = String.Format(SELECT_EventFilter + request, CoreConstants.RepositoryDatabase,
             CoreConstants.RepositoryEventFiltersTable);
          using (SqlCommand command = new SqlCommand(cmdStr, connection))
          {
              using (SqlDataReader reader = command.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      EventFilter filter = new EventFilter();
                      column = 0;

                      filter.Id = reader.GetInt32(column++);
                      filter.Name = reader.GetString(column++);
                      filter.Description = reader.GetString(column++);
                      filter.RuleType = (EventType)reader.GetInt32(column++);
                      filter.TargetInstanceList = reader.GetString(column++);
                      filter.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++));

                      retVal.Add(filter);
                  }
              }
          }

          foreach (EventFilter filter in retVal)
          {
              // Get the associated conditions
              ArrayList conditions = SelectEventFilterConditions(filter, connection);
              foreach (EventCondition condition in conditions)
                  filter.AddCondition(condition);
          }
          return retVal;
      }
      public static EventFilter SelectEventFilterById(int id, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectEventFilterById(id, connection) ;
         }
      }

      public static EventFilter SelectEventFilterById(int id, SqlConnection connection)
      {
         EventFilter filter = null ;

         ValidateConnection(connection) ;
         string cmdStr = String.Format(SELECT_EventFilterId, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryEventFiltersTable, id) ;
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               if(reader.HasRows)
               {
                  filter = new EventFilter() ;

                  reader.Read() ;
                  int column = 0 ;

                  filter.Id = reader.GetInt32(column++) ;
                  filter.Name = reader.GetString(column++) ;
                  filter.Description = reader.GetString(column++) ;
                  filter.RuleType = (EventType)reader.GetInt32(column++) ;
                  filter.TargetInstanceList = reader.GetString(column++) ;
                  filter.Enabled = !SqlByte.Zero.Equals(reader.GetSqlByte(column++)) ;
               }
            }
         }
         if(filter == null)
            return null ;

         // Conditions
         ArrayList conditions = SelectEventFilterConditions(filter, connection) ;
         foreach(EventCondition condition in conditions)
            filter.AddCondition(condition) ;

         return filter; 
      }

      public static bool InsertEventFilter(EventFilter filter, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         if(filter == null)
            throw new ArgumentNullException("filter") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return InsertEventFilter(filter, connection) ;
         }
      }

      public static bool InsertEventFilter(EventFilter filter, SqlConnection connection)
      {
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         ValidateConnection(connection) ;

         using(SqlTransaction trans = connection.BeginTransaction())
         {
            using(SqlCommand command = new SqlCommand())
            {
               command.Transaction = trans ;
               command.Connection = connection ;
               command.CommandText = String.Format(INSERT_EventFilter, CoreConstants.RepositoryDatabase,
                  CoreConstants.RepositoryEventFiltersTable, SQLHelpers.CreateSafeString(filter.Name), 
                  SQLHelpers.CreateSafeString(filter.Description), (int)filter.RuleType, 
                  SQLHelpers.CreateSafeString(filter.TargetInstanceList), filter.Enabled ? 1 : 0) ;

               object o = command.ExecuteScalar() ;
               if(!(o is Decimal))
               {
                  trans.Rollback() ;
                  return false ;
               }
               filter.Id = Decimal.ToInt32((Decimal)o) ;

               // Conditions
               foreach(EventCondition condition in filter.Conditions)
               {
                  if(!InsertEventFilterCondition(condition, connection, trans))
                  {
                     trans.Rollback() ;
                     return false ;
                  }
               }

               trans.Commit() ;
               return true ;
            }
         }
      }

      public static int DeleteEventFilter(EventFilter filter, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         if(filter == null)
            throw new ArgumentNullException("filter") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteEventFilter(filter.Id, connection) ;
         }
      }

      public static int DeleteEventFilter(EventFilter filter, SqlConnection connection)
      {
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         ValidateConnection(connection) ;
         return DeleteEventFilter(filter.Id, connection) ;
      }

      public static int DeleteEventFilter(int filterId, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteEventFilter(filterId, connection) ;
         }
      }

      public static int DeleteEventFilter(int filterId, SqlConnection connection)
      {
         int retVal = 0 ;
         ValidateConnection(connection) ;
         using(SqlTransaction trans = connection.BeginTransaction())
         {
            try
            {
               DeleteEventFilterConditions(filterId, connection, trans) ;
               using(SqlCommand command = new SqlCommand())
               {
                  command.Transaction = trans ;
                  command.Connection = connection ;
                  command.CommandText = String.Format(DELETE_EventFilter, CoreConstants.RepositoryDatabase,
                     CoreConstants.RepositoryEventFiltersTable, filterId) ;

                  retVal = command.ExecuteNonQuery() ;
                  trans.Commit() ;
               }
            }
            catch
            {
               trans.Rollback() ;
            }
         }
         return retVal ;
      }

      public static int UpdateEventFilter(EventFilter filter, string connectionString)
      {
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return UpdateEventFilter(filter, connection) ;
         }
      }

      public static int UpdateEventFilter(EventFilter filter, SqlConnection connection)
      {
         bool bDone = false ;
         int retVal ;
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         ValidateConnection(connection) ;
         using(SqlTransaction trans = connection.BeginTransaction())
         {
            try
            {
               using(SqlCommand command = new SqlCommand())
               {
                  command.Transaction = trans ;
                  command.Connection = connection ;
                  command.CommandText = String.Format(UPDATE_EventFilter, CoreConstants.RepositoryDatabase,
                     CoreConstants.RepositoryEventFiltersTable, SQLHelpers.CreateSafeString(filter.Name), 
                     SQLHelpers.CreateSafeString(filter.Description), (int)filter.RuleType, 
                     SQLHelpers.CreateSafeString(filter.TargetInstanceList), filter.Enabled ? 1 : 0, filter.Id) ;
                  retVal = command.ExecuteNonQuery() ;
                  if(retVal == 0)
                     return 0 ;
                  foreach(EventCondition condition in filter.Conditions)
                  {
                     if(condition.Dirty)
                     {
                        if(condition.Id != EventRule.NULL_ID)
                        {
                           if(UpdateEventFilterCondition(condition, connection, trans) != 1)
                              return 0 ;
                        }
                        else
                        {
                           if(!InsertEventFilterCondition(condition, connection, trans))
                              return 0 ;
                        }
                     }
                  }
                  foreach(EventCondition condition in filter.RemovedConditions)
                  {
                     if(DeleteEventFilterCondition(condition, connection, trans) != 1)
                        return 0 ;
                  }
               }
               bDone = true ;
            }
            finally
            {
               if(!bDone)
                  trans.Rollback() ;
               else
                  trans.Commit() ;
            }
         }
         return retVal ;
      }

      public static int UpdateEventFilterEnabled(EventFilter filter, string connectionString)
      {
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return UpdateEventFilterEnabled(filter, connection) ;
         }
      }

      public static int UpdateEventFilterEnabled(EventFilter filter, SqlConnection connection)
      {
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         ValidateConnection(connection) ;

         using(SqlCommand command = new SqlCommand())
         {
            command.Connection = connection ;
            command.CommandText = String.Format(UPDATE_EventFilterEnable, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventFiltersTable, filter.Enabled ? 1 : 0, filter.Id) ;
            return command.ExecuteNonQuery() ;
         }
      }


      #endregion // Event Filters

      #region Conditions

      public static ArrayList SelectEventFilterConditions(EventFilter parentRule, string connectionString)
      {
         if(parentRule == null)
            throw new ArgumentNullException("parentRule") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return SelectEventFilterConditions(parentRule, connection) ;
         }
      }

      public static ArrayList SelectEventFilterConditions(EventFilter parentRule, SqlConnection connection)
      {
         ArrayList retVal = new ArrayList() ;
         int column = 0 ;

         if(parentRule == null)
            throw new ArgumentNullException("parentRule") ;
         ValidateConnection(connection) ;

         if(_eventFields == null)
            throw new CoreException("Alertable fields have not been initialized.") ;

         string cmdStr = String.Format(SELECT_EventFilterCondition, CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryEventFilterConditionsTable, parentRule.Id) ;
         using(SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using(SqlDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  EventCondition condition = new EventCondition() ;
                  column = 0 ;

                  condition.Id = reader.GetInt32(column++) ;
                  condition.TargetEventField = (EventField)_eventFields[reader.GetInt32(column++)] ;
                  condition.MatchString = reader.GetString(column++) ;
                  condition.ParentRule = parentRule ; 

                  retVal.Add(condition) ;
               }
            }
         }
         return retVal ;
      }

      public static bool InsertEventFilterCondition(EventCondition condition, string connectionString)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return InsertEventFilterCondition(condition, connection) ;
         }
      }

      public static bool InsertEventFilterCondition(EventCondition condition, SqlConnection connection)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         ValidateConnection(connection) ;
         return InsertEventFilterCondition(condition, connection, null) ;
      }

      private static bool InsertEventFilterCondition(EventCondition condition, SqlConnection connection, SqlTransaction trans)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         ValidateConnection(connection) ;
         using(SqlCommand command = new SqlCommand())
         {
            if(trans != null)
               command.Transaction = trans ;
            command.Connection = connection ;
            command.CommandText = String.Format(INSERT_EventFilterCondition, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventFilterConditionsTable, condition.RuleId, condition.FieldId, 
               SQLHelpers.CreateSafeString(condition.MatchString)) ;
            object o = command.ExecuteScalar() ;
            if(o is Decimal)
            {
               condition.Id = Decimal.ToInt32((Decimal)o) ;
               return true ;
            }
         }
         return false ;
      }


      public static int DeleteEventFilterCondition(EventCondition condition, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         if(condition == null)
            throw new ArgumentNullException("condition") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteEventFilterCondition(condition, connection) ;
         }
      }

      public static int DeleteEventFilterCondition(EventCondition condition, SqlConnection connection)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         ValidateConnection(connection) ;
         return DeleteEventFilterCondition(condition, connection, null) ;
      }

      private static int DeleteEventFilterCondition(EventCondition condition, SqlConnection connection, SqlTransaction trans)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         ValidateConnection(connection) ;
         using(SqlCommand command = new SqlCommand())
         {
            if(trans != null)
               command.Transaction = trans ;
            command.Connection = connection ;
            command.CommandText = String.Format(DELETE_EventFilterCondition, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventFilterConditionsTable, condition.Id) ;

            return command.ExecuteNonQuery() ;
         }
      }

      public static int DeleteEventFilterConditions(EventFilter filter, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         if(filter == null)
            throw new ArgumentNullException("filter") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteEventFilterConditions(filter.Id, connection) ;
         }
      }

      public static int DeleteEventFilterConditions(EventFilter filter, SqlConnection connection)
      {
         if(filter == null)
            throw new ArgumentNullException("filter") ;
         ValidateConnection(connection) ;
         return DeleteEventFilterConditions(filter.Id, connection) ;
      }

      public static int DeleteEventFilterConditions(int filterId, string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return DeleteEventFilterConditions(filterId, connection) ;
         }
      }

      public static int DeleteEventFilterConditions(int filterId, SqlConnection connection)
      {
         ValidateConnection(connection) ;
         return DeleteEventFilterConditions(filterId, connection, null) ;
      }

      private static int DeleteEventFilterConditions(int filterId, SqlConnection connection, SqlTransaction trans)
      {
         ValidateConnection(connection) ;
         using(SqlCommand command = new SqlCommand())
         {
            if(trans != null)
               command.Transaction = trans ;
            command.Connection = connection ;
            command.CommandText = String.Format(DELETE_EventFilterConditions, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventFilterConditionsTable, filterId) ;

            return command.ExecuteNonQuery() ;
         }
      }

      public static int UpdateEventFilterCondition(EventCondition condition, string connectionString)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;

         using(SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open() ;
            return UpdateEventFilterCondition(condition, connection) ;
         }
      }

      public static int UpdateEventFilterCondition(EventCondition condition, SqlConnection connection)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         ValidateConnection(connection) ;
         return UpdateEventFilterCondition(condition, connection, null) ;
      }

      private static int UpdateEventFilterCondition(EventCondition condition, SqlConnection connection, SqlTransaction trans)
      {
         if(condition == null)
            throw new ArgumentNullException("condition") ;
         ValidateConnection(connection) ;

         using(SqlCommand command = new SqlCommand())
         {
            if(trans != null)
               command.Transaction = trans ;
            command.Connection = connection ;
            command.CommandText = String.Format(UPDATE_EventFilterCondition, CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryEventFilterConditionsTable, SQLHelpers.CreateSafeString(condition.MatchString), 
               condition.Id) ;
            return command.ExecuteNonQuery() ;
         }
      }

      #endregion // Conditions
   }
}
