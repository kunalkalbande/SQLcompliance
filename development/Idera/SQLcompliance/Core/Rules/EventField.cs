
using System.Xml.Serialization;

namespace Idera.SQLcompliance.Core.Rules
{
	/// <summary>
	/// Summary description for EventFieldInfo.
	/// </summary>
	public class EventField
	{
		#region Member Variables

		// Database columns
		private int _fieldId ;
		private MatchType _matchType ;
		private EventType _type ;
		private string _columnName ;
		private string _displayName ;
		private string _description ;

		#endregion

		#region Properties

      [XmlAttribute]
		public int Id
		{
			get { return _fieldId ; }
			set { _fieldId = value ; }
		}

      [XmlAttribute()]
		public EventType RuleType
		{
			get { return _type ; }
			set { _type = value ; }
		}

      [XmlAttribute()]
      public string ColumnName
		{
			get { return _columnName ; }
			set { _columnName = value ; }
		}

      [XmlAttribute()]
      public string DisplayName
		{
			get { return _displayName ; }
			set { _displayName = value ; }
		}

      [XmlAttribute()]
      public string Description
		{
			get { return _description ; }
			set { _description = value ; }
		}

      [XmlAttribute()]
      public MatchType DataFormat
		{
			get { return _matchType ; }
			set { _matchType = value ; }
		}
		
		#endregion

		#region Construction/Destruction

		public EventField()
		{
		}

		public EventField(EventType ruleType, string columnName, MatchType matchType, string displayName, string description)
		{
			_type = ruleType ;
			_columnName = columnName ;
			_displayName = displayName ;
			_description = description ;
			_matchType = matchType; 
		}

		#endregion

		public override string ToString()
		{
			return DisplayName ;
		}

	}
}
