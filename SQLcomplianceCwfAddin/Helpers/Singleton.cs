using System;
using Idera.SQLcompliance.Core;
using System.Collections.Generic;

namespace SQLcomplianceCwfAddin.Helpers
{
    public abstract class Singleton<T> where T : class, new()
    {
        #region Private Members
        private static T m_instance;
        #endregion

        #region Properties

        public static T Instance
        {
            get { return m_instance ?? (m_instance = new T()); }
        }

        #endregion

        #region Public Methods

        public string GetTranslatedInstanceName(string instanceName)
        {
            return Transformer.Instance.TranslateServerName(instanceName);
        }

        public List<string> GetTranslatedInstancesNames(List<string> instances)
        {
            var translatedInstances = new List<string>();

            foreach (var instance in instances)
            {
                string translatedInstance = GetTranslatedInstanceName(instance);
                translatedInstances.Add(translatedInstance);
            }

            return translatedInstances;
        }

        public void LogWarning(string message)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Always, message, ErrorLog.Severity.Warning);
        }

        public void LogError(string message)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Always, message, ErrorLog.Severity.Error);
        }

        public void LogError(Exception ex)
        {
            var message = string.Format("{0}. Details: {1}", ex.Message, ex.ToString());
            ErrorLog.Instance.Write(ErrorLog.Level.Always, message, ErrorLog.Severity.Error);
        }

        public void LogAndThrowException(string message)
        {
            LogError(message);
            throw new Exception(message);
        }

        public void LogAndThrowException(string message,Exception innerException)
        {
            LogError(message);
            throw new Exception(message, innerException);
        }

        #endregion
    }
}
