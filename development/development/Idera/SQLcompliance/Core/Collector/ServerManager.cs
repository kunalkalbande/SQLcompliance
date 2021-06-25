using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Core.Collector
{
	/// <summary>
	/// Summary description for ServerManager.
	/// </summary>
	public class ServerManager : MarshalByRefObject
	{
      #region Constructors and Properties

      //------------------------------------------------------------		
      // Constructor
      //------------------------------------------------------------		
      /// <summary>
      /// 
      /// </summary>
      public ServerManager() 
		{
		}

      #endregion

      //------------------------------------------------------------		
      // Ping - Used to check if collector service is running
      //------------------------------------------------------------		
      public bool
         Ping()
      {
         return true;
      }

      //------------------------------------------------------------		
      // StopTraceJobs - Kills all trace activity for an instance
      //                 in preparation for an uninstall
      //------------------------------------------------------------		
      public void
         StopTraceJobs( 
            string            instanceName
         )
      {
         CollectionServer.Instance.JobPool.StopInstanceJobs( instanceName );
      }

      public void UpdateAlertRules()
      {
         CollectionServer.Instance.LoadAlertRules() ;
      }

      public void UpdateEventFilters()
      {
         CollectionServer.Instance.LoadEventFilters() ;
      }

      public void UpdateAlertingConfiguration()
      {
         CollectionServer.Instance.LoadAlertingConfiguration() ;
      }

      public void SetReindexFlag(bool reindex)
      {
         CollectionServer.Instance.SetReindexFlag(reindex);
      }

      public void SetCollectionServerLogLevel(int level)
      {
         CollectionServer.Instance.SetLogLevel(level);
      }
	}
}
