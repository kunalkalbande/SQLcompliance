// *******************************************************************
//                  ***** IMPORTANT NOTE !!!!! *****
// *******************************************************************
// This file is created dynamically by the NANT build scripts in the
// case of a beta version. If you make changes to this file, you
// also need ot make changes to hte generated file in the main
// build scripts (For SQLcompliance its SQLcompliance.build 
// *******************************************************************
using System;

namespace Idera.SQLcompliance.Core.Beta
{
	/// <summary>
	/// Summary description for BetaConstants.
	/// </summary>
	public class BetaConstants
	{
		private BetaConstants()
		{
		}
		
	   // These are flags and dates to trigger a beta release
	   //  * beta labels in splash screens, startup messages, about forms
	   //  * check of expiration date (if dropdate not MaxValue)
	   
      public static bool      betaRelease          = false;
      public static DateTime  betaDropDeadDate     = DateTime.MaxValue;

      // public static DateTime  m_betaDropDeadDate   
      //   = new DateTime( 2006,     // year
      //                   1,        // month
      //                   1,        //day
      //                   0,0,0,0); //time
	}
}
