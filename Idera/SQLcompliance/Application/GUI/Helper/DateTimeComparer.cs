using System;
using System.Collections;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for DateTimeComparer.
	/// </summary>
	public class DateTimeComparer : IComparer
	{
		public DateTimeComparer()
		{
		}

      int IComparer.Compare( object x, object y )
      {
         if( x is DateTime && y is DateTime  )
        {
           int retval = ((DateTime)x).CompareTo( (DateTime)y );
           return retval;
         }
         throw new ArgumentException( "Can only compare timedate values" );
      }
	}
}
