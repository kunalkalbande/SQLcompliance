using System;
using System.Collections;
using System.Text ;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for SmtpAction.
	/// </summary>
	public class SmtpAction : IAlertAction
	{
		#region Member Variables

		private int _actionId ;
		private int _ruleId ;
		private string _actionData ;

		private bool _isValid ;
		private bool _isDirty ;

		private AlertRule _parentRule ;

		private ArrayList _emailList ;
		private SmtpMessage _message ;

		#endregion

		#region Properties

		public string[] AddressList
		{
			get 
			{ 
				string[] retVal = new string[_emailList.Count];
				for(int i = 0 ; i < _emailList.Count ; i++)
				{
					retVal[i] = (string)_emailList[i] ;
				}
				return retVal ;
			}
			set 
			{ 
				_emailList = new ArrayList(value) ; 
				_isValid = (_emailList.Count > 0) ;
				UpdateActionData() ;
			}
		}

		public SmtpMessage Message
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
			get { return ActionType.SMTP; }
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
			get { return _isValid ; }
		}

		#endregion

		#region Construction/Destruction

		public SmtpAction()
		{
			_actionId = AlertingConfiguration.NULL_ID ;
			Reset() ;
		}

		public IAlertAction Clone()
		{
			SmtpAction retVal = (SmtpAction)MemberwiseClone() ;

			retVal._message = _message.Clone() ;
			retVal._emailList = (ArrayList)_emailList.Clone() ;

			return retVal ;
		}

		public void Reset()
		{
			_isDirty = false ;
			_emailList = new ArrayList() ;
			_message = new SmtpMessage() ;
		}

		#endregion

		public void AddAddress(string emailAddress)
		{
			if(!_emailList.Contains(emailAddress))
			{
				_emailList.Add(emailAddress) ;
				_isValid = (_emailList.Count > 0) ;
				UpdateActionData() ;
			}
		}

		public void RemoveAddress(string emailAddress)
		{
			_emailList.Remove(emailAddress) ;
			_isValid = (_emailList.Count > 0) ;
			UpdateActionData() ;
		}

		public override string ToString()
		{
			return "Email Notification" ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
			SmtpAction action = (SmtpAction)obj;     

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
			map["subject"] = Message.Subject ;
			map["body"] = Message.Body ;
			StringBuilder sb = new StringBuilder() ;
			for(int i = 0 ; i < _emailList.Count ; i++)
			{
				sb.Append(_emailList[i]) ;
				if(i < _emailList.Count - 1)
					sb.Append(",") ;
			}
			map["email"] = sb.ToString() ;
			_actionData = KeyValueParser.BuildString(map) ;
		}

		private void ParseActionData()
		{
			try
			{
				Hashtable map = KeyValueParser.ParseString(_actionData) ;
				_message.Subject = (string)map["subject"] ;
				_message.Body = (string)map["body"] ;
				string s = (string)map["email"] ;
				AddressList = s.Split(new char[] {','}) ;
			}
			catch
			{
				_isValid = false ;
			}
		}

		private string BuildAddressString()
		{
			StringBuilder retVal = new StringBuilder() ;

			for(int i = 0 ; i < _emailList.Count ; i++)
			{
				retVal.Append(_emailList[i]) ;
				if(i < _emailList.Count - 1)
					retVal.Append(";") ;
			}
			return retVal.ToString() ;

		}

		public ActionResult GenerateInitialResult()
		{
			ActionResult retVal = new ActionResult() ;

			retVal.ActionData = _actionData ;
			retVal.Status = ActionResultStatus.Uninitialized ;

			return retVal ;
		}
	}

	public class SmtpMessage
	{
		private string _body ;
		private string _subject ;

		public SmtpMessage()
		{
			_body = "message" ;
			_subject = "subject" ;
		}

		public string Body
		{
			get { return _body ; }
			set 
			{ 
				if(value == null)
					_body = "" ;
				else
					_body = value ; 
			}
		}

		public string Subject
		{
			get { return _subject ; }
			set 
			{ 
				if(value == null)
					_subject = "" ;
				else
					_subject = value ; 
			}
		}

		public SmtpMessage Clone()
		{
			return (SmtpMessage)MemberwiseClone() ;
		}
	}
}
