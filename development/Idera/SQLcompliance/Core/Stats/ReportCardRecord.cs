using System ;
using System.Collections.Generic ;
using System.Data.SqlClient ;

namespace Idera.SQLcompliance.Core.Stats
{
   public class ReportCardRecord
   {
      private const string SELECT_ReportCards = "SELECT srvId,statId,warningThreshold,errorThreshold,period,enabled FROM {0}..ReportCard" ;
      private const string SELECT_ServerReportCards = "SELECT srvId,statId,warningThreshold,errorThreshold,period,enabled FROM {0}..ReportCard WHERE srvId={1}";
      private const string SELECT_StatisticReportCards = "SELECT srvId,statId,warningThreshold,errorThreshold,period,enabled FROM {0}..ReportCard WHERE statId={1}";
      private const string SELECT_ReportCard = "SELECT srvId,statId,warningThreshold,errorThreshold,period,enabled FROM {0}..ReportCard WHERE srvId={1} AND statId={2}";
      private const string INSERT_ReportCard = "INSERT INTO {0}..ReportCard (srvId,statId,warningThreshold,errorThreshold,period,enabled) VALUES({1},{2},{3},{4},{5},{6})";
      private const string UPDATE_ReportCard = "UPDATE {0}..ReportCard SET warningThreshold={1},errorThreshold={2},period={3},enabled={4} WHERE srvId={5} AND statId={6}";

      private const string INSERT_DefaultReportCard = "INSERT INTO {0}..DefaultReportCard (statId,warningThreshold,errorThreshold,period,enabled) VALUES({1},{2},{3},{4},{5})";
      private const string UPDATE_DefaultReportCard = "UPDATE {0}..DefaultReportCard SET warningThreshold={1},errorThreshold={2},period={3},enabled={4} WHERE statId={5}";
      private const string SELECT_DefaultServerReportCards = "SELECT statId,warningThreshold,errorThreshold,period,enabled FROM {0}..DefaultReportCard";
      private const string SELECT_IderaDefaultServerReportCards = "SELECT statId,warningThreshold,errorThreshold,period,enabled FROM {0}..IderaDefaultReportCard";
     
      private int _srvId ;
      private int _statisticId ;
      private int _warningThreshold ;
      private int _criticalThreshold ;
      private int _period ;
      private bool _enabled ;

      public ReportCardRecord(int serverId, StatsCategory category)
      {
         _srvId = serverId ;
         _statisticId = (int)category ;
         _warningThreshold = -1 ;
         _criticalThreshold = -1 ;
         _period = 96 ; // one day
      }

      public ReportCardRecord(StatsCategory category)
      {
          _statisticId = (int)category;
          _warningThreshold = -1;
          _criticalThreshold = -1;
          _period = 96; // one day
      }
      
      private ReportCardRecord()
      {
         // internal
      }

      public int SrvId
      {
         get { return _srvId; }
         set { _srvId = value; }
      }

      public int StatisticId
      {
         get { return _statisticId; }
         set { _statisticId = value; }
      }

      public int WarningThreshold
      {
         get { return _warningThreshold; }
         set { _warningThreshold = value; }
      }

      public int CriticalThreshold
      {
         get { return _criticalThreshold; }
         set { _criticalThreshold = value; }
      }

      public int Period
      {
         get { return _period; }
         set { _period = value; }
      }


      public bool Enabled
      {
         get { return _enabled; }
         set { _enabled = value; }
      }

      public string GetLowestThresholdString()
      {
         string periodStr;

         if (_period == 4)
            periodStr = "hour";
         else if (_period == 96)
            periodStr = "day";
         else
            periodStr = String.Format("{0} minutes", _period * 15);
         if (_enabled && _warningThreshold > 0)
            return String.Format("{0}/{1}", _warningThreshold, periodStr);
         else if (_enabled && _criticalThreshold > 0)
            return String.Format("{0}/{1}", _criticalThreshold, periodStr);
         else
            return "None";
      }

      public string GetWarningThresholdString()
      {
         string periodStr ;

         if(_period == 4)
            periodStr = "hour" ;
         else if(_period == 96)
            periodStr = "day" ;
         else
            periodStr = String.Format("{0} minutes", _period * 15) ;
         if (_enabled && _warningThreshold > 0)
            return String.Format("{0}/{1}", _warningThreshold, periodStr) ;
         else
            return "None" ;
      }

      public string GetCriticalThresholdString()
      {
         string periodStr;

         if (_period == 4)
            periodStr = "hour";
         else if (_period == 96)
            periodStr = "day";
         else
            periodStr = String.Format("{0} minutes", _period * 15);
         if (_enabled && _criticalThreshold > 0)
            return String.Format("{0}/{1}", _criticalThreshold, periodStr);
         else
            return "None";
      }

      public static List<ReportCardRecord> GetReportCardEntries(SqlConnection conn)
      {
         List<ReportCardRecord> retVal = new List<ReportCardRecord>() ;
         string query = String.Format(SELECT_ReportCards, CoreConstants.RepositoryDatabase) ;
         using(SqlCommand cmd = new SqlCommand(query, conn))
         {
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
               while(reader.Read())
               {
                  int i = 0 ;
                  ReportCardRecord record = new ReportCardRecord() ;
                  record.SrvId = SQLHelpers.GetInt32(reader, i++) ;
                  record.StatisticId = SQLHelpers.GetInt32(reader, i++) ;
                  record.WarningThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.CriticalThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.Period = SQLHelpers.GetInt32(reader, i++);
                  record.Enabled = SQLHelpers.ByteToBool(reader, i++) ;
                  retVal.Add(record) ;
               }
            }
         }
         return retVal ;
      }

      public static List<ReportCardRecord> GetServerReportCardEntries(SqlConnection conn, int srvId,SqlTransaction transaction=null)
      {
         List<ReportCardRecord> retVal = new List<ReportCardRecord>();
         string query = String.Format(SELECT_ServerReportCards, CoreConstants.RepositoryDatabase, srvId);
         using (SqlCommand cmd = new SqlCommand(query, conn))
         {
             if (transaction != null)
                 cmd.Transaction = transaction;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  int i = 0;
                  ReportCardRecord record = new ReportCardRecord();
                  record.SrvId = SQLHelpers.GetInt32(reader, i++);
                  record.StatisticId = SQLHelpers.GetInt32(reader, i++);
                  record.WarningThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.CriticalThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.Period = SQLHelpers.GetInt32(reader, i++);
                  record.Enabled = SQLHelpers.ByteToBool(reader, i++);
                  retVal.Add(record);
               }
            }
         }
         return retVal;
      }

      public static List<ReportCardRecord> GetDefaultServerReportCardEntries(SqlConnection conn, bool loadIderaDefault = false,SqlTransaction transaction = null)
      {
          List<ReportCardRecord> retVal = new List<ReportCardRecord>();
          string query;

          if (loadIderaDefault)
              query = String.Format(SELECT_IderaDefaultServerReportCards, CoreConstants.RepositoryDatabase);
          else
              query = String.Format(SELECT_DefaultServerReportCards, CoreConstants.RepositoryDatabase);

          using (SqlCommand cmd = new SqlCommand(query, conn))
          {
              if (transaction != null)
                  cmd.Transaction = transaction;

              using (SqlDataReader reader = cmd.ExecuteReader())
              {
                  while (reader.Read())
                  {
                      int i = 0;
                      ReportCardRecord record = new ReportCardRecord();
                      record.StatisticId = SQLHelpers.GetInt32(reader, i++);
                      record.WarningThreshold = SQLHelpers.GetInt32(reader, i++);
                      record.CriticalThreshold = SQLHelpers.GetInt32(reader, i++);
                      record.Period = SQLHelpers.GetInt32(reader, i++);
                      record.Enabled = (Boolean)reader[i++];
                      retVal.Add(record);
                  }
              }
          }
          return retVal;
      }
      public static List<ReportCardRecord> GetStatisticReportCardEntries(SqlConnection conn, int statisticId)
      {
         List<ReportCardRecord> retVal = new List<ReportCardRecord>();
         string query = String.Format(SELECT_StatisticReportCards, CoreConstants.RepositoryDatabase, statisticId);
         using (SqlCommand cmd = new SqlCommand(query, conn))
         {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  int i = 0;
                  ReportCardRecord record = new ReportCardRecord();
                  record.SrvId = SQLHelpers.GetInt32(reader, i++);
                  record.StatisticId = SQLHelpers.GetInt32(reader, i++);
                  record.WarningThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.CriticalThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.Period = SQLHelpers.GetInt32(reader, i++);
                  record.Enabled = SQLHelpers.ByteToBool(reader, i++);
                  retVal.Add(record);
               }
            }
         }
         return retVal;
      }

      public static ReportCardRecord GetReportCardEntry(SqlConnection conn, int srvId, int statisticId)
      {
         string query = String.Format(SELECT_ReportCard, CoreConstants.RepositoryDatabase, srvId, statisticId);
         using (SqlCommand cmd = new SqlCommand(query, conn))
         {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               if (reader.Read())
               {
                  int i = 0;
                  ReportCardRecord record = new ReportCardRecord();
                  record.SrvId = SQLHelpers.GetInt32(reader, i++);
                  record.StatisticId = SQLHelpers.GetInt32(reader, i++);
                  record.WarningThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.CriticalThreshold = SQLHelpers.GetInt32(reader, i++);
                  record.Period = SQLHelpers.GetInt32(reader, i++);
                  record.Enabled = SQLHelpers.ByteToBool(reader, i++);
                  return record;
               }
            }
         }
         return null;
      }

      public void Write(SqlConnection conn,SqlTransaction transaction = null)
      {
         string query = String.Format(INSERT_ReportCard, CoreConstants.RepositoryDatabase,
            _srvId,
            _statisticId,
            _warningThreshold,
            _criticalThreshold,
            _period,
            _enabled ? 1 : 0);

         using (SqlCommand cmd = new SqlCommand(query, conn))
         {
             if (transaction != null)
                 cmd.Transaction = transaction;

            cmd.ExecuteScalar();
         }
      }

      public void WriteDefault(SqlConnection conn)
      {
          string query = String.Format(INSERT_DefaultReportCard, CoreConstants.RepositoryDatabase,
             _statisticId,
             _warningThreshold,
             _criticalThreshold,
             _period,
             _enabled ? 1 : 0);

          using (SqlCommand cmd = new SqlCommand(query, conn))
          {
              cmd.ExecuteScalar();
          }
      }

      public void Update(SqlConnection conn,SqlTransaction transaction=null)
      {
         string query = String.Format(UPDATE_ReportCard, CoreConstants.RepositoryDatabase,
            _warningThreshold,
            _criticalThreshold,
            _period,
            _enabled ? 1 : 0,
            _srvId, _statisticId);

         using (SqlCommand cmd = new SqlCommand(query, conn))
         {
             if (transaction != null)
                 cmd.Transaction = transaction;

            cmd.ExecuteScalar();
         }
      }

      public void UpdateDefault(SqlConnection conn)
      {
          string query = String.Format(UPDATE_DefaultReportCard, CoreConstants.RepositoryDatabase,
             _warningThreshold,
             _criticalThreshold,
             _period,
             _enabled ? 1 : 0,
             _statisticId);

          using (SqlCommand cmd = new SqlCommand(query, conn))
          {
              cmd.ExecuteScalar();
          }
      }

   }
}
