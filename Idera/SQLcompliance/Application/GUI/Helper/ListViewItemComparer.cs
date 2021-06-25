using System;
using System.Collections;
using System.Windows.Forms;


namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for ListViewItemComparer.
	/// </summary>
   public class ListViewItemComparer : IComparer
   {
      private int col;
      private int dir;
        
      public ListViewItemComparer()
      {
         col = 0;
         dir = 1;
      }
      public ListViewItemComparer(int column, int direction)
      {
         col = column;
         dir = direction;
      }
      public int Compare(object x, object y)
      {
         try
         {
            if ( dir == 1 )
               return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            else
               return String.Compare(((ListViewItem)y).SubItems[col].Text, ((ListViewItem)x).SubItems[col].Text);
         }
         catch (Exception )
         {
            return 0;
         }
       }
    }
}
