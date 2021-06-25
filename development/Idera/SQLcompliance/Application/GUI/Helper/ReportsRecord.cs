using System;
using System.Data.SqlClient;
using System.Net;
using Idera.SQLcompliance.Core;


namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for ReportsRecord.
	/// </summary>
	public class ReportsRecord
	{
      private string _reportServer;
      private string _reportServerDirectory;
      private string _reportManagerDirectory;
      private int _port;
      private bool _useSsl;
      private string _userName;
      private string _password;
      private string _repository;
      private string _targetDirectory;
      private bool _overwriteExisting;
      private bool _reportsDeployed  ;


      public string ReportServer
      {
         get { return _reportServer; }
         set { _reportServer = value; }
      }

      public string ReportServerDirectory
      {
         get { return _reportServerDirectory; }
         set { _reportServerDirectory = value; }
      }

      public string ReportManagerDirectory
      {
         get { return _reportManagerDirectory; }
         set { _reportManagerDirectory = value; }
      }

      public int Port
      {
         get { return _port; }
         set { _port = value; }
      }

      public bool UseSsl
      {
         get { return _useSsl; }
         set { _useSsl = value; }
      }

      public string UserName
      {
         get { return _userName; }
         set { _userName = value; }
      }

      public string Password
      {
         get { return _password; }
         set { _password = value; }
      }

      public string Repository
      {
         get { return _repository; }
         set { _repository = value; }
      }

      public string TargetDirectory
      {
         get { return _targetDirectory; }
         set 
         { 
            _targetDirectory = value;
            if (_targetDirectory.StartsWith("/"))
               _targetDirectory = _targetDirectory.Substring(1);
         }
      }

      public bool OverwriteExisting
      {
         get { return _overwriteExisting; }
         set { _overwriteExisting = value; }
      }

      public bool ReportsDeployed
      {
         get { return _reportsDeployed; }
      }

      private ReportsRecord()
      {
         _overwriteExisting = false;
         _reportsDeployed = true;
      }

      public string GetReportServerUrl()
      {
         string prefix = _useSsl ? "https" : "http";
         string folder = _reportServerDirectory;

         // Trim slashes
         if (folder.StartsWith("/"))
            folder = folder.Substring(1);
         if (folder.EndsWith("/"))
            folder = folder.Substring(0, folder.Length - 1);

         return String.Format("{0}://{1}:{2}/{3}/", prefix, _reportServer, _port, folder);
      }

      public string GetReportManagerUrl(bool showDeployed)
      {
         string prefix = _useSsl ? "https" : "http";
         string folder = _reportManagerDirectory;

         // Trim slashes
         if (folder.StartsWith("/"))
            folder = folder.Substring(1);
         if (folder.EndsWith("/"))
            folder = folder.Substring(0, folder.Length - 1);

         string deployed = _targetDirectory.Replace("/", "%2f").Replace(" ", "+") ;
         
         if(!showDeployed)
            return String.Format("{0}://{1}:{2}/{3}/", prefix, _reportServer, _port, folder);
         else
            return String.Format("{0}://{1}:{2}/{3}/Pages/Folder.aspx?ItemPath=%2f{4}&ViewMode=List", prefix, _reportServer, _port, folder, deployed);

      }

		
      //-------------------------------------------------------------
      // GetMainUrl
      //-------------------------------------------------------------
      public string GetMainUrl()
      {
         string retval = "";
         
         try
         {
         }
         catch
         {
            throw new Exception( "Invalid mainTemplate defined in repository.");
         }
         
         return retval;
      }

      public bool IsAdvancedConnection()
      {
         return _port != 80 ||
            _useSsl ||
            !_reportServerDirectory.Equals("ReportServer") ||
            !_reportManagerDirectory.Equals("Reports");
      }


	   //-------------------------------------------------------------
      // Read - read record from repository
      //-------------------------------------------------------------
		public static ReportsRecord Read(SqlConnection  conn)
		{
		   string cmdStr="";
         ReportsRecord retVal = new ReportsRecord();
		   
		   try
		   {
		      cmdStr = GetSelectSQL();
		      
		      using ( SqlCommand cmd = new SqlCommand( cmdStr, conn ) )
		      {
		         using ( SqlDataReader reader = cmd.ExecuteReader() )
		         {
                  if (reader.Read())
                  {
                     int col = 0;
                     retVal._reportServer = SQLHelpers.GetString(reader, col++);
                     retVal._reportServerDirectory = SQLHelpers.GetString(reader, col++);
                     retVal._reportManagerDirectory = SQLHelpers.GetString(reader, col++);
                     retVal._port = SQLHelpers.GetInt32(reader, col++);
                     retVal._useSsl = SQLHelpers.ByteToBool(reader, col++);
                     retVal._userName = SQLHelpers.GetString(reader, col++);
                     retVal._repository = SQLHelpers.GetString(reader, col++);
                     retVal._targetDirectory = SQLHelpers.GetString(reader, col++);
                     retVal._reportsDeployed = true;
                  }
                  else
                  {
                     retVal._reportServer = Dns.GetHostName();
                     retVal._reportServerDirectory = "ReportServer";
                     retVal._reportManagerDirectory = "Reports";
                     retVal._port = 80;
                     retVal._useSsl = false;
                     if (String.IsNullOrEmpty(Environment.UserDomainName))
                        retVal._userName = Environment.UserName;
                     else
                        retVal._userName = String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
                     retVal._repository = Globals.RepositoryServer;
                     retVal._targetDirectory = "SQL Compliance Manager Reports";
                     retVal._reportsDeployed = false;
                  }
		         }
		      }
            return retVal;
		   }
		   catch ( Exception ex )
		   {
		      ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                               "Reports::Read",
		                               cmdStr,
		                               ex );
            throw ex;		                               
		   }
		}
		
      //-------------------------------------------------------------
      // Write - write record to repository
      //-------------------------------------------------------------
		public void Write(SqlConnection  conn)
		{
		   string cmdStr="";
		   
		   try
		   {
		      cmdStr = GetUpdateSQL();
		      
		      using ( SqlCommand cmd = new SqlCommand( cmdStr, Globals.Repository.Connection ) )
		      {
		         cmd.ExecuteNonQuery();
		      }
		   }
		   catch ( Exception ex )
		   {
		      ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                               "Reports::Write",
		                               cmdStr,
		                               ex );
            throw ex;		                               
		   }
		}
	
		#region SQL

      //-------------------------------------------------------------
      // GetSelectSQL
      //--------------------------------------------------------------
      private static string GetSelectSQL()       
      {
          string tmp = "SELECT reportServer,"+
                              "serverVirtualDirectory,"+
                              "managerVirtualDirectory,"+
                              "port,"+
                              "useSsl,"+
                              "userName,"+
                              "repository,"+
                              "targetDirectory "+
                        "FROM {0}..{1}";
                        
         return string.Format(tmp, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryReportsTable );
      }
      
      //-------------------------------------------------------------
      // GetUpdateSQL
      //--------------------------------------------------------------
      private string GetUpdateSQL()
      {

         return String.Format("DELETE FROM {0}..{1};INSERT INTO {0}..{1} (" +
            "reportServer, serverVirtualDirectory, managerVirtualDirectory, port, useSsl, " +
            "userName, repository, targetDirectory) VALUES ({2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
            CoreConstants.RepositoryDatabase,
            CoreConstants.RepositoryReportsTable,
            SQLHelpers.CreateSafeString(_reportServer),
            SQLHelpers.CreateSafeString(_reportServerDirectory),
            SQLHelpers.CreateSafeString(_reportManagerDirectory),
            _port,
            _useSsl ? 1 : 0,
            SQLHelpers.CreateSafeString(_userName),
            SQLHelpers.CreateSafeString(_repository),
            SQLHelpers.CreateSafeString(_targetDirectory)) ;
      }
      
		
		#endregion
	}
}
