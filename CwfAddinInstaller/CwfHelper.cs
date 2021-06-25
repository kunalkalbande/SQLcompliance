using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SQLcomplianceCwfAddin.RestService;
using System.Threading;
using Microsoft.Win32;
using CwfAddinInstaller.WizardPages;
using System.Security.Cryptography;

namespace CwfAddinInstaller
{
    public class CwfHelper
    {
        #region members

        public int oldProductId;
        private readonly FormInstaller _host;
        private bool _flag;
        private readonly FormUpload _uploadHelper;
        private static readonly Version MinimumVersionOfDashboard = new Version("4.2.0");

        internal const string ErrorInvalidCoreAdminCredentials = "Invalid Core Administrator Credentials.";
        internal const string ErrorConnectionFailure = "Unable to Connect to Dashboard. Please check the Dashboard URL.";
        internal const string ErrorWebrequestFailed = "Unable to make web request.";

        #endregion

        public CwfHelper(FormInstaller host)
        {
            _uploadHelper = new FormUpload(host);
            _host = host;
        }

        #region properties

        private Products m_registeredProducts;

        internal Products RegisteredProducts
        {
            get
            {
                if (m_registeredProducts == null)
                {
                    LoadRegisteredProducts();    
                }

                return m_registeredProducts;
            }
        }

        internal string CwfUrl { get; set; }

        internal string CwfUser { get; set; }

        internal string CwfPassword { get; set; }

        internal string CwfToken
        {
            get
            {
                return Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(CwfUser + ":" + CwfPassword, "\\\\", "\\")));
            }
        }

        internal string CwfProductInstance { get; set; }

        internal Products ProductsToBeUpgraded { get; set; }

        internal bool UpgradeAllProductInstances { get; set; }

        internal string RepositoryInstance { get; set; }

        internal string RepositoryDatabase { get; set; }

        internal bool IsDashboardOnRemoteHost { get; set; }

        #endregion

        internal string LoadRegisteredProducts()
        {
            _host.Log("Loading list of registered product instances...");
            var url = String.Format("{0}/IderaCoreServices/v1/Products?shortname={1}",
                                    CwfUrl,
                                    RestServiceConstants.ProductShortName);
            string errorMessage;
            var response = GetRequest(url, out errorMessage);

            m_registeredProducts = JsonHelper.FromJson<Products>(response);

            Products oldProduct = new Products();
            oldProduct = GetRegisteredSqlCmProducts();
            foreach (var product in oldProduct)
            {
                if (product.Name.Equals("SQLCM"))
                {
                    oldProductId = product.Id;
                }
            }

            if (string.IsNullOrEmpty(errorMessage))
                _host.Log("Registered product instances: {0}", RegisteredProducts.Count);
            else
                _host.Log("Failed to load registered product instances: {0}", errorMessage);

            return errorMessage;
        }

        public Version GetCurrentVersion(out string errorMessage)
        {
            var url = string.Format("{0}/IderaCoreServices/v1/Version", CwfUrl);
            var response = GetRequest(url, out errorMessage);

            if (string.IsNullOrEmpty(response)) return null;
            if (!string.IsNullOrEmpty(errorMessage)) return null;

            var versionString = response.Trim('\"');

            Version version;

            return Version.TryParse(versionString, out version) ? version : null;
        }

        public Products GetRegisteredSqlCmProducts()
        {
            var products = new Products();

            foreach (var product in RegisteredProducts)
            {
                if (product.ShortName.Equals(RestServiceConstants.ProductShortName, StringComparison.InvariantCultureIgnoreCase))
                {
                    products.Add(product);
                }
            }

            return products;
        }

        public Products GetUpdatingRegisteredSqlCmProductsWithLowerVersionThen(Version version)
        {
            var products = new Products();
            var registeredSqlCmProducts = GetRegisteredSqlCmProducts();

            foreach (var product in registeredSqlCmProducts)
            {
                var productVersion = new Version(product.Version);
                if (productVersion < version)
                {
                    products.Add(product);
                }
            }

            return products;
        }

        internal bool CanConnectToDashboard()
        {
            var url = string.Format("{0}/IderaCoreServices/v1/Status", CwfUrl);

            string errorMessage;
            var response = GetRequest(url, out errorMessage);

            // if we are able to connect to dashboard
            return !string.IsNullOrEmpty(response) || errorMessage.Equals(ErrorInvalidCoreAdminCredentials, StringComparison.InvariantCultureIgnoreCase);
        }

        internal bool RegisterProduct()
        {
            SetCwfUrlAndTokenInRegistry();
            _host.Log("Checking If Product is Registered.");

            if (ProductsToBeUpgraded == null)
            {
                _host.Log("Registering the product.");
                RegisterNewProduct();
            }
            else
            {
                
                var messageFormat = "{0} upgrading product: ( Display Name = {1}; Version = {2}; Product ID = {3} ) with product of version {4}.";

                foreach (var product in ProductsToBeUpgraded)
                {
                    try
                    {
                        _host.Log(string.Format(messageFormat, "Started", product.InstanceName, product.Version, product.Id, RestServiceConstants.ProductVersion));
                        UpgradeProduct(product.Id);
                        //UnregisterProduct(product.Id);
                        //RegisterNewProduct();
                    }
                    catch (Exception ex)
                    {
                        _host.Log(string.Format(messageFormat, "Failed", product.InstanceName, product.Version, product.Id, RestServiceConstants.ProductVersion));
                        _host.Log(string.Format("due to the following error: {0}", ex));
                    }

                }
            }

            return true;
        }
        internal void UnregisterProduct(int productId)
        {
            var postURL = string.Format("{0}/IderaCoreServices/v1/Products/{1}", CwfUrl,productId);
            var request = (HttpWebRequest)WebRequest.Create(postURL);

            request.Method = "DELETE";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            var header = "Basic " + _host.CwfHelper.CwfToken;
            request.Headers.Add("Authorization", header);
            var encoding = new UTF8Encoding();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes("");
            request.ContentLength = bytes.Length;

            _host.Log("Initialize request");
            _host.Log("Request DELETE, URL = {0}", postURL);
            _host.Log("Request Authorization = {0}", header);

            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;
                _host.Log("StatusCode = {0}", response.StatusCode);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    _host.Log(message);
                    throw new ApplicationException(message);
                }

                // grab the response
                _host.Log("Response ContentLength = {0}", response.ContentLength);
                if (response.ContentLength > 0)
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                }
            }
        }

        #region private methods

        private void RegisterNewProduct()
        {
            var postParameters = GetDataToRegister();

            // create request and receive response
            var postUrl = string.Format("{0}/IderaCoreServices/v1/RegisterProduct/", CwfUrl);
            Thread.Sleep(3000);
            MakeRequest(postUrl, postParameters);
            SetRegistryValue(postParameters);
        }

        public void SetRegistryValue(Dictionary<string, object> postParameters)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
            if (key == null)
                key = hklm.CreateSubKey(@"Software\Idera\SQLCM");
            key.SetValue("Version", postParameters.Where(z => z.Key == "Version").FirstOrDefault().Value);
            key.SetValue("ServiceHost", Dns.GetHostName());
            key.SetValue("ServicePort", RestServiceConstants.ServicePort);
            key.SetValue("Administrator", CwfUser);
            key.SetValue("IsRegisteredToDashboard", RestServiceConstants.IsRegisteredToDashboard);
            key.SetValue("DisplayName", CwfProductInstance);
            key.SetValue("SQLcmSQLServerName", RepositoryInstance);
            string cwfPass = QuickEncrypt(CwfPassword);
            key.SetValue("ServiceAdminPassword", cwfPass);
        }

        public void SetCwfUrlAndTokenInRegistry()
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
            if (key == null)
                key = hklm.CreateSubKey(@"Software\Idera\SQLCM");
            key.SetValue("CwfUrl", CwfUrl);
            key.SetValue("CwfToken", CwfToken);

            var hklmchn = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey lmkey = hklmchn.OpenSubKey(@"Software\Idera\LM\SQLcm", true);
            if (lmkey == null)
                lmkey = hklmchn.CreateSubKey(@"Software\Idera\LM\SQLcm");
            lmkey.SetValue("BaseUrl", CwfUrl);
        }

        public bool NeedUpgrade(out int productId)
        {
            if (RegisteredProducts.Count == 0)
            {
                productId = -1;
                return false;
            }

            var installerVersion = new Version(RestServiceConstants.ProductVersion);
            foreach (var product in RegisteredProducts)
            {
                if (!product.InstanceName.Equals(CwfProductInstance, StringComparison.InvariantCultureIgnoreCase)) 
                    continue;

                var currentVersion = new Version(product.Version);
                var value = installerVersion.CompareTo(currentVersion);
                productId = product.Id;

                return value != 0 && value != -1;
            }

            productId = -1;
            return true;
        }

        private void UpgradeProduct(int productId)
        {
            var url = string.Format("{0}/IderaCoreServices/v1/RegisterProduct/Upgrade/{1}", CwfUrl, productId);
            var postParameters = GetDataToUpgrade();
            MakeRequest(url, postParameters);
            SetRegistryValue(postParameters);
        }

        private void MakeRequest(string postUrl, Dictionary<string, object> postParameters)
        {
            // Create request and receive response
            Thread.Sleep(3000);
            var webResponse = _uploadHelper.MultipartFormDataPost(postUrl, string.Empty, postParameters);
            // Process response
            var responseReader = new StreamReader(webResponse.GetResponseStream());
            var fullResponse = responseReader.ReadToEnd();
            _host.Log("Product has been Upgraded/Registered");

            var product = JsonHelper.FromJson<Product>(fullResponse);
            _host.Log("Upgraded/Registered Product Id {0}", product.Id);
            webResponse.Close();
        }

        private Dictionary<string, object> GetDataToUpgrade()
        {
            _host.Log("Reading AddIn zip file '{0}'.", _host.CwfAddInZip);
            if (!File.Exists(_host.CwfAddInZip))
                _host.Log("AddIn zip file missiong.");

            var fs = new FileStream(_host.CwfAddInZip, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            var postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", RestServiceConstants.ProductName);
            postParameters.Add("ShortName", RestServiceConstants.ProductShortName);
            postParameters.Add("Version", RestServiceConstants.ProductVersion);
            postParameters.Add("JarFile", RestServiceConstants.JarFile);
            postParameters.Add("Description", RestServiceConstants.ProductDescription);
            postParameters.Add("DefaultPage", RestServiceConstants.DefaultPage);
            postParameters.Add("RestFile", RestServiceConstants.AssemblyFile);
            postParameters.Add("ProductServicesHost", System.Environment.MachineName);
            postParameters.Add("ProductServicesPort", RestServiceConstants.ServicePort);
            postParameters.Add("Location", String.Join(";", RepositoryInstance, "SQLcompliance"));
            postParameters.Add("IsWarEnabled", RestServiceConstants.IsWarEnabled);
            postParameters.Add("WarFileName", RestServiceConstants.WarFileName);
            postParameters.Add("product", new FormUpload.FileParameter(data, RestServiceConstants.ZipFile, "application/x-zip-compressed"));
            _host.Log("POST Parameters:");
            foreach (var key in postParameters.Keys)
            {
                // we will not log password
                _host.Log("   {0}: {1}", key, key.Equals("ConnectionPassword") ? FormInstaller.NoPassowrdlogging : postParameters[key]);
            }

            return postParameters;
        }

        private Dictionary<string, object>  GetDataToRegister()
        {
            _host.Log("Reading AddIn zip file '{0}'.", _host.CwfAddInZip);
            if (!File.Exists(_host.CwfAddInZip))
                _host.Log("AddIn zip file missiong.");

            var fs = new FileStream(_host.CwfAddInZip, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            var postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", RestServiceConstants.ProductName);
            postParameters.Add("ShortName", RestServiceConstants.ProductShortName);
            postParameters.Add("Version", RestServiceConstants.ProductVersion);
            postParameters.Add("Status", RestServiceConstants.Status);
            postParameters.Add("Location", String.Join(";", RepositoryInstance, "SQLcompliance"));
            postParameters.Add("ConnectionUser", CwfUser);
            postParameters.Add("ConnectionPassword", CwfPassword);
            postParameters.Add("RegisteringUser", CwfUser);
            postParameters.Add("WebURL", RestServiceConstants.WebUrl);
            postParameters.Add("RestURL", RestServiceConstants.RestUrl);
            postParameters.Add("JarFile", RestServiceConstants.JarFile);
            postParameters.Add("Description", RestServiceConstants.ProductDescription);
            postParameters.Add("DefaultPage", RestServiceConstants.DefaultPage);
            postParameters.Add("InstanceName", CwfProductInstance);
            postParameters.Add("RestFile", RestServiceConstants.AssemblyFile);
            postParameters.Add("ProductServicesHost", System.Environment.MachineName);
            postParameters.Add("ProductServicesPort", RestServiceConstants.ServicePort);
            postParameters.Add("IsWarEnabled", RestServiceConstants.IsWarEnabled);
            postParameters.Add("WarFileName", RestServiceConstants.WarFileName);
            postParameters.Add("product", new FormUpload.FileParameter(data, RestServiceConstants.ZipFile, "application/x-zip-compressed"));

            _host.Log("POST Parameters:");
            foreach (var key in postParameters.Keys)
            {
                // we will not log password
                _host.Log("   {0}: {1}", key, key.Equals("ConnectionPassword") ? FormInstaller.NoPassowrdlogging : postParameters[key]);
            }

            return postParameters;
        }

        private string GetRequest(string url, out string errorMessage)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + CwfToken);

            try
            {
                _host.Log("Making web request: {0}", url);
                var response = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.ProtocolError:
                        errorMessage = ErrorInvalidCoreAdminCredentials;
                        _host.Log("{2}\r\nError: {0}\r\nStack Trace: {1}", e.Message, e.StackTrace, ErrorInvalidCoreAdminCredentials);
                        return string.Empty;

                    case WebExceptionStatus.ConnectFailure:
                        errorMessage = ErrorConnectionFailure;
                        _host.Log("{2}\r\nError: {0}\r\nStack Trace: {1}", e.Message, e.StackTrace, ErrorConnectionFailure);
                        return string.Empty;

                    default:
                        errorMessage = e.Message;
                        _host.Log("{2}\r\nError: {0}\r\nStack Trace: {1}", e.Message, e.StackTrace, ErrorWebrequestFailed);
                        return string.Empty;
                }
            }
            catch (Exception e)
            {
                _host.Log("{1}: {0}", e, ErrorWebrequestFailed);
                errorMessage = e.Message;
                return string.Empty;
            }

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
                        if (responseStream == null)
                        {
                            errorMessage = string.Empty;
                            return responseValue;
                        }

                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }

                errorMessage = string.Empty;
                return responseValue;
            }
        }

        #endregion

        public static string NormalizeInstanceName(string instanceName)
        {
            if (instanceName == null) return null;
            instanceName = instanceName.ToUpper();

            if (instanceName.StartsWith("(LOCAL)", StringComparison.InvariantCultureIgnoreCase))
                instanceName = instanceName.Replace("(LOCAL)", Environment.MachineName);
            else if (instanceName.StartsWith(".", StringComparison.InvariantCultureIgnoreCase))
                instanceName = instanceName.Replace(".", Environment.MachineName);

            return instanceName;
        }

        public static bool IsInstanceNameNormalized(string instanceName)
        {
            if (instanceName == null) return true;
            instanceName = instanceName.ToUpper();

            return !instanceName.StartsWith("(LOCAL)", StringComparison.InvariantCultureIgnoreCase) && 
                !instanceName.StartsWith(".", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CoreUpgradeNeaded(Version installedDashboardVersion)
        {
            if (installedDashboardVersion == null) return true;
            return installedDashboardVersion < MinimumVersionOfDashboard;
        }

        internal static string QuickEncrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return (plaintext);
            }
            string encrypted;
            try
            {
                using (TripleDES algorithm = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create())
                {
                    try
                    {
                        algorithm.Mode = CipherMode.ECB;
                        algorithm.Key = GenerateKey(algorithm);
                        using (ICryptoTransform transform = algorithm.CreateEncryptor())
                        {
                            byte[] buffer = UnicodeEncoding.Unicode.GetBytes(plaintext);
                            buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                            encrypted = Convert.ToBase64String(buffer);
                        }
                    }
                    finally
                    {
                        algorithm.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return (encrypted);
        }

        private static byte[] GenerateKey(SymmetricAlgorithm algorithm)
        {
            string constKey = "SQLcompliance";
            var sTemp = constKey.PadRight(24, ' ');
            // convert the secret key to byte array
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
    }
}
