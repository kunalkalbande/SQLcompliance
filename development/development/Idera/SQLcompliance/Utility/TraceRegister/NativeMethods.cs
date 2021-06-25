using System;
using System.Runtime.InteropServices;

namespace Idera.SQLcompliance.Utility.TraceRegister
{
   /// <summary>
   /// Native methods container class for SQLsecureCore.
   /// </summary>	
   internal class NativeMethods
   {

      #region Constructor

      /// <summary>
      /// Private constructor; this class cannot be instantiated.
      /// </summary>
      private NativeMethods() {}

      #endregion


      [DllImport("kernel32.dll", SetLastError = true)]
      internal static extern void FlushFileBuffers(IntPtr handle);

      unsafe public static int GetHashCode(string str)
      {
         int length = str.Length;
         fixed (char* c = str)
         {
            char* cc = c;
            char* end = cc + str.Length - 1;
            int h = 5381;
            for (; cc <= end; cc++)
            {
               h = ((h << 5) + h) ^ *cc;
            }
            return h;
         }
      }
   }
}
