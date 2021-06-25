using System ;
using System.Data.SqlClient ;
using Idera.SQLcompliance.Core ;

namespace Idera.SQLcompliance.Application.GUI.SQL
{
	public class SQLDirect
	{
		#region Properties
		
      private     bool            _ownConnection = false;
      private     SqlConnection   _conn = null;

	   public  SqlConnection   Connection
	   {
	      get { return _conn; }
	      set { _conn=value; _ownConnection = false; }
	   }
		
		#endregion
		
		#region GetLastError

		static private string errMsg;
		static public string GetLastError()
		{
		   return errMsg;
		}
		
		#endregion
	   
	   #region Connection Management
	   
	   //-----------------------------------------------------------------------------
	   // OpenConnection - open a connection to a SQL server
	   //-----------------------------------------------------------------------------
	   public bool OpenConnection(string serverName)
	   {
	      bool retval ;
	      
	      try
	      {
	         string strConn = String.Format( "server={0};" +
	                                         "integrated security=SSPI;" +
                                            "Connect Timeout=30;" + 
	                                         "Application Name='{1}';",
	                                         serverName,
	                                         CoreConstants.ManagementConsoleName );

            // Cleanup any previous connections if necessary
            CloseConnection();

            _conn = new SqlConnection( strConn );
	         _conn.Open();
	         
	         _ownConnection = true;
	         
            errMsg = "";
	         retval = true;
	      }
	      catch (Exception ex )
	      {
            errMsg = ex.Message;
	         retval = false;
	      }

	      return retval;
	   }
	   
	   //-----------------------------------------------------------------------------
	   // CloseConnection - close the connection to the SQLsecure configuration database
	   //-----------------------------------------------------------------------------
	   public void CloseConnection()
	   {
	      if ( _ownConnection && _conn != null )
	      {
            _conn.Close();
            _conn.Dispose();
            _conn = null;
	      }
	   }
	   
	   #endregion
		

	}
}
