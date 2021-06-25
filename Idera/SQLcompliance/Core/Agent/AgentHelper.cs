using System ;
using System.Collections ;
using System.Data.SqlClient ;
using Idera.SQLcompliance.Core.Collector ;
using Idera.SQLcompliance.Core.Remoting;

namespace Idera.SQLcompliance.Core.Agent
{
   public class AgentHelper
   {
      public static SqlConnection GetConnection(string server)
      {
         string connectionString = String.Format("server={0};" +
                                                 "database={1};" +
                                                 "integrated security=SSPI;" +
                                                 "Connect Timeout=30;" +
                                                 "Application Name='{2}';",
                                                 server,
                                                 CoreConstants.RepositoryDatabase,
                                                 CoreConstants.ManagementConsoleName) ;
         SqlConnection conn = new SqlConnection(connectionString);
         conn.Open();
         return conn;
      }

      //-------------------------------------------------------------
      // LoadDatabases
      //--------------------------------------------------------------
      public static IList LoadAllDatabases(SQLcomplianceConfiguration configuration,
                                                           ServerRecord targetServer)
      {
         IList dbList = null;
         IList sysList = null;

         // load database list via agent (if deployed)
         if (targetServer.IsDeployed && targetServer.IsRunning)
         {
             string url = EndPointUrlBuilder.GetUrl(typeof(AgentManager), configuration.Server, configuration.ServerPort);
            try
            {
               AgentManager manager = CoreRemoteObjectsProvider.AgentManager(configuration.Server, configuration.ServerPort);

               dbList = manager.GetRawUserDatabases(targetServer.Instance);
               sysList = manager.GetRawSystemDatabases(targetServer.Instance);
            }
            catch (Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("LoadDatabases: URL: {0} Instance {1}", url, targetServer.Instance),
                                        ex,
                                        ErrorLog.Severity.Warning);
               dbList = null;
            }
         }

         // try straight connection to SQL Server if agent connection failed
         if (dbList == null)
         {
            try
            {
               using(SqlConnection conn = GetConnection(targetServer.Instance))
               {
                  dbList = RawSQL.GetUserDatabases(conn) ;
                  sysList = RawSQL.GetSystemDatabases(conn);
               }
            }catch(Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                        String.Format("LoadDatabases: Direction to Instance {0}", targetServer.Instance),
                                        ex,
                                        ErrorLog.Severity.Warning);
               dbList = null;

            }
         }
         if (dbList != null && sysList != null)
         {
            foreach (object o in sysList)
               dbList.Add(o);
         }
         return dbList ;
      }
   }
}
