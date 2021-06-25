using System;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
	/// <summary>
	/// Summary description for MacroDefinition.
	/// </summary>
	public class MacroDefinition
	{
		#region Member Variables

		private string _name ; 
		private string _description ;
		private string _value ; 

		#endregion

		#region Properties

		public string Name
		{
			get { return _name ; }
			set { _name = value ; }
		}

		public string Description
		{
			get { return _description ; }
			set { _description = value ; }
		}

		public string Value
		{
			get { return _value ; }
			set { _value = value ; }
		}

		#endregion

		#region Construction/Destruction

		public MacroDefinition()
		{
		}

		#endregion

		public override string ToString()
		{
			return _name ; 
		}

	}
}
