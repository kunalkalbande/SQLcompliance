using System ;
using System.Text;
using System.Collections ;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
    public enum DataRuleType
    {
        SensitiveColumn = 1,
        ColumnValueChanged,
        ColumnValueChangedBad,
        SensitiveColumnViaDataset       //SQLCM -5470 v5.6
    };

   // <summary>
	/// Summary description for AlertRule.
	/// </summary>
	public class DataAlertRule : EventRule
	{
      #region Member Variables

		// Database columns
        private AlertLevel _level;
        private DataRuleType _ruleType;
        private DataAlertComparison _comparison;
        private bool _emailAction;
        private bool _logAction;
        private bool _snmpTrapAction;
        private SNMPConfiguration _snmpConfiguration;
        private string _messageData;
        private string _ruleTypeName;
        private string _database;
        private string _table;
        private string _column;
        private string _schema;
        private bool _applicationName;
        private bool _loginName;
        private bool _rowCount;




      private string _messageTitle ;
      private string _messageBody ;
      private string[] _recipients ;
      private ErrorLog.Severity _logEntryType ;
	   

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

      [XmlAttribute("ApplicationName")]
      public bool ApplicationName
      {
          get { return _applicationName; }
          set { _applicationName = value; }
      }
      [XmlAttribute("LoginName")]
      public bool LoginName
      {
          get { return _loginName; }
          set { _loginName = value; }
      }
        
        [XmlAttribute("RowCount")]
        public bool RowCount
        {
            get { return _rowCount; }
            set { _rowCount = value; }
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

      [XmlElement("DataRuleType")]
      public DataRuleType DataType
      {
         get { return _ruleType; }
         set { _ruleType = value; }
      }

      [XmlAttribute("DataRuleTypeName")]
      public string DataRuleTypeName
      {
         get { return _ruleTypeName; }
         set { _ruleTypeName = value; }
      }

      [XmlElement("Comparison")]
      public DataAlertComparison Comparison
      {
         get { return _comparison; }
         set { _comparison = value; }
      }

      [XmlElement("Instance")]
      public string Instance
      {
         get 
         {
            if (_targetInstances.Length == 0)
               return null;
            else
               return _targetInstances[0]; 
         }
         set
         {
            if (value != null)
            {
               if (_targetInstances.Length == 0)
                  _targetInstances = new string[1];
               _targetInstances[0] = value;
            }
         }
      }

      [XmlElement("Database")]
      public string Database
      {
         get { return _database; }
         set { _database = value; }
      }

      [XmlElement("Schema")]
      public string Schema
      {
         get { return _schema; }
         set { _schema = value; }
      }

      [XmlElement("Table")]
      public string Table
      {
         get { return _table; }
         set { _table = value; }
      }

      public string FullTableName
      {
         get 
         {
            if (String.IsNullOrEmpty(_schema))
               return _table;
            else
               return String.Format("{0}.{1}", _schema, _table);
         }
         set
         {
            if (String.IsNullOrEmpty(value))
            {
               _table = value;
               _schema = value;
            }
            else
            {
               bool multipleTables = value.IndexOf(',') != -1;
               int index = value.IndexOf('.');
               if (index == -1 || multipleTables)
               {
                  _table = value;
               }
               else
               {
                  _schema = value.Substring(0, index);
                  _table = value.Substring(index + 1);
               }
            }
         }
      }

      [XmlElement("Column")]
      public string Column
      {
         get { return _column; }
         set { _column = value; }
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

      [XmlIgnore]
      public bool IsRuleValid
      {
         get
         {
            if (String.IsNullOrEmpty(Instance)  ||
                String.IsNullOrEmpty(_database) ||
                String.IsNullOrEmpty(_table)    ||
                String.IsNullOrEmpty(_column))
               return false;
                foreach (EventCondition condition in _conditionMap.Values)
                    if (condition.FieldId != (int)AlertableEventFields.applicationName && 
                        condition.FieldId != (int)AlertableEventFields.loginName &&
                        condition.FieldId != (int)AlertableEventFields.eventCategory &&
                        condition.FieldId != (int)AlertableEventFields.success &&        //SQLCM -5470 v5.6
                        !condition.IsValid)
                        return false;
               return true;
         }
      }

		#endregion

		#region Construction/Destruction

		public DataAlertRule() : base()
		{
         _comparison = new DataAlertComparison();
			_level = AlertLevel.Medium;
         _type = EventType.Data;
         _recipients = new string[0];
         _messageBody = AlertingConfiguration.DataAlertMessageBody ;
         _messageTitle = AlertingConfiguration.DataAlertMessageTitle ;
         _database = "<ALL>";
         _table = "<ALL>";
         _column = "<ALL>";
         UpdateMessageData() ;
         _isDirty = false ;
      }

      public bool MatchesDatabase(string database)
      {
         if (String.Compare(_database, "<ALL>", true) == 0)
            return true;
         return (String.Equals(_database, database, StringComparison.OrdinalIgnoreCase));
      }

      public bool MatchesTable(string table)
      {
         if (String.Compare(_table, "<ALL>", true) == 0)
            return true;

         if (table.IndexOf('.') == -1)
         {
             string pattern = @"\b" + Regex.Escape(table) + @"\b";
             if (Regex.Match(_table, pattern, RegexOptions.IgnoreCase).Success)
             {
                 return true;
             }
             else
                 return false;
             //return (String.Equals(_table, table, StringComparison.OrdinalIgnoreCase));
         }
         else
             return (String.Equals(FullTableName, table, StringComparison.OrdinalIgnoreCase));
      }

      public bool MatchesColumn(string column)
      {
         if (String.Compare(_column, "<ALL>", true) == 0)
            return true;
         return (String.Equals(_column, column, StringComparison.OrdinalIgnoreCase));
      }
      //SQLCM -5470 v5.6
      public bool MatchesDataSetColumn(string column)
      {
          if (String.Compare(_column, "<ALL>", true) == 0)
              return true;
          return(_column.ToUpper().Contains(column.ToUpper()));
          //return (String.(_column, column, StringComparison.OrdinalIgnoreCase));
      }
      //SQLCM -5470 v5.6 - END

      public new DataAlertRule Clone()
		{
         DataAlertRule retVal = (DataAlertRule)base.Clone();

          if (_recipients != null)
          {
              retVal._recipients = new string[_recipients.Length];
              for (int i = 0; i < _recipients.Length; i++)
                  retVal._recipients[i] = _recipients[i];
          }
          else
              retVal._recipients = new string[0];

          if (_comparison != null)
          {
              retVal._comparison = new DataAlertComparison();
              retVal._comparison.Value = _comparison.Value;
              retVal._comparison.Operator = _comparison.Operator;
              retVal._comparison.Id = _comparison.Id;
          }
          if (_targetInstances != null)
          {
              retVal._targetInstances = new string[_targetInstances.Length];
              for (int i = 0; i < _targetInstances.Length; i++)
                  retVal._targetInstances[i] = _targetInstances[i];
          }
          else
          {
              retVal._targetInstances = new string[0];

              retVal._removedConditions = new ArrayList();
              retVal._conditionMap = new Hashtable();

              foreach (EventCondition condition in _conditionMap.Values)
                  retVal.AddCondition(condition.Clone());
          }
         
          return retVal;
      }

		#endregion

      public string GetStringForComparisonInsert()
      {
         StringBuilder builder = new StringBuilder();
         builder.AppendFormat("({0}){1}", _database.Length, _database);
         builder.AppendFormat("({0}){1}", _table.Length, _table);
         builder.AppendFormat("({0}){1}", _column.Length, _column);
         builder.AppendFormat("(1){0}", (int)_comparison.Operator);
         builder.AppendFormat("({0}){1}", _comparison.Value.ToString().Length, _comparison.Value.ToString());

         return builder.ToString();
      }

      public void InsertComparison(string comparison)
      {
         _database = GetNextValue(ref comparison);
         _table = GetNextValue(ref comparison);
         _column = GetNextValue(ref comparison);
         _comparison.Operator = (ComparisonOperator)(Convert.ToInt32(GetNextValue(ref comparison)));
         _comparison.Value = Convert.ToInt64(GetNextValue(ref comparison));
      }

      private string GetNextValue(ref string input)
      {
         string value = "";
         int length;
         int index = input.IndexOf("(");

         if (input != null)
         {

            try
            {
               if (index != -1)
               {
                  input = input.Substring(index + 1);
                  index = input.IndexOf(")");
                  length = Int32.Parse(input.Substring(0, index));
                  input = input.Substring(index + 1);
                  value = input.Substring(0, length);
                  input = input.Substring(length);

               }
            }
            catch (Exception e)
            {
               throw new Exception("Improperly formed Data alert comparison value.", e);
            }
         }
         return value;
      }

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

         if (this.GetType() != obj.GetType()) return false;

         if(!base.Equals(obj)) return false ;

			// safe because of the GetType check
         DataAlertRule rule = (DataAlertRule)obj;

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
