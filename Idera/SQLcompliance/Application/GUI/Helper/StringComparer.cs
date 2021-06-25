using System;
using System.Collections;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for StringComparer.
	/// </summary>
	public class StringComparer : IComparer
	{
		public StringComparer()
		{
		}

      int IComparer.Compare( object x, object y )
      {
         if( x is string && y is string  )
        {
           int retval = String.Compare((string)x, (string)y, true );
           return retval;
         }
         throw new ArgumentException( "Can only compare string values" );
      }
	}
}
