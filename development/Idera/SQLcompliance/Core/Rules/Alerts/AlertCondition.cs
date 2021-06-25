using System;
using System.Collections;
using System.Text ;
using Idera.SQLcompliance.Core.TimeZoneHelper ;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for AlertCondition.
	/// </summary>
	public class AlertCondition
	{
		#region Member Variables

		// Database columns
		private int _conditionId ;
		private int _ruleId ;
		private int _fieldId ;
		private string _matchString ;

		private AlertRule _parentRule ;
		private EventField _columnInfo ;

		private bool _isValid ;
		private bool _isDirty ;

		// Date MatchType
		private bool[] _days ; // 0 is Sunday, 6 is Saturday. 
		private DateTime _startTime ;
		private int _duration ;
		private string _timeZoneStandardName ;


		// String MatchType
		private string[] _targetStrings ;
		private WildMatch[] _stringComparers ;
		private bool _inclusive ;

		// Bool MatchType
		private bool _boolValue ;

		// Integer MatchType
		private int[] _targetInts ;

		#endregion

		#region Properties

		public int Id
		{
			get { return _conditionId ; }
			set { _conditionId = value ; }
		}

		public int RuleId
		{
			get 
			{ 
				if(_parentRule != null)
					return _parentRule.Id ;
				else
					return _ruleId ; 
			}
			set { _ruleId = value ; }
		}

		public int FieldId
		{
			get 
			{ 
				if(_columnInfo != null)
					return _columnInfo.Id ;
				else
					return _fieldId ; 
			}
			set { _fieldId = value ; }
		}

		public string MatchString
		{
			get { return _matchString ; }
			set 
			{ 
				_matchString = value ; 
				ParseMatchString() ;
			}
		}

		public EventField TargetEventField
		{
			get { return _columnInfo ; }
			set 
			{ 
				if(value != null)
					_fieldId = value.Id ;
				_columnInfo = value ;
				if(_columnInfo.DataFormat == MatchType.Bool)
					_isValid = true ;
				UpdateMatchString() ;
			}
		}

		public AlertRule ParentRule
		{
			get { return _parentRule ; }
			set 
			{ 
				if(value != null)
					_ruleId = value.Id ;
				_parentRule = value ; 
			}
		}

		public MatchType ConditionType
		{
			get { return _columnInfo.DataFormat ; }
		}

		public bool IsValid
		{
			get { return _isValid ; }
		}

		public bool BooleanValue
		{
			get { return _boolValue ; }
			set 
			{ 
				_boolValue = value ; 
				UpdateMatchString() ;
			}
		}

		public string[] StringArray
		{
			get { return _targetStrings ; }
			set 
			{ 
            if(value == null)
               _targetStrings = new string[0] ;
            else
				   _targetStrings = value ; 
				_isValid = (_targetStrings.Length > 0) ;
				if(_isValid)
					UpdateWildMatchArray() ;
				UpdateMatchString() ;
			}
		}

		public bool Inclusive
		{
			get { return _inclusive ; }
			set 
			{ 
			   _inclusive = value ; 
				UpdateMatchString() ;
			}
		}

		public int[] IntegerArray
		{
			get { return _targetInts ; }
			set
			{
            if(value == null)
               _targetInts = new int[0] ;
            else
				   _targetInts = value ; 
				_isValid = (_targetInts.Length > 0) ;
				UpdateMatchString() ;
			}
		}

		public DateTime StartTime
		{
			get { return _startTime ; }
			set 
			{ 
				_startTime = value ; 
				UpdateMatchString() ;
			}
		}

		public int Duration
		{
			get { return _duration ; }
			set 
			{ 
				_duration = value ; 
				UpdateMatchString() ;
			}
		}

		public DayOfWeek[] DaysOfWeek
		{
			get
			{
				int count = 0 ;
				foreach(bool b in _days)
				{
					if(b)
						count++ ;
				}
				DayOfWeek[] retVal = new DayOfWeek[count] ;
				count = 0 ;
				if(_days[0]) retVal[count++] = DayOfWeek.Sunday ;
				if(_days[1]) retVal[count++] = DayOfWeek.Monday ;
				if(_days[2]) retVal[count++] = DayOfWeek.Tuesday ;
				if(_days[3]) retVal[count++] = DayOfWeek.Wednesday ;
				if(_days[4]) retVal[count++] = DayOfWeek.Thursday ;
				if(_days[5]) retVal[count++] = DayOfWeek.Friday ;
				if(_days[6]) retVal[count++] = DayOfWeek.Saturday ;
				return retVal ;
			}

			set
			{
				for(int i = 0 ; i < 7 ; i++)
					_days[i] = false ;
				if(value.Length > 0)
					_isValid = true ;
				else
					_isValid = false ;
				foreach(DayOfWeek day in value)
				{
					switch(day)
					{
						case DayOfWeek.Sunday:
							_days[0] = true ;
							break ;
						case DayOfWeek.Monday:
							_days[1] = true ;
							break ;
						case DayOfWeek.Tuesday:
							_days[2] = true ;
							break ;
						case DayOfWeek.Wednesday:
							_days[3] = true ;
							break ;
						case DayOfWeek.Thursday:
							_days[4] = true ;
							break ;
						case DayOfWeek.Friday:
							_days[5] = true ;
							break ;
						case DayOfWeek.Saturday:
							_days[6] = true ;
							break ;
					}
				}
				UpdateMatchString() ;
			}
		}

		public string TimeZoneName
		{
			get { return _timeZoneStandardName ; }
			set 
			{ 
				_timeZoneStandardName = value ; 
				UpdateMatchString() ;
			}
		}

		public bool Dirty
		{
			get { return _isDirty ; }
			set { _isDirty = value ; }
		}

		#endregion

		#region Construction/Destruction

		public AlertCondition()
		{
			_conditionId = AlertingConfiguration.NULL_ID ;
			_days = new bool[7] ;
			_isDirty = false ;
         _matchString = "" ;
         _targetInts = new int[0] ;
         _targetStrings = new string[0] ;
         _stringComparers = new WildMatch[0] ;
         _inclusive = true ;
		}

		public AlertCondition(EventField targetColumn)
		{
			_conditionId = AlertingConfiguration.NULL_ID ;
			_columnInfo = targetColumn ;
			_targetStrings = null ;
		   _inclusive = true ;
			_days = new bool[7] ;
			for(int i = 0 ; i < 7 ; i++)
				_days[i] = false ;
			_timeZoneStandardName = TimeZoneInfo.CurrentTimeZone.TimeZoneStruct.StandardName ;
			_startTime = new DateTime(2000, 1, 1, 0, 0, 0, 0) ;
			_duration = 1 ;
         _matchString = "" ;
         _targetInts = new int[0] ;
         _targetStrings = new string[0] ;
         _stringComparers = new WildMatch[0] ;
         if(targetColumn.DataFormat == MatchType.Bool)
         {
            _isValid = true ;
            UpdateMatchString() ;
         }
         /*
         switch(targetColumn.DataFormat)
         {
            case MatchType.String:
               _targetStrings = new string[] {} ;
               _isValid = false ;
               break ;
            case MatchType.Bool:
               _isValid = true ;
               UpdateMatchString() ;
               break ;
            case MatchType.Integer:
               _targetInts = new int[] {} ;
               _isValid = false ;
               break ;
         }*/
			_isDirty = false ;
		}

		public AlertCondition Clone()
		{
			AlertCondition retVal = (AlertCondition)MemberwiseClone() ;

			retVal._days = new bool[7] ;
			for(int i = 0 ; i < 7 ; i++)
				retVal._days[i] = _days[i] ;
			
			if(_targetInts != null)
			{
				retVal._targetInts = new int[_targetInts.Length] ;
				for(int i = 0 ; i < _targetInts.Length ; i++)
					retVal._targetInts[i] = _targetInts[i] ;
			}else
            _targetInts = new int[0] ;

			if(_targetStrings != null)
			{
				retVal._targetStrings = new string[_targetStrings.Length] ;
				for(int i = 0 ; i < _targetStrings.Length ; i++)
					retVal._targetStrings[i] = _targetStrings[i] ;
			}else
            _targetStrings = new string[0] ;
         UpdateWildMatchArray(); 

			return retVal; 
		}

		private void UpdateWildMatchArray()
		{
			if(_targetStrings == null)
			{
				_stringComparers = null ;
				return ;
			}
			_stringComparers = new WildMatch[_targetStrings.Length] ;
			for(int i = 0 ; i < _targetStrings.Length ; i++)
				_stringComparers[i] = new WildMatch(_targetStrings[i]) ;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
			AlertCondition condition = (AlertCondition)obj;     

			// use this pattern to compare value members
			if(!_conditionId.Equals(condition._conditionId)) return false ;
			if(!RuleId.Equals(condition.RuleId)) return false ;
			if(!FieldId.Equals(condition.FieldId)) return false ;

			// use this pattern to compare reference members
			if(!Object.Equals(_matchString, condition._matchString)) return false ;

			return true;		
		}

      public override int GetHashCode()
      {
         return _conditionId ;
      }
      
      private void UpdateMatchString()
		{
			Hashtable map = new Hashtable() ;

			_isDirty = true ;
         /*
			if(!_isValid)
			{
				_matchString = "" ;
				return ;
			}*/
			switch(_columnInfo.DataFormat)
			{
               /*
				case MatchType.Date:
					map["days"] = String.Format("{0},{1},{2},{3},{4},{5},{6}", _days[0]?1:0, _days[1]?1:0, _days[2]?1:0, _days[3]?1:0, _days[4]?1:0, _days[5]?1:0, _days[6]?1:0) ;
					map["start"] = _startTime.ToShortTimeString() ;
					map["duration"] = _duration.ToString() ;
					map["timezone"] = _timeZoneStandardName ;
					break ;
               */
				case MatchType.String:
					map["include"] = (_inclusive ? "1" : "0") ;
					map["count"] = _targetStrings.Length.ToString() ;
					for(int i = 0 ; i < _targetStrings.Length ; i++)
						map[i.ToString()] = _targetStrings[i] ;
					break ;
				case MatchType.Bool:
					map["value"] = _boolValue.ToString() ;
					break ;
				case MatchType.Integer:
               map["include"] = (_inclusive ? "1" : "0") ;
               StringBuilder sb = new StringBuilder(128) ;
					for(int i = 0 ; i < _targetInts.Length ; i++)
					{
						sb.Append(_targetInts[i]) ;
						if(i < _targetInts.Length - 1)
							sb.Append(",") ;
					}
					map["value"] = sb.ToString() ;
					break ;
			}
			_matchString = KeyValueParser.BuildString(map) ;
		}

		private void ParseMatchString()
		{
			try
			{
				Hashtable map = KeyValueParser.ParseString(_matchString) ;
				switch(_columnInfo.DataFormat)
				{
                  /*
					case MatchType.Date:
						string[] intStrs = map["days"].Split(new char[] {','}) ;
						for(int i = 0 ; i < 7 ; i++)
							_days[i] = intStrs[i].Equals("1") ;
						_startTime = DateTime.Parse(map["start"]) ;
						_duration = Int32.Parse(map["duration"]) ;
						_timeZoneStandardName = map["timezone"] ;
						break ;
                  */
					case MatchType.String:
						int count = Int32.Parse((string)map["count"]) ;
				      _inclusive = map["include"].Equals("1") ;
						_targetStrings = new string[count] ;
						for(int i = 0 ; i < count ; i++)
							_targetStrings[i] = (string)map[i.ToString()] ;
                  _isValid = (count > 0) ;
						UpdateWildMatchArray() ;
						break ;
					case MatchType.Bool:
						_boolValue = Boolean.Parse((string)map["value"]) ;
                  _isValid = true ;
						break ;
					case MatchType.Integer:
                  _inclusive = map["include"].Equals("1") ;
                  string temp = (string)map["value"] ;
                  if(temp != null && temp.Trim().Length > 0)
                  {
                     string[] sInts = ((string)map["value"]).Split(new char[] {','}) ;
                     _targetInts = new int[sInts.Length] ;
                     for(int i = 0 ; i < sInts.Length ; i++)
                        _targetInts[i] = Int32.Parse(sInts[i]) ;
                     _isValid = true ;
                  }
                  else
                  {
                     _targetInts = new int[0] ;
                     _isValid = false ;
                  }
						break ;
				}
			}
			catch
			{
				_isValid = false ;
			}
		}

		// We expect UTC time
		public bool Matches(DateTime time)
		{
			TimeZoneInfo tzi = AlertingConfiguration.GetTimeZone(_timeZoneStandardName) ;
			DateTime convertedTime = TimeZoneInfo.ToLocalTime(tzi, time) ;

			switch(convertedTime.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					if(!_days[0]) return false ;
					break ;
				case DayOfWeek.Monday:
					if(!_days[1]) return false ;
					break ;
				case DayOfWeek.Tuesday:
					if(!_days[2]) return false ;
					break ;
				case DayOfWeek.Wednesday:
					if(!_days[3]) return false ;
					break ;
				case DayOfWeek.Thursday:
					if(!_days[4]) return false ;
					break ;
				case DayOfWeek.Friday:
					if(!_days[5]) return false ;
					break ;
				case DayOfWeek.Saturday:
					if(!_days[6]) return false ;
					break ;
			}

			DateTime startTime = new DateTime(convertedTime.Year, convertedTime.Month, convertedTime.Day, _startTime.Hour, _startTime.Minute, 0, 0) ;
			DateTime endTime = startTime.AddHours(_duration) ;
			if(convertedTime >= startTime && convertedTime <= endTime)
				return true ;
			else
				return false ;
		}

		public bool Matches(String str)
		{
			foreach(WildMatch comparer in _stringComparers)
				if(comparer.Match(str))
					return _inclusive ;
			return !_inclusive ;
		}

		public bool Matches(Boolean bValue)
		{
			return (bValue == _boolValue) ;
		}

		public bool Matches(Int32 nValue)
		{
			foreach(int i in _targetInts)
				if(nValue == i)
					return _inclusive ;
			return !_inclusive ;
		}
	}
}
