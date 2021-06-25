using System;
using System.Collections;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for EventLogAction.
	/// </summary>
	public class EventLogAction : IAlertAction
	{
		#region Member Variables

		public static readonly string EventLogMessage = @"SQLCompliance Manager Alert
Alert Id: $AlertId$
Time: $AlertTime$
Level: $AlertLevel$
Source Event Id: $EventId$" ;

		private int _actionId ;
		private int _ruleId ;
		private string _actionData ;

		private bool _isValid ;
		private bool _isDirty ;

		private ErrorLog.Severity _logEntryType ;
		private string _message ;

		private AlertRule _parentRule ;

		#endregion

		#region Properties

		public int Id
		{
			get { return _actionId ; }
			set { _actionId = value ; }
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

		public ActionType ActionType
		{
			get { return ActionType.EventLog ; }
		}

		public string ActionData
		{
			get { return _actionData ; }
			set 
			{ 
				_actionData = value ; 
				ParseActionData() ;
			} 
		}

		public bool IsValid
		{
			get { return _isValid; }
		}

		public ErrorLog.Severity LogEntryType
		{
			get { return _logEntryType ; }
			set 
			{ 
				_logEntryType = value ;
				UpdateActionData() ;
			}
		}

		public string Message
		{
			get { return _message ; }
			set 
			{
				_message = value ; 
				UpdateActionData() ;
			}
		}

		public bool Dirty
		{
			get { return _isDirty ; }
			set { _isDirty = value ; }
		}

		#endregion

		#region Construction/Destruction

		public EventLogAction()
		{
			_actionId = AlertingConfiguration.NULL_ID ;
			Reset() ;
		}

		public void Reset()
		{
			_isDirty = false ;
			_isValid = true ;
			_logEntryType = ErrorLog.Severity.Warning ;
			_message = EventLogMessage ;
			UpdateActionData() ;
		}

		public IAlertAction Clone()
		{
			EventLogAction retVal = (EventLogAction)MemberwiseClone() ;

			return retVal ;
		}

		#endregion

		public override string ToString()
		{
			return "Application Event Log Entry" ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
			EventLogAction action = (EventLogAction)obj;     

			// use this pattern to compare value members
			if(!_actionId.Equals(action._actionId)) return false ;
			if(!RuleId.Equals(action.RuleId)) return false ;

			// use this pattern to compare reference members
			if(!Object.Equals(_actionData, action._actionData)) return false ;

			return true;
		}

      public override int GetHashCode()
      {
         return _actionId ;
      }

		private void UpdateActionData()
		{
			Hashtable map = new Hashtable() ;

			_isDirty = true ;
			if(!_isValid)
			{
				_actionData = "" ;
				return ;
			}
			map["level"] = ((int)_logEntryType).ToString() ;
			map["message"] = _message ;
			_actionData = KeyValueParser.BuildString(map) ;
		}

		private void ParseActionData()
		{
			try
			{
				Hashtable map = KeyValueParser.ParseString(_actionData) ;
				_logEntryType = (ErrorLog.Severity)Int32.Parse((string)map["level"]) ;
				_message = (string)map["message"] ;
			}
			catch
			{
				_isValid = false ;
			}
		}

		public ActionResult GenerateInitialResult()
		{
			ActionResult retVal = new ActionResult() ;

			retVal.ActionData = _actionData ;
			retVal.Status = ActionResultStatus.Pending ;

			return retVal ;
		}

	}
}
