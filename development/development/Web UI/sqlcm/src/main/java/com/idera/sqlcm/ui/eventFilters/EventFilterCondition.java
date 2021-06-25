package com.idera.sqlcm.ui.eventFilters;


import java.util.HashMap;
import java.util.Map;

import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.Severity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;

public class EventFilterCondition {
	private boolean _inclusive;
	private boolean _blanks; // This is for empty or any number of spaces
	private boolean _nulls; // This is for null values
	private boolean _enabled;

	private boolean _boolValue;
	String[] _targetStrings;


	private int _conditionId;
	private int _fieldId;
	private String _matchString;	
	private int[] _targetInts;
	
	public Map<String,String> map = new HashMap<String, String>();
   
	public EventFilterCondition() {
		
		
		_matchString = "";
		_blanks = false;
		set_nulls(false);
		_targetInts = new int[0];
		_targetStrings = new String[0];
		// _stringComparers = new WildMatch[0] ;
		_inclusive = true;
	}

	public String UpdateMatchString(EventField eventtype, EventFilterCondition eventCondition) throws Exception{

		switch(eventtype.getDataFormat())
		{

		case String:
			if(eventCondition._inclusive){
				map.put("include","1");
			}else{
				map.put("include","0");}

			map.put("count", Integer.toString(eventCondition._targetStrings.length));

			if(eventCondition._blanks){
				map.put("blanks","1");
			}else{
				map.put("blanks","0");}

			if(eventCondition._nulls){
				map.put("nulls","1");
			}else{
				map.put("nulls","0");
			}

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
	 
	public boolean is_boolValue() {
		return _boolValue;
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
	
	public boolean is_enabled() {
		return _enabled;
	}

	public void set_enabled(boolean _enabled) {
		this._enabled = _enabled;
	}
}
