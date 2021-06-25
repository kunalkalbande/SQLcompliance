using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CwfAddinInstaller
{
    internal static class StringExtensions
    {
        public static MemoryStream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);

            if (stream.Position > 0) 
                stream.Position = 0;

            return (stream);
        }
    }

    public static class JsonHelper
    {
        public static T FromJson<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json)) return (null);
            try
            {
                var deserializer = new DataContractJsonSerializer(typeof(T));
                using (var s = json.ToStream())
                {
                    return (deserializer.ReadObject(s) as T);
                }
            }
            catch (Exception ex)
            {
                return (null);
            }
        }
    }
}
