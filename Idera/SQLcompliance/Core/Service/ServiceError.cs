using System;

namespace Idera.SQLcompliance.Core.Service
{
	/// <summary>
	/// Summary description for ServiceErrors.
	/// </summary>
	public class ServiceError
	{
	   static private int m_maxErrorValue = 24;
	
	   static private string[] m_errorMessages = 
	   {
         "Success",
         "Not Supported",
         "Access Denied",
         "Dependent Services Running",
         "Invalid Service Control",
         "Service Cannot Accept Control",
         "Service Not Active",
         "Service Request Timeout",
         "Unknown Failure. Possible cause: Service executable file missing.",
         "Path Not Found",
         "Service Already Running",
         "Service Database Locked",
         "Service Dependency Deleted",
         "Service Dependency Failure",
         "Service Disabled",
         "Service Logon Failure",
         "Service Marked For Deletion",
         "Service No Thread",
         "Status Circular Dependency",
         "Status Duplicate Name",
         "Status Invalid Name",
         "Status Invalid Parameter",
         "Status Invalid Service Account",
         "Status Service Exists",
         "Service Already Paused",
      };

		private ServiceError()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		public static string
		   GetMessageText(
		      UInt32            errorCode
		   )
		{
		   if ( errorCode < 0 || errorCode > m_maxErrorValue )
		   {
		      return String.Format( "Unknown error code: {0}", errorCode );
		   }
		   return m_errorMessages[errorCode];
		}   
	}
}
