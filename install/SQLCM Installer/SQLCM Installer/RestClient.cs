using SQLCM_Installer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace CwfAddinInstaller
{
    internal class FormUpload
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly MainForm _host;

        internal FormUpload(MainForm host)
        {
            _host = host;
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
            var header = "Basic " + _host.CwfHelper.CwfToken;
            request.Headers.Add("Authorization", header);

            // Send the form data to the request.
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            var needsClrf = false;

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

        internal class FileParameter
        {
            public FileParameter(byte[] file)
                : this(file, null)
            {
            }

            public FileParameter(byte[] file, string filename)
                : this(file, filename, null)
            {
            }

            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }

            public byte[] File { get; set; }

            public string FileName { get; set; }

            public string ContentType { get; set; }

            public override string ToString()
            {
                var toStringBuilder = new StringBuilder();
                toStringBuilder.AppendFormat("FileName: {0}\r\n", FileName);
                toStringBuilder.AppendFormat("ContentType: {0}\r\n", ContentType);
                toStringBuilder.AppendFormat("FileLength: {0}", File.Length);
                return toStringBuilder.ToString();
            }
        }
    }
}