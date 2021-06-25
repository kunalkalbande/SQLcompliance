using SQLcomplianceIndexUpgrade.Forms;
using System;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQLcomplianceIndexUpgrade
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Logger.Initialize("SQLcomplianceIndexUpgrade", 5, 1000000);
            String connectionString = args[0].Replace(@"\\", @"\").ToString();

            string queryTimeOutDurationFromConfig = "";
            int queryTimeOut;
            queryTimeOutDurationFromConfig = ConfigurationManager.AppSettings.Get("QueryTimeoutDuration");

            if (queryTimeOutDurationFromConfig != null && queryTimeOutDurationFromConfig != "")
                queryTimeOut = Int32.Parse(queryTimeOutDurationFromConfig);
            else
                queryTimeOut = 3600;

            String IsOffline = String.Empty;
            if (args.Length > 1 && !String.IsNullOrEmpty(args[1]) && String.Equals(args[1].Trim().ToString(), "ONLINE=OFF"))
            {
                IsOffline = args[1].Trim().ToString();
                Logger.Info(IsOffline + " property is available for indexing.");
                BuildOfflineIndex(connectionString, queryTimeOut);
            }
            else
                BuildOnlineIndex(connectionString, queryTimeOut);
        }

        static void BuildOfflineIndex(String connectionString, int queryTimeoutDuration)
        {
            String edition = string.Empty;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SQLHelpers.CheckConnection(sqlConnection);
                    Logger.Info("Creating new index after upgrade, if doesn't exist.");
                    RawSQL.BuildIndexes(sqlConnection, queryTimeoutDuration);
                    Logger.Info("Checking SQL repository version.");
                    int repositorySqlVersion = SQLHelpers.GetSqlVersion(sqlConnection);                  
                    if (repositorySqlVersion >= 11)
                    {
                        //SQLCM:6269 Check for SQL Server Enterprise Edition for 2012 and 2014(Data Compression is not supported except Enterprise Edition)
                        ArrayList values = SQLHelpers.GetServerProperties(sqlConnection, "Edition");
                        if (values != null)
                        {
                            edition = Convert.ToString(values[0]);
                            if (SQLHelpers.GetSqlVersion(sqlConnection) <= 12 && (!edition.Contains("Enterprise")))
                            {
                                Logger.Info(String.Format("Data Compression is not supported for SQL Server version {0} and edition {1}.", repositorySqlVersion, edition));
                                return;
                            }
                        }
                        
                        Logger.Info("Checking event database indexes compression type.");
                        int upgradeCount = RawSQL.CheckIndexes(sqlConnection, true, queryTimeoutDuration);
                        if (upgradeCount > 0)
                        {
                            Logger.Info("Online Index page compression upgrade has been started.");
                            try
                            {
                                RawSQL.UpgradeIndexes(sqlConnection, queryTimeoutDuration);
                            }
                            catch (Exception)
                            {
                                Logger.Info("Offline Index page compression failed.");
                            }
                            Logger.Info("Index page compression upgrade has been done successfully");
                        }
                        else
                        {
                            Logger.Info("Indexes have been already upgraded to page compression.");
                        }
                        try
                        {
                            Logger.Info("Changing Datatype for DataChange and ColumnChange Identity Column.");
                            RawSQL.UpdateIdentityDatatype(sqlConnection, queryTimeoutDuration);
                            Logger.Info("Datatype changed successfully.");

                            Logger.Info("Creating indexes for Event database after upgrade.");
                            RawSQL.UpdateEventDatabaseIndexes(sqlConnection, queryTimeoutDuration);
                            Logger.Info("Index creation completed.");                            
                        }
                        catch (Exception)
                        {
                            Logger.Info("Event database index creation failure.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Unable to perform indexing :" + ex.Message.ToString() + ex.StackTrace);
            }
        }

        static void BuildOnlineIndex(String connectionString, int queryTimeoutDuration)
        {
            string edition = string.Empty;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SQLHelpers.CheckConnection(sqlConnection);

                    Logger.Info("Checking SQL repository version.");
                    int repositorySqlVersion = SQLHelpers.GetSqlVersion(sqlConnection);

                    //SQLCM:6269 Check for SQL Server Enterprise Edition for 2012 and 2014(Data Compression is not supported except Enterprise Edition)
                    ArrayList values = SQLHelpers.GetServerProperties(sqlConnection, "Edition");
                    if (values != null)
                    {
                        edition = Convert.ToString(values[0]);
                        if (repositorySqlVersion >= 11  && repositorySqlVersion <= 12 && (!edition.Contains("Enterprise")))
                        {
                            Logger.Info("Creating new index after upgrade, if doesn't exist.");
                            RawSQL.BuildIndexes(sqlConnection, queryTimeoutDuration);
                            Logger.Info(String.Format("Data Compression is not supported for SQL Server version {0} and edition {1}.", repositorySqlVersion, edition));
                            return;
                        }
                    }

                    int createCount = RawSQL.CheckIndexes(sqlConnection,false, queryTimeoutDuration);
                    if (createCount < 7)
                    {
                        Logger.Info("Creating new index after upgrade, if doesn't exist.");
                        RawSQL.BuildOnlineIndexes(sqlConnection, queryTimeoutDuration);
                    }                                     
                    int upgradeCount = RawSQL.CheckIndexes(sqlConnection, true, queryTimeoutDuration);
                    if (repositorySqlVersion >= 11)
                    {                        
                        Logger.Info("Checking event database indexes compression type.");
                        if (upgradeCount > 0)
                        {
                            Logger.Info("Online Index page compression upgrade has been started.");
                            RawSQL.UpgradeIndexesOnline(sqlConnection, queryTimeoutDuration);
                            Logger.Info("Index page compression upgrade has been done successfully");
                        }
                        else
                        {
                            Logger.Info("Indexes have been already upgraded to page compression.");
                        }
                    }
                    try
                    {
                        Logger.Info("Changing Datatype for DataChange and ColumnChange Identity Column.");
                        RawSQL.UpdateIdentityDatatype(sqlConnection, queryTimeoutDuration);
                        Logger.Info("Datatype changed successfully.");

                        Logger.Info("Creating online indexes for Event database after upgrade.");
                        RawSQL.UpdateEventDatabaseOnlineIndexes(sqlConnection, queryTimeoutDuration);
                        Logger.Info("Index creation completed.");
                    }
                    catch (Exception)
                    {
                        Logger.Info("Event database index creation failure.");
                    }
                }
            }
            catch (Exception ex)
            {
                Form_MessageBox frm = new Form_MessageBox();
                frm.ShowDialog();
                Logger.Info("Online Index page compression failed, retry offline index page compression using command line.");
                Logger.Info("Unable to perform indexing :" + ex.Message.ToString() + ex.StackTrace);
                return;
            }
        }
    }
}
