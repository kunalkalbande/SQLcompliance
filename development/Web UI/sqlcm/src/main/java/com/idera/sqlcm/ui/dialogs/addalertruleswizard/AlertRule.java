package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

import java.util.Hashtable;
import java.util.Map;
import java.util.TreeMap;

public class AlertRule extends EventRule{
	public enum AlertLevel
	{
		Low,
		Medium,
		High,
		Severe
	} ;

	public enum ActionType
	{
		EventLog,
		SMTP
	} ;
	
	public AlertLevel _level ;
	public boolean _emailAction ;
	public boolean _logAction ;
	public boolean _snmpTrapAction;
	public String _messageData ;
	public String _messageTitle ;
	public String _messageBody ;
	public String[] _recipients ;
	Map<String,String> map = new TreeMap<String,String>();
	
	public AlertLevel get_level() {
		return _level;
	}

	public void set_level(AlertLevel _level) {
		this._level = _level;
	}

	public boolean is_emailAction() {
		return _emailAction;
	}

	public void set_emailAction(boolean _emailAction) {
		this._emailAction = _emailAction;
	}

	public boolean is_logAction() {
		return _logAction;
	}

	public void set_logAction(boolean _logAction) {
		this._logAction = _logAction;
	}

	public boolean is_snmpTrapAction() {
		return _snmpTrapAction;
	}

	public void set_snmpTrapAction(boolean _snmpTrapAction) {
		this._snmpTrapAction = _snmpTrapAction;
	}

	public String get_messageData() {
		return _messageData;
	}

	public void set_messageData(String _messageData) {
		this._messageData = _messageData;
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

	public String[] get_recipients() {
		return _recipients;
	}

	public void set_recipients(String[] _recipients) {
		this._recipients = _recipients;
	}
    
	public String UpdateMessageData(EventField eventtype, AlertRule alertRule ) throws Exception
	{
		map.put("title",_messageTitle) ;
		map.put("body",_messageBody) ;
		map.put("recipients",alertRule.get_recipients().toString()) ;
		_messageData = KeyValueParser.BuildString(map) ;
		set_messageData(_messageData) ;
        return _messageData;
	}
}
