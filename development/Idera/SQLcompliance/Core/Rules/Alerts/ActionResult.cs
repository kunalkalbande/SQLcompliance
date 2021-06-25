using System;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	public enum ActionResultStatus
	{
		Success = 0,
		Failure,
		Pending,
		Uninitialized
	}

	/// <summary>
	/// Summary description for AlertActionResult.
	/// </summary>
	public class ActionResult
	{
		#region Member Variables

		// Database columns
		private int _resultId ;
		private int _alertId ;
		private int _actionId ;
		private ActionResultStatus _status ;
		private string _actionData ;
		private string _resultData ;

		private Alert _parentAlert ;
		private IAlertAction _parentAction ;

		#endregion

		#region Properties

		public int Id
		{
			get { return _resultId ; }
			set { _resultId = value ; }
		}

		public int AlertId
		{
			get 
			{
				if(_parentAlert != null)
					return _parentAlert.Id ;
				else
					return _alertId ; 
			}
			set { _alertId = value ; }
		}

		public int ActionId
		{
			get { return _actionId ; }
			set { _actionId = value ; }
		}

		public ActionResultStatus Status
		{
			get { return _status ; }
			set { _status = value ; }
		}

		public string ActionData
		{
			get { return _actionData ; }
			set { _actionData = value ; }
		}

		public string ResultData
		{
			get { return _resultData ; }
			set { _resultData = value ; }
		}

		public Alert ParentAlert
		{
			get { return _parentAlert ; }
			set 
			{ 
				if(value != null)
					_alertId = value.Id ;
				_parentAlert = value ; 
			}
		}

		public IAlertAction ParentAction
		{
			get { return _parentAction ; }
			set 
			{
				if(value != null)
					_actionId = value.Id ;
				_parentAction = value ; 
			}
		}

		#endregion

		#region Construction/Destruction

		public ActionResult()
		{
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (this.GetType() != obj.GetType()) return false;

			// safe because of the GetType check
			ActionResult result = (ActionResult)obj;     

			// use this pattern to compare value members
			if(!_resultId.Equals(result._resultId)) return false ;
			if(!AlertId.Equals(result.AlertId)) return false ;
			if(!_actionId.Equals(result._actionId)) return false ;
			if(!_status.Equals(result._status)) return false ;

			if(!Object.Equals(_resultData, result._resultData)) return false ;
			if(!Object.Equals(_actionData, result._actionData)) return false ;

			return true;
		}

      public override int GetHashCode()
      {
         return _resultId ;
      }
	}
}
