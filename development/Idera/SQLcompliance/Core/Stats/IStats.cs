
namespace Idera.SQLcompliance.Core.Stats
{
   // Statistics categories
   public enum StatsCategory
   {
      Unknown = 0,
      AuditedInstance = 1,
      AuditedDatabase = 2,
      //ProcessedEvents = 3,  //Not used.  Use EventProcessed instead.
      Alerts = 4,
      PrivUserEvents = 5,
      FailedLogin = 6,
      UserDefinedEvents = 7,
      Admin = 8,
      DDL = 9,
      Security = 10,
      DML = 11,
      Insert = 12,
      Update = 13,
      Delete = 14,
      Select = 15,
      Logins = 16,
      HDSpace = 17,
      IntegrityCheck = 18,
      Execute = 19,
      EventReceived = 20,
      EventProcessed = 21,
      EventFiltered = 22,
     
      //start sqlcm5.6-5363
      Logout=23,
      MaxValue = 24,//modified 23 to 24
        //end sqlcm5.6 - 5353
    }


   public interface IStats
   {
      #region Properties


      #endregion

      #region Methods

      bool Update();

      #endregion
   }

}
