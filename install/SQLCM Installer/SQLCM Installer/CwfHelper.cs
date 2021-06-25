using CwfAddinInstaller;
using CWFInnstallerService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SQLCM_Installer
{
    public class CwfHelper
    {
        public int oldProductId;
        private readonly MainForm _host;
        private readonly FormUpload _uploadHelper;
        private string _cwfUser = string.Empty;
        private string _cwfPassword = string.Empty;
        private string _cwfToken = string.Empty;

        internal const string ErrorInvalidCoreAdminCredentials = "Invalid Core Administrator Credentials.";
        internal const string ErrorConnectionFailure = "Unable to Connect to Dashboard. Please check the Dashboard URL.";
        internal const string ErrorWebrequestFailed = "Unable to make web request.";

        public CwfHelper(MainForm host)
        {
            _uploadHelper = new FormUpload(host);
            _host = host;
        }

        internal string CwfUrl { get; set; }

        internal string CwfUser {
            get
            { 
                return _cwfUser; 
            }
            set
            { 
                _cwfUser = value;
                _cwfToken = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(CwfUser + ":" + CwfPassword, "\\\\", "\\")));
            } 
        }

        internal string CwfPassword {
            get
            { 
                return _cwfPassword; 
            }
            set
            {
                _cwfPassword = value;
                _cwfToken = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(CwfUser + ":" + CwfPassword, "\\\\", "\\")));
            }
        }

        internal string CwfProductInstance { get; set; }

        private CMProducts m_registeredProducts;
        internal CMProducts ProductsToBeUpgraded { get; set; }

        internal string CwfToken
        {
            get
            {
                return _cwfToken;
            }
            set
            {
                _cwfToken = value;
            }
        }

        internal CMProducts RegisteredProducts
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

        internal bool CanConnectToDashboard()
        {
            var url = string.Format("{0}/IderaCoreServices/v1/Status", CwfUrl);

            string errorMessage;

            var response = GetRequest(url, out errorMessage);
            // if we are able to connect to dashboard
            return !string.IsNullOrEmpty(response) || errorMessage.Equals(ErrorInvalidCoreAdminCredentials, StringComparison.InvariantCultureIgnoreCase);
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

        internal string LoadRegisteredProducts()
        {
            var url = String.Format("{0}/IderaCoreServices/v1/Products?shortname={1}",
                                    CwfUrl,
                                    Constants.ProductShortName);
            string errorMessage;
            var response = GetRequest(url, out errorMessage);

            m_registeredProducts = CwfAddinInstaller.JsonHelper.FromJson<CMProducts>(response);

            CMProducts oldProduct = new CMProducts();
            oldProduct = GetRegisteredSqlCmProducts();
            foreach (var product in oldProduct)
            {
                if (product.Name.Equals("SQLCM"))
                {
                    oldProductId = product.Id;
                }
            }

            return errorMessage;
        }

        public List<string> GetRegisteredSQLCMProductNames()
        {
            List<string> productList = new List<string>();
            foreach (var product in RegisteredProducts)
            {
                if (product.ShortName.Equals(Constants.ProductShortName, StringComparison.InvariantCultureIgnoreCase))
                {
                    productList.Add(product.Name);
                }
            }
            return productList;
        }

        public CMProducts GetRegisteredSqlCmProducts()
        {
            var products = new CMProducts();

            foreach (var product in RegisteredProducts)
            {
                if (product.ShortName.Equals(Constants.ProductShortName, StringComparison.InvariantCultureIgnoreCase))
                {
                    products.Add(product);
                }
            }

            return products;
        }

        internal bool RegisterProduct()
        {
            SetCwfUrlAndTokenInRegistry();

            if (ProductsToBeUpgraded == null)
            {
                try
                {
                    RegisterNewProduct();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                foreach (var product in ProductsToBeUpgraded)
                {
                    try
                    {
                        UpgradeProduct(product.Id);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return false;
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

        private void RegisterNewProduct()
        {
            var postParameters = GetDataToRegister();

            // create request and receive response
            var postUrl = string.Format("{0}/IderaCoreServices/v1/RegisterProduct/", CwfUrl);
            Thread.Sleep(3000);
            MakeRequest(postUrl, postParameters);
            SetRegistryValue(postParameters);
        }

        private void UpgradeProduct(int productId)
        {
            var url = string.Format("{0}/IderaCoreServices/v1/RegisterProduct/Upgrade/{1}", CwfUrl, productId);
            var postParameters = GetDataToUpgrade();
            MakeRequest(url, postParameters);
            SetRegistryValue(postParameters);
        }

        private Dictionary<string, object> GetDataToRegister()
        {
            //_host.Log("Reading AddIn zip file '{0}'.", _host.CwfAddInZip);
            //if (!File.Exists(_host.CwfAddInZip))
                //_host.Log("AddIn zip file missiong.");

            var fs = new FileStream(_host.CwfAddInZip, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            var postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", Constants.ProductName);
            postParameters.Add("ShortName", Constants.ProductShortName);
            postParameters.Add("Version", Constants.ProductVersion);
            postParameters.Add("Status", Constants.Status);
            postParameters.Add("Location", String.Join(";", InstallProperties.CMSQLServerInstanceName, "SQLcompliance"));
            postParameters.Add("ConnectionUser", CwfUser);
            postParameters.Add("ConnectionPassword", CwfPassword);
            postParameters.Add("RegisteringUser", CwfUser);
            postParameters.Add("WebURL", Constants.WebUrl);
            postParameters.Add("RestURL", Constants.RestUrl);
            postParameters.Add("JarFile", Constants.JarFile);
            postParameters.Add("Description", Constants.ProductDescription);
            postParameters.Add("DefaultPage", Constants.DefaultPage);
            postParameters.Add("InstanceName", CwfProductInstance);
            postParameters.Add("RestFile", Constants.AssemblyFile);
            postParameters.Add("ProductServicesHost", System.Environment.MachineName);
            postParameters.Add("ProductServicesPort", Constants.ServicePort);
            postParameters.Add("IsWarEnabled", Constants.IsWarEnabled);
            postParameters.Add("WarFileName", Constants.WarFileName);
            postParameters.Add("product", new FormUpload.FileParameter(data, Constants.ZipFile, "application/x-zip-compressed"));
            return postParameters;
        }

        private Dictionary<string, object> GetDataToUpgrade()
        {
            /*_host.Log("Reading AddIn zip file '{0}'.", _host.CwfAddInZip);
            if (!File.Exists(_host.CwfAddInZip))
                _host.Log("AddIn zip file missiong.");*/

            var fs = new FileStream(_host.CwfAddInZip, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            var postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", Constants.ProductName);
            postParameters.Add("ShortName", Constants.ProductShortName);
            postParameters.Add("Version", Constants.ProductVersion);
            postParameters.Add("JarFile", Constants.JarFile);
            postParameters.Add("Description", Constants.ProductDescription);
            postParameters.Add("DefaultPage", Constants.DefaultPage);
            postParameters.Add("RestFile", Constants.AssemblyFile);
            postParameters.Add("ProductServicesHost", System.Environment.MachineName);
            postParameters.Add("ProductServicesPort", Constants.ServicePort);
            postParameters.Add("Location", String.Join(";", InstallProperties.CMSQLServerInstanceName, "SQLcompliance"));
            postParameters.Add("IsWarEnabled", Constants.IsWarEnabled);
            postParameters.Add("WarFileName", Constants.WarFileName);
            postParameters.Add("product", new FormUpload.FileParameter(data, Constants.ZipFile, "application/x-zip-compressed"));
            /*_host.Log("POST Parameters:");
            foreach (var key in postParameters.Keys)
            {
                // we will not log password
                _host.Log("   {0}: {1}", key, key.Equals("ConnectionPassword") ? FormInstaller.NoPassowrdlogging : postParameters[key]);
            }*/

            return postParameters;
        }

        public void SetRegistryValue(Dictionary<string, object> postParameters)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
            if (key == null)
                key = hklm.CreateSubKey(@"Software\Idera\SQLCM");
            key.SetValue("Version", postParameters.Where(z => z.Key == "Version").FirstOrDefault().Value);
            key.SetValue("ServiceHost", Dns.GetHostName());
            key.SetValue("ServicePort", Constants.ServicePort);
            key.SetValue("IsRegisteredToDashboard", Constants.IsRegisteredToDashboard);
            key.SetValue("DisplayName", CwfProductInstance);
            key.SetValue("SQLcmSQLServerName", InstallProperties.CMSQLServerInstanceName);
            if (!InstallProperties.IsUpgradeRadioSelection)
            {
                key.SetValue("Administrator", CwfUser);
                string cwfPass = QuickEncrypt(CwfPassword);
                key.SetValue("ServiceAdminPassword", cwfPass);
            }
        }

        private void MakeRequest(string postUrl, Dictionary<string, object> postParameters)
        {
            // Create request and receive response
            Thread.Sleep(3000);
            var webResponse = _uploadHelper.MultipartFormDataPost(postUrl, string.Empty, postParameters);
            // Process response
            var responseReader = new StreamReader(webResponse.GetResponseStream());
            var fullResponse = responseReader.ReadToEnd();

            var product = CwfAddinInstaller.JsonHelper.FromJson<Product>(fullResponse);
            webResponse.Close();
        }

        private string GetRequest(string url, out string errorMessage)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "application/json";
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + CwfToken);

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException e)
                {
                    switch (e.Status)
                    {
                        case WebExceptionStatus.ProtocolError:
                            errorMessage = ErrorInvalidCoreAdminCredentials;
                            return string.Empty;

                        case WebExceptionStatus.ConnectFailure:
                            errorMessage = ErrorConnectionFailure;
                            return string.Empty;

                        default:
                            errorMessage = e.Message;
                            return string.Empty;
                    }
                }
                catch (Exception e)
                {
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
            catch
            {
                errorMessage = Constants.InvalidRemoteDashboardValues;
                return string.Empty;
            }
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
