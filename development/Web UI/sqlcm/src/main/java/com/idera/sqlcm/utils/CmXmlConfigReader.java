package com.idera.sqlcm.utils;
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

import org.json.JSONException;
import org.json.JSONObject;
import org.json.XML;

import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.alerts.AlertsExportDialog;


public class CmXmlConfigReader {
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(AlertsExportDialog.class);
	//private static String configXmlPath = "D:\\ideraCodeBase\\SQLcm\\development\\Web UI\\sqlcm\\src\\main\\java\\com\\idera\\sqlcm\\utils\\CmConfig.xml";
	private static String configXmlFile = "CmConfig.xml";
	public static JSONObject configJsonObj = null;

	public static void loadXmlConfig() throws Exception  {
		try {
			InputStream  is = CmXmlConfigReader.class.getResourceAsStream(configXmlFile);
			BufferedReader br = new BufferedReader(new InputStreamReader(is));
			String line = br.readLine(); StringBuilder sb = new StringBuilder(); 
			while(line != null){ 
				sb.append(line).append("\n"); 
				line = br.readLine(); 
			} 
			String fileAsString = sb.toString();
			configJsonObj = XML.toJSONObject(fileAsString);
			System.out.println(configJsonObj);
			
		} catch (FileNotFoundException e) {
			logger.error(e, "File not found");
			WebUtil.showErrorBox(e,
					"ERROR: File not found");
			throw e;
		} catch (IOException e) {

			logger.error(e, "Error while opening the file");
			WebUtil.showErrorBox(e,
					"ERROR: opening the file");
			throw e;
		} catch (JSONException e) {

			logger.error(e, "JSON exception");
			WebUtil.showErrorBox(e,
					"ERROR: JSON exception");
			throw e;
		}
		catch(Exception e) {
			logger.error(e, "Exception occured during loading Xml");
			WebUtil.showErrorBox(e,
					"ERROR: Cannot load CmConfig.xml");
			throw e;
		}

	}
	
	public static boolean getBooleanValue(String page, String element){
		if(configJsonObj == null) {
			try {
				loadXmlConfig();
			} catch (Exception e) {
				return false;
			}
		}
		JSONObject pageOptionObj;
		try {
			pageOptionObj = configJsonObj.getJSONObject(page);
			return  pageOptionObj.getBoolean(element) ;
		} catch (Exception e) {
			WebUtil.showErrorBox(e,
					"ERROR while fetching the value from xml");
		}

		return true;
	}
	/*public static void main(String args[]) {
		loadConfigLocally();
		System.out.println(getBooleanValue(AUDITED_INSTANCE_PAGE, AUDITED_INSTANCE_OPTION));
	}*/
	
	
}
