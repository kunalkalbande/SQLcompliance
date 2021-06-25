using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Idera.SQLcompliance.Core
{
   class SensitiveColumnColumnsRecord
   {
      #region Fields

      private int _srvId;
      private int _dbId;
      private int _objectId;
      private string _name;
        private string _type;
        private int _columnId;

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

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public int ColumnId
        {
            get { return _columnId; }
            set { _columnId = value; }
        }
      #endregion

      #region Public Methods

        public static List<SensitiveColumnTableRecord> GetAuditedColumns(SqlConnection conn, List<SensitiveColumnTableRecord> tables)
      {
         if (tables == null || tables.Count == 0)
                return null;

         Dictionary<int, SensitiveColumnTableRecord> tmpList = new Dictionary<int, SensitiveColumnTableRecord>();
            List<SensitiveColumnColumnsRecord> colList = new List<SensitiveColumnColumnsRecord>();
            List<SensitiveColumnTableRecord> auditedTablesRecord = new List<SensitiveColumnTableRecord>();
         StringBuilder list = new StringBuilder();
         list.AppendFormat("{0}", tables[0].ObjectId);
         tmpList.Add(tables[0].ObjectId, tables[0]);
         for (int i = 1; i < tables.Count; i++)
         {
            list.AppendFormat(", {0}", tables[i].ObjectId);
            tmpList.Add(tables[i].ObjectId, tables[i]);
         }
            string stmt = String.Format("SELECT srvId, dbId, objectId, name, columnId, type FROM {0} WHERE objectId in ({1}) " +
                                      "AND srvId = {2} AND dbId = {3} ORDER BY objectId ASC",
                                      CoreConstants.RepositorySensitiveColumnColumnsTable,
                                      list.ToString(),
                                      tables[0].SrvId,
                                      tables[0].DbId);

         using (SqlCommand cmd = new SqlCommand(stmt, conn))
         {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {

               while (reader.Read())
               {
                  SensitiveColumnColumnsRecord col = new SensitiveColumnColumnsRecord();
                  col.Load(reader);
                        colList.Add(col);
                    }
                }
            }
            foreach (SensitiveColumnTableRecord o in tables)
            {
                if (!o.SelectedColumns)
                {
                    o.Type = CoreConstants.SensitiveColumnIndividualTableType;
                    auditedTablesRecord.Add(o);
                }
            }

            var groupedList = colList.FindAll(x => x.Type != null && x.Type != CoreConstants.IndividualColumnType)
                .GroupBy(u => u.ColumnId)
                .Select(grp => grp.ToList())
                .ToList();
            groupedList.AddRange(colList.FindAll(x => x.Type == null || x.Type
                == CoreConstants.IndividualColumnType).GroupBy(u => u.ObjectId)
                .Select(grp => grp.ToList()).ToList());
            foreach (var group in groupedList)
            {
                SensitiveColumnTableRecord sctItem = new SensitiveColumnTableRecord();
                List<string> tblNameOfColumn = new List<string>();
                var builder = new System.Text.StringBuilder();
                foreach (var user in group)
                {
                    sctItem.ColumnId = user.ColumnId;
                    sctItem.Type = user.Type;
                    sctItem.ObjectId = user.ObjectId;
                    sctItem.DbId = user.DbId;
                    sctItem.SrvId = user.SrvId;
                    sctItem.AddColumn(user.Name);

                    // Find the 'Value' on the basis of the 'Key'
                    var value = tmpList[user.ObjectId];
                    sctItem.tableIdMap[String.Format("{0}.{1}",value.SchemaName, value.TableName)] = user.ObjectId;

                    //Set the properties from the 'Value'
                    sctItem.SchemaName = value.SchemaName;
                    tblNameOfColumn.Add(value.TableName);
                }

                //Get unique table names of columns to create an Appended string which will be the new Table name
                var unique_items = new HashSet<string>(tblNameOfColumn);

                sctItem.TableName = String.Join(",",unique_items);

                //Add the records
                auditedTablesRecord.Add(sctItem);
         }
            return auditedTablesRecord;
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
                                   ",type" +
                                   ",columnId" +
                                ") VALUES ({2},{3},{4},{5},{6},{7});";
         return string.Format(tmp,
                               CoreConstants.RepositorySensitiveColumnColumnsTable,
                               (withLocking) ? "WITH (TABLOCKX) " : "",
                               _srvId,
                               _dbId,
                               _objectId,
                                     SQLHelpers.CreateSafeString(_name),
                                     SQLHelpers.CreateSafeString(_type),
                                     _columnId);
      }

      //-------------------------------------------------------------
      // GetDeleteAllSQL - Create UPDATE SQL to delete a database
      //--------------------------------------------------------------
      public static string GetDeleteAllSQL(int serverId, int databaseId, int? objectid, bool withLocking)
      {
         // if objectid is null then delete for all tables
         string cmdStr = String.Format("DELETE FROM {0} {1} where srvId = {2} and dbId={3} {4};",
                                        CoreConstants.RepositorySensitiveColumnColumnsTable,
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
            if (!reader.IsDBNull(4))
                _columnId = reader.GetInt32(4);
            _type = reader.GetString(5);
            if (string.IsNullOrEmpty(_type))
                _type = CoreConstants.IndividualColumnType;
      }

      #endregion

   }
}
