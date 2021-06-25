using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using SQLcomplianceRegistrationService.Extensions;

namespace SQLcomplianceRegistrationService.Helpers
{
    public static class JsonHelper
    {
        public static T FromJSON<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json)) return (null);
            try
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                using (MemoryStream s = json.ToStream())
                {
                    return (deserializer.ReadObject(s) as T);
                }
            }
            catch (Exception ex)
            {
                //ExceptionLogger.Log(string.Format("JsonHelper.FromJSON<{0}>({1})", typeof(T), json), ex);
                return (null);
            }
        }
        
    }
}
