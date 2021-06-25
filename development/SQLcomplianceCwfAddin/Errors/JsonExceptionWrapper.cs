using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SQLcomplianceCwfAddin.Errors
{
    [DataContract]
    public class JsonExceptionWrapper
    {
        public JsonExceptionWrapper(Exception ex)
        {
            ApplicationID = 1400;
            if (null == ex)
                return;

            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;

            // try to pull extended error code information (This should work for Win32Exception and SqlException objects)
            if (ex is ExternalException)
                ExtendedErrorCode = ((ExternalException) ex).ErrorCode;

            if (ex is SqlException)
                ExtendedErrorNumber = ((SqlException) ex).Number;

            if (null != ex.InnerException)
                InnerException = new JsonExceptionWrapper(ex.InnerException);
        }

        [DataMember(Order = 1)]
        public int ApplicationID { get; set; }

        [DataMember(Order = 2)]
        public int ExtendedErrorCode { get; set; }

        [DataMember(Order = 3)]
        public int ExtendedErrorNumber { get; set; }

        [DataMember(Order = 4)]
        public string Message { get; set; }

        [DataMember(Order = 5)]
        public string Source { get; set; }

        [DataMember(Order = 6)]
        public string StackTrace { get; set; }

        [DataMember(Order = 7)]
        public JsonExceptionWrapper InnerException { get; set; }
    }
}