using System;
using System.Runtime.InteropServices;

namespace Idera.SQLcompliance.Core
{
	/// <summary>
	/// Native methods container class for SQLsecureCore.
	/// Wrapper routines for low level security routines in InstallUtil.DLL
	/// </summary>	
	public class InstallUtil
	{
		/// <summary>
		/// Private constructor; this class cannot be instantiated.
		/// </summary>
		private InstallUtil() {}

      //----------------------------------------------------------------------------------------
      // Create a directory and set initial permissions
      //----------------------------------------------------------------------------------------
	   [DllImport("InstallUtilLib")]
	   internal static extern int CreateDirAndGiveFullControl(
		   [In]string dirPathIn,// Path of directory to create.
		   [In]string sidIn// Account SID to give full control to.
      );

		//----------------------------------------------------------------------------------------
		// Validate the supplied account credentials to be a legitimate domain account
		//----------------------------------------------------------------------------------------
		[DllImport("InstallUtilLib")]
		public static extern int VerifyPassword([In]string accountIn, [In]string passwordIn) ;

		//----------------------------------------------------------------------------------------
		// Grant rights to account
		//----------------------------------------------------------------------------------------
		[DllImport("InstallUtilLib")]
        public static extern int 
         GiveLogonAsServicePriv(
            [In]string accountIn );
	}
}
