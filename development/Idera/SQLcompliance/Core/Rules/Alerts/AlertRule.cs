using System ;
using System.Collections ;
using System.Xml.Serialization;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	public enum AlertLevel
	{
		Low = 1,
		Medium,
		High,
		Severe
	} ;

	public enum ActionType
	{
		EventLog = 1,
		SMTP
	} ;


	/// <summary>
	/// Summary description for AlertRule.
	/// </summary>
	public class AlertRule : EventRule
	{
		#region Member Variables

		// Database columns
		private AlertLevel _level ;
      private bool _emailAction ;
      private bool _logAction ;
        private bool _snmpTrapAction;
	    private SNMPConfiguration _snmpConfiguration;
      private string _messageData ;

      private string _messageTitle ;
      private string _messageBody ;
      private string[] _recipients ;
      private ErrorLog.Severity _logEntryType ;
        private bool _alertRuleTimeFrameEnabled;
        private string _alertRuleTimeFrameStartTime;
        private string _alertRuleTimeFrameEndTime;
        private string _alertRuleTimeFrameDaysOfWeek;
        private bool _emailSummaryAction;
        private int _summaryEmailFrequency;
        private string _lastEmailSummarySendTime;


        #endregion

        #region Properties

        [XmlAttribute("AlertLevel" )]
		public AlertLevel Level
		{
			get { return _level ; }
			set 
			{ 
				_isDirty = true ;
				_level = value ; 
			}
		}

      [XmlAttribute("HasEmailAction")]
      public bool HasEmailAction
	   {
	      get { return _emailAction ; }
	      set 
         { 
            _isDirty = true ;
            _emailAction = value ; 
         }
	   }

      [XmlAttribute("HasLogAction")]
      public bool HasLogAction
	   {
	      get { return _logAction ; }
	      set 
         { 
            _isDirty = true ;
            _logAction = value ; 
         }
	   }

        [XmlAttribute("HasSnmpTrapAction")]
        public bool HasSnmpTrapAction
        {
            get { return _snmpTrapAction; }
            set
            {
                _isDirty = true;
                _snmpTrapAction = value;
            }
        }

        [XmlElement("SnmpConfiguration")]
	    public SNMPConfiguration SnmpConfiguration
	    {
	        get { return _snmpConfiguration; }
            set { _snmpConfiguration = value; }
	    }

      [XmlElement("MessageData")]
      public string MessageData
	   {
	      get { return _messageData ; }
	      set 
         { 
            _messageData = value ; 
            ParseMessageData() ;
         }
	   }

      [XmlElement("MessageTitle")]
      public string MessageTitle
	   {
	      get { return _messageTitle ; }
	      set 
         { 
            _messageTitle = value ; 
            UpdateMessageData() ;
         }
	   }

      [XmlElement("MessageBody")]
      public string MessageBody
	   {
	      get { return _messageBody ; }
	      set 
         { 
            _messageBody = value ; 
            UpdateMessageData() ;
         }
	   }

      [XmlElement("Recipient")]
      public string[] Recipients
	   {
	      get { return _recipients ; }
	      set 
         { 
            _recipients = value ; 
            UpdateMessageData() ;
         }
	   }

      [XmlElement("Severity")]
      public ErrorLog.Severity LogEntryType
	   {
	      get { return _logEntryType ; }
	      set 
         { 
            _logEntryType = value ; 
            UpdateMessageData() ;
         }
	   }

      [XmlIgnore]
      public string RecipientList
      {
         get
         {
            try
            {
               return String.Join( ",",
                                   _recipients );
            }
            catch
            {
               return "";
            }
         }

         set
         {
            _isDirty = true ;
            if(value != null && value.Trim().Length > 0)
               _recipients = value.Split(new char[] {','}) ;
            else
               _recipients = new string[0] ;
            UpdateMessageData() ;
         }
      }

        [XmlElement("IsAlertRuleTimeFrameEnabled")]
        public bool IsAlertRuleTimeFrameEnabled
        {
            get { return _alertRuleTimeFrameEnabled; }
            set { _alertRuleTimeFrameEnabled = value; }
        }

        [XmlElement("AlertRuleTimeFrameStartTime")]
        public string AlertRuleTimeFrameStartTime
        {
            get { return _alertRuleTimeFrameStartTime; }
            set { _alertRuleTimeFrameStartTime = value; }
        }

        [XmlElement("AlertRuleTimeFrameEndTime")]
        public string AlertRuleTimeFrameEndTime
        {
            get { return _alertRuleTimeFrameEndTime; }
            set { _alertRuleTimeFrameEndTime = value; }
        }

        [XmlElement("AlertRuleTimeFrameDaysOfWeek")]
        public string AlertRuleTimeFrameDaysOfWeek
        {
            get { return _alertRuleTimeFrameDaysOfWeek; }
            set { _alertRuleTimeFrameDaysOfWeek = value; }
        }

        [XmlAttribute("HasEmailSummaryAction")]
        public bool HasEmailSummaryAction
        {
            get { return _emailSummaryAction; }
            set { _emailSummaryAction = value; }
        }

        [XmlAttribute("SummaryEmailFrequency")]
        public int SummaryEmailFrequency
        {
            get { return _summaryEmailFrequency; }
            set { _summaryEmailFrequency = value; }
        }

        [XmlAttribute("LastEmailSummarySendTime")]
        public string LastEmailSummarySendTime
        {
            get { return _lastEmailSummarySendTime; }
            set { _lastEmailSummarySendTime = value; }
        }
        #endregion

        #region Construction/Destruction

        public AlertRule() :base()
		{
			_level = AlertLevel.Medium ;
         _recipients = new string[0] ;
         _messageBody = AlertingConfiguration.AlertMessageBody ;
         _messageTitle = AlertingConfiguration.AlertMessageTitle ;
         UpdateMessageData() ;
         _isDirty = false ;
      }

		public new AlertRule Clone()
		{
			AlertRule retVal = (AlertRule)base.Clone() ;

         if(_recipients != null)
         {
            retVal._recipients = new string[_recipients.Length] ;
            for(int i = 0 ; i < _recipients.Length ; i++)
               retVal._recipients[i] = _recipients[i] ;
         }else
            retVal._recipients = new string[0] ;

			return retVal ;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

         if (this.GetType() != obj.GetType()) return false;

         if(!base.Equals(obj)) return false ;

			// safe because of the GetType check
			AlertRule rule = (AlertRule)obj;

			// use this pattern to compare value members
			if(!_level.Equals(rule._level)) return false ;
         if(!_emailAction.Equals(rule._emailAction)) return false ;
         if(!_logAction.Equals(rule._logAction)) return false ;


			// use this pattern to compare reference members
         if(_emailAction)
         {
            if(!Object.Equals(_messageData, rule._messageData)) return false ;

            if(!_recipients.Length.Equals(rule._recipients.Length)) return false ;
            ArrayList addresses = new ArrayList(_recipients) ;
            foreach(string s in rule._recipients)
            {
               if(addresses.Contains(s))
                  addresses.Remove(s) ;
               else
                  return false ;
            }
         }
         if(_emailAction)
         {
            if(!_logEntryType.Equals(rule._logEntryType)) return false ;
            if(!Object.Equals(_messageData, rule._messageData)) return false ;
         }
			return true;
		}

      public override int GetHashCode()
      {
         return base.GetHashCode() ;
      }

      private void UpdateMessageData()
      {
         Hashtable map = new Hashtable() ;

         _isDirty = true ;
         map["title"] = _messageTitle ;
         map["body"] = _messageBody ;
         map["severity"] = ((int)_logEntryType).ToString() ;
         map["recipients"] = RecipientList ;
         _messageData = KeyValueParser.BuildString(map) ;
      }

      private void ParseMessageData()
      {
         try
         {
            Hashtable map = KeyValueParser.ParseString(_messageData) ;
            _messageTitle = (string)map["title"] ;
            _messageBody = (string)map["body"] ;
            _logEntryType = (ErrorLog.Severity)Int32.Parse((string)map["severity"]) ;
            string temp = (string)map["recipients"] ;
            if(temp != null && temp.Trim().Length > 0)
               _recipients = ((string)map["recipients"]).Split(new char[] {','}) ;
            else
               _recipients = new string[0] ;
         }
         catch
         {
         }
      }

	}
}
