using System;
using System.IO;
using System.Text;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for BulkInsert.
	/// </summary>
	internal class BulkInsert
	{
	   private string            m_fileName;
	   private string            m_databaseName;
	   private int               m_rows = 0;
      private ASCIIEncoding   m_encoding;
      private Repository        m_rep;
      
      private FileStream        m_fileStream;
      

      //------------------------------------------------------------------	   
      // Constructor
      //------------------------------------------------------------------	   
		public
		   BulkInsert(
		      string        traceFileName,
		      string        databaseName
		   )
		{
		   m_fileName     = Path.ChangeExtension( traceFileName, ".bcp" );
		   m_databaseName = databaseName;
		   m_rows         = 0;
		   m_encoding     = new ASCIIEncoding();
         m_rep          = new Repository();
		}
		
      //------------------------------------------------------------------	   
      // WriteRow
      //------------------------------------------------------------------	   
		internal void
		   WriteRow(
		      string   row
		   )
		{
		   if ( m_rows == 0 )
		   {
            m_rep.OpenConnection( m_databaseName );
            m_fileStream = File.Create( m_fileName, 16384 );
		   }
		   
		   Byte[] bytes = m_encoding.GetBytes(row);
         m_fileStream.Write( bytes, 0, bytes.Length );
         m_rows++;
/*		   
		   if ( m_rows == maxRows )
		   {
		      DoInsert()
		      Truncate File
		      m_rows = 0;
		   }
*/		   
		}
		
      //------------------------------------------------------------------	   
      // CloseLoadAndDelete
      //------------------------------------------------------------------	   
		internal void
		   CloseLoadAndDelete()
		{
		   // Load via Bulk Insert into events database
		   if ( m_rows != 0 )
		   {
		      // Close
		      m_fileStream.Close();
		      
		      DoInsert();
		      
		      m_rep.CloseConnection();
		   
		      // Delete
/*		      
            try
            {
               File.Delete( m_fileName );
            }
            catch( Exception ex)
            {
		         ErrorLog.Instance.Write( "BulkInsert::Delete",
		                                 m_fileName,
		                                 ex,
		                                 ErrorLog.Severity.Warning );
            }
*/            
		   }
		}
		
      //------------------------------------------------------------------	   
      // DoInsert
      //------------------------------------------------------------------	   
      private int
         DoInsert()
      {
         int nRows = -1;
         string bcpCommand = "";
         try
         {
            bcpCommand = String.Format( "BULK INSERT {0}..{1} FROM {2}" +
                                         " WITH (" +
                                         "    BATCHSIZE = 1000," +
                                         "    DATAFILETYPE = 'char'," +
                                         "    FIELDTERMINATOR = '\\0\\0'," +
                                         "    ROWTERMINATOR = '\\0\\0\\n'" +
                                         " )",
                                        SQLHelpers.CreateSafeDatabaseName(m_databaseName),
                                        CoreConstants.RepositoryEventsTable,
                                        SQLHelpers.CreateSafeString(m_fileName) );
            using ( SqlCommand cmd = new SqlCommand( bcpCommand, m_rep.connection ) )
            {
	            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
               nRows = cmd.ExecuteNonQuery();
            }
         }
         catch ( Exception ex )
         {
		      ErrorLog.Instance.Write( String.Format( "BulkInsert::DoInsert ({0})", m_fileName),
		                               bcpCommand,
		                               ex,
		                               ErrorLog.Severity.Warning );
		      throw ex;
         }                                            
         return nRows;
      }
	}
}
