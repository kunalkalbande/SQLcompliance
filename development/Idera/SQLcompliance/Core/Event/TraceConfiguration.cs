using System;
using System.Collections;
using System.Diagnostics;

namespace Idera.SQLcompliance.Core.Event
{
	/// <summary>
	/// Summary description for TraceConfiguration.
	/// </summary>
   //-----------------------------------------------------------------------
   // This is the base class for holding and managing a trace configuration
   //-----------------------------------------------------------------------
   // TODO: Is it necessary to serialize this class?
   //-----------------------------------------------------------------------
   public class TraceConfiguration //: IDisposable
	{
      #region Private data members
      //-----------------------------------------------------------------------
      // Required data for setting a SQL Server event trace.
      //-----------------------------------------------------------------------
      ArrayList               events;
      bool []                 columns;
      ArrayList               filters;
      ArrayList               serverRoles;
      ArrayList               objects;
      ArrayList               databases;

      internal string []      privUsers;
      internal int    []      privEvents;
      internal bool           privSELECT;
      internal bool           privDML;

      string                  instanceAlias;
      TraceInfo               traceInfo;

      int                     version;
      int                     sequence;
      TraceLevel              level;
      TraceCategory           category;
      bool                    keepSQL;
      bool                    keepAdminSQL; //By Hemant

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
            ValidateTraceCategory( value );
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

      public bool KeepAdminSQL // By Hemant
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
		public TraceConfiguration()
         :this( null, null, null )
		{
		}

      
      public TraceConfiguration(
            TraceEventId[] traceEvents,
            TraceColumnId[] traceColumns,
            TraceFilter [] traceFilters 
         )
      {

         if( traceEvents == null )
         {
            events = new ArrayList();
         }
         else
         {
            events = new ArrayList( traceEvents.Length );
            for( int i = 0; i < traceEvents.Length; i++ )
               events.Add( traceEvents[i] );
         }

         columns = new bool[ EventHelper.TraceColumnCount];
         if( traceColumns != null )
         {
            for( int i = 0; i < traceColumns.Length; i++ )
               columns[ (int)traceColumns[i] ] = true;
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
      //  Add an event to trace
      //-----------------------------------------------------------------------
      public virtual void 
         AddEvent (
            TraceEventId id 
         )
      {
         EventHelper.ValidateEventId( id );
         if( !events.Contains( id ) )
            events.Add( id );
      }
      //-----------------------------------------------------------------------
      //  Remove an event to trace
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveEvent (
            TraceEventId id 
         )
      {
         events.Remove( id );
      }

      //-----------------------------------------------------------------------
      //  Add a data column to trace
      //-----------------------------------------------------------------------
      public virtual void 
         AddColumn (
            TraceColumnId colId 
         )
      {
         EventHelper.ValidateColumnId( colId );
         columns[ (int)colId ] = true;
      }

      //-----------------------------------------------------------------------
      //  Remove a data column from trace
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveColumn (
            TraceColumnId colId 
         )
      {
         EventHelper.ValidateColumnId( colId );
         columns[ (int)colId ] = false;
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
      public virtual TraceEventId []
         GetTraceEvents ()
      {
         return (TraceEventId [])events.ToArray( typeof(TraceEventId ));
      }

      //----------------------------------------------------------------------
      // GetTraceEventsAsIntArray
      //----------------------------------------------------------------------
     /// <summary>
      /// GetTraceEvents: Get trace events as an array of interger values
      /// </summary>
      /// <returns></returns>
      public virtual int []
         GetTraceEventsAsIntArray()
      {
         return (int [])events.ToArray( typeof(TraceEventId));
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
      // GetTraceColumns
      //----------------------------------------------------------------------
      /// <summary>
      /// GetTraceColumns
      /// </summary>
      /// <returns></returns>
      public virtual TraceColumnId []
         GetTraceColumns()
      {
         ArrayList cols = new ArrayList(EventHelper.TraceCommonColumns.Length);

         for( int i = 1; i < columns.Length; i++ )
         {
            if( columns[i] ) 
               cols.Add( (TraceColumnId)i );
         }

         return (TraceColumnId [])cols.ToArray( typeof(TraceColumnId));
      }

      //----------------------------------------------------------------------
      // GetTraceColumnsAsIntArray
      //----------------------------------------------------------------------
     public virtual int []
         GetTraceColumnsAsIntArray()
      {
         ArrayList cols = new ArrayList(EventHelper.TraceCommonColumns.Length);

         for( int i = 1; i < columns.Length; i++ )
         {
            if( columns[i] ) 
               cols.Add( i );
         }

         return (int [])cols.ToArray( typeof(int));
      }

     //----------------------------------------------------------------------
      // ValidateTraceCategory
      //----------------------------------------------------------------------
      protected void
         ValidateTraceCategory (
            TraceCategory category
         )
      {
          if (category > TraceCategory.DataChangeWithDetails ||
             category < TraceCategory.ServerTrace )
            throw new CoreException( String.Format (CoreConstants.Exception_Format_InvalidTraceCategory, (int)category ));
      }

      #endregion
/*
      #region IDisposable Members

      public virtual void Dispose()
      {
         if( events != null )
         {
            events.Clear();
            events = null;
         }

         if( filters != null )
         {
            filters.Clear();
            filters = null;
         }
      }

      #endregion*/
   }

}
