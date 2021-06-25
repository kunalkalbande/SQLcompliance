using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using Microsoft.Win32;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.Event
{
   
	/// <summary>
	/// Summary description for Trace.
	/// </summary>
   public class Trace : IDisposable
	{
      #region Private Data Members

      TraceConfiguration      configuration;
      TraceInfo               info;
      string                  connStr;
      SqlConnection           traceConn;

      #endregion

      #region Constructors

		public Trace() : this( null, null )
		{
      }

      public Trace( 
         TraceConfiguration    config,
         string                connStr
         )
      {
         if( config == null )
            configuration = new TraceConfiguration();

         this.connStr = connStr;
         configuration = config;
         info = new TraceInfo();

         try
         {
            if( connStr != null )
               traceConn = new SqlConnection( connStr );
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( e );
            traceConn = null;
         }

      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         // TODO:  Add Trace.Dispose implementation
      }

      #endregion

      #region Public Methods
      //-----------------------------------------------------------------------
      /// <summary>
      /// Creates a trace, set events, columns and filters and then starts
      /// the trace.  Exceptions are thrown when errors occur.
      /// </summary>
      /// <returns>bool</returns>
      //-----------------------------------------------------------------------
      public bool
         Start()
      {
         return true;
      }

      //-----------------------------------------------------------------------
      /// <summary>
      /// Stops a trace without closing it.  SQL Server won't flush the
      /// event records in its buffer until the trace is closed.  Exceptions
      /// are thrown when errors occur.
      /// </summary>
      /// <returns>bool</returns> 
      public bool
         Stop()
      {
         return true;
      }

      //-----------------------------------------------------------------------
      /// <summary>
      /// Closes a trace so that the buffered events are flushed to the trace
      /// file.
      /// </summary>
      /// <returns></returns>
      //-----------------------------------------------------------------------
      public bool
         Close()
      {
         return true;
      }

      //-----------------------------------------------------------------------
      /// <summary>
      /// Get current trace information.
      /// </summary>
      /// <returns></returns>
      //-----------------------------------------------------------------------
      public TraceInfo
         GetTraceInfo()
      {
         TraceInfo info = new TraceInfo();


         return info;
      }
      #endregion
   }
}
