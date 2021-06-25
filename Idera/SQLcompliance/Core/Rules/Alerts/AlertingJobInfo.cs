using System;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for AlertingJobInfo.
	/// </summary>
	public class AlertingJobInfo
	{
		#region Member Variables

	   private int _highWatermark ;
      private int _alertHighWatermark ;
      private string _targetInstance ;
      private string _serverDbName ;
      private RuleProcessor _processor ;
      private DataRuleProcessor _dataProcessor;
      private AlertingConfiguration _configuration ;
      private JobCompleteCallback _callback ;
      private bool _doBADAlertProcessing = false;
      private int _eventCountForBAD = 0;

		#endregion

		#region Properties

	   public int AlertHighWatermark
	   {
	      get { return _alertHighWatermark ; }
	      set { _alertHighWatermark = value ; }
	   }

	   public int HighWatermark
      {
         get { return _highWatermark ; }
         set { _highWatermark = value ; }
      }

      public RuleProcessor Processor
      {
         get { return _processor ; }
         set { _processor = value ; }
      }

      public DataRuleProcessor DataProcessor
      {
         get { return _dataProcessor; }
         set { _dataProcessor = value; }
      }

	   public AlertingConfiguration Configuration
	   {
	      get { return _configuration ; }
	      set { _configuration = value ; }
	   }

	   public string TargetInstance
	   {
	      get { return _targetInstance ; }
	      set { _targetInstance = value ; }
	   }

	   public string ServerDbName
	   {
	      get { return _serverDbName ; }
	      set { _serverDbName = value ; }
	   }

	   public string ConnectionString
	   {
	      get 
         {
            if(_configuration != null)
               return _configuration.ConnectionString ;
            else
               return null ;
         }
	   }

      public JobCompleteCallback JobCompleteHandler
      {
         get { return _callback ; }
         set { _callback = value ; }
      }

      public bool DoBADAlertProcessing
      {
          get { return _doBADAlertProcessing; }
          set { _doBADAlertProcessing = value; }
      }
      public int EventCountForBAD
      {
          get { return _eventCountForBAD; }
          set { _eventCountForBAD = value; }
      }


	   #endregion

		#region Construction/Destruction

		public AlertingJobInfo()
		{
		}

		#endregion
	}
}
