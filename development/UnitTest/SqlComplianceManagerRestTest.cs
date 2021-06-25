using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Idera.SQLcompliance.Core.Cwf;
using Idera.SQLcompliance.Core.Event;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginCommon;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RemoveServers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings;
using UnitTest.Properties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;

namespace UnitTest
{
    /// <summary>
    /// Unit tests for SQLcm CWF AddIn REST API.
    /// To run this class a SQLcm product should be registered and CWF core service must be running.
    /// All configuration settings to connect REST API are in App.config. Change them according to the machine.
    /// </summary>
    [TestClass]
    public class SqlComplianceManagerRestTest
    {
        #region declares
        private int SERVER_ID = 1;
        private int REMOTE_SERVER_ID = 2;
        private int DATABASE_ID = 1;
        private string DATABASE_NAME = "test";
        private string SERVER_INSTANCE = "RTDEVPC";
        private string CLUSTER_SERVER_INSTANCE = "AOAGLISTENER";
        private string REMOTE_SERVER_INSTANCE = "IF036\\MSSQLSERVER_2";

        private static string _restApiUrl;
        private static string _token;
        private static int _productId;
        private static int _days;
        #endregion

        #region private methods

        private static string GetAuthorizationHeader()
        {
            return string.Format("Basic {0}", _token);
        }

        /// <summary>
        /// This method returns an HttpWebRequest object given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method">request method like GET, POST, etc.</param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest(string url, string method)
        {
            try
            {
                var uri = new Uri(url);

                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Host = string.Format("{0}:{1}", uri.Host, uri.Port);
                request.Method = method;
                request.ContentType = "application/json";

                request.Headers.Add(HttpRequestHeader.Authorization, GetAuthorizationHeader());
                return request;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// This method returns the JSON response in string format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string MakeGetRequest(string url)
        {
            var request = CreateRequest(url, "GET");
            var responseValue = string.Empty;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                request = null;
            }

            return responseValue;
        }

        /// <summary>
        /// This method returns the JSON response in string format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private string MakePostRequest(string url, String postData)
        {
            var request = CreateRequest(url, "POST");
            var responseValue = string.Empty;
            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                request = null;
            }

            return responseValue;
        }

        #endregion

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // read settings from app.config
            var cwfHost = Settings.Default.CwfHost;
            var cwfport = Settings.Default.CwfPort;
            var cwfUser = Settings.Default.CwfUser;
            var cwfPassword = Settings.Default.CwfPassword;
            _productId = Settings.Default.ProductId;
            _days = Settings.Default.Days;

            Console.Write("Configuration settings to run tests are as follows:");
            Console.WriteLine("CWF Host: {0}", cwfHost);
            Console.WriteLine("CWF Port: {0}", cwfport);
            Console.WriteLine("CWF User: {0}", cwfUser);
            Console.WriteLine("CWF Password: {0}", cwfPassword);
            Console.WriteLine("Product ID: {0}", _productId);
            Console.WriteLine("Days Constant: {0}", _days);
            Console.WriteLine();

            _restApiUrl = string.Format("http://{0}:{1}/sqlcm/v1/{2}", cwfHost, cwfport, _productId);
            Console.WriteLine("REST API URL: {0}", _restApiUrl);

            _token = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(cwfUser + ":" + cwfPassword, "\\\\", "\\")));
        }

        [AssemblyCleanup]
        public static void CleanUp()
        {
            _restApiUrl = null;
            _token = null;
        }

        [TestMethod]
        public void GetAllInstancesStatusTest()
        {
            var requestUrl = string.Format("{0}/GetAllInstancesStatus", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<AuditedServerStatus>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAllInstancesAlertsTest()
        {
            var requestUrl = string.Format("{0}/GetAllInstancesAlerts", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<ServerAlert>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetEnvironmentObjectsTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, -1, (byte)EnvironmentObjectType.Root);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                return;

            // get all audited databases
            requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, result[0].Id, (byte)result[0].Type);
            jsonResult = MakeGetRequest(requestUrl);
            result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetMyCmEnvironmentTest()
        {
            var requestUrl = string.Format("{0}/GetEnvironmentDetails", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<EnvironmentDetails>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetArchivedPropertiesTest()
        {
            var requestUrl = string.Format("{0}/GetArchiveProperties?archive={1}", _restApiUrl, "SQLcmArchive_RTDEVPC_2015_Q2");
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<EnvironmentDetails>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetEnvironmentAlertStatusTest()
        {
            var requestUrl = string.Format("{0}/GetEnvironmentAlertStatus", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<EnvironmentAlertStatus>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditedInstancesAlertTest()
        {
            // get all alerts
            var requestUrl = string.Format("{0}/GetAuditedInstancesAlert?alertType={1}", _restApiUrl, (byte)AlertType.All);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<ServerAlert>>(jsonResult);
            if (result == null)
                Assert.Fail();

            // get data alerts
            requestUrl = string.Format("{0}/GetAuditedInstancesAlert?alertType={1}", _restApiUrl, (byte)AlertType.Data);
            jsonResult = MakeGetRequest(requestUrl);
            result = JsonHelper.FromJson<List<ServerAlert>>(jsonResult);
            if (result == null)
                Assert.Fail();

            // get event alerts
            requestUrl = string.Format("{0}/GetAuditedInstancesAlert?alertType={1}", _restApiUrl, (byte)AlertType.Event);
            jsonResult = MakeGetRequest(requestUrl);
            result = JsonHelper.FromJson<List<ServerAlert>>(jsonResult);
            if (result == null)
                Assert.Fail();

            // get status alerts
            requestUrl = string.Format("{0}/GetAuditedInstancesAlert?alertType={1}", _restApiUrl, (byte)AlertType.Status);
            jsonResult = MakeGetRequest(requestUrl);
            result = JsonHelper.FromJson<List<ServerAlert>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetEventsTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, -1, (byte)EnvironmentObjectType.Root);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");

            requestUrl = string.Format("{0}/GetEvents?instanceId={1}&days={2}", _restApiUrl, result[0].Id, _days);
            jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<List<ServerAlert>>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetRecentDatabaseActivityTest()
        {
            var requestUrl = string.Format("{0}/GetRecentDatabaseActivity?instanceId={1}&databaseId={2}&days={3}", _restApiUrl, 1, 3, 30);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<KeyValuePair<RestStatsCategory, List<RestStatsData>>>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetStatsDataTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, -1, (byte)EnvironmentObjectType.Root);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");

            requestUrl = string.Format("{0}/GetStatsData?instanceId={1}&days={2}&category={3}", _restApiUrl, result[0].Id, _days, (byte)RestStatsCategory.Ddl);
            jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<List<KeyValuePair<RestStatsCategory, List<RestStatsData>>>>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetDatabaseEventsTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, -1, (byte)EnvironmentObjectType.Root);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");

            SERVER_ID = result[0].Id;

            // get all audited databases
            requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, SERVER_ID, (byte)result[0].Type);
            jsonResult = MakeGetRequest(requestUrl);
            result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No monitered database found.");

            // get event stats
            requestUrl = string.Format("{0}/GetDatabaseStatsData?instanceId={1}&databaseId={2}&days={3}&category={4}", _restApiUrl, SERVER_ID, result[0].Id, _days, (byte)RestStatsCategory.EventProcessed);
            jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<List<RestStatsData>>(jsonResult);
            if (result1 == null)
                Assert.Fail();

            // get failed login stats
            requestUrl = string.Format("{0}/GetDatabaseStatsData?instanceId={1}&databaseId={2}&days={3}&category={4}", _restApiUrl, SERVER_ID, result[0].Id, _days, (byte)RestStatsCategory.FailedLogin);
            jsonResult = MakeGetRequest(requestUrl);
            result1 = JsonHelper.FromJson<List<RestStatsData>>(jsonResult);
            if (result1 == null)
                Assert.Fail();

            // get security stats
            requestUrl = string.Format("{0}/GetDatabaseStatsData?instanceId={1}&databaseId={2}&days={3}&category={4}", _restApiUrl, SERVER_ID, result[0].Id, _days, (byte)RestStatsCategory.Security);
            jsonResult = MakeGetRequest(requestUrl);
            result1 = JsonHelper.FromJson<List<RestStatsData>>(jsonResult);
            if (result1 == null)
                Assert.Fail();

            // get DDL stats
            requestUrl = string.Format("{0}/GetDatabaseStatsData?instanceId={1}&databaseId={2}&days={3}&category={4}", _restApiUrl, SERVER_ID, result[0].Id, _days, (byte)RestStatsCategory.Ddl);
            jsonResult = MakeGetRequest(requestUrl);
            result1 = JsonHelper.FromJson<List<RestStatsData>>(jsonResult);
            if (result1 == null)
                Assert.Fail();

            // get priviledged user stats
            requestUrl = string.Format("{0}/GetDatabaseStatsData?instanceId={1}&databaseId={2}&days={3}&category={4}", _restApiUrl, SERVER_ID, result[0].Id, _days, (byte)RestStatsCategory.PrivUserEvents);
            jsonResult = MakeGetRequest(requestUrl);
            result1 = JsonHelper.FromJson<List<RestStatsData>>(jsonResult);
            if (result1 == null)
                Assert.Fail();

            // get all stats
            requestUrl = string.Format("{0}/GetDatabaseStatsData?instanceId={1}&databaseId={2}&days={3}&category={4}", _restApiUrl, SERVER_ID, result[0].Id, _days, (byte)RestStatsCategory.Unknown);
            jsonResult = MakeGetRequest(requestUrl);
            result1 = JsonHelper.FromJson<List<RestStatsData>>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void CanAddOneMoreInstanceTest()
        {
            var requestUrl = string.Format("{0}/CanAddOneMoreInstance", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (string.IsNullOrEmpty(jsonResult))
                Assert.Fail();
        }

        [TestMethod]
        public void GetCmLicensesTest()
        {
            var requestUrl = string.Format("{0}/GetCmLicenses", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (string.IsNullOrEmpty(jsonResult))
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditedSqlServerTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/GetEnvironmentObjects?objectId={1}&type={2}", _restApiUrl, -1, (byte)EnvironmentObjectType.Root);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");

            SERVER_ID = result[0].Id;

            requestUrl = string.Format("{0}/GetAuditedSqlServer?id={1}", _restApiUrl, SERVER_ID);
            jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<AuditedServerStatus>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditedSqlServerTest2()
        {
            var days = 7;
            var requestUrl = string.Format("{0}/GetAuditedSqlServer?id={1}&days={2}", _restApiUrl, SERVER_ID, days);
            var jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<AuditedServerStatus>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetFilteredAuditedInstancesStatuses()
        {
            var requestUrl = string.Format("{0}/GetFilteredAuditedInstancesStatus", _restApiUrl);
            FilteredRegisteredInstancesStatusRequest statusRequest = new FilteredRegisteredInstancesStatusRequest();
            statusRequest.SqlVersion = "Microsoft SQL Server 2012";
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(statusRequest));
            var result = JsonHelper.FromJson<List<AuditedServerStatus>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditedDatabaseTest()
        {
            var requestUrl = string.Format("{0}/GetAuditedDatabase?id={1}&serverId={2}", _restApiUrl, DATABASE_ID, SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<AuditedServer>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditedDatabasesForInstaceTest()
        {
            var requestUrl = string.Format("{0}/GetAuditedDatabasesForInstance?parentId={1}", _restApiUrl, SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<AuditedServer>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetNotRegisteredDatabasesForInstanceTest()
        {
            var requestUrl = string.Format("{0}/GetNotRegisteredDatabasesForInstance?parentId={1}", _restApiUrl, REMOTE_SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            var result1 = JsonHelper.FromJson<AuditedServer>(jsonResult);
            if (result1 == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetDatabaseAvailabilityGroups()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//GetDatabaseAvailabilityGroups", _restApiUrl, 1);
            var dummyDatabases = new List<AuditedDatabaseInfo>
            {
                new AuditedDatabaseInfo
                {
                    Id = DATABASE_ID,
                    ServerId = SERVER_ID,
                    Name = DATABASE_NAME,
                },
            };

            var jsonRequest = JsonHelper.ToJson(dummyDatabases);
            var jsonResult = MakePostRequest(requestUrl, jsonRequest);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");
        }

        [TestMethod]
        public void GetServerRoleUsersForInstanceTest()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//GetServerRoleUsersForInstance?serverId={1}", _restApiUrl, 1);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");
        }

        [TestMethod]
        public void VerifyPermissionsTest()
        {
            var requestUrl = string.Format("{0}//VerifyPermissions?serverId={1}", _restApiUrl, SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            var result = JsonHelper.FromJson<PermissionChecksStatus>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetDatabaseTableListTest()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//GetDatabaseTableList", _restApiUrl);
            var filter = new DatabaseTableFilter
            {
                ServerId = SERVER_ID,
                DatabaseName = DATABASE_NAME,
                TableNameSearchText = ".",
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(filter));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();

            if (result.Count == 0)
                Assert.Fail("No SQL server instance found.");
        }

        [TestMethod]
        //Default Settings
        public void AddDatabasesWithDefaultSettingsTest()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddDatabases", _restApiUrl);

            var auditDatabaseSettings = new AuditDatabaseSettings()
            {
                DatabaseList = new List<AuditedDatabaseInfo>()
                {
                    new AuditedDatabaseInfo
                    {
                        Id = 9,
                        ServerId = SERVER_ID,
                        Name = "test",
                    },
                },

                CollectionLevel = AuditCollectionLevel.Default,
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditDatabaseSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        //Custom Settings
        public void AddDatabasesCustomSettingsTest()
        {
            var requestUrl = string.Format("{0}//AddDatabases", _restApiUrl);

            var auditDatabaseSettings = new AuditDatabaseSettings()
            {
                DatabaseList = new List<AuditedDatabaseInfo>()
                {
                    new AuditedDatabaseInfo
                    {
                        Id = 9,
                        ServerId = SERVER_ID,
                        Name = "Test",
                    },
                },

                CollectionLevel = AuditCollectionLevel.Custom,

                AuditedActivities = new AuditActivity()
                {
                    AuditAdmin = true,
                    AuditCaptureSQL = true,
                    AuditCaptureTrans = true,
                    AuditDDL = true,
                    AuditDML = true,
                    AuditSecurity = true,
                    AuditSELECT = true,
                    AuditAccessCheck = AccessCheckFilter.FailureOnly,
                },

                DmlSelectFilters = new DmlSelectFilters
                {
                    AuditDmlAll = false,
                    AuditUserTables = AuditUserTables.All,
                    AuditSystemTables = true,
                    AuditStoredProcedures = true,
                    AuditDmlOther = true,
                },

                TrustedRolesAndUsers = new ServerRolesAndUsers()
                {
                    RoleList = new List<ServerRole>()
                    {
                        new ServerRole()
                        {
                            FullName = "System Administrators",
                            Id = 16,
                            Name = "System Administrators",
                        },
                    },

                    UserList = new List<ServerLogin>()
                    {
                        new ServerLogin()
                        {
                            Name = "RTDEVPC\\Administrator",
                            Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA=="
                        }
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditDatabaseSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        //[TestMethod]
        ////Custom Settings
        //public void AddDatabasesWithRegulationSettingsTest()
        //{
        //    var requestUrl = string.Format("{0}//AddDatabases", _restApiUrl);

        //    var auditDatabaseSettings = new AuditDatabaseSettings()
        //    {
        //        DatabaseList = new List<AuditedDatabaseInfo>()
        //        {
        //            new AuditedDatabaseInfo
        //            {
        //                Id = 9,
        //                ServerId = SERVER_ID,
        //                Name = "Test",

        //                SensitiveColumnTableList = new List<DatabaseObject>
        //                {
        //                        new DatabaseObject()
        //                        {
        //                            Id = 0,
        //                            ObjectId = 565577053,
        //                            ServerId = SERVER_ID,
        //                            RowLimit = 0,
        //                            ColumnList = null,
        //                            SelectedColumns = false,
        //                            TableName = "Tests",
        //                            FullTableName = "dbo.Tests",
        //                            SchemaName = "dbo",
        //                            ObjectType = ObjectType.Table,
        //                        },
        //                },
        //            },
        //        },

        //        CollectionLevel = AuditCollectionLevel.Regulation,

        //        RegulationSettings = new AuditRegulationSettings()
        //        {
        //            PCI = true,
        //            HIPAA = true,
        //        },

        //        PrivilegedRolesAndUsers = new ServerRolesAndUsers()
        //        {
        //            UserList = new List<ServerLogin>()
        //            {
        //                new ServerLogin()
        //                {
        //                    Sid = "AQ==",
        //                    Name = "sa",
        //                },

        //                new ServerLogin()
        //                {
        //                    Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA==",
        //                    Name = "RTDEVPC\\Administrator",
        //                },
        //            },

        //            RoleList = new List<ServerRole>()
        //            {
        //                new ServerRole()
        //                {
        //                    Id = 16,
        //                    Name = "sysadmin",
        //                    FullName = "System Administrators",
        //                },

        //                new ServerRole()
        //                {
        //                    Id = 32,
        //                    Name = "securityadmin",
        //                    FullName = "Security Administrators",
        //                },
        //            },
        //        },
        //    };

        //    var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditDatabaseSettings));
        //    var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
        //    if (result == null)
        //        Assert.Fail();
        //}

        //[TestMethod]
        ////Add database with specified server audit settings
        //public void AddDatabaseTest4()
        //{
        //    var requestUrl = string.Format("{0}//AddDatabases", _restApiUrl);

        //    var auditDatabaseSettings = new AuditDatabaseSettings()
        //    {
        //        DatabaseList = new List<AuditedDatabaseInfo>()
        //        {
        //            new AuditedDatabaseInfo
        //            {
        //                Id = 9,
        //                ServerId = SERVER_ID,
        //                Name = "Test",

        //                SensitiveColumnTableList = new List<DatabaseObject>
        //                {
        //                        new DatabaseObject()
        //                        {
        //                            Id = 0,
        //                            ObjectId = 565577053,
        //                            ServerId = SERVER_ID,
        //                            RowLimit = 0,
        //                            ColumnList = null,
        //                            SelectedColumns = false,
        //                            TableName = "Tests",
        //                            FullTableName = "dbo.Tests",
        //                            SchemaName = "dbo",
        //                            ObjectType = ObjectType.Table,
        //                        },
        //                },
        //            },
        //        },

        //        CollectionLevel = AuditCollectionLevel.Custom,

        //        UpdateServerSettings = true,

        //        ServerSettingsToBeUpdated = new ServerSettingsData
        //        {
        //            ServerId = SERVER_ID,
        //            ServerAuditedActivities = new AuditActivity()
        //            {
        //                AuditLogins = true,
        //                AuditFailedLogins = true,
        //                AuditSecurity = true,
        //                AuditDDL = true,
        //                AuditAdmin = true,
        //                AuditDefinedEvents = true,
        //                AuditAccessCheck = AccessCheckFilter.SuccessOnly,
        //            },
        //            DatabasePermissions = DatabaseReadAccessLevel.EventsOnly,

        //        },

        //        PrivilegedRolesAndUsers = new ServerRolesAndUsers()
        //        {
        //            UserList = new List<ServerLogin>()
        //            {
        //                new ServerLogin()
        //                {
        //                    Sid = "AQ==",
        //                    Name = "sa",
        //                },

        //                new ServerLogin()
        //                {
        //                    Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA==",
        //                    Name = "RTDEVPC\\Administrator",
        //                },
        //            },

        //            RoleList = new List<ServerRole>()
        //            {
        //                new ServerRole()
        //                {
        //                    Id = 16,
        //                    Name = "sysadmin",
        //                    FullName = "System Administrators",
        //                },

        //                new ServerRole()
        //                {
        //                    Id = 32,
        //                    Name = "securityadmin",
        //                    FullName = "Security Administrators",
        //                },
        //            },
        //        },

        //        AuditedActivities = new AuditActivity()
        //        {
        //            AuditAdmin = true,
        //            AuditCaptureSQL = true,
        //            AuditCaptureTrans = true,
        //            AuditDDL = true,
        //            AuditDML = true,
        //            AuditSecurity = true,
        //            AuditSELECT = true,
        //            AuditAccessCheck = AccessCheckFilter.FailureOnly,
        //        },
        //    };

        //    var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditDatabaseSettings));
        //    var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
        //    if (result == null)
        //        Assert.Fail();
        //}

        //[TestMethod]
        ////Add database with specified regulation and server audit settings
        //public void AddDatabasesTest5()
        //{
        //    var requestUrl = string.Format("{0}//AddDatabases", _restApiUrl);

        //    var auditDatabaseSettings = new AuditDatabaseSettings()
        //    {
        //        DatabaseList = new List<AuditedDatabaseInfo>()
        //        {
        //            new AuditedDatabaseInfo
        //            {
        //                Id = 9,
        //                ServerId = SERVER_ID,
        //                Name = "Test",

        //                SensitiveColumnTableList = new List<DatabaseObject>
        //                {
        //                        new DatabaseObject()
        //                        {
        //                            Id = 0,
        //                            ObjectId = 565577053,
        //                            ServerId = SERVER_ID,
        //                            RowLimit = 0,
        //                            ColumnList = null,
        //                            SelectedColumns = false,
        //                            TableName = "Tests",
        //                            FullTableName = "dbo.Tests",
        //                            SchemaName = "dbo",
        //                            ObjectType = ObjectType.Table,
        //                        },
        //                },
        //            },
        //        },

        //        CollectionLevel = AuditCollectionLevel.Regulation,

        //        RegulationSettings = new AuditRegulationSettings()
        //        {
        //            PCI = true,
        //            HIPAA = true,
        //        },

        //        PrivilegedRolesAndUsers = new ServerRolesAndUsers()
        //        {
        //            UserList = new List<ServerLogin>()
        //            {
        //                new ServerLogin()
        //                {
        //                    Sid = "AQ==",
        //                    Name = "sa",
        //                },

        //                new ServerLogin()
        //                {
        //                    Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA==",
        //                    Name = "RTDEVPC\\Administrator",
        //                },
        //            },

        //            RoleList = new List<ServerRole>()
        //            {
        //                new ServerRole()
        //                {
        //                    Id = 16,
        //                    Name = "sysadmin",
        //                    FullName = "System Administrators",
        //                },

        //                new ServerRole()
        //                {
        //                    Id = 32,
        //                    Name = "securityadmin",
        //                    FullName = "Security Administrators",
        //                },
        //            },
        //        },

        //        UpdateServerSettings = true,

        //        ServerSettingsToBeUpdated = new ServerSettingsData
        //        {
        //            ServerId = SERVER_ID,
        //            ServerAuditedActivities = new AuditActivity()
        //            {
        //                AuditLogins = true,
        //                AuditFailedLogins = true,
        //                AuditSecurity = true,
        //                AuditDDL = true,
        //                AuditAdmin = true,
        //                AuditDefinedEvents = true,
        //                AuditAccessCheck = AccessCheckFilter.SuccessOnly,
        //            },
        //            DatabasePermissions = DatabaseReadAccessLevel.EventsOnly,

        //        },
        //    };

        //    var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditDatabaseSettings));
        //    var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
        //    if (result == null)
        //        Assert.Fail();
        //}


        [TestMethod]
        public void RemoveDatabaseTest()
        {
            var removeDatabaseRequest = new RemoveDatabaseRequest
            {
                DatabaseId = DATABASE_ID,
            };
            var requestUrl = string.Format("{0}//RemoveDatabase", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(removeDatabaseRequest));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void EnableAuditingForDatabasesTest()
        {
            var databases = new EnableAuditForDatabases()
            {
                DatabaseIdList = new List<int>()
                {
                    18
                },
                Enable = false,
            };

            var requestUrl = string.Format("{0}//EnableAuditingForDatabases", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(databases));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditDatabasePropertiesTest()
        {
            var requestUrl = string.Format("{0}//GetAuditDatabaseProperties?databaseId={1}", _restApiUrl, DATABASE_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void UpdateAuditDatabasePropertiesTest()
        {
            var requestUrl = string.Format("{0}//UpdateAuditDatabaseProperties", _restApiUrl);

            var auditDatabaseProperties = new AuditDatabaseProperties()
            {
                DatabaseId = DATABASE_ID,
                ServerInstance = SERVER_INSTANCE,
                DatabaseName = DATABASE_NAME,
                Description = "test description",

                //General - Status
                AuditingEnableStatus = true,
                CreatedDateTime = DateTime.Parse("05/01/15 11:56:29 AM"),
                LastModifiedDateTime = DateTime.Parse("05/14/15 09:53:46 AM"),
                LastChangedStatusDateTime = DateTime.Parse("05/06/15 08:54:03 AM"),

                AuditedActivities = new AuditActivity()
                {
                    AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                    AuditAdmin = true,
                    AuditAllUserActivities = false,
                    AuditCaptureSQL = true,
                    AuditCaptureTrans = true,
                    AuditDDL = true,
                    AuditDML = true,
                    AuditFailedLogins = false,
                    AuditSecurity = true,
                    AuditSELECT = true,
                    AuditDefinedEvents = false,
                    AuditLogins = false,
                },

                AuditedPrivilegedUserActivities = new AuditActivity()
                {
                    AuditAccessCheck = AccessCheckFilter.FailureOnly,
                    AuditAdmin = true,
                    AuditAllUserActivities = false,
                    AuditCaptureSQL = true,
                    AuditCaptureTrans = true,
                    AuditDDL = true,
                    AuditDML = true,
                    AuditFailedLogins = true,
                    AuditSecurity = true,
                    AuditSELECT = true,
                    AuditDefinedEvents = true,
                    AuditLogins = true,
                },

                DmlSelectFilters = new DmlSelectFilters()
                {
                    AuditDmlAll = false,
                    AuditDmlOther = true,
                    AuditStoredProcedures = true,
                    AuditSystemTables = true,
                    AuditUserTables = AuditUserTables.Following,

                    UserTableList = new List<DatabaseObject>()
                    {
                        new DatabaseObject()
                        {
                            Id = 245575913,
                            ObjectId = 245575913,
                            DatabaseId = DATABASE_ID,
                            ServerId = SERVER_ID,
                            RowLimit = 0,
                            ColumnList = null,
                            SelectedColumns = false,
                            TableName = "Table1",
                            FullTableName = "dbo.Table1",
                            SchemaName = "dbo",
                            ObjectType = ObjectType.Table,
                        },
                    }
                },


                TrustedRolesAndUsers = new ServerRolesAndUsers()
                {
                    RoleList = new List<ServerRole>()
                    {
                        new ServerRole()
                        {
                            FullName = "System Administrators",
                            Id = 16,
                            Name = "System Administrators",
                        },
                    },

                    UserList = new List<ServerLogin>()
                    {
                        new ServerLogin()
                        {
                            Name = "RTDEVPC\\Administrator",
                            Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA=="
                        }
                    },
                },

                PrivilegedRolesAndUsers = new ServerRolesAndUsers()
                {
                    RoleList = new List<ServerRole>()
                    {
                        new ServerRole()
                        {
                            FullName = "System Administrators",
                            Id = 16,
                            Name = "System Administrators",
                        },
                    },

                    UserList = new List<ServerLogin>()
                    {
                        new ServerLogin()
                        {
                            Name = "RTDEVPC\\Administrator",
                            Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA=="
                        }
                    },
                },

                AuditBeforeAfterData = new AuditBeforeAfterData()
                {
                    BeforeAfterTableColumnDictionary = new Dictionary<string, DatabaseObject>()
                    {
                        {
                            "dbo.Table1",
                            new DatabaseObject()
                            {
                                Id = 245575913,
                                ObjectId = 245575913,
                                DatabaseId = DATABASE_ID,
                                ServerId = SERVER_ID,
                                RowLimit = 0,
                                ColumnList = null,
                                SelectedColumns = false,
                                TableName = "Table1",
                                FullTableName = "dbo.Table1",
                                SchemaName = "dbo",
                                ObjectType = ObjectType.Table,
                            }
                        },                        

                        {
                            "dbo.Table2",
                            new DatabaseObject()
                            {
                                Id = 261575970,
                                ObjectId = 261575970,
                                DatabaseId = DATABASE_ID,
                                ServerId = SERVER_ID,
                                RowLimit = 0,
                                ColumnList = null,
                                SelectedColumns = false,
                                TableName = "Table2",
                                FullTableName = "dbo.Table2",
                                SchemaName = "dbo",
                                ObjectType = ObjectType.Table,
                            }
                        },   

                        {
                            "dbo.Table3",
                            new DatabaseObject()
                            {
                                Id = 277576027,
                                ObjectId = 277576027,
                                DatabaseId = DATABASE_ID,
                                ServerId = SERVER_ID,
                                RowLimit = 0,
                                ColumnList = null,
                                SelectedColumns = false,
                                TableName = "Table3",
                                FullTableName = "dbo.Table3",
                                SchemaName = "dbo",
                                ObjectType = ObjectType.Table,
                            }
                        },   

                    },


                    ClrStatus = new ClrStatus()
                    {
                        IsConfigured = true,
                        IsRunning = true,
                        StatusMessage = "CLR is enabled for RTDEVPC."
                    },

                    ColumnsSupported = true,
                    IsAvailable = true,
                    MissingTableStatusMessage = null,
                    StatusMessaage = null,
                },

                SensitiveColumnTableData = new SensitiveColumnTableData()
                {
                    ColumnsSupported = false,
                    MissingTableStatusMessage = null,
                    SensitiveTableColumnDictionary = new Dictionary<string, DatabaseObject>()
                    {
                        {
                            "dbo.Table3",
                            new DatabaseObject()
                            {
                                Id = 277576027,
                                ObjectId = 277576027,
                                DatabaseId = DATABASE_ID,
                                ServerId = SERVER_ID,
                                RowLimit = 0,
                                ColumnList = null,
                                SelectedColumns = false,
                                TableName = "Table3",
                                FullTableName = "dbo.Table3",
                                SchemaName = "dbo",
                                ObjectType = ObjectType.Table,
                            }
                        },  
                    },

                    StatusMessaage = null,
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditDatabaseProperties));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetRegulationTypeListTest()
        {
            var requestUrl = string.Format("{0}//GetRegulationTypeList", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetRegulationSectionDictionaryTest()
        {
            var requestUrl = string.Format("{0}//GetRegulationSectionDictionary", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void ValidateCredentialsTest()
        {
            var status = new Credentials
            {
                Account = @"RTDEVPC\Administrator",
                Password = "abc12345678",
            };

            var requestUrl = string.Format("{0}//ValidateCredentials", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(status));
            if (jsonResult == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetAuditedInstancesTest()
        {
            var requestUrl = string.Format("{0}//GetAuditedInstances", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAllNotRegisteredInstanceNameListTest()
        {
            var requestUrl = string.Format("{0}//GetAllNotRegisteredInstanceNameList", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAuditServerPropertiesTest()
        {
            var requestUrl = string.Format("{0}//GetAuditServerProperties?serverId={1}", _restApiUrl, SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        // Simple update
        public void UpdateAuditServerPropertiesTest()
        {
            var requestUrl = string.Format("{0}//UpdateAuditServerProperties", _restApiUrl);

            var auditServerProperties = new AuditServerProperties()
            {
                ServerId = SERVER_ID,
                GeneralProperties = new ServerGeneralProperties
                {
                    Instance = SERVER_INSTANCE,
                    InstancePort = null,
                    InstanceServer = null,
                    InstanceVersion = "2012",
                    IsClustered = false,
                    Description = "My description",
                    StatusMessage = "SQL Trace Error",
                    CreatedDateTime = DateTime.Parse("06/08/15 04:33:22 PM"),
                    LastModifiedDateTime = DateTime.Parse("06/08/15 04:33:22 PM"),
                    LastHeartbeatDateTime = DateTime.Parse("06/10/15 10:06:05 AM"),
                    EventsReceivedDateTime = DateTime.Parse("06/10/15 10:04:05 AM"),
                    IsAuditEnabled = true,
                    IsAuditedServer = true,
                    LastAgentUpdateDateTime = DateTime.Parse("06/10/15 11:02:05 AM"),
                    AuditSettingsUpdateEnabled = false,
                    EventsDatabaseName = "SQLcompliance_RTDEVPC",
                    IsDatabaseIntegrityOk = true,
                    LastIntegrityCheckDateTime = null,
                    LastIntegrityCheckResultsStatus = IntegrityCheckStatus.None,
                    LastArchiveCheckDateTime = null,
                    LastArchiveCheckResultsStatus = ArchiveCheckStatus.None,
                },

                AuditedActivities = new AuditActivity
                {
                    AuditDDL = true,
                    AuditSecurity = true,
                    AuditAdmin = false,
                    AuditDML = false,
                    AuditSELECT = false,
                    AuditCaptureSQL = false,
                    AuditCaptureTrans = false,
                    AllowCaptureSql = false,
                    IsAgentVersionSupported = false,
                    AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                    AuditAllUserActivities = false,
                    AuditLogins = false,
                    AuditFailedLogins = true,
                    AuditDefinedEvents = false,
                },

                PrivilegedRolesAndUsers = new ServerRolesAndUsers()
                {
                    RoleList = new List<ServerRole>()
                    {
                        new ServerRole()
                        {
                            FullName = "System Administrators",
                            Id = 16,
                            Name = "System Administrators",
                        },
                    },

                    UserList = new List<ServerLogin>()
                    {
                        new ServerLogin()
                        {
                            Name = "RTDEVPC\\Administrator",
                            Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA=="
                        }
                    },
                },

                AuditedPrivilegedUserActivities = new AuditActivity
                {
                    AuditDDL = true,
                    AuditSecurity = true,
                    AuditAdmin = true,
                    AuditDML = false,
                    AuditSELECT = false,
                    AuditCaptureSQL = false,
                    AuditCaptureTrans = false,
                    AllowCaptureSql = true,
                    IsAgentVersionSupported = true,
                    AuditAccessCheck = AccessCheckFilter.NoFilter,
                    AuditAllUserActivities = false,
                    AuditLogins = true,
                    AuditFailedLogins = true,
                    AuditDefinedEvents = false,
                },

                AuditThresholdsData = new ThresholdsData
                {

                    ThresholdList = new List<ReportCard>
                     {
                         new ReportCard
                         {
                            CriticalThreshold = 150,
                            Enabled= false,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.PrivUserEvents,
                            WarningThreshold = 100,                             
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 150,
                            Enabled = false,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.Alerts,
                            WarningThreshold = 100,
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 150,
                            Enabled = false,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.FailedLogin,
                            WarningThreshold = 100,
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 150,
                            Enabled = false,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.Ddl,
                            WarningThreshold = 100,
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 150,
                            Enabled = false,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.Security,
                            WarningThreshold = 100,
                         },

                         new ReportCard
                         {

                            CriticalThreshold = 150,
                            Enabled = false,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.EventProcessed,
                            WarningThreshold = 100,                         
                        },
                     },
                },

                ServerAdvancedProperties = new ServerAdvancedProperties
                {
                    SQLStatementLimit = 512,
                    DefaultDatabasePermissions = DatabaseReadAccessLevel.All,
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerProperties));
            if (jsonResult == null)
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public void UpdateAuditServerPropertiesTest2()
        {
            var requestUrl = string.Format("{0}//UpdateAuditServerProperties", _restApiUrl);

            var auditServerProperties = new AuditServerProperties()
            {
                ServerId = SERVER_ID,
                GeneralProperties = new ServerGeneralProperties
                {
                    Description = "Test description",
                    CreatedDateTime = null,
                    LastModifiedDateTime = null,
                    LastHeartbeatDateTime = null,
                    EventsReceivedDateTime = null,
                    LastAgentUpdateDateTime = null,
                    LastIntegrityCheckDateTime = null,
                    LastArchiveCheckDateTime = null,
                },

                AuditedActivities = new AuditActivity
                {
                    AuditLogins = true,
                    AuditFailedLogins = true,
                    AuditDDL = true,
                    AuditAdmin = true,
                    AuditSecurity = true,
                    AuditDefinedEvents = true,
                    AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                },

                PrivilegedRolesAndUsers = new ServerRolesAndUsers()
                {
                    RoleList = new List<ServerRole>()
                    {
                        new ServerRole()
                        {
                            FullName = "System Administrators",
                            Id = 16,
                            Name = "System Administrators",
                        },
                    },

                    UserList = new List<ServerLogin>()
                    {
                        new ServerLogin()
                        {
                            Name = "RTDEVPC\\Administrator",
                            Sid = "AQUAAAAAAAUVAAAAmoPsEowxmvjyaowZ9AEAAA=="
                        }
                    },
                },

                AuditedPrivilegedUserActivities = new AuditActivity
                {
                    AuditAllUserActivities = false,
                    AuditLogins = true,
                    AuditFailedLogins = true,
                    AuditDDL = true,
                    AuditSecurity = true,
                    AuditAdmin = true,
                    AuditDML = true,
                    AuditSELECT = true,
                    AuditDefinedEvents = true,
                    AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                    AuditCaptureSQL = true,
                    AuditCaptureTrans = true,
                },

                AuditThresholdsData = new ThresholdsData
                {

                    ThresholdList = new List<ReportCard>
                     {
                         new ReportCard
                         {
                            CriticalThreshold = 201,
                            Enabled= true,
                            Period = 96,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.PrivUserEvents,
                            WarningThreshold = 110,                             
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 202,
                            Enabled = true,
                            Period = 96,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.Alerts,
                            WarningThreshold = 111,
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 203,
                            Enabled = true,
                            Period = 96,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.FailedLogin,
                            WarningThreshold = 112,
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 204,
                            Enabled = true,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.Ddl,
                            WarningThreshold = 113,
                         },

                         new ReportCard
                         {
                            CriticalThreshold = 205,
                            Enabled = true,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.Security,
                            WarningThreshold = 114,
                         },

                         new ReportCard
                         {

                            CriticalThreshold = 206,
                            Enabled = true,
                            Period = 4,
                            ServerId = SERVER_ID,
                            StatisticCategory = RestStatsCategory.EventProcessed,
                            WarningThreshold = 115,                         
                        },
                     },
                },

                ServerAdvancedProperties = new ServerAdvancedProperties
                {
                    SQLStatementLimit = 256,
                    DefaultDatabasePermissions = DatabaseReadAccessLevel.EventsOnly,
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerProperties));
            if (jsonResult == null)
            {
                Assert.Fail();
            }

        }

        [TestMethod]
        public void UpdateAuditConfigurationForServerTest()
        {
            var requestUrl = string.Format("{0}//UpdateAuditConfigurationForServer", _restApiUrl);

            var eventRequest = new UpdateAuditConfigurationRequest
                               {
                                   ServerId = SERVER_ID,
                               };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(eventRequest));
            var result = JsonHelper.FromJson<EventsResponse>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAgentDeploymentPropertiesForInstanceTest()
        {
            var instanceRequest = new InstanceRequest()
            {
                Instance = SERVER_INSTANCE,
            };

            var requestUrl = string.Format("{0}//GetAgentDeploymentPropertiesForInstance", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(instanceRequest));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void IsInstanceAvailableTest()
        {
            var instanceRequest = new InstanceRequest()
            {
                Instance = REMOTE_SERVER_INSTANCE,
            };

            var requestUrl = string.Format("{0}//IsInstanceAvailable", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(instanceRequest));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAgentPropertiesTest()
        {
            var requestUrl = string.Format("{0}//GetAgentProperties?serverId={1}", _restApiUrl, SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        // Simple update
        public void UpdateAgentPropertiesTest()
        {
            var requestUrl = string.Format("{0}//UpdateAgentProperties", _restApiUrl);

            var agentProperties = new AgentProperties()
            {
                ServerId = SERVER_ID,
                GeneralProperties = new AgentGeneralProperties()
                {
                    AgentSettings = new AgentSettings()
                    {
                        HeartbeatInterval = 6,
                        LoggingLevel = LoggingLevel.Verbose,
                    },
                },

                Deployment = new AgentDeployment()
                {
                    WasManuallyDeployed = false,
                },

                TraceOptions = new AgentTraceOptions()
                {
                    AgentTraceDirectory = @"C:\Program Files\Idera\SQLcompliance\AgentTraceFiles",
                    TraceFileRolloverSize = 6,
                    CollectionInterval = 3,
                    ForceCollectionInterval = 7,
                    TraceStartTimeout = 31,
                    TemperDetectionInterval = 61,
                    TraceDirectorySizeLimit = 3,
                    UnattendedTimeLimit = 8,
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(agentProperties));
            if (jsonResult == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        // AlreadyDeploy And Keeping Existing Audit Data
        //Default Settings
        public void AddServersTest1()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerList = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = "RTDEVPC",
                    ExistingAuditData = ExistingAuditData.Keep,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.AlreadyDeployed,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = true,
                        IsDeployedManually = false,
                        AgentTraceDirectory = null,
                        AgentServiceCredentials = null,
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerList));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // AlreadyDeploy And Delete Existing Audit Data
        //Default Settings
        public void AddServersTest2()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerList = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = "RTDEVPC",
                    ExistingAuditData = ExistingAuditData.Delete,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.AlreadyDeployed,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = true,
                        IsDeployedManually = false,
                        AgentTraceDirectory = null,
                        AgentServiceCredentials = null,
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerList));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // Deploy Now
        //Default Settings
        public void AddServerTest3()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerSettings = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = REMOTE_SERVER_INSTANCE,
                    ExistingAuditData = ExistingAuditData.Keep,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.Now,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = false,
                        IsDeployedManually = false,
                        AgentTraceDirectory = null,
                        AgentServiceCredentials = new Credentials
                        {
                            Account = "RTDEVPC\\Administrator",
                            Password = "abc1234567",
                        },
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // Deploy Manually
        //Default Settings
        public void AddServerTest4()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerSettings = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = REMOTE_SERVER_INSTANCE,
                    ExistingAuditData = ExistingAuditData.Keep,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.Manually,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = false,
                        IsDeployedManually = true,
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // Deploy Later
        //Default Settings
        public void AddServerTest5()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerSettings = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = REMOTE_SERVER_INSTANCE,
                    ExistingAuditData = ExistingAuditData.Keep,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.Later,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = false,
                        IsDeployedManually = false,
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // Deploy Manually not existing instance
        //Default Settings
        public void AddServerTest6()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerSettings = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = "NOTEXISTING",
                    Description = "",
                    ExistingAuditData = ExistingAuditData.Keep,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.Manually,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = false,
                        IsDeployedManually = false,
                        AgentServiceCredentials = null,
                        AgentTraceDirectory = null,
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // Deploy Now not existing instance
        //Default Settings
        public void AddServerTest7()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//AddServers", _restApiUrl);
            var auditServerSettings = new List<AuditServerSettings>
            {
                new AuditServerSettings()
                {
                    Instance = "NOTEXISTING2",
                    Description = "",
                    ExistingAuditData = ExistingAuditData.Keep,
                    IsVirtualServer = false,
                    AgentDeployStatus = AgentDeployStatus.Now,
                    AgentDeploymentProperties = new AgentDeploymentProperties
                    {
                        IsDeployed = false,
                        IsDeployedManually = false,
                        AgentServiceCredentials = new Credentials
                        {
                            Account = "Domain\\user",
                            Password = "user",
                        },
                        AgentTraceDirectory = "c:\\temp",
                    },
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(auditServerSettings));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        // Deploy Later
        //Default Settings
        public void ImportServerInstancesTest()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//ImportServerInstances", _restApiUrl);

            var importInstanceListRequest = new ImportInstanceListRequest
            {
                InstanceList = new List<string>()
                {
                   SERVER_INSTANCE,
                   REMOTE_SERVER_INSTANCE,
                   CLUSTER_SERVER_INSTANCE,
                },
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(importInstanceListRequest));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void RemoveServersTest()
        {
            var removeServerRequest = new RemoveServersRequest
        {
            ServerIdList = new List<int>
            {
                    SERVER_ID,
                },

            DeleteEventsDatabase = true,
        };

            var requestUrl = string.Format("{0}//RemoveServers", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(removeServerRequest));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        /// Case with deleting remote instacne
        public void RemoveServersTest2()
        {
            var removeServerRequest = new RemoveServersRequest
            {
                ServerIdList = new List<int>
            {
                    REMOTE_SERVER_ID,
                },

                DeleteEventsDatabase = true,
            };

            var requestUrl = string.Format("{0}//RemoveServers", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(removeServerRequest));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void EnableAuditingForServersTest()
        {
            var servers = new EnableAuditForServers()
            {
                ServerIdList = new List<int>()
                {
                    SERVER_ID,
                },
                Enable = true,
            };

            var requestUrl = string.Format("{0}//EnableAuditingForServers", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(servers));
            var result = JsonHelper.FromJson<List<EnvironmentObject>>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetServerClrStatusTest()
        {
            var requestUrl = string.Format("{0}//GetServerClrStatus?serverId={1}", _restApiUrl, SERVER_ID);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void EnableClrTest()
        {
            var status = new ClrStatus
            {
                ServerId = SERVER_ID,
                Enable = true,
            };

            var requestUrl = string.Format("{0}//EnableClr", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(status));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void SetViewTimeoutTest()
        {
            var input = new ViewSettings()
            {
                ViewId = "1000",
                Timeout = 1,
                Filter = "Test12"
            };

            var requestUrl = string.Format("{0}//SetViewSettings", _restApiUrl);
            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(input));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetViewSettingsTest()
        {
            string viewId = "1000";
            var requestUrl = string.Format("{0}//GetViewSettings?viewId={1}", _restApiUrl, viewId);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetColumnListTest()
        {
            string tableName = "dbo.tests";
            var requestUrl = string.Format("{0}//GetColumnList?databaseId={1}&tableName={2}", _restApiUrl, DATABASE_ID, tableName);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }


        /// <summary>
        /// GetAuditedEvents for Instance 
        /// </summary>
        [TestMethod]
        public void GetAuditedEventsTest()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//GetAuditedEvents", _restApiUrl);
            var eventRequest =
                new FilteredEventRequest()
                {
                    ServerId = SERVER_ID,
                    Page = 1,
                    PageSize = 10,
                    SortColumn = "Time",
                    SortDirection = 1,
                    TableId = -159434233,
                };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(eventRequest));
            var result = JsonHelper.FromJson<EventsResponse>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        /// <summary>
        /// GetAuditedEvents for database 
        /// </summary>
        [TestMethod]
        public void GetAuditedEventsTest2()
        {
            // get all server logins and roles
            var requestUrl = string.Format("{0}//GetAuditedEvents", _restApiUrl);
            var eventRequest =
                new FilteredEventRequest()
                {
                    ServerId = SERVER_ID,
                    DatabaseId = DATABASE_ID,
                    Page = 1,
                    PageSize = 10,
                    SortColumn = "Time",
                    SortDirection = 1,
                };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(eventRequest));
            var result = JsonHelper.FromJson<EventsResponse>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetEventsByIntervalForDatabaseTest()
        {
            // get all server logins and roles
            var days = 7;
            var requestUrl = string.Format("{0}//GetEventsByIntervalForDatabase?serverId={1}&databaseId={2}&days={3}", _restApiUrl, SERVER_ID, DATABASE_ID, days);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetFilteredAlerts()
        {
            var requestUrl = string.Format("{0}//GetFilteredAlerts", _restApiUrl);
            var alertRequest =
                new FilteredAlertRequest()
                {

                };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(alertRequest));
            var result = JsonHelper.FromJson<FilteredAlertsResponse>(jsonResult);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void PushAlertsToCwfDashboardTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/PushAlertsToCwfDashboard", _restApiUrl);

            var dummyAlerts = new List<Alert>
            {
                new Alert()
                {
                    AlertCategory = "Dummy",
                    Database = "Dummy",
                    Instance = "Dummy",
                    LastActiveTime = DateTime.UtcNow,
                    Metric = "Dummy",
                    ProductId = _productId,
                    Severity = "Dummy",
                    StartTime = DateTime.UtcNow,
                    Summary = "Dummy",
                    Table = "Dummy",
                    Value = "Dummy"
                },
                new Alert()
                {
                    AlertCategory = "Dummy1",
                    Database = "Dummy1",
                    Instance = "Dummy1",
                    LastActiveTime = DateTime.UtcNow,
                    Metric = "Dummy1",
                    ProductId = _productId,
                    Severity = "Dummy1",
                    StartTime = DateTime.UtcNow,
                    Summary = "Dummy1",
                    Table = "Dummy1",
                    Value = "Dummy1"
                }
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(dummyAlerts));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void DismissAlertsGroupForInstanceTest()
        {
            // get all instance names
            var requestUrl = string.Format("{0}/DismissAlertsGroupForInstance", _restApiUrl);

            var dummyGroupAlert = new DismissAlertsGroupRequest
            {
                InstanceId = SERVER_ID,
                AlertType = AlertType.Event,
                AlertLevel = AlertLevel.Medium
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(dummyGroupAlert));
            if (jsonResult == null)
                Assert.Fail();
        }
        
        [TestMethod]
        public void UpgradeAgent()
        {
            var requestUrl = string.Format("{0}//UpgradeAgent", _restApiUrl);

            var request = new UpgradeAgentRequest()
            {
                ServerId = SERVER_ID,
                Account = "RTDEVPC\\Administrator",
                Password = "abc1234567"
            };

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(request));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void CheckAgentStatus()
        {
            var requestUrl = string.Format("{0}//CheckAgentStatus", _restApiUrl);

            var request = "RTDEVPC\\SQL2008";

            var jsonResult = MakePostRequest(requestUrl, JsonHelper.ToJson(request));
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAllUserSettings()
        {
            var requestUrl = string.Format("{0}//GetAllUserSettings", _restApiUrl);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetUserSettings()
        {
            var requestUrl = string.Format("{0}//GetAllUserSettings?dashboardUserId={1}", _restApiUrl, 2);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void CreateUpdateUserSettings()
        {
            var requestUrl = string.Format("{0}//CreateUpdateUserSettings", _restApiUrl);

            var model = new UserSettingsModel()
            {
                Account = @"RTDEVPC\Administrator",
                DashboardUserId = 2,
                Email = "email@em.com",
                SessionTimout = (int)TimeSpan.FromMinutes(2).TotalMilliseconds,
                Subscribed = true
            };

            var result = MakePostRequest(requestUrl, JsonHelper.ToJson(model));
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void DeleteUserSettings()
        {
            var requestUrl = string.Format("{0}//DeleteUserSettings", _restApiUrl);
            var model = new DeleteUserSettingsRequest
            {
                DashbloardUserIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 }
            };

            var result = MakePostRequest(requestUrl, JsonHelper.ToJson(model));
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAlertsGroups()
        {
            var requestUrl = string.Format("{0}//GetAlertsGroups?instanceId={1}", _restApiUrl, 1);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAlerts()
        {
            var requestUrl = string.Format("{0}//GetAlerts?instanceId={1}&alertType={2}&alertLevel={3}&pageSize={4}&page={5}", _restApiUrl, 1, 1, 1, 10, 0);
            var jsonResult = MakeGetRequest(requestUrl);
            if (jsonResult == null)
                Assert.Fail();
        }

        [TestMethod]
        public void CheckInstanceRegisteredStatusTest()
        {
            var requestUrl = string.Format("{0}//CheckInstanceRegisteredStatus", _restApiUrl);

            var request = new CheckInstanceRegisteredRequest()
            {
                Instance = SERVER_INSTANCE,
            };

            var result = MakePostRequest(requestUrl, JsonHelper.ToJson(request));
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetManagedInstancesTest()
        {
            var requestUrl = string.Format("{0}//GetManagedInstances", _restApiUrl);

            var request = new PaginationRequest()
            {
                Page = 1,
                PageSize = 10,
                SortColumn = "instance",
                SortDirection = 0
            };

            var result = MakePostRequest(requestUrl, JsonHelper.ToJson(request));
            if (result == null)
                Assert.Fail();
        }


        [TestMethod]
        public void GetManagedInstance()
        {
            var requestUrl = string.Format("{0}//GetManagedInstance?id={1}", _restApiUrl, 1);

            var result = MakeGetRequest(requestUrl);
            if (result == null)
                Assert.Fail();
        }


        [TestMethod]
        public void UpdateManagedInstance()
        {
            var requestUrl = string.Format("{0}//UpdateManagedInstance", _restApiUrl);

            var request = new ManagedInstanceProperties()
            {
                Comments = "test",
                Credentials = new ManagedCredentials
                {
                    Account = @"test\test",
                    AccountType = SqlServerSecurityModel.Integrated,
                    Password = "password"
                },
                DataCollectionSettings = new DataCollectionSettings
                {
                    CollectionInterval = 1,
                    KeepDataFor = 2
                },
                Id = 1,
                InstanceName = @"instance\name",
                Location = "location",
                Owner = "owner"
            };

            var result = MakePostRequest(requestUrl, JsonHelper.ToJson(request));
            if (result == null)
                Assert.Fail();
        }

        //SQLCM 5.4 start
        [TestMethod]
        public void GetProfileDetails()
        {
            var requestUrl = string.Format("{0}//GetProfileDetails", _restApiUrl);

            var result = MakeGetRequest(requestUrl);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetActiveProfile()
        {
            var requestUrl = string.Format("{0}//GetActiveProfile", _restApiUrl);

            var result = MakeGetRequest(requestUrl);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetTableDetailsForAll()
        {
            var requestUrl = string.Format("{0}//GetTableDetailsForAll", _restApiUrl);

            var result = MakeGetRequest(requestUrl);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetColumnDetailsForAll()
        {
            var requestUrl = string.Format("{0}//GetColumnDetailsForAll", _restApiUrl);

            var result = MakeGetRequest(requestUrl);
            if (result == null)
                Assert.Fail();
        }

        [TestMethod]
        public void GetAllDatabasesForInstance()
        {
            var requestUrl = string.Format("{0}//GetAllDatabasesForInstance", _restApiUrl);

            var result = MakeGetRequest(requestUrl);
            if (result == null)
                Assert.Fail();
        }
        //SQLCM 5.4 end
    }
}
