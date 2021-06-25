using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Idera.SQLcompliance.Core.Event
{
	/// <summary>
	/// Summary description for AuditLogsConfiguration.
	/// </summary>
   //-----------------------------------------------------------------------
   // This is the base class for holding and managing a audit logs configuration
   //-----------------------------------------------------------------------
   // TODO: Is it necessary to serialize this class?
   //-----------------------------------------------------------------------
   public class AuditLogConfiguration //: IDisposable
	{
      #region Private data members
      //-----------------------------------------------------------------------
      // Required data for setting a SQL Server event trace.
      //-----------------------------------------------------------------------
      ArrayList               events;
      ArrayList                 columns;
      ArrayList               filters;
      ArrayList               serverRoles;
      ArrayList               objects;
      ArrayList               databases;

      internal string []      privUsers;

      string                  instanceAlias;
      TraceInfo               traceInfo;

      int                     version;
      int                     sequence;
      TraceLevel              level;
      TraceCategory           category;
      bool                    keepSQL;
      bool                    keepAdminSQL;

      #endregion

      #region Properties
      public string InstanceAlias
      {
         get{ return instanceAlias; }
         set{ instanceAlias = value; }
      }

      public int Version
      {
         get { return version; }
         set { version = value; }
      }

      public virtual TraceLevel Level
      {
         get { return level; }
         set { level = value; }
      }

      public virtual TraceCategory Category
      {
         get { return category; }
         set 
         { 
            ValidateAuditLogCategory( value );
            category = value; 
         }
      }

      public int Sequence
      {
         get { return sequence; }
         set { sequence = value; }
      }

         public DateTime StopTime
      {
         get { return traceInfo.StopTime; }
         set { traceInfo.StopTime = value; }
      }

      public long MaxFileSize
      {
         get { return traceInfo.MaxSize; }
         set { traceInfo.MaxSize = value; }
      }

      protected string fileNamePrefix;
      public string FileNamePrefix
      {
         get { return fileNamePrefix; }
         set { fileNamePrefix = value; }
      }

      public string FileName
      {
         get { return traceInfo.FileName; }
         set { traceInfo.FileName = value; }
      }

      public string TraceDirectory
      {
          get { return Path.GetDirectoryName(traceInfo.FileName); }
      }

      public string SessionName
      {
          get { return Path.GetFileName(traceInfo.FileName); }
      }

      public TraceOption Options
      {
         get { return traceInfo.Options; }
         set { traceInfo.Options = value; }
      }

      public int [] ServerRoles
      {
         get { return (int [])serverRoles.ToArray( typeof(int)); }
         set 
         { 
            serverRoles.Clear();
            if( value != null )
               for( int i = 0; i < value.Length; i++ )
                  serverRoles.Add( value[i] );
         }
      }

      public string [] Databases
      {
         get { return (string [])databases.ToArray( typeof(string)); }
         set
         {
            databases.Clear();
            if( value != null )
            {
               for( int i = 0; i < value.Length; i++ )
                  databases.Add( value[i] );
            }
         }

      }

      public bool KeepSQL
      {
         get { return keepSQL; }
         set { keepSQL = value; }
      }

      public bool KeepAdminSQL 
      {
          get { return keepAdminSQL; }
          set { keepAdminSQL = value; }
      }
      
      public int PrivUserCount
      {
         get
         {
            return privUsers == null ? 0 : privUsers.Length;
         }
      }

      #endregion

      #region Constructors
		public AuditLogConfiguration()
         :this( null, null, null )
		{
		}


        public AuditLogConfiguration(
            AuditLogEventID[] auditLogEvents,
            AuditLogColumnId[] auditColumns,
            TraceFilter [] traceFilters 
         )
      {

         if( auditLogEvents == null )
         {
            events = new ArrayList();
         }
         else
         {
            events = new ArrayList( auditLogEvents.Length );
            for( int i = 0; i < auditLogEvents.Length; i++ )
               events.Add( auditLogEvents[i] );
         }

         columns = new ArrayList();
         if( auditColumns != null )
         {
             columns = new ArrayList(auditColumns.Length);
             for (int i = 0; i < auditColumns.Length; i++)
                 columns.Add(auditColumns[i]);
         }

         if( traceFilters == null )
         {
            filters = new ArrayList();
         }
         else
         {
            filters = new ArrayList( traceFilters.Length );
            for( int i = 0; i < traceFilters.Length; i++ )
               filters.Add( traceFilters[i] );
         }

         traceInfo = new TraceInfo();
         traceInfo.MaxSize = CoreConstants.Agent_Default_MaxTraceSize;
         traceInfo.Options = (TraceOption)CoreConstants.Agent_Default_TraceOptions;
         traceInfo.FileName = instanceAlias;

         serverRoles = new ArrayList();
         objects = new ArrayList();
         databases = new ArrayList();

      }
      #endregion

      #region Public Methods
      //-----------------------------------------------------------------------
      //  Implementation note: It is necessary to have separate methods for
      //     adding and removing columns and events.  Their base types are
      //     both integers.
      //-----------------------------------------------------------------------
      //  Add an event to AuditLog
      //-----------------------------------------------------------------------
      public virtual void 
         AddEvent (
            AuditLogEventID id 
         )
      {
         if( !events.Contains( id ) )
            events.Add( id );
      }
      //-----------------------------------------------------------------------
      //  Remove an event from AuditLog
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveEvent (
            AuditLogEventID id 
         )
      {
         events.Remove( id );
      }

      //-----------------------------------------------------------------------
      //  Add a data column to trace
      //-----------------------------------------------------------------------
      public virtual void 
         AddColumn (
            AuditLogColumnId colId 
         )
      {
          if (!columns.Contains(colId))
            columns.Add(colId);
      }

      //-----------------------------------------------------------------------
      //  Remove a data column from trace
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveColumn (
            AuditLogColumnId colId 
         )
      {
          if (!columns.Contains(colId))
              columns.Remove(colId);
      }

      //-----------------------------------------------------------------------
      //  Add a trace filter
      //-----------------------------------------------------------------------
      public virtual void 
         AddFilter (
            TraceFilter filter 
         )
      {
         if( !filters.Contains( filter ) )
            filters.Add( filter );
      }

      //-----------------------------------------------------------------------
      //  Remove a trace filter
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveFilter (
            TraceFilter filter 
         )
      {
         filters.Remove( filter );
      }


      /// <summary>
      /// AddServerRole: add a server role to audit
      /// </summary>
      /// <param name="serverRole"></param>
      public virtual void
         AddServerRole(
         int serverRole
         )
      {
         // check for duplicate
         if( !serverRoles.Contains( serverRole ) )
            serverRoles.Add( serverRole );
      }

      /// <summary>
      /// RemoveServerRole: Remove a server role from auditing
      /// </summary>
      /// <param name="serverRole"></param>
      public virtual void
         RemoveServerRole(
         int serverRole
         )
      {
         serverRoles.Remove( serverRole );
      }

      //----------------------------------------------------------------------
      // GetTraceEvents
      //----------------------------------------------------------------------
      /// <summary>
      /// GetTraceEvents
      /// </summary>
      /// <returns></returns>
      public virtual AuditLogEventID []
         GetAuditLogEvents ()
      {
         return (AuditLogEventID [])events.ToArray( typeof(AuditLogEventID ));
      }

      //----------------------------------------------------------------------
      // GetTraceEventsAsIntArray
      //----------------------------------------------------------------------
     /// <summary>
      /// GetTraceEvents: Get trace events as an array of interger values
      /// </summary>
      /// <returns></returns>
      public virtual int []
         GetAuditLogEventsAsIntArray()
      {
         return (int [])events.ToArray( typeof(AuditLogEventID));
      }

      //----------------------------------------------------------------------
      // GetTraceFilters
      //----------------------------------------------------------------------
     /// <summary>
      /// GetTraceFilters
      /// </summary>
      /// <returns></returns>
      public virtual TraceFilter []
         GetTraceFilters ()
      {

         return (TraceFilter [])filters.ToArray( typeof( TraceFilter));
      }

     //----------------------------------------------------------------------
      // GetAuditLogColumns
      //----------------------------------------------------------------------
      /// <summary>
      /// GetAuditLogColumns
      /// </summary>
      /// <returns></returns>
      public virtual string []
         GetAuditLogColumns()
      {
          return (string[])columns.ToArray(typeof(string));
      }
             

     //----------------------------------------------------------------------
      // ValidateTraceCategory
      //----------------------------------------------------------------------
      protected void
         ValidateAuditLogCategory (
            TraceCategory category
         )
      {
          if (category > TraceCategory.DataChangeWithDetails ||
             category < TraceCategory.ServerTrace )
            throw new CoreException( String.Format (CoreConstants.Exception_Format_InvalidTraceCategory, (int)category ));
      }
      #endregion
   }

}
