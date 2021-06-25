using System;
using System.Data;

namespace Idera.SQLcompliance.Core.Stats
{
   class StatsRow
   {
      #region Data Members

      int dbId;
      StatsCategory cat;
      DateTime date;
      int count;

      #endregion
     
      #region Constants
      
      public readonly string ColDbId = StatsDAL.ColDBId;
      public readonly string ColCategory = StatsDAL.ColCategory;
      public readonly string ColDate = StatsDAL.ColDate;
      public readonly string ColCount = StatsDAL.ColCount;
      
      #endregion

      #region Constructors

      public StatsRow ( DataRow row )
      {
         dbId = StatsDAL.GetIntValue( row, ColDbId );
         cat = (StatsCategory)StatsDAL.GetIntValue( row, ColCategory );
         date = StatsDAL.GetDateTimeValue( row, ColDate );
         count = StatsDAL.GetIntValue( row, ColCount );
      }

      #endregion

      #region Properties

      public int DbId
      {
         get{ return dbId; }
      }

      public StatsCategory Category
      {
         get { return cat; }
      }
      
      public DateTime Date
      {
         get { return date; }
      }
      
      public int Count
      {
         get { return count; }
      }
      
      #endregion
   }
}
