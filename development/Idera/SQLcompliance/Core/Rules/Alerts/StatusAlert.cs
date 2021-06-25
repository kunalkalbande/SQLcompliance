using System;
using System.Collections ;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for Alert.
	/// </summary>
	public class StatusAlert
	{
		#region Member Variables

		// Database columns
		private int _alertId ;
      private EventType _type;
		private int _ruleId ;
		private string _instance ;
      private string _computerName;
		private StatusRuleType _eventType ;
		private DateTime _created ;
		private AlertLevel _level ;
      private NotificationStatus _emailStatus ;
      private NotificationStatus _logStatus ;
	    private NotificationStatus _snmpTrapStatus;
      private string _messageData ;
      private string _ruleName ;
      private string _statusRuleTypeName;

		// Support/Convenience fields
		private StatusAlertRule _parentRule ;
		private object _statusData ;
      private string _messageTitle ;
      private string _messageBody ;
      private ErrorLog.Severity _eventLogSeverity ;
      private string[] _recipients ;
      private StatusAlertThreshold _threshold;

		#endregion

		#region Properties

		public int Id
		{
			get { return _alertId ; }
			set { _alertId = value ; }
		}

      public EventType AlertType
		{
			get { return _type ; }
			set { _type = value ; }
		}

      public StatusAlertThreshold Threshold
      {
         get { return _threshold; }
         set { _threshold = value; }
      }

      public string StatusRuleTypeName
      {
         get { return _statusRuleTypeName; }
         set { _statusRuleTypeName = value; }
      }

		public StatusAlertRule ParentRule
		{
			get { return _parentRule ; }
			set { _parentRule = value ; }
		}

		public string Instance
		{
			get { return _instance ; }
			set { _instance = value ; }
		}

      public string ComputerName
      {
         get { return _computerName; }
         set { _computerName = value; }
      }

		public DateTime Created
		{
			get { return _created ; }
			set { _created = value ; }
		}

		public AlertLevel Level
		{
			get { return _level ; }
			set { _level = value ; }
		}

		public int ParentRuleId
		{
			get { return _ruleId ; }
			set { _ruleId = value ; }
		}

		public StatusRuleType EventType
		{
			get { return _eventType ; }
			set { _eventType = value ; }
		}

		public object StatusData
		{
         get { return _statusData; }
         set { _statusData = value; }
		}

	   public NotificationStatus EmailStatus
	   {
	      get { return _emailStatus ; }
	      set { _emailStatus = value ; }
	   }

	   public NotificationStatus LogStatus
	   {
	      get { return _logStatus ; }
	      set { _logStatus = value ; }
	   }

	    public NotificationStatus SnmpTrapStatus
	    {
            get { return _snmpTrapStatus; }
            set { _snmpTrapStatus = value; }
	    }

	   public string MessageData
	   {
	      get { return _messageData ; }
	      set 
         { 
            _messageData = value ; 
            ParseMessageData() ;
         }
	   }

	   public string RuleName
	   {
	      get { return _ruleName ; }
	      set { _ruleName = value ; }
	   }

	   public string MessageTitle
	   {
         set
         {
            _messageTitle = value ; 
            UpdateMessageData() ;
         }
	      get { return _messageTitle ; }
	   }

	   public string MessageBody
	   {
         set 
         {
            _messageBody = value ; 
            UpdateMessageData() ;
         }
	      get { return _messageBody ; }
	   }
      
      public ErrorLog.Severity EventLogSeverity
      {
         get { return _eventLogSeverity ; }
      }

      public string[] Recipients
      {
         get { return _recipients ; }
      }

	   #endregion

		#region Construction/Destruction

      public StatusAlert()
		{
         _created = DateTime.Now ;
			_statusData = null ;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
         StatusAlert alert = (StatusAlert)obj;     

			// use this pattern to compare value members
			if(!_alertId.Equals(alert._alertId)) return false ;
			if(!_type.Equals(alert._type)) return false ;
			if(!_ruleId.Equals(alert._ruleId)) return false ;
			if(!_created.Equals(alert._created)) return false ;
			if(!_level.Equals(alert._level)) return false ;

         if(!_emailStatus.Equals(alert._emailStatus)) return false ;
         if(!_logStatus.Equals(alert._logStatus)) return false ;
         
         if(!Object.Equals(_messageData, alert._messageData)) return false ;
         if(!Object.Equals(_ruleName, alert._ruleName)) return false ;
			return true;
		}

      private void UpdateMessageData()
      {
         Hashtable map = new Hashtable() ;

         map["title"] = _messageTitle ;
         map["body"] = _messageBody ;
         map["severity"] = ((int)_eventLogSeverity).ToString() ;
         map["recipients"] = String.Join(",", _recipients) ;
         _messageData = KeyValueParser.BuildString(map) ;
      }

      private void ParseMessageData()
      {
         try
         {
            Hashtable map = KeyValueParser.ParseString(_messageData) ;
            _messageTitle = (string)map["title"] ;
            _messageBody = (string)map["body"] ;
            string temp = (string)map["recipients"] ;

            if (temp != null && temp.Trim().Length > 0)
            {
               _recipients = temp.Split(new char[] { ',' });
            }
            else
            {
               _recipients = new string[0];
            }
            _eventLogSeverity = (ErrorLog.Severity)Int32.Parse((string)map["severity"]) ;
         }
         catch
         {
         }
      }

      public override int GetHashCode()
      {
         return _alertId ;
      }

	}

   public class StatusAlertLevelDescending : IComparer<StatusAlert> 
   {
      public int Compare(StatusAlert alert1, StatusAlert alert2)
      {
         return alert2.Level.CompareTo(alert1.Level);
      }
   }
}
