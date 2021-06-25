using System;
using System.Collections ;

namespace Idera.SQLcompliance.Core.Rules
{
	public class CMEventType : IComparable
	{
		#region Member Variables

		private string _name ;
		private int _typeId ;
		private int _categoryId ;
      private bool _excludable;

		#endregion

		#region Properties

		public string Name
		{
			get { return _name ; }
			set { _name = value ; }
		}

		public int TypeId
		{
			get { return _typeId ; }
			set { _typeId = value ; }
		}

		public int CategoryId
		{
			get { return _categoryId ; }
			set { _categoryId = value ; }
		}

      public bool Excludable
      {
         get { return _excludable ; }
         set { _excludable = value ; }
      }

		#endregion

		#region Construction/Destruction

		public CMEventType(string name, int id)
		{
			_name = name ; 
			_typeId = id ;
		}

		#endregion

		public override string ToString()
		{
			return _name ;
		}

		public int CompareTo(object obj)
		{
			if(obj is CMEventType)
				return _typeId.CompareTo(((CMEventType)obj)._typeId) ;
			else
				return _typeId.CompareTo(obj) ;
		}
	}


}
