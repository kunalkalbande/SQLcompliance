namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for AlertAction.
	/// </summary>
	public interface IAlertAction
	{
		#region Properties

		int Id
		{
			get ;
			set ;
		}

		int RuleId
		{
			get ;
			set ;
		}

		AlertRule ParentRule
		{
			get ;
			set ;
		}

		ActionType ActionType
		{
			get ;
		}

		string ActionData
		{
			get ;
			set ;
		}

		bool IsValid
		{
			get ;
		}

		bool Dirty
		{
			get ; 
			set ;
		}

		#endregion

		void Reset() ;
	
		IAlertAction Clone() ;

		ActionResult GenerateInitialResult() ;
	}

}
