using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for RecentlyUsedList.
	/// </summary>
	public class RecentlyUsedList
	{
		private int _maxItems     = 5;
		List<string> _items;

      public static RecentlyUsedList Instance ;

      static RecentlyUsedList()
      {
         Instance = new RecentlyUsedList() ;
      }

	   //-----------------
      // Property: Count
      //-----------------
      public int Count
      { 
         get { return _items == null ? 0 : _items.Count ;} 
      }

      //---------
      // GetItem
      //---------
		public string GetItem( int ndx )
		{
		   if ( ndx < _items.Count && ndx >= 0 )
		      return _items[ndx];
		   else
		      return "";
		}

      //-------------
      // Constructor		
      //-------------
		private RecentlyUsedList()
		{
		   _items = Settings.Default.RecentServers ;
		}

//	   //-----------------------------------------------------------
//		// WriteToRegistry - Write RU list to application registry
//		//-----------------------------------------------------------
//		public void
//		   WriteToRegistry(
//		      RegistryKey       rks,
//		      string            valueNamePrefix
//	      )
//		{
//		   string valueName;
//		   
//		   try
//		   {
//		      // Write number of items
//		      valueName = String.Format( "{0}_{1}", valueNamePrefix, "Count");
//            rks.SetValue(valueName, nItems );
//            
//            // write items
//		      for (int i=0; i<nItems; i++)
//		      {
//    	         valueName = String.Format( "{0}_{1}", valueNamePrefix, i);
//               rks.SetValue(valueName, item[i] );
//		      }
//		   }
//		   catch (Exception)
//		   {
//		   }
//		}
//		
//		//-----------------------------------------------------------
//		// ReadFromRegistry - Read RU list from application registry
//		//-----------------------------------------------------------
//		public void
//		   ReadFromRegistry(
//		      RegistryKey       rks,
//		      string            valueNamePrefix
//         )
//		{
//		   string  valueName;
//		   int     itemsRead = 0;
//
//         try
//         {		   
//		      // Get number of items
//		      valueName = String.Format( "{0}_{1}", valueNamePrefix, "Count");
//            nItems = (int)rks.GetValue(valueName);
//            if ( nItems > maxItems ) nItems = maxItems;
//   		   
//		      // read items
//		      for (int i=0; i<nItems; i++)
//		      {
//   		      valueName = String.Format( "{0}_{1}", valueNamePrefix, i);
//               item[i] = (string)rks.GetValue(valueName);
//               itemsRead ++;
//		      }
//		   }
//		   catch( Exception )
//		   {
//		   }
//		   
//		   nItems = itemsRead;
//		}
		
		//-----------------------------------------------------------
		// Insert - Add new item to front of list; if item was 
		//          already in list - remove its old entry 
		//
		//          This code is pretty inefficient but who cares
		//          with five items - it is simple and works
		//-----------------------------------------------------------
		public void Insert( string newItem ) 
		{
		   // find existing item and remove
         if(_items.Contains(newItem))
            _items.Remove(newItem) ;

         // Put item at beginning of list
         _items.Insert(0, newItem) ;

         // Dump extras beyond our max
         if(_items.Count > _maxItems)
            _items.RemoveRange(_maxItems, _items.Count - _maxItems) ;

		   /*
		   int ndx;
		   for ( ndx=0; ndx<nItems; ndx++ ) if ( item[ndx]==newItem ) break;

         if ( ndx == nItems ) // not found
         {
            // not found; push everybody down
            if ( nItems < maxItems) nItems++;
            for (int i=nItems-1; i>0; i-- )
            {
               item[i] = item[i-1];
            }
         }	
         else if ( ndx != 0 )
         {
            // overwrite and move to front
            for (int i=ndx; i>0; i-- )
            {
               item[i] = item[i-1];
            }
         }	  
         item[0] = newItem;*/
		} 
	}
}
