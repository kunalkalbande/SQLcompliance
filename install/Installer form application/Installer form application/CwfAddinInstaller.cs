using Microsoft.Win32;
using SQLcomplianceCwfAddin.RestService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Installer_form_application
{
    class CwfAddinInstaller
    {
        private Products m_registeredProducts;
        private readonly Version m_installerVersion = new Version(RestServiceConstants.ProductVersion);
        public string CwfToken = Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(properties.SPSUsername + ":" + properties.SPSPassword, "\\\\", "\\")));
        int oldProductId;
        public static bool IsSQLCMUpgrade = false;
        string MachineName = String.Empty;
        public string CwfUrl = String.Empty;

        public CwfAddinInstaller()
        {
            if (properties.localInstall)
                MachineName = Dns.GetHostName();
            else
                MachineName = properties.RemoteHostname;
            CwfUrl = string.Format("http://{0}:{1}", MachineName, properties.CoreServicesPort);
        }
        

        internal bool RegisterProduct(string currentPath)
        {
            if (CheckforUpgrade())
            {
                try
                {
                    UpgradeProduct(oldProductId, currentPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    RegisterNewProduct(currentPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void UpgradeProduct(int productId, String currentPath)
        {
            var url = string.Format("{0}/IderaCoreServices/v1/RegisterProduct/Upgrade/{1}", CwfUrl, productId);
            var postParameters = GetDataToUpgrade(currentPath);
            MakeRequest(url, postParameters);
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
            key.SetValue("Administrator", properties.SPSUsername);
            key.SetValue("IsRegisteredToDashboard", RestServiceConstants.IsRegisteredToDashboard);
            key.SetValue("DisplayName", properties.DisplayName);
            key.SetValue("SQLcmSQLServerName", properties.JMInstance);
            key.SetValue("ServiceAdminPassword", QuickEncrypt(properties.SPSPassword));
        }

        private Dictionary<string, object> GetDataToUpgrade(String currentPath)
        {
            string zipPath;
            if (Environment.Is64BitOperatingSystem)
                zipPath = currentPath + "\\Full\\x64\\SqlCompliancemanager.zip";
            else
                zipPath = currentPath + "\\Full\\x86\\SqlCompliancemanager.zip";
            var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
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
            postParameters.Add("Location", String.Join(";", properties.JMInstance, "SQLcompliance"));
            postParameters.Add("product", new FileParameter(data, RestServiceConstants.ZipFile, "application/x-zip-compressed"));

            return postParameters;
        }

        internal void UnregisterProduct(int productId)
        {
            var postURL = string.Format("{0}/IderaCoreServices/v1/Products/{1}", CwfUrl, productId);
            var request = (HttpWebRequest)WebRequest.Create(postURL);

            request.Method = "DELETE";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            var header = "Basic " + CwfToken;
            request.Headers.Add("Authorization", header);
            var encoding = new UTF8Encoding();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes("");
            request.ContentLength = bytes.Length;

            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
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
            }
        }

        internal bool CheckforUpgrade()
        {
            bool isUpgrade = false;
            var allRegistredSqlCmProducts = GetRegisteredSqlCmProducts();
            if (allRegistredSqlCmProducts == null)
            {
                isUpgrade = false;
                return isUpgrade;
            }

            foreach (var product in allRegistredSqlCmProducts)
            {
                var productVersion = new Version(product.Version);
                string location = product.Location.Replace(".", Environment.MachineName).ToUpper().Replace("(LOCAL)", Environment.MachineName);
                if (product.Name.Equals("SQLCM") && location.StartsWith(properties.JMInstance.Replace(".", Environment.MachineName).ToUpper().Replace("(LOCAL)", Environment.MachineName)))
                {
                    if (m_installerVersion <= productVersion)
                    {
                        MessageBox.Show("SQLCM product installer have the lower or similar version. Please upgrade product with upper version of SQLCM Product.", "Similar Version Installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                    else
                    {
                        oldProductId = product.Id;
                        isUpgrade = true;
                        IsSQLCMUpgrade = true;
                        break;
                    }
                }
            }
            return isUpgrade;
        }

        internal Products GetRegisteredSqlCmProducts()
        {
            var url = String.Format("{0}/IderaCoreServices/v1/Products?shortname={1}",
                                    CwfUrl,
                                    RestServiceConstants.ProductShortName);
            string errorMessage;
            var response = GetRequest(url, out errorMessage, CwfToken);

            m_registeredProducts = NewJsonHelper.FromJson<Products>(response);

            return m_registeredProducts;
        }

        public string GetRequest(string url, out string errorMessage, string cwfToken)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + cwfToken);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
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

        internal void RegisterNewProduct(string currentPath)
        {
            var postParameters = GetDataToRegister(currentPath);
            var postUrl = string.Format("{0}/IderaCoreServices/v1/RegisterProduct/", CwfUrl);
            Thread.Sleep(3000);
            MakeRequest(postUrl, postParameters);
            SetRegistryValue(postParameters);
        }

        private Dictionary<string, object> GetDataToRegister(string currentPath)
        {
            string zipPath;
            if(Environment.Is64BitOperatingSystem)
                zipPath = currentPath + "\\Full\\x64\\SqlCompliancemanager.zip";
            else
                zipPath = currentPath + "\\Full\\x86\\SqlCompliancemanager.zip";
            var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            var postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", RestServiceConstants.ProductName);
            postParameters.Add("ShortName", RestServiceConstants.ProductShortName);
            postParameters.Add("Version", RestServiceConstants.ProductVersion);
            postParameters.Add("Status", RestServiceConstants.Status);
            postParameters.Add("Location", String.Join(";", properties.JMInstance, "SQLcompliance"));
            postParameters.Add("ConnectionUser", properties.SPSUsername);
            postParameters.Add("ConnectionPassword", properties.SPSPassword);
            postParameters.Add("RegisteringUser", properties.SPSUsername);
            postParameters.Add("WebURL", RestServiceConstants.WebUrl);
            postParameters.Add("RestURL", RestServiceConstants.RestUrl);
            postParameters.Add("JarFile", RestServiceConstants.JarFile);
            postParameters.Add("Description", RestServiceConstants.ProductDescription);
            postParameters.Add("DefaultPage", RestServiceConstants.DefaultPage);
            postParameters.Add("InstanceName", properties.DisplayName);
            postParameters.Add("RestFile", RestServiceConstants.AssemblyFile);
            postParameters.Add("ProductServicesHost", System.Environment.MachineName);
            postParameters.Add("ProductServicesPort", RestServiceConstants.ServicePort);
            postParameters.Add("product", new FileParameter(data, RestServiceConstants.ZipFile, "application/x-zip-compressed"));

            return postParameters;
        }

        private void MakeRequest(string postUrl, Dictionary<string, object> postParameters)
        {
            // Create request and receive response
            Thread.Sleep(3000);
            var webResponse = MultipartFormDataPost(postUrl, string.Empty, postParameters);
            // Process response
            var responseReader = new StreamReader(webResponse.GetResponseStream());
            var fullResponse = responseReader.ReadToEnd();

            var product = JsonCWFHelper.FromJson<Product>(fullResponse);
            webResponse.Close();
        }

        public HttpWebResponse MultipartFormDataPost(string postUrl,
                                                     string userAgent,
                                                     Dictionary<string, object> postParameters)
        {
            var formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            var contentType = "multipart/form-data; boundary=" + formDataBoundary;
            Thread.Sleep(3000);
            var formData = GetMultipartFormData(postParameters, formDataBoundary);
            Thread.Sleep(3000);
            return PostForm(postUrl, userAgent, contentType, formData);
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            var needsClrf = false;
            Encoding _encoding = Encoding.UTF8;

            foreach (var param in postParameters)
            {
                // Add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsClrf)
                    formDataStream.Write(_encoding.GetBytes("\r\n"), 0, _encoding.GetByteCount("\r\n"));

                needsClrf = true;

                if (param.Value is FileParameter)
                {
                    var fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    var header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                                                boundary,
                                                param.Key,
                                                fileToUpload.FileName ?? param.Key,
                                                fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(_encoding.GetBytes(header), 0, _encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    var postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                                                 boundary,
                                                 param.Key,
                                                 param.Value);
                    formDataStream.Write(_encoding.GetBytes(postData), 0, _encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            var footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(_encoding.GetBytes(footer), 0, _encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            var formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        private HttpWebResponse PostForm(string postUrl,
                                                string userAgent,
                                                string contentType,
                                                byte[] formData)
        {
            var request = WebRequest.Create(postUrl) as HttpWebRequest;
            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            request.Accept = "application/json";
            var header = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Regex.Replace(properties.SPSUsername + ":" + properties.SPSPassword, "\\\\", "\\")));
            request.Headers.Add("Authorization", header);

            // Send the form data to the request.
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
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
