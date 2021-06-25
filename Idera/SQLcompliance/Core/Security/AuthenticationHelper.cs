using System;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Idera.SQLcompliance.Core.Security
{
	/// <summary>
	/// Helper methods for Windows authentication
	/// </summary>
	/// <remarks>Constructor is private so that class cannot be instantiated</remarks>
	internal sealed class AuthenticationHelper
	{

		#region Constructor (private)

		/// <summary>
		/// Private constructor
		/// </summary>
		private AuthenticationHelper() {}

		#endregion		

		#region Internal methods

		/// <summary>
		/// Gets the current thread's WindowsIdentity.
		/// </summary>
		/// <remarks>
		/// If fullUsername is present, uses Win32 LogonUser call to return identity;
		/// otherwise, gets WindowsIdentity directly from thread's credentials passed from remoting call
		/// </remarks>
		/// <param name="server"></param>
		/// <param name="fullUsername"></param>
		/// <param name="password"></param>
		/// <returns>The WindowsIdentity that can be used for impersonation</returns>
		internal static WindowsIdentity GetCurrentIdentity(string fullUsername, string password) {

			WindowsIdentity currentIdentity;
			if (fullUsername != null) { // User authenticated manually			

				IntPtr tokenHandle = IntPtr.Zero;
				IntPtr dupeTokenHandle = IntPtr.Zero;

				string domain = ParseDomainFromFullUsername(fullUsername);
				string username = ParseUsernameFromFullUsername(fullUsername);					
				
				// Call LogonUser to obtain a handle to an access token	
				bool returnValue = NativeMethods.LogonUser(username, domain, password, 
					NativeMethods.LOGON32_LOGON_INTERACTIVE, NativeMethods.LOGON32_PROVIDER_DEFAULT,
					ref tokenHandle);

				if (!returnValue) {
					int returnCode = Marshal.GetLastWin32Error();
					if (returnCode == 0x00000522) { // "A required privilege was not held by the user"
						throw new CoreException(String.Format(CoreConstants.Exception_CouldNotImpersonateUser + ", error message: {0}", NativeMethods.GetErrorMessage(returnCode)));
					} else {
						throw new SqlComplianceSecurityException(String.Format("Could not logon user, error message: {0}", NativeMethods.GetErrorMessage(returnCode)));
					}
				}

				// Duplicate the token, so that we can get a primary token for impersonation
				returnValue = NativeMethods.DuplicateToken(tokenHandle, NativeMethods.SECURITY_IMPERSONATION, ref dupeTokenHandle);

				if (!returnValue) {
					int returnCode = Marshal.GetLastWin32Error();
					throw new SqlComplianceSecurityException(String.Format("Could not logon user (with impersonation token), error message: {0}", NativeMethods.GetErrorMessage(returnCode)));
				}				

				// Generate the WindowsIdentity from the duplicate (primary) token
				currentIdentity = new WindowsIdentity(dupeTokenHandle);

				// Close the access token handle
				NativeMethods.CloseHandle(tokenHandle);
				NativeMethods.CloseHandle(dupeTokenHandle);

			} else { 

				// Use authentication credentials from remoting security
				currentIdentity = (WindowsIdentity)Thread.CurrentPrincipal.Identity;
	
			}

			return currentIdentity;

		}		

		/// <summary>
		/// Parses the Windows domain from a full domain\username string
		/// </summary>
		/// <param name="fullUsername">The full domain\username string</param>
		/// <returns>The Windows domain</returns>
		internal static string ParseDomainFromFullUsername(string fullUsername) {
			int index = fullUsername.IndexOf(@"\");
			if (index > 0) {				
				return fullUsername.Substring(0, index);	
			} else {
				return "."; // use local account database
			}			
		}			
		
		/// <summary>
		/// Parses the username from a full domain\username string
		/// </summary>
		/// <param name="fullUsername">The full domain\username string</param>
		/// <returns>The username</returns>
		internal static string ParseUsernameFromFullUsername(string fullUsername) {
			int index = fullUsername.IndexOf(@"\") + 1;
			if (index > 0) {				
				return fullUsername.Substring(index, (fullUsername.Length - index));
			} else {
				return fullUsername;
			}
		}

		/// <summary>
		/// Get the username for the current backup/restore operation
		/// </summary>
		/// <returns></returns>
		internal static string
		   GetOperationUsername(
		      string      sqlServerUsername
		   )
		{
			if (sqlServerUsername != null)
			{
				return sqlServerUsername;
			}
			else
			{
				return Thread.CurrentPrincipal.Identity.Name;
			}
		}

		#endregion
		
	}
}
