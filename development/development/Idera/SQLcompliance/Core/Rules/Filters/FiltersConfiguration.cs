using System;
using System.Collections;
using System.Collections.Generic;
using Idera.SQLcompliance.Core.Rules.Alerts;

namespace Idera.SQLcompliance.Core.Rules.Filters
{
	/// <summary>
	/// Summary description for FiltersConfiguration.
	/// </summary>
	public class FiltersConfiguration
	{
      #region Member Variables

      private string _hostInstance ;
      private string _connectionString ;

      private Hashtable _sqlServerFields ;

      private Dictionary<int, CMEventCategory> _sqlServerCategories ;

      private Hashtable _sqlServerEvents ;

      #endregion

      #region Properties

      public CMEventCategory[] SqlServerCategories
      {
         get 
         { 
            CMEventCategory[] retVal = new CMEventCategory[_sqlServerCategories.Values.Count] ;
            _sqlServerCategories.Values.CopyTo(retVal, 0) ;
            return retVal ;
         }
      }

      public Hashtable SqlServerEventMap
      {
         get { return _sqlServerEvents ; }
      }
      
      public string ConnectionString
      {
         get { return _connectionString ; }
      }

      public string HostInstance
      {
         get { return _hostInstance ; }
      }

      #endregion

      #region Construction/Destruction

      public FiltersConfiguration()
      {
         _sqlServerCategories = new Dictionary<int, CMEventCategory>();

         _sqlServerFields = new Hashtable() ;

         _sqlServerEvents = new Hashtable() ;
      }

      #endregion

      public void InitializeEventFields()
	  {
          BuildEventFields();
	  }

      public void Initialize(string hostInstance)
      {
         _hostInstance = hostInstance ;
         _connectionString = String.Format( "server={0};" +
            "database={1};" +
            "integrated security=SSPI;" +
            "Connect Timeout=30;" + 
            "Application Name='{2}';",
            hostInstance,
            CoreConstants.RepositoryDatabase,
            CoreConstants.ManagementConsoleName );

         InitializeEventFields();

         // Sql Server Event Types
         _sqlServerCategories = RulesDal.SelectEventCategories(_connectionString) ;
         // Gut Unsuppored filtering categories
         _sqlServerCategories.Remove(-1) ;  // Invalid
         _sqlServerCategories.Remove(0) ;   // Integrity Check
         _sqlServerCategories.Remove(8) ;   // Broker
         foreach(CMEventCategory category in _sqlServerCategories.Values)
         {
            foreach(CMEventType evType in category.EventTypes)
            {
               _sqlServerEvents.Add(evType.TypeId, evType) ;
            }
         }
      }

      private void BuildEventFields()
      {
         EventField field ;
         
         field = new EventField() ;
         field.Id = (int)AlertableEventFields.eventType ;
         field.DisplayName = "Type" ;
         field.ColumnName = "eventType" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Integer ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.eventCategory ;
         field.DisplayName = "Category" ;
         field.ColumnName = "eventCategory" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Integer ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.applicationName ;
         field.DisplayName = "Application Name" ;
         field.ColumnName = "applicationName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.loginName ;
         field.DisplayName = "Login Name" ;
         field.ColumnName = "loginName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.success ;
         field.DisplayName = "Access Check Passed" ;
         field.ColumnName = "success" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Bool ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.databaseName ;
         field.DisplayName = "Database" ;
         field.ColumnName = "databaseName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.objectName ;
         field.DisplayName = "Object Name" ;
         field.ColumnName = "objectName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.objectType ;
         field.DisplayName = "Object Type" ;
         field.ColumnName = "objectType" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Integer ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.privilegedUser ;
         field.DisplayName = "Privileged User" ;
         field.ColumnName = "privilegedUser" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.Bool ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField() ;
         field.Id = (int)AlertableEventFields.serverName ;
         field.DisplayName = "SQL Server" ;
         field.ColumnName = "serverName" ;
         field.RuleType = EventType.SqlServer ;
         field.DataFormat = MatchType.String ;
         _sqlServerFields[field.Id] = field ;

         field = new EventField();
         field.Id = (int)AlertableEventFields.hostName;
         field.DisplayName = "Hostname";
         field.ColumnName = "hostName";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;

         field = new EventField();
         field.Id = (int)AlertableEventFields.sessionLoginName;
         field.DisplayName = "Session Login";
         field.ColumnName = "sessionLoginName";
         field.RuleType = EventType.SqlServer;
         field.DataFormat = MatchType.String;
         _sqlServerFields[field.Id] = field;


         FiltersDal.SqlServerFilterFields = _sqlServerFields;
      }

      public CMEventType LookupEventType(int id, EventType alertType)
      {
         switch(alertType)
         {
            case EventType.SqlServer:
               return (CMEventType)_sqlServerEvents[id] ;
            default:
               return null; 
         }
      }

      public EventField LookupAlertableEventField(AlertableEventFields fieldId)
      {
         try
         {
            return (EventField)_sqlServerFields[(int)fieldId] ;
         }
         catch(Exception)
         {
            return null ;
         }
      }

      public CMEventCategory LookupCategory(int categoryId)
      {
         return _sqlServerCategories[categoryId] ;
      }
   }
}
