namespace Idera.SQLcompliance.Core.Stats
{
   public class StatsData
   {
      #region Data members
      StatsCategory cat;
      int count;
      int oldCount;
      bool inDb;
      bool needUpdate = false;
      #endregion

      #region Properties
      public int Count
      {
         get { return count; }
         set { count = value; }
      }

      public bool inDatabase
      {
         get { return inDb; }
         set { inDb = value; }
      }
      
      public bool NeedUpdate
      {
         get { return needUpdate; }
      }
      
      public StatsCategory Category
      {
         get { return cat; }
      }
      
      #endregion

      #region Constructor

      public StatsData(StatsCategory category, int value, bool newData)
      {
         cat = category;
         count = value;
         oldCount = value;
         if( newData )
         {
            needUpdate = true;
            inDb = false;
         }
         else 
            inDb = true;
         
      }

      #endregion

      #region Public methods

      public void Add(int value)
      {
         count += value;
         needUpdate = true;
      }
      
      public void SetUpdated()
      {
         oldCount = count;
         inDb = true;
         needUpdate = false;
      }
      
      public void Revert()
      {
         count = oldCount;
         needUpdate = false;
      }
      
      #endregion
   }
}
