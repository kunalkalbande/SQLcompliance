package com.idera.sqlcm.ui.dialogs.addalertruleswizard;


import java.util.HashMap;
import java.util.Map;

import com.google.common.base.Joiner;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.Severity;

public class EventCondition {
	private boolean _inclusive;
	private boolean _blanks; // This is for empty or any number of spaces
	private boolean _nulls; // This is for null values
	private boolean _boolValue;
	String[] _targetStrings;

	private int[] _targetInts;
	boolean _isDirty;
	private int _conditionId;
	private int _ruleId;
	private int _fieldId;
	private String _matchString;
	private EventField _columnInfo;
	private boolean _isValid;
	private boolean[] _days; // 0 is Sunday, 6 is Saturday.
	// private WildMatch[] _stringComparers ;
	private int _duration;
	private String _timeZoneStandardName;
	public Map<String,String> map = new HashMap<String, String>();
    private boolean _emailAction ;
    private boolean _logAction ;
    private boolean _snmpTrapAction;
    private String _messageData ;

    private String _messageTitle ;
    private String _messageBody ;
    private String[] _recipients ;
	public EventCondition() {
		// _conditionId = EventRule.NULL_ID ;
		_days = new boolean[7];
		_isDirty = false;
		_matchString = "";
		_blanks = false;
		set_nulls(false);
		_targetInts = new int[0];
		_targetStrings = new String[0];
		// _stringComparers = new WildMatch[0] ;
		_inclusive = true;
	}

	public String UpdateMatchString(EventField eventtype, EventCondition eventCondition) throws Exception{

		_isDirty = true;
		switch(eventtype.getDataFormat())
		{

		case String:
			if(eventCondition._inclusive){
				map.put("include","1");
			}else{
				map.put("include","0");}

			if(eventCondition._blanks){
				map.put("blanks","1");
			}else{
				map.put("blanks","0");}

			if(eventCondition._nulls){
				map.put("nulls","1");
			}else{
				map.put("nulls","0");
			}

			map.put("count", Integer.toString(eventCondition._targetStrings.length));
			for(int i = 0 ; i < eventCondition._targetStrings.length ; i++)
				map.put(Integer.toString(i), _targetStrings[i]);
			break ;

		case Bool:
			map.put("value",Boolean.toString(eventCondition._boolValue));
			break ;

		case Integer:
			if(_inclusive){
				map.put("include","1");
			}else{
				map.put("include","0");}

			StringBuilder sb = new StringBuilder(128) ;
			for(int i = 0 ; i < eventCondition._targetInts.length ; i++)
			{
				sb.append(eventCondition._targetInts[i]) ;
				if(i < eventCondition._targetInts.length - 1)
					sb.append(",") ;
			}
			map.put("value",sb.toString());
			break ;
		}
		_matchString = KeyValueParser.BuildString(map) ;
		
		return _matchString;
	}
	
	 public String UpdateMessageData(EventField eventtype, EventCondition eventCondition) throws Exception{
        _isDirty = true ;
        String recipients = Joiner.on(",").skipNulls().join(eventCondition.get_targetStrings()).replace(",", ",");
		map.put("title",eventCondition._messageTitle);
		map.put("body",eventCondition._messageBody);
		map.put("severity","0");
		map.put("recipients",recipients);
		String _matchString = KeyValueParser.BuildString(map);	
        set_messageData(_matchString) ;
        return _matchString;
     }

	public boolean is_boolValue() {
		return _boolValue;
	}

	public String get_messageTitle() {
		return _messageTitle;
	}

	public void set_messageTitle(String _messageTitle) {
		this._messageTitle = _messageTitle;
	}

	public String get_messageBody() {
		return _messageBody;
	}

	public void set_messageBody(String _messageBody) {
		this._messageBody = _messageBody;
	}

	public void set_boolValue(boolean _boolValue) {
		this._boolValue = _boolValue;
	}

	public boolean is_nulls() {
		return _nulls;
	}

	public void set_nulls(boolean _nulls) {
		this._nulls = _nulls;
	}
	public boolean is_inclusive() {
		return _inclusive;
	}

	public void set_inclusive(boolean _inclusive) {
		this._inclusive = _inclusive;
	}

	public boolean is_blanks() {
		return _blanks;
	}

	public void set_blanks(boolean _blanks) {
		this._blanks = _blanks;
	}

	public String[] get_targetStrings() {
		return _targetStrings;
	}

	public void set_targetStrings(String[] _targetStrings) {
		this._targetStrings = _targetStrings;
	}
	
	public int[] get_targetInts() {
		return _targetInts;
	}

	public void set_targetInts(int[] _targetInts) {
		this._targetInts = _targetInts;
	}

	public String get_messageData() {
		return _messageData;
	}

	public void set_messageData(String _messageData) {
		this._messageData = _messageData;
	}

	public String[] get_recipients() {
		return _recipients;
	}

	public void set_recipients(String[] _recipients) {
		this._recipients = _recipients;
	}
}
