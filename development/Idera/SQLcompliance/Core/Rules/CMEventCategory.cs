using System.Collections.Generic ;

namespace Idera.SQLcompliance.Core.Rules
{
   public class CMEventCategory
   {
      #region Member Variables

      private string _name ;
      private int _categoryId ;
      private List<CMEventType> _eventTypes ;
      private bool _excludable ;

      #endregion

      #region Properties

      public string Name
      {
         get { return _name ; }
         set { _name = value ; }
      }

      public int CategoryId
      {
         get { return _categoryId ; }
         set { _categoryId = value ; }
      }

      public IList<CMEventType> EventTypes
      {
         get { return new List<CMEventType>(_eventTypes) ; }
      }

      public bool Excludable
      {
         get { return _excludable ; }
         set { _excludable = value ; }
      }

      #endregion

      #region Construction/Destruction

      public CMEventCategory()
      {
         _eventTypes = new List<CMEventType>() ;
         _excludable = false ;
      }

      public CMEventCategory(string sName, int id) : this()
      {
         _name = sName ;
         _categoryId = id ;
      }

      #endregion

      public void AddEventType(CMEventType item)
      {
         if(!_eventTypes.Contains(item))
         {
            item.CategoryId = _categoryId ;
            _eventTypes.Add(item) ;
            if (item.Excludable)
               _excludable = true;
         }
      }

      public override string ToString()
      {
         return _name ;
      }
   }
}