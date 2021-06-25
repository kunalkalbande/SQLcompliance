using System;
using System.Text;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Summary description for InternalAlert.
	/// </summary>
	internal class InternalAlert
	{
      #region Constructor

		public InternalAlert()
		{
		}

      #endregion
      
      //-----------------------------------------------------------------------      
      // Raise - variant
      //-----------------------------------------------------------------------      
      public static void
         Raise(
            string   alert
         )
      {
         Raise( null, alert, null );
      }
      
      //-----------------------------------------------------------------------      
      // Raise - variant
      //-----------------------------------------------------------------------      
      public static void
         Raise(
            string   server,
            string   alert
         )
      {
         Raise( server, alert, null );
      }

      //-----------------------------------------------------------------------      
      // Raise - real thing
      //-----------------------------------------------------------------------      
      public static void
         Raise(
            string   server,
            string   alert,
            string   extraInfo
         )
      {
         StringBuilder sb = new StringBuilder( "SQL Compliance Manager Alert: ");
         
         if ( server  !=null || server   !="" ) sb.AppendFormat( "Server: {0}\n",   server );
         sb.AppendFormat( "{0}", alert );
         if ( extraInfo!=null || extraInfo !="" ) sb.AppendFormat( "\n{0} ", extraInfo );
         
         // for now, just log alerts to event log always
         ErrorLog.Instance.Write( ErrorLog.Level.Always,
                                  sb.ToString(),
                                  ErrorLog.Severity.Error );

         // TODO: Do something when an alert is raised - write to internal tables etc
      }
	}
}
