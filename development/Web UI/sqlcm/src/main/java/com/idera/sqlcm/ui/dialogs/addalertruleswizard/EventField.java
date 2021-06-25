package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

public class EventField {
	// Database columns
	private int _fieldId ;
	private MatchType _matchType ;
	private Severity _severity;
	private EventType _type ;
	private String _columnName ;
	private String _displayName ;
	private String _description ;
	
	public enum MatchType
	   {
	      String,
	      Bool,
	      Integer
	   } ;
	   
	   public enum EventType
	   {
	      SqlServer,
	      Status,
	      Data
	   }

	   public enum AlertLevel
		{
			Low ,
			Medium,
			High,
			Severe
		} ;
		public enum Severity
		{
			Informational,
			Warning,
			Error
		}
	public String get_description() {
		return _description;
	}

	public void set_description(String _description) {
		this._description = _description;
	}

	public String get_displayName() {
		return _displayName;
	}

	public void set_displayName(String _displayName) {
		this._displayName = _displayName;
	}

	public String get_columnName() {
		return _columnName;
	}

	public void set_columnName(String _columnName) {
		this._columnName = _columnName;
	}

	public EventType get_type() {
		return _type;
	}

	public void set_type(EventType _type) {
		this._type = _type;
	}

	public MatchType get_matchType() {
		return _matchType;
	}

	public void set_matchType(MatchType _matchType) {
		this._matchType = _matchType;
	}

	public int get_fieldId() {
		return _fieldId;
	}

	public void set_fieldId(int _fieldId) {
		this._fieldId = _fieldId;
	}	   
	
	public MatchType getDataFormat(){
		return _matchType ;
	}
	
	public void setDataFormat(MatchType _matchType){
		this._matchType = _matchType;
	}

	public Severity get_severity() {
		return _severity;
	}

	public void set_severity(Severity _severity) {
		this._severity = _severity;
	}
}
