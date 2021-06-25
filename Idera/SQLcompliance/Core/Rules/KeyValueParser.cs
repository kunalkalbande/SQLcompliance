using System;
using System.Collections;
using System.Text ;

namespace Idera.SQLcompliance.Core.Rules
{
	/// <summary>
	/// Summary description for KeyValueParser.
	/// </summary>
	public class KeyValueParser
	{
		#region Member Variables

		#endregion

		#region Properties

		#endregion

		#region Construction/Destruction

		public KeyValueParser()
		{
		}

		#endregion

		public static Hashtable ParseString(string s)
		{
			if(s == null)
				throw new ArgumentNullException("s") ;

			Hashtable retVal = new Hashtable() ;
			string active = s ;
			int index = s.IndexOf("(") ;

			try
			{
				while(index != -1)
				{
					string sKey, sValue ;
					int length ;
				
					sKey = active.Substring(0, index) ;
					active = active.Substring(index + 1) ;
					index = active.IndexOf(")") ;
					length = Int32.Parse(active.Substring(0, index)) ;
					active = active.Substring(index + 1) ;
					sValue = active.Substring(0, length) ;
					active = active.Substring(length) ;
					retVal.Add(sKey, sValue) ;
					index = active.IndexOf("(") ;
				}
			}
			catch(Exception e)
			{
				throw new Exception("Improperly formed KeyValue string.", e); 
			}
			if(active.Length > 0)
				throw new Exception("Improperly formed KeyValue string."); 
			return retVal ;
		}

		public static string BuildString(Hashtable map)
		{
			if(map == null)
				throw new ArgumentNullException("map") ;
			StringBuilder retVal = new StringBuilder() ;

         // Sort the keys so the final match string is sorted.
         //  This allows for equality detection on string-type conditions
         //  and multiple integers
         ArrayList keys = new ArrayList() ;
         keys.AddRange(map.Keys) ;
         keys.Sort() ;

         foreach(string sKey in keys)
         {
            object mapValue = map[sKey] ;
            if(sKey.IndexOf("(") != -1)
               throw new Exception("Invalid Key Entry") ;
            retVal.AppendFormat("{0}({1}){2}", sKey, ((string)mapValue).Length, mapValue) ;
         }
			return retVal.ToString() ;
		}
	}


}
