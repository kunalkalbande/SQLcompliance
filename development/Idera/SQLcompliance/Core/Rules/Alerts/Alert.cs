using System;
using System.Collections ;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
   public enum NotificationStatus
   {
      None = 0,
      Pending,
      Failed,
      Complete
   } ;

	/// <summary>
	/// Summary description for Alert.
	/// </summary>
	public class Alert
	{
		#region Member Variables

		// Database columns
		private int _alertId ;
		private EventType _type ;
		private int _ruleId ;
		private int _eventId ;
		private string _instance ;
		private int _eventType ;
		private DateTime _created ;
		private AlertLevel _level ;
      private NotificationStatus _emailStatus ;
      private NotificationStatus _logStatus ;
        private NotificationStatus _snmpTrapStatus;
      private string _messageData ;
      private string _ruleName ;

		// Support/Convenience fields
		private AlertRule _parentRule ;
		//private ActionResultCollection _actionResults ;
		private object _eventData ;
      private string _messageTitle ;
      private string _messageBody ;
      private ErrorLog.Severity _eventLogSeverity ;
      private string[] _recipients ;
	    

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

		public AlertRule ParentRule
		{
			get { return _parentRule ; }
			set { _parentRule = value ; }
		}

		public string Instance
		{
			get { return _instance ; }
			set { _instance = value ; }
		}

		public int EventId
		{
			get { return _eventId ; }
			set { _eventId = value ; }
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

      /*
		public ActionResult[] ActionResults
		{
			get { return _actionResults.ToArray() ; }
		}*/

		public int EventType
		{
			get { return _eventType ; }
			set { _eventType = value ; }
		}

		public object EventData
		{
			get { return _eventData ; }
			set { _eventData = value ; }
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

		public Alert()
		{
			//_actionResults = new ActionResultCollection() ;
         _created = DateTime.Now ;
			_eventData = null ;
		}

		#endregion

      /*
		public void AddActionResult(ActionResult result)
		{
			result.ParentAlert = this ;
			_actionResults.Add(result) ;
		}*/

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
			Alert alert = (Alert)obj;     

			// use this pattern to compare value members
			if(!_alertId.Equals(alert._alertId)) return false ;
			if(!_type.Equals(alert._type)) return false ;
			if(!_ruleId.Equals(alert._ruleId)) return false ;
			if(!_eventId.Equals(alert._eventId)) return false ;
			if(!_created.Equals(alert._created)) return false ;
			if(!_level.Equals(alert._level)) return false ;

         if(!_emailStatus.Equals(alert._emailStatus)) return false ;
         if(!_logStatus.Equals(alert._logStatus)) return false ;
		    if (!_snmpTrapStatus.Equals(alert._snmpTrapStatus)) return false;
         
         if(!Object.Equals(_messageData, alert._messageData)) return false ;
         if(!Object.Equals(_ruleName, alert._ruleName)) return false ;

			// Action Results
         /*
			if(!_actionResults.Count.Equals(alert._actionResults.Count)) return false ;
			ActionResultCollection tempList = (ActionResultCollection)alert._actionResults.Clone() ;
			foreach(ActionResult result in _actionResults)
			{
				ActionResult matchedResult = null ;
				foreach(ActionResult result2 in tempList)
				{
					if(Object.Equals(result, result2))
					{
						matchedResult = result2 ;
						break ;
					}
				}
				if(matchedResult == null)
					return false ;
				else
					tempList.Remove(matchedResult) ;
			}*/


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
            if(temp != null && temp.Trim().Length > 0)
               _recipients = temp.Split(new char[] {','}) ;
            else
               _recipients = new string[0] ;
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

   public class AlertLevelDescending : IComparer<Alert> 
   {
      public int Compare(Alert alert1, Alert alert2)
      {
         return alert2.Level.CompareTo(alert1.Level);
      }
   }
}
