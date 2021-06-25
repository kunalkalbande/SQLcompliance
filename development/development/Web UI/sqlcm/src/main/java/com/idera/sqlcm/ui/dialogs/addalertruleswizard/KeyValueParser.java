package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import javax.swing.text.html.HTMLDocument.Iterator;
import javax.xml.crypto.KeySelector;

import org.ocpsoft.common.util.Iterators;
import org.ocpsoft.rewrite.annotation.Convert;

 public class KeyValueParser
	{
		public KeyValueParser()
		{
		}

		public static Map ParseString(String s) throws Exception
		{
			Map<String, String> EventNodeDataValue = new HashMap<String, String>();
			String active = s;
			int index = s.indexOf("(") ;
			int keyIndex = 0;

			try
			{
				while (index != -1) {
					String sKey, sValue;
					String length;

					sKey = active.substring(0, index);
					active = active.substring(index + 1);
					index = active.indexOf(")");
					length = active.substring(0, index);
					active = active.substring(index + 1);
					sValue = (active.subSequence(0, Integer.parseInt(length))
							.toString());
					active = (active.substring(Integer.parseInt(length))
							.toString());
					if(sKey.isEmpty()){
						sKey = Integer.toString(keyIndex);
						keyIndex++;
					}
					EventNodeDataValue.put(sKey, sValue);
					index = active.indexOf("(");
				}
			}
			catch(Exception e)
			{
				throw new Exception("Improperly formed KeyValue string.", e); 
			}
			if(active.length() > 0)
				throw new Exception("Improperly formed KeyValue string."); 
			return EventNodeDataValue ;
		}

		public Map<String, String> MatchStringParser(Map<String, String> EventFilterConditionData){
			Map<String, String> EventNodeDataValue = new HashMap<String, String>();
			for (Map.Entry<String, String> EventDataValues : EventFilterConditionData
					.entrySet()) {
				String strEventDataVal = EventDataValues.getValue();
				String[] strArray;
				strArray = strEventDataVal.split(",");
				strEventDataVal = strArray[2];
				if (strEventDataVal == null)
					try {
						throw new Exception("KeyValue string should not be null.");
					} catch (Exception e2) {
						// TODO Auto-generated catch block
						e2.printStackTrace();
					}

				
				String active = strEventDataVal;
				int index = strEventDataVal.indexOf("(");

				try {
					while (index != -1) {
						String sKey, sValue;
						String length;

						sKey = active.substring(0, index);
						active = active.substring(index + 1);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length))
								.toString());
						EventNodeDataValue.put(sKey, sValue);
						index = active.indexOf("(");
					}
				} catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
	
			}
			return EventNodeDataValue ;		
		}
		
		
		public static String BuildString(Map<String, String> map) throws Exception
		{
			if(map == null)
			{
				throw new Exception("Argument null Exception") ;
			}
			 StringBuilder retVal = new StringBuilder() ;
	         ArrayList keys = new ArrayList() ;
	         keys.addAll(map.values());
	         
	         Map<String,String> treemap = new TreeMap<String,String>(map);
	         
	         for(Map.Entry<String,String> entry : treemap.entrySet())
	         {
	            String mapValue = entry.getValue() ;
	            if(entry.getValue().indexOf("(") != -1)
	            throw new Exception("Invalid Key Entry") ;
	            retVal.append(entry.getKey());
	            retVal.append("(" + ((String)mapValue).length()+ ")");
	            retVal.append(mapValue);
	         }
				return retVal.toString() ;
		}
	}