using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLcomplianceRegistrationService.Properties;
using System.IO;
using System.Net;
using SQLcomplianceRegistrationService.Helpers;
using TracerX;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Win32;
using System.ServiceModel.Web;

namespace SQLcomplianceRegistrationService
{
    public class SQLcmProduct : IProduct
    {
        private int _productId = 0;
        private static readonly Logger LogX = Logger.GetLogger("SQLcomplianceRegistrationServiceLogger");

        public void SetDashboardLocation(NotifyProduct product)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
            if (key == null)
                key = hklm.CreateSubKey(@"Software\Idera\SQLCM");
            key.SetValue("Version", Constants.ProductVersion);
            key.SetValue("DashboardHost", product.Host);
            key.SetValue("DashboardPort", product.Port.ToString());
            key.SetValue("DashboardAdministrator", product.User);
            key.SetValue("IsRegisteredToDashboard", product.IsRegistered);
            key.SetValue("ProductID", product.ProductID);
            key.SetValue("DisplayName", product.DisplayName);
            // Use the migration AttemptID to change the local mappings of Instances, Databases and Tags
        }
        public Object GetValueFromRegistry(string Name)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey Key = hklm.OpenSubKey(@"Software\Idera\SQLCM");
            Object value = Key.GetValue(Name);
            if (value == null) return "";
            return value;
        }
        public void SetValueInRegistry(string Name, Object value)
        {
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey Key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
            Key.SetValue(Name, value);
        }
        public void UnregisterSampleProduct(string notify = "true")
        {
            int id = Convert.ToInt32(GetValueFromRegistry("ProductID"));
            string host = GetValueFromRegistry("DashboardHost").ToString();
            string port = GetValueFromRegistry("DashboardPort").ToString();
            try
            {
                string url = String.Format("http://{0}:{1}",host,port) ;
                string postURL = url + "/IderaCoreServices/v1/Products/" + id;
                if (notify == "false")
                {
                    postURL += "?notify=false";
                }
                LogX.InfoFormat("Delete URL: {0}", postURL);
                UnregisterProductFromCWF(postURL);
                var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
                RegistryKey key = hklm.OpenSubKey(@"Software\Idera\SQLCM", true);
                key.SetValue("IsRegisteredToDashboard", false);
                key.SetValue("ProductID", 0);
                LogX.Info("Update the Settings File");
                LogX.InfoFormat("Deleted Product Id {0}", id);
                LogX.Info("Settings File Updated");
            }
            catch (Exception e)
            {
                LogX.ErrorFormat("Error While UnRegister Of Product: {0}", e.Message.ToString());
                throw e;
            }
        }

        public void RegisterSampleProduct()
        {
            LogX.Info("Checking If Product is Registered");
            int productId;
            if (!NeedUpgrade(out productId))
            {
                LogX.Info("Product has already been Registered");
                return;
            }
            if (productId == 0)
            {
                LogX.Info("Registering the product");
                RegisterProduct();
            }
            else
            {
                LogX.Info("Upgrading the product");
                UpgradeProduct(productId);
            }
        }

        private void MakeRequest(string postURL, Dictionary<string, object> postParameters)
        {
            // Create request and receive response
            
            string userAgent = "";
            Settings.Default.Reload();
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters, Constants.ServiceAdminUser, Constants.ServiceAdminPassword);
            // Process response
            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            LogX.Info("Product has been Upgraded/Registered");

            Product product = JsonHelper.FromJSON<Product>(fullResponse);
            _productId = product.Id;
            LogX.InfoFormat("Upgraded/Registered Product Id {0}", product.Id);
            Constants.IsRegistered = true;
            Constants.ProductId = product.Id;
            LogX.Info("Saved the product info in settings file");
            webResponse.Close();
            NotifyProduct notify = new NotifyProduct(Constants.DashboardHost, Convert.ToInt32(Constants.DashboardPort),Constants.ServiceAdminUser, _productId,Constants.Instance);
            SetDashboardLocation(notify);
        }

        private Dictionary<string, object> GetDataToUpgrade()
        {
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "SqlComplianceManager.zip", FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", Constants.ProductName);
            postParameters.Add("ShortName", Constants.ShortName);
            postParameters.Add("Version", Settings.Default.Version);
            postParameters.Add("JarFile", Constants.JarFile);
            postParameters.Add("Description", Constants.Description);
            postParameters.Add("DefaultPage", Constants.DefaultPage);
            postParameters.Add("IsWarEnabled", Constants.IsWarEnabled);
            postParameters.Add("WarFileName", Constants.WarFileName);
            postParameters.Add("product", new FormUpload.FileParameter(data, Constants.ZipFile, "application/x-zip-compressed"));

            return postParameters;
        }
        private void UpgradeProduct(int productId)
        {        
            string url = Settings.Default.CoreServicesUrl + "/IderaCoreServices/v1/RegisterProduct/Upgrade/" + productId;
            Dictionary<string, object> postParameters = GetDataToUpgrade();
            MakeRequest(url, postParameters);
        }
        private Dictionary<string, object> GetDataToRegister()
        {
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "SqlComplianceManager.zip", FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", Constants.ProductName);
            postParameters.Add("ShortName", Constants.ShortName);
            postParameters.Add("Version", Settings.Default.Version);
            postParameters.Add("Status", Constants.Status);
            postParameters.Add("Location", String.Join(";", GetValueFromRegistry("SQLcmSQLServerName"), Constants.SQLcmRepository));
            postParameters.Add("ConnectionUser", Constants.ServiceAdminUser);
            postParameters.Add("ConnectionPassword", Constants.ServiceAdminPassword);
            postParameters.Add("RegisteringUser", Constants.ServiceAdminUser);
            postParameters.Add("WebURL", Constants.WebUrl);
            postParameters.Add("RestURL", Constants.RestUrl);
            postParameters.Add("JarFile", Constants.JarFile);
            postParameters.Add("Description", Constants.Description);
            postParameters.Add("DefaultPage", Constants.DefaultPage);
            postParameters.Add("InstanceName", GetValueFromRegistry("SQLcmSQLServerName"));
            postParameters.Add("RestFile", Constants.AssemblyFile);
            postParameters.Add("ProductServicesHost", System.Environment.MachineName);
            postParameters.Add("ProductServicesPort", Constants.ServicePort);
            postParameters.Add("IsTaggable", Constants.IsTaggable);
            postParameters.Add("IsWarEnabled", Constants.IsWarEnabled);
            postParameters.Add("WarFileName", Constants.WarFileName);
            postParameters.Add("product", new FormUpload.FileParameter(data, Constants.ZipFile, "application/x-zip-compressed"));
            return postParameters;
        }
        private void RegisterProduct()
        {
            Dictionary<string, object> postParameters = GetDataToRegister();
            // Create request and receive response
            string postURL = Settings.Default.CoreServicesUrl + "/IderaCoreServices/v1/RegisterProduct/";
            MakeRequest(postURL, postParameters);
        }

        public Stream GetProductData()
        {
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            var parts = PluginCommon.Validator.getUsernamePasswordFromHeader(header);
            bool isProductRegistered = Convert.ToBoolean(GetValueFromRegistry("IsRegisteredToDashboard"));
            if (isProductRegistered)
                throw new WebFaultException(HttpStatusCode.BadRequest);
            Dictionary<string, object> postParameters = GetDataToRegister();
            if (!(parts[0].ToLower().Equals(Constants.ServiceAdminUser.ToLower())))
            {
                LogX.ErrorFormat("User not service user");
                throw new WebFaultException(HttpStatusCode.Unauthorized);
            }
            else if (!PluginCommon.Validator.ValidateUser(parts[0], parts[1]))
            {
                LogX.ErrorFormat("User not windows user");
                throw new WebFaultException(HttpStatusCode.Unauthorized);
            }
            else
            {
                Constants.ServiceAdminPassword = parts[1];
            }
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = FormUpload.GetMultiPartFormDataForResponse(postParameters);
            return new MemoryStream(formData);
        }
        private bool NeedUpgrade(out int productId)
        {
            string URL = String.Format(Settings.Default.CoreServicesUrl + "/IderaCoreServices/v1/Products/{0}?version={1}&instancename={2}", Constants.ProductName, Constants.ProductVersion, Constants.Instance);
            string response = GetRequest(URL);
            Products _products = JsonHelper.FromJSON<Products>(response);
            if (_products.Count == 0)
            {
                productId = 0;
                return true;
            }
            Version InstallerVersion = new Version(Settings.Default.Version);
            foreach (Product product in _products)
            {
                if(product.InstanceName == Constants.Instance)
                {
                    Version CurrentVersion = new Version(_products[0].Version);
                    var value = InstallerVersion.CompareTo(CurrentVersion);
                    productId = _products[0].Id;
                    return (value == 0 || value == -1) ? false : true;
                }
            }
            productId = 0;
            return true;
            
        }

        private string GetRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            string username = Constants.ServiceAdminUser;
            string password = Constants.ServiceAdminPassword;
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));
            request.Headers.Add(HttpRequestHeader.Authorization, header);
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
            }
            catch(WebException e)
            {
                //log the error
                LogX.ErrorFormat("Unable to Get the Product Details: {0}",e);
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
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                }

                return responseValue;
            }
        }
        

        private string UnregisterProductFromCWF(string postUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(postUrl);

            request.Method = "DELETE";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            string username = Constants.ServiceAdminUser;
            string password = Constants.ServiceAdminPassword;
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));
            request.Headers.Add(HttpRequestHeader.Authorization, header);
            var encoding = new UTF8Encoding();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes("");
            request.ContentLength = bytes.Length;
            try
            {
                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                LogX.ErrorFormat("Unable to Delete the Product Details: {0}", e);
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
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                }
                return responseValue;
            }
        }
    }
}
