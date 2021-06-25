using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Idera.SQLcompliance.Core
{
   class DataChangeColumnsRecord
   {
      #region Fields

      private int _srvId;
      private int _dbId;
      private int _objectId;
      private string _name;

      #endregion

      #region Properties

      public int SrvId
      {
         get { return _srvId; }
         set { _srvId = value; }
      }

      public int DbId
      {
         get { return _dbId; }
         set { _dbId = value; }
      }

      public int ObjectId
      {
         get { return _objectId; }
         set { _objectId = value; }
      }

      public string Name
      {
         get
         {
            return _name;
         }
         set
         {
            _name = value;
         }
      }

      #endregion

      #region Public Methods

      public static void GetAuditedColumns(SqlConnection conn, List<DataChangeTableRecord> tables)
      {
         if (tables == null || tables.Count == 0)
            return;

         Dictionary<int, DataChangeTableRecord> tmpList = new Dictionary<int, DataChangeTableRecord>();
         StringBuilder list = new StringBuilder();
         list.AppendFormat("{0}", tables[0].ObjectId);
         tmpList.Add(tables[0].ObjectId, tables[0]);
         for (int i = 1; i < tables.Count; i++)
         {
            list.AppendFormat(", {0}", tables[i].ObjectId);
            tmpList.Add(tables[i].ObjectId, tables[i]);
         }
         string stmt = String.Format("SELECT srvId, dbId, objectId, name FROM {0} WHERE objectId in ({1}) " +
                                      "AND srvId = {2} AND dbId = {3} ORDER BY objectId ASC",
                                      CoreConstants.RepositoryDataChangeColumnsTable,
                                      list.ToString(),
                                      tables[0].SrvId,
                                      tables[0].DbId);

         using (SqlCommand cmd = new SqlCommand(stmt, conn))
         {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {

               while (reader.Read())
               {
                  DataChangeColumnsRecord col = new DataChangeColumnsRecord();
                  col.Load(reader);
                  tmpList[col.ObjectId].AddColumn(col.Name);
               }
            }
         }
      }

      //-------------------------------------------------------------
      // GetInsertSQL
      //--------------------------------------------------------------
      public string GetInsertSQL(bool withLocking)
      {
         string tmp = "INSERT INTO {0} {1}" +
                          "(" +
                             "srvId" +
                             ",dbId" +
                             ",objectId" +
                             ",name" +
                          ") VALUES ({2},{3},{4},{5});";
         return string.Format(tmp,
                               CoreConstants.RepositoryDataChangeColumnsTable,
                               (withLocking) ? "WITH (TABLOCKX) " : "",
                               _srvId,
                               _dbId,
                               _objectId,
                               SQLHelpers.CreateSafeString(_name));
      }

      //-------------------------------------------------------------
      // GetDeleteAllSQL - Create UPDATE SQL to delete a database
      //--------------------------------------------------------------
      public static string GetDeleteAllSQL(int serverId, int databaseId, int? objectid, bool withLocking)
      {
         // if objectid is null then delete for all tables
         string cmdStr = String.Format("DELETE FROM {0} {1} where srvId = {2} and dbId={3} {4};",
                                        CoreConstants.RepositoryDataChangeColumnsTable,
                                        (withLocking) ? "WITH (TABLOCKX) " : "",
                                        serverId, 
                                        databaseId, 
                                        objectid.HasValue ? string.Format("and objectid={0}", objectid) : string.Empty);
         return cmdStr;
      }

      #endregion

      #region Private Methods

      private void Load(SqlDataReader reader)
      {
         _srvId = reader.GetInt32(0);
         _dbId = reader.GetInt32(1);
         _objectId = reader.GetInt32(2);
         _name = reader.GetString(3);
      }

      #endregion

   }
}
