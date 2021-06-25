using System;
using System.Collections;
using System.IO;

namespace Idera.SQLcompliance.Utility.TraceRegister
{
	/// <summary>
	/// Summary description for FileSorter.
	/// </summary>
   public class FileInfoComparer : IComparer
   {
      #region IComparer Members

      public int Compare( object x, object y)
      {
         int rc = 0;
         try
         {
            rc = Comparer.DefaultInvariant.Compare( ((FileInfo)x).LastWriteTime, ((FileInfo)y).LastWriteTime );
         }
         catch
         {
            rc = -1;
         }
         return rc;
      }

      #endregion

   }
}
