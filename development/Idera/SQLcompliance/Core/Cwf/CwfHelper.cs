using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using PluginCommon;
using CWFContracts = Idera.SQLcompliance.Core.CWFDataContracts;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Core.Cwf
{
    public class CwfHelper
    {
        #region members

        private static CwfHelper _instance;
        private bool _isInitialized;
        private readonly Dictionary<string, Idera.SQLcompliance.Core.CWFDataContracts.Product> _productsCache;
        private Users _users; 
        private readonly List<KeyValuePair<string, string>> _cwfDashboardConnectionSetings;
        private string CwfUrl = string.Empty;
        private string CwfToken = string.Empty;

        #endregion

        #region constructor\destructor

        private CwfHelper()
        {
            _productsCache = new Dictionary<string, Idera.SQLcompliance.Core.CWFDataContracts.Product>();
            _cwfDashboardConnectionSetings = new List<KeyValuePair<string, string>>();
            _isInitialized = false;
        }

        ~CwfHelper()
        {
            _isInitialized = false;
            _productsCache.Clear();
            _cwfDashboardConnectionSetings.Clear();
        }

        #endregion

        #region properties

        public static CwfHelper Instance
        {
            get { return _instance ?? (_instance = new CwfHelper()); }
        }

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        /// <summary>
        /// First saved CWF dashboard url.
        /// </summary>
        /*private string CwfUrl
        {
            get { return _cwfDashboardConnectionSetings[0].Key; }
        }

        /// <summary>
        /// First saved CWF dashboard authorization token.
        /// </summary>
        private string CwfToken
        {
            get { return _cwfDashboardConnectionSetings[0].Value; }
        }*/

        #endregion

        #region private methods

        private string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }

        private string GetAuthorizationHeader()
        {
            return string.Format("Basic {0}", CwfToken);
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
                ErrorLog.Instance.Write(ErrorLog.Level.Always, ex);
            }

            return null;
        }

        /// <summary>
        /// This method returns the JSON response in strin format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private string SendPostData(string url, String postData)
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
                ErrorLog.Instance.Write(ErrorLog.Level.Always, ex);
            }
            finally
            {
                request = null;
            }

            return responseValue;
        }

        private void GetUsers()
        {
            _users = null;
            if (!_isInitialized)
                return;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Retrieving list of CWF users...");

            var request = CreateRequest(string.Format("{0}/IderaCoreServices/v1/Users", CwfUrl), "GET");
            request.ContentLength = 0;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    if (response.ContentLength > 0)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue = reader.ReadToEnd();
                                }
                            }
                        }
                    }

                    _users = JsonHelper.FromJson<Users>(responseValue);

                    // set web access permissions in database based on existing CWF users
                    foreach (var user in _users)
                    {
                        var loginAccount = new LoginAccount(user.Account);
                        loginAccount.WebApplicationAccess = user.IsEnabled;
                        loginAccount.Set();
                    }

                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("CWF Users Retrieved: {0}", _users.Count));
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, e);
            }
        }

        private void GetProducts()
        {
            _productsCache.Clear();
            if (!_isInitialized)
                return;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Retrieving list of registered SQLCM product instances...");

            var request = CreateRequest(string.Format("{0}/IderaCoreServices/v1/Products?shortname={1}", CwfUrl, CoreConstants.PRODUCT_SHORT_NAME), "GET");
            request.ContentLength = 0;

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var responseValue = string.Empty;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    if (response.ContentLength > 0)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue = reader.ReadToEnd();
                                }
                            }
                        }
                    }

                    // add products to cache
                    var products = JsonHelper.FromJson<Idera.SQLcompliance.Core.CWFDataContracts.Products>(responseValue);
                    foreach (var product in products)
                        _productsCache.Add(product.InstanceName, product);

                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("Registered SQLCM Product Instances: {0}", products.Count));
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, e, ErrorLog.Severity.Error);
            }
        }

        /// <summary>
        /// This method returns product IDs with their REST URLs.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<int, string>> GetProductRestUrls()
        {
            var restUrls = new List<KeyValuePair<int, string>>();
            if (!_isInitialized)
                return restUrls;

            foreach (var product in _productsCache.Values)
                restUrls.Add(new KeyValuePair<int, string>(product.Id, string.Format("{0}{1}", CwfUrl, product.RestURL)));

            return restUrls;
        } 

        #endregion

        public void Initialize(string serverInstance)
        {
            // set repository server if missing
            if (string.IsNullOrEmpty(Repository.ServerInstance))
                Repository.ServerInstance = serverInstance;

            /*var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(" SELECT [CwfUrl]");
            sqlBuilder.AppendLine("       ,[CwfToken]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Cwf]");

            var repository = new Repository();

            try
            {
                repository.OpenConnection();
                using (var command = repository.connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = sqlBuilder.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            _cwfDashboardConnectionSetings.Add(new KeyValuePair<string, string>(reader.GetString(0), 
                                                                                                reader.GetString(1)));
                    }
                }*/
            try
            {
                SetCwfUrlAndToken();
                _cwfDashboardConnectionSetings.Add(new KeyValuePair<string, string>(CwfUrl, CwfToken));

                if (string.IsNullOrEmpty(CwfUrl) || string.IsNullOrEmpty(CwfToken))
                    _isInitialized = false;
                else
                {
                    _isInitialized = true;
                    GetProducts();
                    GetUsers();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, new Exception("Failed to read CWF settings from repository database.", ex), ErrorLog.Severity.Warning);
                _isInitialized = false;
            }
            /*finally
            {
                if (repository.connection.State == ConnectionState.Open)
                    repository.CloseConnection();
            }*/
        }

        /// <summary>
        /// This method pushes alerts to all registered SQLcm products.
        /// </summary>
        /// <param name="alerts"></param>
        /// <returns></returns>
        public void PushAlertsToDashboard(List<Alert> alerts)
        {
            if (alerts == null || alerts.Count == 0)
                return;

            if (!_isInitialized)
                return;

            try
            {
                foreach (var productRestUrl in GetProductRestUrls())
                {
                    // update product ID for each alert
                    foreach (var alert in alerts)
                        alert.ProductId = productRestUrl.Key;

                    var alertsJson = JsonHelper.ToJson(alerts);

                    var requestUrl = string.Format("{0}{1}", productRestUrl.Value, "/PushAlertsToCwfDashboard");
                    SendPostData(requestUrl, alertsJson);
                }
            }
            catch
            {
                // do nothing
            }
        }

        /// <summary>
        /// This method pushes instances to dashboard.
        /// </summary>
        /// <returns></returns>
        public void PushInstancesToDashboard()
        {
            try
            {
                foreach (var productRestUrl in GetProductRestUrls())
                {
                    var requestUrl = string.Format("{0}{1}", productRestUrl.Value, "/PushInstancesToCwfDashboard");
                    SendPostData(requestUrl, null);
                }
            }
            catch
            {
                // do nothing
            }
        }

        public List<KeyValuePair<string, string>> GetProductWebUrls(bool forceReload)
        {
            var dashboardUrls = new List<KeyValuePair<string, string>>();
            if (!_isInitialized)
                return dashboardUrls;

            if (forceReload)
                GetProducts();

            foreach (var product in _productsCache.Values)
            {
                var instanceDashboardUrl = string.Format("{0}{1}/{2}/{3}", CwfUrl, product.WebURL, product.InstanceName, product.DefaultPage);
                instanceDashboardUrl = instanceDashboardUrl.Replace("http://", "https://");
                instanceDashboardUrl = instanceDashboardUrl.Replace(":9292", ":9291");

                dashboardUrls.Add(new KeyValuePair<string, string>(product.InstanceName, instanceDashboardUrl));
            }

            return dashboardUrls;
        }

        public bool SynchronizeUsersWithCwf(LoginAccount usertoSync = null)
        {
            if (!_isInitialized)
                return false;

            var success = true;
            var repository = new Repository();
            try
            {
                repository.OpenConnection();

                ICollection loginList = RawSQL.GetServerLogins(repository.connection);
                if (loginList == null || loginList.Count == 0)
                    return false;

                // list of users to sync with cwf
                var users = new List<Idera.SQLcompliance.Core.CWFDataContracts.User>();

                foreach (RawLoginObject login in loginList)
                {
                    // if single user passed for sync, just sync it
                    if (usertoSync != null &&
                        usertoSync.Name.Equals(login.name, StringComparison.InvariantCultureIgnoreCase))
                    {

                        var user = new Idera.SQLcompliance.Core.CWFDataContracts.User();
                        user.Account = usertoSync.Name;
                        user.Sid = new SecurityIdentifier(login.sid, 0).Value;
                        user.UserType = "U";
                        user.IsEnabled = usertoSync.WebApplicationAccess;
                        users.Add(user);
                        break;
                    }

                    if (usertoSync == null && 
                        login.isntname == 1 && 
                        login.isntuser == 1)
                    {
                        var loginAccount = new LoginAccount(login.name);

                        var user = new Idera.SQLcompliance.Core.CWFDataContracts.User();
                        user.Account = loginAccount.Name;
                        user.Sid = new SecurityIdentifier(login.sid, 0).Value;
                        user.UserType = "U";
                        user.IsEnabled = loginAccount.WebApplicationAccess;
                        users.Add(user);
                    }
                }

                var usersJson = JsonHelper.ToJson(users);
                try
                {
                    foreach (var productRestUrl in GetProductRestUrls())
                    {
                        var requestUrl = string.Format("{0}{1}", productRestUrl.Value, "/SyncUsers");

                        // try to sync user
                        var response = Boolean.Parse(SendPostData(requestUrl, usersJson));
                        if (!response)
                            success = false;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, ex, ErrorLog.Severity.Warning);
                    success = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, ex, ErrorLog.Severity.Warning);
            }
            finally
            {
                repository.CloseConnection();
            }

            return success;
        }

        private void SetCwfUrlAndToken()
        {
            RegistryKey view;
            if (Environment.Is64BitOperatingSystem)
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            try
            {
                using (RegistryKey cmKey = view.OpenSubKey(@"SOFTWARE\Idera\SQLCM", false))
                {
                    object registryCwfUrl = cmKey.GetValue("CwfUrl");
                    object registryCwfToken = cmKey.GetValue("CwfToken");
                    if (registryCwfUrl != null && registryCwfToken != null)
                    {
                        CwfUrl = registryCwfUrl.ToString();
                        CwfToken = registryCwfToken.ToString();
                    }
                }
                if (string.IsNullOrEmpty(CwfUrl) || string.IsNullOrEmpty(CwfToken))
                {
                    var sqlBuilder = new StringBuilder();
                    sqlBuilder.AppendLine(" SELECT [CwfUrl]");
                    sqlBuilder.AppendLine("       ,[CwfToken]");
                    sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Cwf]");

                    var repository = new Repository();


                    repository.OpenConnection();
                    using (var command = repository.connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = sqlBuilder.ToString();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CwfUrl = reader.GetString(0);
                                CwfToken = reader.GetString(1);
                            }
                        }
                    }
                    repository.CloseConnection();

                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
