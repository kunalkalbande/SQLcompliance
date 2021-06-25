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
   public class XeTraceConfiguration //: IDisposable
	{
      #region Private data members
      //-----------------------------------------------------------------------
      // Required data for setting a SQL Server event trace.
      //-----------------------------------------------------------------------
      ArrayList               events;
      ArrayList               columns;
      ArrayList               filters;
      ArrayList               serverRoles;
      ArrayList               objects;
      ArrayList               databases;

      internal string []      privUsers;
      internal int    []      privEvents;
      internal bool           privSELECT;
      internal bool           privDML;
      internal bool           privSELECTXE;   
      string                  instanceAlias;
      TraceInfo               traceInfo;

      int                     version;
      int                     sequence;
      TraceLevel              level;
      TraceCategory           category;
      bool                    keepSQLXE;
      string                  xeSessionName;

      
      //bool                    keepAdminSQL; 
      

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

      public string XESessionName
      {
          get { return xeSessionName; }
          set { xeSessionName = value; }
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

      public bool KeepSQLXE
      {
         get { return keepSQLXE; }
         set { keepSQLXE = value; }
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
		public XeTraceConfiguration()
         :this( null, null, null )
		{
		}

      
      public XeTraceConfiguration(
            string[] traceEventsxe,
            string[] traceColumnsxe,
            TraceFilter [] traceFilters 
         )
      {

          if (traceEventsxe == null)
         {
            events = new ArrayList();
         }
         else
         {
             events = new ArrayList(traceEventsxe.Length);
             for (int i = 0; i < traceEventsxe.Length; i++)
                 events.Add(traceEventsxe[i]);
         }

          if (traceEventsxe == null)
          {
              columns = new ArrayList();
          }
          
          else
         {
             columns = new ArrayList(traceColumnsxe.Length);
             for (int i = 0; i < traceColumnsxe.Length; i++)
                 columns.Add(traceColumnsxe[i]);
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
      //  5.4 XE
      //-----------------------------------------------------------------------
      public virtual void 
         AddEvent (
            string id 
         )
      {
         if( !events.Contains( id ) )
            events.Add( id );
      }
      //-----------------------------------------------------------------------
      //  Remove an event to trace
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveEvent (
            string id 
         )
      {
         events.Remove( id );
      }

      //-----------------------------------------------------------------------
      //  Add a data column to trace
      //-----------------------------------------------------------------------
      public virtual void 
         AddColumn (
            string colId 
         )
      {
         //EventHelper.ValidateColumnId( colId );
          columns.Add(colId);
      }

      //-----------------------------------------------------------------------
      //  Remove a data column from trace
      //-----------------------------------------------------------------------
      public virtual void 
         RemoveColumn (
            string colId 
         )
      {
         //EventHelper.ValidateColumnId( colId );
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
      public virtual string []
         GetTraceEventsXE ()
      {
          return (string[])events.ToArray(typeof(string));
      }

      //----------------------------------------------------------------------
      // GetTraceEventsAsIntArray
      //----------------------------------------------------------------------
     /// <summary>
      /// GetTraceEvents: Get trace events as an array of interger values
      /// </summary>
      /// <returns></returns>
      //public virtual int []
      //   GetTraceEventsAsIntArray()
      //{
      //   return (int [])events.ToArray( typeof(TraceEventId));
      //}

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
      public virtual string []
         GetTraceColumnsXE()
      {
              return (string [])columns.ToArray( typeof(string));
      }

      //----------------------------------------------------------------------
      // GetTraceColumnsAsIntArray
      //----------------------------------------------------------------------
     //public virtual int []
     //    GetTraceColumnsAsIntArray()
     // {
     //    ArrayList cols = new ArrayList(EventHelper.TraceCommonColumns.Length);

     //    for( int i = 1; i < columns.Length; i++ )
     //    {
     //       if( columns[i] ) 
     //          cols.Add( i );
     //    }

     //    return (int [])cols.ToArray( typeof(int));
     // }

     //----------------------------------------------------------------------
      // ValidateTraceCategory
      //----------------------------------------------------------------------
      protected void
         ValidateTraceCategory (
            TraceCategory category
         )
      {
          if (category > TraceCategory.DBPrivilegedUsers ||
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
