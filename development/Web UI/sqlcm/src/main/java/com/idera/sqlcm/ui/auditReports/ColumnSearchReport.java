package com.idera.sqlcm.ui.auditReports;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.Writer;
import java.text.*;
import java.util.*;

import javax.activation.MimetypesFileTypeMap;

import org.apache.commons.io.IOUtils;
import org.zkoss.zul.Filedownload;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMColumnDetails;
import com.idera.sqlcm.entities.CMTableDetails;

public class ColumnSearchReport {
	 private static final String COMMA_DELIMITER = ",";

	 private static final String NEW_LINE_SEPARATOR = "\n";

	  

	 //CSV file header

	 private static final String FILE_HEADER = "DatabaseName, Schema.Table, Columns Matched, Column Type, Max Length, String Matched";


	 public static void writeCsvFile(List<CMTableDetails> values, List<CMColumnDetails> value1, String instanceName) {

	     try {
	    	 
	    	 String instance=instanceName.replaceAll("[^a-zA-Z0-9]", "");
	    	 DateFormat df = new SimpleDateFormat("yyyyMMdd-HHmmss");
	    	 Calendar currentDate = Calendar.getInstance(Utils.getClientTimeZone());
	  	   	 String dates=df.format(currentDate.getTime());
	  	   	 
	         String file= FILE_HEADER.toString();

	         file=file+NEW_LINE_SEPARATOR;
	          
	         Iterator itr= values.iterator();
	         
	         while(itr.hasNext()){
	         
	        	 CMTableDetails cmTableDtl=(CMTableDetails)itr.next();
	        	 Iterator itr1=value1.iterator();
	        	 while(itr1.hasNext())
	        	 {
		        	 CMColumnDetails cmColumnDtl= (CMColumnDetails)itr1.next();
		        	 if(cmTableDtl.getSchemaTableName().equals(cmColumnDtl.getTableName()))
		        	 {
		        		 file=file+String.valueOf(cmTableDtl.getDatabaseName());

		        		 file=file+COMMA_DELIMITER;

		        		 file=file+cmTableDtl.getSchemaTableName();

			             file=file+COMMA_DELIMITER;

			             file=file+String.valueOf(cmColumnDtl.getFieldName());

			             file=file+COMMA_DELIMITER;
		
			             file=file+String.valueOf(cmColumnDtl.getDataType());

			             file=file+COMMA_DELIMITER;

			             file=file+String.valueOf(cmColumnDtl.getSize());

			             file=file+COMMA_DELIMITER;

			             file=file+String.valueOf(cmColumnDtl.getMatchStr());
			             
			             file=file+NEW_LINE_SEPARATOR;
		        	 }
	        	 }
	         }

	         InputStream ab=IOUtils.toInputStream(file, "UTF-8");

		        try {
		        	Filedownload.save(ab, "csv", instance+"-Export-"+dates+".csv");

		        } catch (Exception e) {
		            // TODO Auto-generated catch block
		            e.printStackTrace();
		        }

	          

	     } catch (Exception e) {

	         System.out.println("Error in CsvFileWriter !!!");

	         e.printStackTrace();

	     } 

	     }

}
