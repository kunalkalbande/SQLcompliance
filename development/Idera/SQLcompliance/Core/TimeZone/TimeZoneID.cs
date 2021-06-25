using System;
//using System.Globalization;
//using System.Runtime.InteropServices;
//using Microsoft.Win32;

namespace Idera.SQLcompliance.Core.TimeZoneHelper
{
	/// <summary>
	/// Used for checking the return value of the GetTimeZoneInformation Windows API call
	/// </summary>
	internal enum TimeZoneID 
	{
		Unknown  = 0,
		Standard = 1,
		Daylight = 2,
		Invalid  = unchecked( (int)0xFFFFFFFF )
	}
}
