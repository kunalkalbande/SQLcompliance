using System;
using System.Globalization;
using System.Threading;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// A class used for converting from utc to local time
	/// </summary>
	public class LocalDateTime
	{
		#region Constructor

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static LocalDateTime _instance = new LocalDateTime();

		/// <summary>
		/// Private constructor so no one can create additional instances
		/// </summary>
		private LocalDateTime() {}

		/// <summary>
		/// Returns the singleton instance
		/// </summary>
		public LocalDateTime Instance
		{
			get { return _instance; }
		}

		#endregion

		/// <summary>
		/// Returns the utc time in the local time
		/// </summary>
		/// <param name="utcTime">The UTC time</param>
		/// <returns>Local time</returns>
		public static DateTime GetLocalTimeFromUTC(DateTime utcTime)
		{
			try
			{
				// Return the UTC in the local time zone (convert)
				return TimeZone.CurrentTimeZone.ToLocalTime(utcTime);
			}
			catch(Exception)
			{
				return DateTime.Now;
			}
		}

		/// <summary>
		/// Returns the local time as a string formatted according to the local culture
		/// </summary>
		/// <param name="utcTime">Time in UTC</param>
		/// <returns>Formatted date time string specific to local culture</returns>
		public static string GetFormattedLocalTimeFromUTC(DateTime utcTime)
		{
			try
			{
				// Get the UTC as a local time and then format it in the short date/time pattern specific to the local culture
				return LocalDateTime.GetLocalTimeFromUTC(utcTime).ToString("g", CultureInfo.CurrentCulture); 
			}
			catch(Exception)
			{
				return DateTime.Now.ToString();
			}
		}

		/// <summary>
		/// Returns the given date/time as a string formatted for the specific culture
		/// </summary>
		/// <param name="localTime">Local date time</param>
		/// <returns>A culture specific formatted string of the local time</returns>
		public static string GetFormattedLocalTime(DateTime localTime)
		{
			try
			{
				return localTime.ToString("g", CultureInfo.CurrentCulture);
			}
			catch(Exception)
			{
				return DateTime.Now.ToString();
			}
		}

		/// <summary>
		/// Returns a local date time in a universal sortable format (full UTC) independent of culture.
		/// The string returned is of the form "yyyy-mm-ddThh:mm:ss" (universal timestamp format for SQL Server 2K)
		/// </summary>
		/// <param name="localTime">The local time to convert to UTC</param>
		/// <returns>A string formatted in the universal sortable format</returns>
		public static string GetUTCCultureFreeDateTime(DateTime localTime)
		{
			// Convert to UTC
			DateTime dt = TimeZone.CurrentTimeZone.ToUniversalTime(localTime);

			// Return the UTC in the appropriate format
			return dt.ToString("s");
		}
	}
}
