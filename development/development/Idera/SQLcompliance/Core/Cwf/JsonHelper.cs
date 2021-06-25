using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace Idera.SQLcompliance.Core.Cwf
{
    /// <summary>
    /// For serializing and desirializing json
    /// </summary>
    public static class JsonHelper
    {
        public static T FromJson<T>(string json) where T : class
        {

            if (string.IsNullOrEmpty(json)) return (null);
            try
            {
                return (JsonConvert.DeserializeObject<T>(json) as T);
            }
            catch (Exception ex)
            {
                return (null);
            }

        }

        public static string ToJson<T>(T obj) where T : class
        {
            if (obj == null) return (string.Empty);
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                string json;
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, obj);
                    json = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
                }

                return json;
            }
            catch (Exception ex)
            {
                return (null);
            }
        }
    }
}
