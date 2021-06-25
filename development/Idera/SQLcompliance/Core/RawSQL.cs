using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using System.Runtime.Serialization;
using Idera.SQLcompliance.Core.AlwaysOn;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Data;

namespace Idera.SQLcompliance.Core
{

    [Serializable]
    public class RawDatabaseObject : ISerializable
    {
        public RawDatabaseObject() { }
        public string name;
        public int dbid;

        // V 2.0 fields
        internal int classVersion = CoreConstants.SerializationVersion;

        public string DisplayMember { get { return name; } }
        override public string ToString() { return name; }

        #region Deserialization Constructor
        public RawDatabaseObject(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                name = info.GetString("name");
                dbid = info.GetInt32("dbid");
                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    classVersion = 0;
                }
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("name", name);
                info.AddValue("dbid", dbid);

                if (classVersion >= CoreConstants.SerializationVersion_20)
                    info.AddValue("classVersion", classVersion);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion
    }


    [Serializable]
    public class RawRoleObject : ISerializable
    {
        public RawRoleObject() { }
        public string name;
        public int roleid;
        public string fullName;

        // V 2.0 fields
        internal int classVersion = CoreConstants.SerializationVersion;

        public string DisplayMember { get { return String.Format("{0} ({1})", fullName, name); } }
        override public string ToString() { return String.Format("{0} ({1})", fullName, name); }

        #region Deserialization Constructor
        public RawRoleObject(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                name = info.GetString("name");
                roleid = info.GetInt32("roleid");
                fullName = info.GetString("fullName");

                // V 2.0 fields
                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    classVersion = 0;
                }
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }

        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("name", name);
                info.AddValue("roleid", roleid);
                info.AddValue("fullName", fullName);

                // V 2.0 fields
                if (classVersion < CoreConstants.SerializationVersion_20)
                    return;

                info.AddValue("classVersion", classVersion);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion
    }


    [Serializable]
    public class RawLoginObject : ISerializable
    {
        public RawLoginObject() { }
        public string name;
        public byte[] sid;

        public int denylogin;
        public int isntname;
        public int isntuser;
        public int isntgroup;
        public int hasaccess;

        public int sysadmin;
        public int securityadmin;
        public int serveradmin;
        public int setupadmin;
        public int processadmin;
        public int diskadmin;
        public int dbcreator;

        // V 2.0 fields
        internal int classVersion = CoreConstants.SerializationVersion;

        public int bulkadmin;

        public string DisplayMember { get { return name; } }
        override public string ToString() { return name; }

        #region Deserialization Constructor
        public RawLoginObject(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                name = info.GetString("name");
                sid = info.GetValue("sid", typeof(byte[])) as byte[];

                denylogin = info.GetInt32("denylogin");
                isntname = info.GetInt32("isntname");
                isntuser = info.GetInt32("isntuser");
                isntgroup = info.GetInt32("isntgroup");
                hasaccess = info.GetInt32("hasaccess");
                sysadmin = info.GetInt32("sysadmin");
                securityadmin = info.GetInt32("securityadmin");
                serveradmin = info.GetInt32("serveradmin");
                setupadmin = info.GetInt32("setupadmin");
                processadmin = info.GetInt32("processadmin");
                diskadmin = info.GetInt32("diskadmin");
                dbcreator = info.GetInt32("dbcreator");

                // V 2.0 fields
                try
                {
                    classVersion = info.GetInt32("classVersion");
                    bulkadmin = info.GetInt32("bulkadmin");
                }
                catch
                {
                    classVersion = 0;
                }
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("name", name);
                info.AddValue("sid", sid);

                info.AddValue("denylogin", denylogin);
                info.AddValue("isntname", isntname);
                info.AddValue("isntuser", isntuser);
                info.AddValue("isntgroup", isntgroup);
                info.AddValue("hasaccess", hasaccess);
                info.AddValue("sysadmin", sysadmin);
                info.AddValue("securityadmin", securityadmin);
                info.AddValue("serveradmin", serveradmin);
                info.AddValue("setupadmin", setupadmin);
                info.AddValue("processadmin", processadmin);
                info.AddValue("diskadmin", diskadmin);
                info.AddValue("dbcreator", dbcreator);

                // V 2.0 fields
                if (classVersion < CoreConstants.SerializationVersion_20)
                    return;

                info.AddValue("classVersion", classVersion);
                info.AddValue("bulkadmin", bulkadmin);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion
    }


    [Serializable]
    public class RawTableObject : ISerializable
    {
        public RawTableObject() { }
        private string name;
        public int id;
        public string xtype;

        // V 2.0 fields
        internal int classVersion = CoreConstants.SerializationVersion;

        // V 3.1 fields
        private string schemaName;

        // V 3.2 fields
        private bool hasBlobData;

        // V 4.4 fields
        private bool _isMemoryOptimized;

        private string displayName;

        public int ObjectID
        {
            get { return id; }
            set { id = value; }
        }

        public string FullTableName
        {
            get
            {
                if (classVersion < CoreConstants.SerializationVersion_31)
                    return name;
                else
                    return String.Format("{0}.{1}", schemaName, name);
            }
        }

        public string TableName
        {
            get { return name; }
            set { name = value; }
        }

        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        public bool HasBlobData
        {
            get { return hasBlobData; }
            set { hasBlobData = value; }
        }

        public bool IsMemoryOptimized
        {
            get { return _isMemoryOptimized; }
            set { _isMemoryOptimized = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        override public string ToString() { return name; }

        #region Deserialization Constructor
        public RawTableObject(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                name = info.GetString("name");
                id = info.GetInt32("id");
                xtype = info.GetString("xtype");
                schemaName = "dbo";

                // V 2.0 fields
                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    classVersion = 0;
                }

                // V 3.1 fields
                if (classVersion < CoreConstants.SerializationVersion_31)
                    return;
                schemaName = info.GetString("schemaName");

                // V 3.2 fields
                if (classVersion < CoreConstants.SerializationVersion_32)
                    return;
                hasBlobData = info.GetBoolean("hasBlobData");

                // V 4.4 fields
                if (classVersion < CoreConstants.SerializationVersion_44)
                    return;
                _isMemoryOptimized = info.GetBoolean("isMemoryOptimized");

            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("name", name);
                info.AddValue("id", id);
                info.AddValue("xtype", xtype);

                // V 2.0 fields
                if (classVersion < CoreConstants.SerializationVersion_20)
                    return;
                info.AddValue("classVersion", classVersion);

                // V 3.1 fields
                if (classVersion < CoreConstants.SerializationVersion_31)
                    return;
                info.AddValue("schemaName", schemaName);

                // V 3.2 fields
                if (classVersion < CoreConstants.SerializationVersion_32)
                    return;
                info.AddValue("hasBlobData", hasBlobData);

                // V 4.4 fields
                if (classVersion < CoreConstants.SerializationVersion_44)
                    return;
                info.AddValue("isMemoryOptimized", _isMemoryOptimized);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }

        #endregion
    }
    //SQLCM 5.4 Start
    public class RawTableDetails : ISerializable
    {
        public RawTableDetails() { }
        private string databaseName;
        public string schemaTableName;
        public string name;
        public decimal size;
        public Int64 rowCount;
        public Int32 columnsIdentified;
        // V 2.0 fields
        internal int classVersion = CoreConstants.SerializationVersion;

        // V 3.1 fields
        private string schemaName;


        public string FullTableName
        {
            get
            {
                if (classVersion < CoreConstants.SerializationVersion_31)
                    return name;
                else
                    return String.Format("{0}.{1}", schemaName, name);
            }
        }

        public string TableName
        {
            get { return name; }
            set { name = value; }
        }
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        public string SchemaTableName
        {
            get { return schemaTableName; }
            set { schemaTableName = value; }
        }


        public decimal Size
        {
            get { return size; }
            set { size = value; }
        }

        public Int64 RowCount
        {
            get { return rowCount; }
            set { rowCount = value; }
        }
        public Int32 ColumnsIdentified
        {
            get { return columnsIdentified; }
            set { columnsIdentified = value; }
        }

        override public string ToString() { return DatabaseName; }

        #region Deserialization Constructor
        public RawTableDetails(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                DatabaseName = info.GetString("DatabaseName");
                SchemaTableName = info.GetString("SchemaTable");
                Size = info.GetDecimal("UsedDataSpaceMB");
                RowCount = info.GetInt32("MatchedColumns");
                //ColumnsIdentified = info.GetInt32("xtype");
                schemaName = "dbo";

                // V 2.0 fields
                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    classVersion = 0;
                }

                // V 3.1 fields
                if (classVersion < CoreConstants.SerializationVersion_31)
                    return;
                schemaName = info.GetString("schemaName");

                // V 3.2 fields


            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.GetString("DatabaseName");
                info.GetString("SchemaTable");
                info.GetString("MatchedColumns");// V 2.0 fields
                info.GetDouble("UsedDataSpaceMB");

                if (classVersion < CoreConstants.SerializationVersion_20)
                    return;
                info.AddValue("classVersion", classVersion);

                // V 3.1 fields
                if (classVersion < CoreConstants.SerializationVersion_31)
                    return;
                info.AddValue("schemaName", schemaName);

            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }

        #endregion
    }

    public class RawDatabaseDetails : ISerializable
    {
        public RawDatabaseDetails() { }
        private string databaseName;
        public int dbId;
        public int serverId;


        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public int DbId
        {
            get { return dbId; }
            set { dbId = value; }
        }

        public int ServerId
        {
            get { return serverId; }
            set { serverId = value; }
        }



        override public string ToString() { return DatabaseName; }

        #region Deserialization Constructor
        public RawDatabaseDetails(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                DatabaseName = info.GetString("DatabaseName");
                DbId = info.GetInt32("DbId");
                ServerId = info.GetInt32("ServerId");

                // V 3.2 fields


            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {

                info.GetString("DatabaseName");
                info.GetInt32("DbId");
                info.GetInt32("ServerId");


            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }

        #endregion
    }

    public class RawColumnDetails : ISerializable
    {
        public RawColumnDetails() { }

        public string databaseName;
        public string tableName;
        public string fieldName;
        public string dataType;
        public string matchingStr;
        public int lengthSize;


        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }

        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public string MatchingStr
        {
            get { return matchingStr; }
            set { matchingStr = value; }
        }
        public int LengthSize
        {
            get { return lengthSize; }
            set { lengthSize = value; }
        }


        override public string ToString() { return TableName; }

        #region Deserialization Constructor
        public RawColumnDetails(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                DatabaseName = info.GetString("DatabaseName");
                TableName = info.GetString("TableName");
                FieldName = info.GetString("FieldName");
                DataType = info.GetString("DataType");
                MatchingStr = info.GetString("MatchingStr");
                //LengthSize = info.GetInt32("LengthSize");
                //ColumnsIdentified = info.GetInt32("xtype");

                // V 3.2 fields


            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {

                info.GetString("DatabaseName");
                info.GetString("TableName");
                info.GetString("FieldName");
                info.GetString("DataType");
                info.GetString("MatchingStr");// V 2.0 fields
                                              //info.GetInt32("LengthSize");


            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }

        #endregion
    }





    //SQLCM 5.4 End

    [Serializable]
    public class RawColumnObject : ISerializable
    {
        public RawColumnObject() { }
        private string name;
        private int id;
        private int xtype;
        private DateTime crdateTime;//SQLCM -541/4876/5259 v5.6

        internal int classVersion = CoreConstants.SerializationVersion;

        public string ColumnName
        {
            get { return name; }
            set { name = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Type
        {
            get { return xtype; }
            set { xtype = value; }
        }

        //SQLCM -541/4876/5259 v5.6
        public DateTime CreatedDate
        {
            get { return crdateTime; }
            set { crdateTime = value; }
        }

        public bool HasBlobData
        {
            get
            {
                bool isBlob;
                switch (xtype)
                {
                    case 35:
                    case 165:
                    case 99:
                    case 34:
                    case 173:
                        isBlob = true;
                        break;
                    default:
                        isBlob = false;
                        break;
                }
                return isBlob;
            }
        }

        override public string ToString() { return name; }

        #region Deserialization Constructor
        public RawColumnObject(
           SerializationInfo info,
           StreamingContext context)
        {
            try
            {
                name = info.GetString("name");
                id = info.GetInt32("id");
                xtype = info.GetInt32("xtype");
                crdateTime = info.GetDateTime("crdateTime");//SQLCM -541/4876/5259 v5.6

                try
                {
                    classVersion = info.GetInt32("classVersion");
                }
                catch
                {
                    classVersion = 0;
                }
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("name", name);
                info.AddValue("id", id);
                info.AddValue("xtype", xtype);
                info.AddValue("crdateTime", crdateTime);//SQLCM -541/4876/5259 v5.6

                if (classVersion < CoreConstants.SerializationVersion_20)
                    return;
                info.AddValue("classVersion", classVersion);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }

        #endregion
    }


    /// <summary>
    /// Summary description for RawSQL.
    /// </summary>
    public class RawSQL
    {
        public RawSQL()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Database Logic		
        //-----------------------------------------------------------------------------
        // GetUserDatabases - Gets list of databases from SQLServer instance
        //-----------------------------------------------------------------------------
        static public IList
           GetUserDatabases(
              SqlConnection conn
           )
        {
            IList dbList = null;

            try
            {

                // Load Databases			   
                string cmdstr = "SELECT name, dbid" +
                                  " FROM master..sysdatabases " +
                                " WHERE has_dbaccess(name) = 1 " +
                                  " ORDER by name ASC";

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();

                        while (reader.Read())
                        {
                            RawDatabaseObject raw = new RawDatabaseObject();
                            raw.name = reader.GetString(0);
                            raw.dbid = reader.GetInt16(1);

                            // exclude system databases
                            if (!raw.name.Equals("master") &&
                               !raw.name.Equals("model") &&
                               !raw.name.Equals("msdb") &&
                               !raw.name.Equals("tempdb"))
                            {
                                dbList.Add(raw);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dbList;
        }

        //5.4 start

        static public IList
          GetAllUserDatabases(
             int srvId, SqlConnection conn
          )
        {
            IList dbList = null;

            try
            {

                // Load Databases			   
                string cmdstr = "SELECT name, dbid" +
                                  " FROM master..sysdatabases " +
                                " WHERE has_dbaccess(name) = 1 " +
                                  " ORDER by name ASC";

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();

                        while (reader.Read())
                        {
                            RawDatabaseDetails raw = new RawDatabaseDetails();
                            raw.DatabaseName = reader.GetString(0);
                            raw.DbId = reader.GetInt16(1);
                            raw.ServerId = srvId;
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dbList;
        }

        //5.4 end


        //-----------------------------------------------------------------------------
        // GetSystemDatabases - Gets list of databases from SQLServer instance
        //-----------------------------------------------------------------------------
        static public IList
           GetSystemDatabases(
              SqlConnection conn
           )
        {
            IList dbList = null;

            dbList = new ArrayList();

            RawDatabaseObject raw = new RawDatabaseObject();
            raw.name = "master";
            raw.dbid = 1;
            dbList.Add(raw);

            raw = new RawDatabaseObject();
            raw.name = "tempdb";
            raw.dbid = 2;
            dbList.Add(raw);

            raw = new RawDatabaseObject();
            raw.name = "model";
            raw.dbid = 3;
            dbList.Add(raw);

            raw = new RawDatabaseObject();
            raw.name = "msdb";
            raw.dbid = 4;
            dbList.Add(raw);

            if (SQLHelpers.GetSqlVersion(conn) >= 9)
            {
                raw = new RawDatabaseObject();
                raw.name = "mssqlsystemresource";
                raw.dbid = 32767;
                dbList.Add(raw);
            }

            return dbList;
        }

        //-----------------------------------------------------------------------------
        // GetTables
        //-----------------------------------------------------------------------------
        static public IList
           GetTables(
              SqlConnection conn,
              string dbName
           )
        {
            IList tableList = null;

            try
            {
                // Load Tables			   
                string cmd2005 = "SELECT name,object_id,SCHEMA_NAME(schema_id) as schemaname , " +
                                           "CASE WHEN EXISTS (select * from [{0}].sys.all_columns c " +
                                                                "WHERE c.system_type_id IN (35, 165, 99, 34, 173) " +
                                                                   "AND c.object_id = tbl.object_id ) then 1 else 0 end as hasBlobData, 0 AS is_memory_optimized " +
                                  "FROM [{0}].sys.tables tbl WHERE type='U' ORDER by name ASC";
                string cmd2000 = "SELECT tbl.name as name,tbl.id as [object_id],su.name as [schemaname], " +
                                           "CASE WHEN EXISTS (select * from [{0}]..syscolumns c " +
                                                                "INNER JOIN [{0}]..sysobjects o ON c.id = o.id " +
                                                                "INNER JOIN [{0}]..systypes t ON c.type = t.type " +
                                                                "WHERE c.type IN (35, 165, 99, 34, 173) " +
                                                                   "AND o.id = tbl.id ) then 1 else 0 end as hasBlobData, 0 AS is_memory_optimized " +
                                  "FROM [{0}]..sysobjects as tbl join [{0}]..sysusers su on tbl.uid=su.uid " +
                                  "where tbl.type='U' ORDER by name";
                string cmd2014 = "SELECT name,object_id,SCHEMA_NAME(schema_id) as schemaname , " +
                                         "CASE WHEN EXISTS (select * from [{0}].sys.all_columns c " +
                                                              "WHERE c.system_type_id IN (35, 165, 99, 34, 173) " +
                                                                 "AND c.object_id = tbl.object_id ) then 1 else 0 end as hasBlobData, is_memory_optimized " +
                                "FROM [{0}].sys.tables tbl WHERE type='U' ORDER by name ASC";

                bool isSql2014 = Is2014(conn);

                string cmdstr = String.Format(Is2000(conn) ? cmd2000 : isSql2014 ? cmd2014 : cmd2005, dbName);
                string db = conn.Database;
                conn.ChangeDatabase(dbName);
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        tableList = new ArrayList();

                        while (reader.Read())
                        {
                            int columnIndex = 0;
                            RawTableObject raw = new RawTableObject();
                            raw.TableName = reader.GetString(columnIndex++);
                            raw.id = reader.GetInt32(columnIndex++);
                            raw.SchemaName = reader.GetString(columnIndex++);
                            raw.HasBlobData = reader.GetInt32(columnIndex++) != 0;
                            raw.IsMemoryOptimized = isSql2014 ? reader.GetBoolean(columnIndex) : (reader.GetInt32(columnIndex) != 0);

                            tableList.Add(raw);
                        }
                    }
                }
                conn.ChangeDatabase(db);
            }
            catch (Exception ex)
            {
                tableList = new ArrayList();  // return an empty array
            }

            return tableList;
        }

        //-----------------------------------------------------------------------------
        // GetTables
        //-----------------------------------------------------------------------------
        static public IList GetTables(SqlConnection conn, string dbName, string tableNameSearchText)
        {
            if (string.IsNullOrEmpty(tableNameSearchText))
            {
                return GetTables(conn, dbName);
            }

            IList tableList = null;

            try
            {
                // Load Tables			   
                string cmd2000 = "SELECT tbl.name as name,tbl.id as [object_id],su.name as [schemaname], " +
                                           "CASE WHEN EXISTS (select * from [{0}]..syscolumns c " +
                                                                "INNER JOIN [{0}]..sysobjects o ON c.id = o.id " +
                                                                "INNER JOIN [{0}]..systypes t ON c.type = t.type " +
                                                                "WHERE c.type IN (35, 165, 99, 34, 173) " +
                                                                   "AND o.id = tbl.id ) then 1 else 0 end as hasBlobData, 0 AS is_memory_optimized " +
                                  "FROM [{0}]..sysobjects as tbl join [{0}]..sysusers su on tbl.uid=su.uid " +
                                  "where tbl.type='U' and su.name + '.'+ tbl.name like '%{1}%' ORDER by name";

                string cmd2005 = "SELECT name,object_id,SCHEMA_NAME(schema_id) as schemaname , " +
                                        "CASE WHEN EXISTS (select * from [{0}].sys.all_columns c " +
                                                              "WHERE c.system_type_id IN (35, 165, 99, 34, 173) " +
                                                                 "AND c.object_id = tbl.object_id ) then 1 else 0 end as hasBlobData, 0 AS is_memory_optimized " +
                                  "FROM [{0}].sys.tables tbl WHERE type='U' and SCHEMA_NAME(schema_id) + '.' + name like '%{1}%' ORDER by name ASC";

                string cmd2014 = "SELECT name,object_id,SCHEMA_NAME(schema_id) as schemaname , " +
                                        "CASE WHEN EXISTS (select * from [{0}].sys.all_columns c " +
                                                              "WHERE c.system_type_id IN (35, 165, 99, 34, 173) " +
                                                                 "AND c.object_id = tbl.object_id ) then 1 else 0 end as hasBlobData, is_memory_optimized " +
                                "FROM [{0}].sys.tables tbl WHERE type='U' and SCHEMA_NAME(schema_id) + '.' + name like '%{1}%' ORDER by name ASC";

                bool isSql2014 = Is2014(conn);

                string cmdstr = String.Format(Is2000(conn) ? cmd2000 : isSql2014 ? cmd2014 : cmd2005, dbName, tableNameSearchText);
                string db = conn.Database;
                conn.ChangeDatabase(dbName);
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        tableList = new ArrayList();

                        while (reader.Read())
                        {
                            int columnIndex = 0;
                            RawTableObject raw = new RawTableObject();
                            raw.TableName = reader.GetString(columnIndex++);
                            raw.id = reader.GetInt32(columnIndex++);
                            raw.SchemaName = reader.GetString(columnIndex++);
                            raw.HasBlobData = reader.GetInt32(columnIndex++) != 0;
                            raw.IsMemoryOptimized = isSql2014 ? reader.GetBoolean(columnIndex) : (reader.GetInt32(columnIndex) != 0);
                            tableList.Add(raw);
                        }
                    }
                }
                conn.ChangeDatabase(db);
            }
            catch (Exception ex)
            {
                tableList = new ArrayList();  // return an empty array
            }

            return tableList;
        }

        //SQLCM 5.4 Start


        //static public IList  GetTableDetails(SqlConnection conn,  string dbName )
        //{          
        //    IList tableList = null;

        //    try
        //    {
        //        // Load Tables
        //        string query1 = "USE [Test]; SELECT db_name() as DatabaseName, " +
        //                 "SCHEMA_NAME(t.schema_id)+'.'+t.name AS SchemaTable, " +
        //                 "(SUM(a.used_pages) * 8.0 ) / 1024.0 AS UsedDataSpaceMB, " +
        //                 "COUNT(DISTINCT c.Name) AS MatchedColumns " +
        //                 "FROM sys.tables AS t " +
        //                 "INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID " +
        //                 "INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id " +
        //                 "INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id " +
        //                 "INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id " +
        //                "WHERE ( c.name LIKE '%id%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%contact%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%age%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS) AND " +
        //                 "SCHEMA_NAME(t.schema_id)+'.'+t.name = 'dbo.Table_1' AND " +
        //                 "t.type_desc = 'USER_TABLE' AND " +
        //                 "i.index_id <= 1 " +
        //                 "GROUP BY SCHEMA_NAME(t.schema_id)+'.'+t.name " +
        //                 "ORDER BY DatabaseName, SchemaTable";
        //        string query = string.Format(query1);

        //        string db = conn.Database;
        //        conn.ChangeDatabase(dbName);
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                tableList = new ArrayList();

        //                while (reader.Read())
        //                {
        //                    int columnIndex = 0;
        //                    RawTableDetails raw = new RawTableDetails();
        //                    raw.DatabaseName = reader.GetString(columnIndex++);
        //                    raw.TableName = reader.GetString(columnIndex++);
        //                    raw.Size = reader.GetDecimal(columnIndex++);
        //                    raw.RowCount = reader.GetInt32(columnIndex++);
        //                    //raw.ColumnsIdentified = GetRowCount(dbName, tableName);
        //                    tableList.Add(raw);
        //                }
        //            }
        //        }
        //        conn.ChangeDatabase(db);
        //    }
        //    catch (Exception ex)
        //    {
        //        tableList = new ArrayList();  // return an empty array
        //    }

        //    return tableList;
        //}






        //static public IList GetTableDetails(SqlConnection conn, string dbName, string tableName)
        // {
        //     //if (string.IsNullOrEmpty(tableName))
        //     //{
        //     //    return GetTableDetails(conn, dbName);
        //     //}

        //     IList tableList = null;
        //     try
        //     {
        //         // Load Tables
        //         string query1 = "USE [{0}]; SELECT db_name() as DatabaseName, " +
        //                  "SCHEMA_NAME(t.schema_id)+'.'+t.name AS SchemaTable, " +
        //                  "(SUM(a.used_pages) * 8.0 ) / 1024.0 AS UsedDataSpaceMB, " +
        //                  "COUNT(DISTINCT c.Name) AS MatchedColumns " +
        //                  "FROM sys.tables AS t " +
        //                  "INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID " +
        //                  "INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id " +
        //                  "INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id " +
        //                  "INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id " +
        //                 "WHERE ( c.name LIKE '%id%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%contact%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%age%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS) AND " +
        //                  "SCHEMA_NAME(t.schema_id)+'.'+t.name = '{1}' AND " +
        //                  "t.type_desc = 'USER_TABLE' AND " +
        //                  "i.index_id <= 1 " +
        //                  "GROUP BY SCHEMA_NAME(t.schema_id)+'.'+t.name " +
        //                  "ORDER BY DatabaseName, SchemaTable";
        //         string query = string.Format(query1, dbName, tableName);

        //         Int32 rawCount = GetRowCount(conn, dbName, tableName);
        //         string db = conn.Database;
        //         conn.ChangeDatabase(dbName);
        //         using (SqlCommand cmd = new SqlCommand(query, conn))
        //         {
        //             using (SqlDataReader reader = cmd.ExecuteReader())
        //             {
        //                 tableList = new ArrayList();

        //                 while (reader.Read())
        //                 {
        //                     int columnIndex = 0;
        //                     RawTableDetails raw = new RawTableDetails();
        //                     raw.DatabaseName = reader.GetString(columnIndex++);
        //                     raw.TableName = reader.GetString(columnIndex++);
        //                     raw.Size = reader.GetDecimal(columnIndex++);                           
        //                     raw.ColumnsIdentified = reader.GetInt32(columnIndex++);
        //                     raw.RowCount = rawCount;
        //                     tableList.Add(raw);
        //                 }
        //             }
        //         }
        //         conn.ChangeDatabase(db);
        //     }
        //     catch (Exception ex)
        //     {
        //         tableList = new ArrayList();  // return an empty array
        //     }

        //     return tableList;
        // }

        static public IList GetPofilerData(SqlConnection conn, string profileName)
        {
            IList result = new ArrayList();
            conn.Close();
            conn.Open();
            try
            {
                //SqlConnection sqlConn = new SqlConnection(ConnectionString());
                string query1 = "USE [SQLcompliance]; SELECT definition from [dbo].[SensitiveColumnProfiler] where profileName='{0}' and isStringChecked='1' ";
                string query = string.Format(query1, profileName);
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(reader.GetValue(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        static public IList GetColumnDetailsForAll(SqlConnection conn, SqlConnection remoteSqlConn, IList dbName, string profileName)
        {

            IList tableList = new ArrayList();

            foreach (var dbname in dbName)
            {
                string databaseName = dbname.ToString();
                IList tabList = RawSQL.GetTables(remoteSqlConn, databaseName);
                foreach (RawTableObject table in tabList)
                {
                    string tableName = table.FullTableName;
                    foreach (var rawColumnDetails in GetColumnDetails(conn, remoteSqlConn, databaseName, new[] { tableName },
                        profileName))
                    {
                        tableList.Add(rawColumnDetails);
                    }
                }
            }

            return tableList;
        }


        static public IList<RawColumnDetails> GetColumnDetails(SqlConnection conn, SqlConnection remoteSqlConn, string databaseName, IEnumerable<string> tableNames, string profileName)
        {
            var columnList = new List<RawColumnDetails>();
            try
            {
                IList profileData = RawSQL.GetPofilerData(conn, profileName);
                var splitedProfileDefinitions = new List<string>(profileData.Count);
                foreach (var definition in profileData)
                {
                    splitedProfileDefinitions.AddRange(definition.ToString().Split(','));
                }

                char[] delimiterChars = { '.' };

                StringBuilder sqlBuilder = new StringBuilder();


                string query = string.Format("USE [{0}]; select c.COLUMN_NAME, (TABLE_SCHEMA + '.' + TABLE_NAME), c.DATA_TYPE, t.max_length from INFORMATION_SCHEMA.COLUMNS c inner join sys.types t on c.DATA_TYPE = t.[name] " +
                                                "where (TABLE_SCHEMA + '.' + TABLE_NAME) in ({1}) ", databaseName, string.Join(",", tableNames.Select(tn => "'" + tn + "'")));

                sqlBuilder.Append(query);
                if (splitedProfileDefinitions.Count > 0)
                {
                    int i = 0;
                    foreach (var item in splitedProfileDefinitions)
                    {
                        sqlBuilder.Append(i == 0 ? " AND (" : " or ");

                        sqlBuilder.AppendFormat(" COLUMN_NAME LIKE '{0}' COLLATE Latin1_General_CI_AS ", item);
                        //cmd.Parameters.AddWithValue(paramName, item.ToLower());
                        i++;
                    }
                    sqlBuilder.Append(")");
                }

                string query1 = sqlBuilder.ToString();
                string db = remoteSqlConn.Database;
                if (splitedProfileDefinitions.Count > 0)
                {
                    using (SqlCommand cmd = new SqlCommand(query1, remoteSqlConn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //tableList = new ArrayList();

                            while (reader.Read())
                            {
                                int columnIndex = 0;
                                string matchingString = "";
                                string machingStr = "";
                                matchingString = reader.GetString(columnIndex++);
                                foreach (var item in splitedProfileDefinitions)
                                {
                                    string str = RemoveSpecialCharacters(item.ToString());

                                    if (matchingString.ToLower().Contains(str.ToLower()))
                                    {
                                        machingStr = item.ToString();
                                    }
                                }

                                RawColumnDetails raw = new RawColumnDetails();
                                raw.DatabaseName = databaseName;
                                raw.TableName = reader.GetString(columnIndex++);
                                raw.FieldName = matchingString;//reader.GetString(columnIndex++);
                                raw.DataType = reader.GetString(columnIndex++);
                                raw.MatchingStr = machingStr;
                                raw.LengthSize = reader.GetInt16(columnIndex++);
                                columnList.Add(raw);
                            }
                        }
                    }
                }
                remoteSqlConn.ChangeDatabase(db);

            }
            catch (Exception ex)
            {
                columnList = new List<RawColumnDetails>();  // return an empty array
            }

            return columnList;
        }


        static public IList GetTableDetailsForAll(SqlConnection conn, SqlConnection remoteSqlConn, IList dbName, string profileName)
        {
            //if (string.IsNullOrEmpty(tableName))
            //{
            //    return GetTableDetails(conn, dbName);
            //}
            IList tableList = new ArrayList();

            foreach (var dbname in dbName)
            {

                string databaseName = dbname.ToString();
                IList tabList = RawSQL.GetTables(remoteSqlConn, databaseName);

                foreach (RawTableObject fullName in tabList)
                {
                    foreach (var rawTableDetails in GetTableDetails(conn, remoteSqlConn, databaseName,
                        new[] { fullName.FullTableName }, profileName))
                    {
                        tableList.Add(rawTableDetails);
                    }
                }
            }

            return tableList;
        }

        static public IList<RawTableDetails> GetTableDetails(SqlConnection conn, SqlConnection remoteSqlConn, string databaseName, IEnumerable<string> tableNames, string profileName)
        {
            //if (string.IsNullOrEmpty(tableName))
            //{
            //    return GetTableDetails(conn, dbName);
            //}
            var tableList = new List<RawTableDetails>();
            try
            {

                IList profileData = RawSQL.GetPofilerData(conn, profileName);
                var splitedProfileDefinitions = new List<string>(profileData.Count);
                foreach (var definition in profileData)
                {
                    splitedProfileDefinitions.AddRange(definition.ToString().Split(','));
                }
                StringBuilder sqlBuilder = new StringBuilder();
                // Load Tables
                string query1 = string.Format("USE [{0}]; SELECT db_name() as DatabaseName, " +
                         "SCHEMA_NAME(t.schema_id)+'.'+t.name AS SchemaTable, " +
                         "(SUM(a.used_pages) * 8.0 ) / 1024.0 AS UsedDataSpaceMB, " +
                 "COUNT(DISTINCT c.Name) AS MatchedColumns, " +
                 "(SELECT SUM(row_count) FROM sys.dm_db_partition_stats ps WHERE ps.object_id = OBJECT_ID(SCHEMA_NAME(t.schema_id)+'.'+t.name ) AND (index_id = 0 or index_id = 1)) AS RowsCount " +
                         "FROM sys.tables AS t " +
                         "INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID " +
                         "INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id " +
                         "INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id " +
                         "INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id ", databaseName);
                sqlBuilder.Append(query1);

                if (splitedProfileDefinitions.Count > 0)
                {
                    int i = 0;
                    foreach (var item in splitedProfileDefinitions)
                    {
                        sqlBuilder.Append(i == 0 ? " WHERE (" : " or ");

                        sqlBuilder.AppendFormat(" c.name LIKE '{0}' COLLATE Latin1_General_CI_AS ", item);
                        //cmd.Parameters.AddWithValue(paramName, item.ToLower());
                        i++;
                    }
                    sqlBuilder.Append(")");
                }

                //"WHERE ( c.name LIKE '%id%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%contact%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%age%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS  or  c.name LIKE '%name%' COLLATE Latin1_General_CI_AS) AND " +
                string str = string.Format(" AND SCHEMA_NAME(t.schema_id)+'.'+t.name in ({0}) AND " +
                               "t.type_desc = 'USER_TABLE' AND " +
                               "i.index_id <= 1 " +
                               "GROUP BY SCHEMA_NAME(t.schema_id)+'.'+t.name " +
                "ORDER BY DatabaseName, SchemaTable", string.Join(",", tableNames.Select(t => "'" + t + "'")));
                sqlBuilder.Append(str);
                string query = sqlBuilder.ToString();

                //var rawCounts = tableNames.ToDictionary(t => t, t => GetRowCount(remoteSqlConn, databaseName, t));
                string db = remoteSqlConn.Database;
                remoteSqlConn.ChangeDatabase(databaseName);
                if (profileData.Count > 0)
                {
                    using (SqlCommand cmd = new SqlCommand(query, remoteSqlConn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //tableList = new ArrayList();

                            while (reader.Read())
                            {
                                int columnIndex = 0;
                                RawTableDetails raw = new RawTableDetails();
                                raw.DatabaseName = reader.GetString(columnIndex++);
                                raw.TableName = reader.GetString(columnIndex++);
                                raw.Size = reader.GetDecimal(columnIndex++);
                                raw.ColumnsIdentified = reader.GetInt32(columnIndex++);
                                raw.RowCount = reader.GetInt64(columnIndex++);
                                //raw.RowCount = rawCounts[raw.TableName];
                                tableList.Add(raw);
                            }
                        }
                    }
                }
                remoteSqlConn.ChangeDatabase(db);
            }
            catch (Exception ex)
            {
                tableList = new List<RawTableDetails>();  // return an empty array
            }

            return tableList;
        }


        static public IList GetDatabaseListForAll(SqlConnection conn, int servId)
        {
            //if (string.IsNullOrEmpty(tableName))
            //{
            //    return GetTableDetails(conn, dbName);
            //}
            IList tableList = new ArrayList();
            try
            {                      // Load Tables
                string query = "Select name, database_id from sys.databases";

                string db = conn.Database;
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //tableList = new ArrayList();

                            while (reader.Read())
                            {
                                int columnIndex = 0;
                                RawDatabaseDetails raw = new RawDatabaseDetails();
                                raw.DatabaseName = reader.GetString(columnIndex++);
                                raw.DbId = reader.GetInt32(columnIndex++);
                                raw.ServerId = servId;

                                tableList.Add(raw);
                            }
                        }
                    }
                }
                conn.ChangeDatabase(db);

            }
            catch (Exception ex)
            {
                tableList = new ArrayList();  // return an empty array
            }

            return tableList;
        }



        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="schemaTable">The schema table.</param>
        /// <returns></returns>
        static public Int32 GetRowCount(SqlConnection conn, string databaseName, string schemaTable)
        {
            Int32 result = 0;
            conn.Close();
            conn.Open();
            try
            {
                //SqlConnection sqlConn = new SqlConnection(ConnectionString());
                string query = string.Format(@"USE [{0}]; SELECT SUM(row_count)
                                            FROM sys.dm_db_partition_stats
                                            WHERE object_id=OBJECT_ID(@schematable) AND 
                                            (index_id=0 or index_id=1)", databaseName);
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@schematable", System.Data.SqlDbType.VarChar);
                    cmd.Parameters["@schematable"].Value = schemaTable;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result = Convert.ToInt32(reader.GetValue(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }



        //SQLCM 5.4 end

        static private bool Is2000(SqlConnection conn)
        {
            string versionString = conn.ServerVersion;
            if (versionString.StartsWith("08"))
                return true;
            else
                return false;
        }

        static private bool Is2014(SqlConnection connection)
        {
            return connection.ServerVersion.StartsWith("12");
        }

        static public List<string> GetBlobTables(SqlConnection conn, string dbName)
        {
            List<string> tableList = new List<string>();
            string db = conn.Database;
            // Load Databases			   
            string cmdfmt = "SELECT DISTINCT o.name,SCHEMA_NAME(o.schema_id) FROM {0}.sys.all_columns c " +
               "INNER JOIN {0}.sys.all_objects o ON c.object_id = o.object_id " +
               "INNER JOIN {0}.sys.types t ON c.system_type_id = t.system_type_id " +
               "WHERE c.system_type_id IN (35, 165, 99, 34, 173) " +
               "AND o.[name] NOT LIKE 'sys%' AND o.[name] <> 'dtproperties' AND o.[type] = 'U'";
            string cmdstr = String.Format(cmdfmt, dbName);

            conn.ChangeDatabase(dbName);
            using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = String.Format("{0}.{1}", reader.GetString(1), reader.GetString(0));
                        tableList.Add(tableName);
                    }
                }
            }
            conn.ChangeDatabase(db);

            return tableList;
        }

        //SQLCM -541/4876/5259 v5.6
        static public IList GetNewDatabasesList(SqlConnection conn, string instanceName, DateTime lastheartbeatTime)
        {
            IList dbList = null;
            string db = conn.Database;
            try
            {
                // SQLCM-5913: Add code to compare UTC time of server's last heart beat with creation time on dbs
                string query = string.Format(@"SELECT dbid, name, DATEADD(SECOND, DATEDIFF(SECOND, GETDATE(), GETUTCDATE()), crdate) FROM master..sysdatabases WHERE has_dbaccess(name) = 1 and LOWER(name) not in ('master', 'model', 'msdb', 'tempdb') and DATEADD(SECOND, DATEDIFF(SECOND, GETDATE(), GETUTCDATE()), crdate) >= '{0}' order by crdate desc", lastheartbeatTime.ToString("yyyy-MM-dd hh:mm:ss"));
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();
                        while (reader.Read())
                        {
                            RawColumnObject raw = new RawColumnObject();
                            raw.Id = reader.GetInt16(0);
                            raw.ColumnName = reader.GetString(1);
                            raw.CreatedDate = reader.GetDateTime(2);
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
            }
            return dbList;
        }
        //SQLCM -541/4876/5259 v5.6 - END

        //-----------------------------------------------------------------------------
        // GetTables
        //-----------------------------------------------------------------------------
        static public IList
           GetColumns(
              SqlConnection conn,
              string dbName,
              string tableName
           )
        {
            IList columnList = null;

            try
            {
                // Load Columns			   
                string cmd2005 = "SELECT name,column_id,system_type_id as xtype FROM [{0}].sys.columns WHERE object_id = OBJECT_ID('[{0}].{1}') ORDER by name ASC";
                string cmd2000 = "SELECT c.name,c.colid as [column_id],c.xtype " +
                                  "FROM [{0}]..syscolumns c inner join [{0}]..sysobjects as t on c.id=t.id " +
                                  "WHERE t.name = '{1}' AND t.[type] = 'U'" +
                                  "ORDER by c.name ASC";

                string cmdstr = String.Format(Is2000(conn) ? cmd2000 : cmd2005, dbName, tableName);
                string db = conn.Database;
                conn.ChangeDatabase(dbName);
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        columnList = new ArrayList();

                        while (reader.Read())
                        {
                            RawColumnObject raw = new RawColumnObject();
                            raw.ColumnName = reader.GetString(0);

                            if (Is2000(conn))
                                raw.Id = reader.GetInt16(1);
                            else
                                raw.Id = reader.GetInt32(1);
                            raw.Type = (int)reader.GetByte(2);
                            columnList.Add(raw);
                        }
                    }
                }
                conn.ChangeDatabase(db);
            }
            catch
            {
                columnList = new ArrayList();  // return an empty array
            }

            return columnList;
        }


        #endregion

        #region Privileged User Lists

        //-----------------------------------------------------------------------------
        // GetServerRoles - Gets list of server roles from SQLServer instance
        //-----------------------------------------------------------------------------
        static public IList
           GetServerRoles(
              SqlConnection conn
           )
        {
            IList roleList = null;

            try
            {
                // Load Server Roles			   
                string cmdstr = "select v1.name, v1.number, v2.name " +
                                "  from master..spt_values v1, master..spt_values v2 " +
                                "  where v1.low = 0 and " +
                                 "     v1.type = 'SRV' and " +
                                 "     v2.low = -1 and " +
                                 "     v2.type = 'SRV' and " +
                                 "     v1.number = v2.number ";

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        roleList = new ArrayList();

                        while (reader.Read())
                        {
                            RawRoleObject raw = new RawRoleObject();
                            raw.name = SQLHelpers.GetString(reader, 0);
                            raw.roleid = SQLHelpers.GetInt32(reader, 1);
                            raw.fullName = SQLHelpers.GetString(reader, 2);
                            roleList.Add(raw);
                        }
                    }
                }
                try
                {
                    cmdstr = "select name,principal_id,name from master.sys.server_principals where type='R' and name<>'public'";

                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //roleList = new ArrayList();

                            var list = roleList.Cast<RawRoleObject>();
                            while (reader.Read())
                            {
                                RawRoleObject raw = new RawRoleObject();
                                raw.name = SQLHelpers.GetString(reader, 0);
                                raw.roleid = SQLHelpers.GetInt32(reader, 1);
                                raw.fullName = SQLHelpers.GetString(reader, 2);
                                if (list.FirstOrDefault(l => l.name == raw.name) == null)
                                    roleList.Add(raw);
                            }
                        }
                    }
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                roleList = new ArrayList();  // return an empty array
                throw ex;
            }

            return roleList;
        }

        //-----------------------------------------------------------------------------
        // GetServerLogins
        //-----------------------------------------------------------------------------
        static public IList
           GetServerLogins(
              SqlConnection conn
           )
        {
            IList loginList = null;
            string cmdstr;
            int sqlVersion;

            try
            {
                sqlVersion = SQLHelpers.GetSqlVersion(conn);
                // server logins

                if (sqlVersion < 9)
                {
                    cmdstr = "SELECT name, sid, " +
                                " isntname, isntuser, isntgroup, " +
                                " denylogin,hasaccess, " +
                                " sysadmin, securityadmin, serveradmin, setupadmin, processadmin, diskadmin, dbcreator " +
                             "FROM master..syslogins " +
                             "WHERE sid <> 0";
                }
                else
                {
                    // SQL 2005 has new server role: bulkadmin
                    cmdstr = "SELECT name, sid, " +
                                " isntname, isntuser, isntgroup, " +
                                " denylogin,hasaccess, " +
                                " sysadmin, securityadmin, serveradmin, setupadmin, processadmin, diskadmin, dbcreator, bulkadmin " +
                             "FROM master..syslogins " +
                             "WHERE sid <> 0";
                }
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        loginList = new ArrayList();

                        while (reader.Read())
                        {
                            RawLoginObject raw = new RawLoginObject();
                            raw.name = SQLHelpers.GetString(reader, 0);

                            if (!reader.IsDBNull(1))
                            {
                                int len = (int)reader.GetBytes(1, 0, null, 0, 0);
                                if (len > 0)
                                {
                                    raw.sid = new byte[len];
                                    reader.GetBytes(1, 0, raw.sid, 0, len);
                                }

                                int col = 2;

                                raw.isntname = SQLHelpers.GetInt32(reader, col++);
                                raw.isntuser = SQLHelpers.GetInt32(reader, col++);
                                raw.isntgroup = SQLHelpers.GetInt32(reader, col++);

                                raw.denylogin = SQLHelpers.GetInt32(reader, col++);
                                raw.hasaccess = SQLHelpers.GetInt32(reader, col++);

                                raw.sysadmin = SQLHelpers.GetInt32(reader, col++);
                                raw.securityadmin = SQLHelpers.GetInt32(reader, col++);
                                raw.serveradmin = SQLHelpers.GetInt32(reader, col++);
                                raw.setupadmin = SQLHelpers.GetInt32(reader, col++);
                                raw.processadmin = SQLHelpers.GetInt32(reader, col++);
                                raw.diskadmin = SQLHelpers.GetInt32(reader, col++);
                                raw.dbcreator = SQLHelpers.GetInt32(reader, col++);
                                if (sqlVersion < 9)
                                    raw.bulkadmin = 0;
                                else
                                    raw.bulkadmin = SQLHelpers.GetInt32(reader, col++);
                            }
                            loginList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loginList = new ArrayList();  // return an empty array
                throw ex;
            }

            return loginList;
        }

        //------------------------------------------------------------------------------------------------
        // HasSufficientPermissions - Checks the permissions defined in the unified User Creation Scripts
        //------------------------------------------------------------------------------------------------
        // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
        static public bool
           HasSufficientPermissions(
              SqlConnection conn
           )
        {
            bool hasSufficientPermissions = false;
            try
            {
                string cmdStr = "USE [master]; EXEC sp_SQLcompliance_PermissionsCheck;";
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    int sufficientPermissions;
                    object obj = cmd.ExecuteScalar();
                    if (obj is System.DBNull)
                    {
                        sufficientPermissions = 0;
                    }
                    else
                    {
                        sufficientPermissions = (int)obj;
                    }
                    if (sufficientPermissions == 1)
                    {
                        hasSufficientPermissions = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // if it fails, you dont have right to do it so you are not a sysadmin
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "HasSufficientPermissions",
                                         ex);
            }
            return hasSufficientPermissions;
        }


        //-----------------------------------------------------------------------------
        // IsCurrentUserSysadmin
        //-----------------------------------------------------------------------------
        static public bool
           IsCurrentUserSysadmin(
              SqlConnection conn
           )
        {
            bool isSysadmin = false;

            try
            {
                string cmdStr = "SELECT IS_SRVROLEMEMBER ('sysadmin')";
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    int sysadmin;
                    object obj = cmd.ExecuteScalar();
                    if (obj is System.DBNull)
                    {
                        sysadmin = 0;
                    }
                    else
                    {
                        sysadmin = (int)obj;
                    }

                    if (sysadmin == 1)
                    {
                        isSysadmin = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // if it fails, you dont have right to do it so you are not a sysadmin
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "IsSysadmin",
                                         ex);
            }

            return isSysadmin;
        }


        //-----------------------------------------------------------------------------
        // IsCurrentUserSysadmin
        //-----------------------------------------------------------------------------
        static public string GetServiceNameOfInstance(string instanceName, SqlConnection conn)
        {
            var serviceName = string.Empty;

            try
            {
                var cmdStr = "select @@SERVICENAME";
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    var obj = cmd.ExecuteScalar();

                    if (!(obj is DBNull))
                    {
                        serviceName = (string)obj;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, string.Format("Failed to get service name for instance '{0}'.", instanceName), ex);
            }

            return serviceName;
        }

        //-----------------------------------------------------------------------------
        // GetLogins
        //-----------------------------------------------------------------------------
        static public IList
           GetLogins(
              SqlConnection conn,
              string databaseName
           )
        {
            IList loginList = null;
            string cmdstr;

            try
            {
                if (databaseName == "")
                {
                    int sqlVersion = SQLHelpers.GetSqlVersion(conn);

                    if (sqlVersion < 9)
                        // server logins
                        cmdstr = "select name,sid from master..sysxlogins where sid <>0";
                    else
                        cmdstr = "SELECT name, sid from master..syslogins where sid <> 0";
                }
                else
                {
                    // database logins
                    cmdstr = String.Format("select name,sid from {0}..sysusers where sid <> 0",
                                            SQLHelpers.CreateSafeDatabaseName(databaseName));
                }

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        loginList = new ArrayList();

                        while (reader.Read())
                        {
                            RawLoginObject raw = new RawLoginObject();
                            raw.name = SQLHelpers.GetString(reader, 0);

                            if (!reader.IsDBNull(1))
                            {
                                int len = (int)reader.GetBytes(1, 0, null, 0, 0);
                                if (len > 0)
                                {
                                    raw.sid = new byte[len];
                                    reader.GetBytes(1, 0, raw.sid, 0, len);
                                }
                            }
                            loginList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loginList = new ArrayList();  // return an empty array
                throw ex;
            }

            return loginList;
        }

        //-----------------------------------------------------------------------------
        // GetReportLogins
        //-----------------------------------------------------------------------------
        static public List<string>
           GetReportLogins(
              SqlConnection conn
           )
        {
            List<string> loginList = null;
            string cmdstr;
            try
            {
                // server logins
                cmdstr = "SELECT name FROM Logins";
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        loginList = new List<string>();

                        while (reader.Read())
                        {
                            loginList.Add(SQLHelpers.GetString(reader, 0).ToLower());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loginList = new List<string>();  // return an empty array
                throw ex;
            }
            return loginList;
        }

        //-----------------------------------------------------------------------------
        // CreateReportLogins
        //-----------------------------------------------------------------------------
        static public void
           InsertReportLogin(List<string> logins,
              SqlConnection conn
           )
        {
            string cmdstr = String.Empty;
            try
            {
                string fmt = "INSERT INTO {0}..{1} ([name]) VALUES";
                cmdstr = String.Format(fmt,
                                    SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                    CoreConstants.RepositoryLoginsTable);

                foreach (var item in logins)
                {
                    if (logins.LastOrDefault().Equals(item))
                    {
                        fmt = "{0} ( {1})";
                        cmdstr = String.Format(fmt,
                                       cmdstr,
                                       SQLHelpers.CreateSafeString(item));
                    }
                    else
                    {
                        fmt = "{0} ( {1}),";
                        cmdstr = String.Format(fmt,
                                       cmdstr,
                                       SQLHelpers.CreateSafeString(item));
                    }
                }

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //-----------------------------------------------------------------------------
        // Remove Report Access
        //-----------------------------------------------------------------------------
        static public void
           RemoveReportAccess(string reportName,
              int uid,
              SqlConnection conn
           )
        {
            string cmdstr = String.Empty;
            try
            {
                string fmt = "INSERT INTO {0}..[{1}] ([rid],[uid]) VALUES ((select ar.[rid] FROM AuditReports ar where reportname = {2}),{3})";
                cmdstr = String.Format(fmt,
                                    SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                    CoreConstants.RepositoryReportAccessTable,
                                    SQLHelpers.CreateSafeString(reportName),
                                    uid);

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //-----------------------------------------------------------------------------
        // Revoke Report Access
        //-----------------------------------------------------------------------------
        static public void
           RevokeReportAccess(string reportName,
              int uid,
              SqlConnection conn
           )
        {
            string cmdstr = String.Empty;
            try
            {
                string fmt = "DELETE FROM {0}..{1} Where [uid] = {2} and [rid] = (select rid from {3} where [reportname] = {4})";
                cmdstr = String.Format(fmt,
                                       SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                       CoreConstants.RepositoryReportAccessTable,
                                       uid,
                                       CoreConstants.RepositoryAuditReportsTable,
                                       SQLHelpers.CreateSafeString(reportName));

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AvailabilityGroup Logic
        //-----------------------------------------------------------------------------
        // GetUserDatabasesForAlwaysOn - Gets list of databases from SQLServer instance
        //-----------------------------------------------------------------------------
        static public IList
           GetUserDatabasesForAlwaysOn(SqlConnection conn, string dbName)
        {
            IList dbList = null;
            try
            {
                String test = dbName;

                string cmdstr = "";
                //dbName = "ss-alwyslist04";
                // For All AG Database available on the Network
                //if (dbName == null)
                //{

                cmdstr = String.Format(" if((select isnull(count(*),0) as list_count from master.sys.availability_group_listeners where dns_name={0} COLLATE SQL_Latin1_General_CP1_CI_AS )>0)\n" +
                               "begin\n" +
                               "SELECT [name], [dbid] FROM (SELECT dbcs.database_name AS [name], \n" +
                               "database_id AS [dbid], AG.name AS [ag_name] from  master.sys.availability_groups AS AG \n" +
                               "LEFT OUTER JOIN master.sys.dm_hadr_availability_group_states as agstates \n" +
                               "ON AG.group_id = agstates.group_id \n" +
                               "INNER JOIN master.sys.availability_replicas AS AR \n" +
                               "ON AG.group_id = AR.group_id \n" +
                               "INNER JOIN master.sys.availability_group_listeners AS AGL ON AG.group_id = AGL.group_id\n" +
                               "INNER JOIN master.sys.dm_hadr_availability_replica_states AS arstates\n" +
                               "ON AR.replica_id = arstates.replica_id AND arstates.is_local = 1 \n" +
                               "INNER JOIN master.sys.dm_hadr_database_replica_cluster_states AS dbcs\n" +
                               " ON arstates.replica_id = dbcs.replica_id\n" +
                               "LEFT OUTER JOIN master.sys.dm_hadr_database_replica_states AS dbrs\n" +
                               "ON dbcs.replica_id = dbrs.replica_id AND dbcs.group_database_id = dbrs.group_database_id\n" +
                               "where AGL.dns_name={1} COLLATE SQL_Latin1_General_CP1_CI_AS \n" +
                               "UNION \n" +
                               "SELECT DISTINCT hdrcs.database_name name \n" +
                               ", drs.database_id dbid, ag.name AS [ag_name] \n" +
                               "FROM master.sys.availability_groups as ag \n" +
                               "JOIN master.sys.availability_replicas as ar ON ag.group_id = ar.group_id \n" +
                               "JOIN master.sys.dm_hadr_database_replica_cluster_states hdrcs ON ar.replica_id = hdrcs.replica_id \n" +
                               "JOIN master.sys.dm_hadr_database_replica_states AS drs ON drs.replica_id = hdrcs.replica_id and hdrcs.group_database_id = drs.group_database_id \n" +
                               "INNER JOIN sys.dm_hadr_availability_group_states AS hags ON hags.group_id = ag.group_id \n" +
                               "WHERE ar.secondary_role_allow_connections = 0) res \n" +
                               "ORDER BY ag_name ASC, [name]  \n" +
                               "end \n" +
                               "else \n" +
                               "begin \n" +
                               "SELECT [name], [dbid] FROM (SELECT name, cast(dbid as int) as dbid FROM master..sysdatabases WHERE has_dbaccess(name) = 1 \n" +
                               "UNION \n" +
                               "SELECT hdrcs.database_name name \n" +
                               ", CAST(drs.database_id AS INT) dbid \n" +
                               "FROM master.sys.availability_groups AS ag \n" +
                               "JOIN master.sys.availability_replicas AS ar ON ag.group_id = ar.group_id \n" +
                               "JOIN master.sys.dm_hadr_database_replica_cluster_states hdrcs ON ar.replica_id = hdrcs.replica_id \n" +
                               "JOIN master.sys.dm_hadr_database_replica_states AS drs ON drs.replica_id = hdrcs.replica_id and hdrcs.group_database_id = drs.group_database_id \n" +
                               "INNER JOIN sys.dm_hadr_availability_group_states AS hags ON hags.group_id = ag.group_id \n" +
                               "WHERE ar.secondary_role_allow_connections = 0) res \n" +
                               "ORDER by name ASC; \n" +
                               "end", SQLHelpers.CreateSafeString(dbName), SQLHelpers.CreateSafeString(dbName));
                // }
                //else if (dbName.Length <= 0)
                //   return dbList;
                // else{
                // Load Databases			   
                //cmdstr = String.Format("SELECT name, dbid" +
                //                  " FROM master..sysdatabases " +
                //                " WHERE name = {0} " +
                //                  " ORDER by name ASC", SQLHelpers.CreateSafeString(dbName));
                //}


                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();

                        while (reader.Read())
                        {
                            RawDatabaseObject raw = new RawDatabaseObject();
                            raw.name = reader.GetString(0);
                            //raw.dbid = (dbName == null) ? reader.GetInt32(1) : reader.GetInt16(1);
                            raw.dbid = reader.GetInt32(1);
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dbList;
        }

        //-----------------------------------------------------------------------------
        // GetAvailabilityGroupDetails - Gets list Availability Groups for given Databases
        //-----------------------------------------------------------------------------
        static public IList
           GetAvailabilityGroupDetails(
              SqlConnection conn,
              List<string> dbNames
           )
        {
            IList dbList = null;
            RawAVGroup raw = null;

            try
            {
                if (dbNames == null || dbNames.Count <= 0)
                    return dbList;
                string inClause = "(";
                foreach (string dbName in dbNames)
                {
                    if (inClause.Length == 1)
                        inClause += "'" + dbName + "'";
                    else
                        inClause += "," + "'" + dbName + "'";
                }
                inClause += ")";

                // Load Availability Group Details for the databases
                string cmdstr = String.Format("select hdrcs.database_name DBName, ag.name AVGName, ar.replica_server_name ReplicaNode, " +
                                 "CASE WHEN hags.primary_replica = ar.replica_server_name THEN CAST(1 AS BIT) ELSE CAST (0 AS BIT) END AS IsPrimary " +
                                 "from master.sys.availability_groups as ag " +
                                 "join master.sys.availability_replicas as ar on ag.group_id = ar.group_id " +
                                 "join master.sys.dm_hadr_database_replica_cluster_states hdrcs on ar.replica_id = hdrcs.replica_id " +
                                 "join master.sys.dm_hadr_database_replica_states AS drs on drs.replica_id = hdrcs.replica_id and hdrcs.group_database_id = drs.group_database_id " +
                                 "INNER JOIN sys.dm_hadr_availability_group_states AS hags ON hags.group_id = ag.group_id " +
                                 "where hdrcs.database_name in {0} " +
                                 "order by hdrcs.database_name, ar.replica_server_name ", inClause);

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();
                        while (reader.Read())
                        {
                            raw = new RawAVGroup();
                            raw.dbName = reader.GetString(0);
                            raw.avgName = reader.GetString(1);
                            raw.replicaServerName = reader.GetString(2);
                            raw.isPrimary = reader.GetBoolean(3);
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dbList;
        }


        //-----------------------------------------------------------------------------
        // GetSecondaryRoleAllowConnections - Gets list Availability Groups for given Databases
        //-----------------------------------------------------------------------------
        static public List<SecondaryRoleDetails>
           GetSecondaryRoleAllowConnections(
              SqlConnection conn,
              string instanceName
           )
        {
            var secondaryDetails = new List<SecondaryRoleDetails>();

            try
            {
                // Load Secondary Role Details fron the instance
                string cmdstr = String.Format("select distinct ar.secondary_role_allow_connections, ag.name AVGName, ar.replica_server_name, " +
                                 "CASE WHEN hags.primary_replica = ar.replica_server_name THEN CAST(1 AS BIT) ELSE CAST (0 AS BIT) END AS IsPrimary " +
                                 "from master.sys.dm_hadr_database_replica_states AS drs " +
                                 "join master.sys.availability_replicas AS ar on drs.replica_id = ar.replica_id " +
                                 "join master.sys.availability_groups as ag on ag.group_id = ar.group_id " +
                                 "INNER JOIN sys.dm_hadr_availability_group_states AS hags ON hags.group_id = ag.group_id " +
                                 "where replica_server_name = '{0}' ", instanceName);

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var secondaryRole = new SecondaryRoleDetails();
                            secondaryRole.secondaryRoleAllowConnections = reader.GetByte(0);
                            secondaryRole.avgName = reader.GetString(1);
                            secondaryRole.replicaServerName = reader.GetString(2);
                            secondaryRole.isPrimary = reader.GetBoolean(3);
                            secondaryDetails.Add(secondaryRole);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return secondaryDetails;
        }

        //-----------------------------------------------------------------------------
        // Get Read Only Secondary Replica Server List
        //-----------------------------------------------------------------------------
        static public List<string> GetReadOnlySecondaryReplicaServerList(SqlConnection conn)
        {
            var replicaList = new List<string>();

            try
            {
                // Load Availability Group Details for the databases
                string query = @"select 
  	                                AR.replica_server_name
                                 from 
	                                master.sys.dm_hadr_availability_replica_states as HADR
	                                INNER JOIN master.sys.availability_replicas AS AR
	                                ON HADR.replica_id = AR.replica_id
                                 where 
	                                HADR.role = 2 --SECONDARY
	                                AND (HADR.operational_state IS NULL
	                                        OR HADR.operational_state = 2 )
                                    AND AR.secondary_role_allow_connections != 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string replicaServer = reader.GetString(0);
                            replicaList.Add(replicaServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "An error occurred retrieving secondary replica servers.", ex);
                throw ex;
            }

            return replicaList;
        }

        static public List<ReplicaNodeInfo> GetAllReplicaNodeInfoList(SqlConnection conn)
        {
            var replicaList = new List<ReplicaNodeInfo>();

            try
            {
                string query = @"select 
	                                AR.replica_server_name as ReplicaServerName,
	                                HADR.role as RoleType,
	                                HADR.operational_state as OperationalStatus
                                from 
	                                master.sys.dm_hadr_availability_replica_states as HADR
	                                INNER JOIN master.sys.availability_replicas AS AR
	                                ON HADR.replica_id = AR.replica_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 5;// this is required to avoid timeout because of server inaccessible
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var replicaInfo = new ReplicaNodeInfo();
                            replicaInfo.ReplicaServerName = reader.GetString(0);
                            replicaInfo.Role = (ReplicaRole)Enum.ToObject(typeof(ReplicaRole), reader.GetByte(1));
                            var sqlIntValue = reader.GetSqlByte(2);

                            replicaInfo.OperationalState = OperationalState.NotLocal;

                            if (!sqlIntValue.IsNull)
                            {
                                var value = Convert.ToInt32(sqlIntValue.Value);
                                replicaInfo.OperationalState = sqlIntValue.IsNull ? OperationalState.NotLocal : (OperationalState)Enum.ToObject(typeof(OperationalState), value);
                            }

                            replicaList.Add(replicaInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "An error occurred retrieving all replica node info list.", ex);
                throw ex;
            }

            return replicaList;
        }

        static public IList
           GetAlwaysOnRoles(
              SqlConnection conn
           )
        {
            IList dbList = null;
            RawAVGroup raw = null;

            try
            {
                // Load Availability Group Details for the databases
                string cmdstr = "SELECT att.AVGName, att.replicaCount, (select ar.replica_server_name from master.sys.dm_hadr_availability_replica_states ars JOIN [master].[sys].[availability_replicas] ar on ar.replica_id = ars.replica_id  where ars.role = 1 AND ar.group_id = ars.group_id) as PrimaryReplica FROM (select ag.name as AVGName, ag.group_id , count(*) as replicaCount from master.sys.availability_groups as ag join master.sys.dm_hadr_availability_replica_states hars on ag.group_id = hars.group_id group by ag.name, ag.group_id) att";

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();
                        while (reader.Read())
                        {
                            raw = new RawAVGroup();
                            raw.avgName = reader.GetString(0);
                            raw.replicaCount = reader.GetInt32(1);
                            raw.primaryServerName = reader.GetString(2);
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred retrieving the Always on Role details, Replica details..",
                                         ex);
                throw ex;
            }

            return dbList;
        }

        #endregion

        #region CLR enablement

        // Check if the CLR support is enabled.
        public static void
           GetCLRStatus(
              SqlConnection conn,
          out bool configured,
          out bool running
           )
        {
            if (SQLHelpers.GetSqlVersion(conn) < 9) // CLR support started since SQL 2005
            {
                configured = false;
                running = false;
                return;
            }

            try
            {
                // Works for the non sysadmin with the suggested permissions in the script
                string cmdStr = "EXEC sp_configure 'clr enabled'";
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        // 4th column contains the current config value;
                        configured = SQLHelpers.GetInt32(reader, 3) == 1 ? true : false;
                        running = SQLHelpers.GetInt32(reader, 4) == 1 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                // if it fails, you dont have right to do it so you are not a sysadmin
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred retrieving CLR Enabled value.",
                                         ex);
                throw ex;
            }
        }

        // Enable CLR on the instance
        public static bool
            EnableCLR(
                   SqlConnection conn
                   )
        {
            return EnableCLR(conn, true);
        }

        // Enable CLR on the instance
        public static bool
           EnableCLR(
              SqlConnection conn,
              bool enable
              )
        {
            if (SQLHelpers.GetSqlVersion(conn) < 9)
                return false;

            try
            {
                string cmdStr = String.Format("EXEC sp_configure 'clr enabled', {0}", enable ? 1 : 0);
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "RECONFIGURE";
                    cmd.ExecuteNonQuery();
                    bool running, enabled;

                    GetCLRStatus(conn, out enabled, out running);
                    return running;
                }
            }
            catch (Exception ex)
            {
                // if it fails, you dont have right to do it so you are not a sysadmin
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred enabling CLR.",
                                         ex);
            }
            return false;
        }

        public static int GetCompatibilityLevel(SqlConnection conn, string dbName)
        {
            try
            {
                string cmdStr = String.Format("SELECT CAST(cmptlevel as int) FROM master..sysdatabases WHERE name={0}",
                   SQLHelpers.CreateSafeString(dbName));
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    object o = cmd.ExecuteScalar();
                    return (int)o;
                }
            }
            catch (Exception ex)
            {
                // if it fails, you dont have right to do it so you are not a sysadmin
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred enabling CLR.",
                                         ex);
                throw ex;
            }

        }

        public static void DeleteReportData(string name, SqlConnection conn)
        {
            string cmdstr = String.Empty;
            try
            {
                string fmt = "DELETE FROM {0}..{1} WHERE [uid] = (select uid from {2} where [name] = {3});DELETE FROM {4}..{5} Where [name] = {6};";
                cmdstr = String.Format(fmt,
                                       SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                       CoreConstants.RepositoryReportAccessTable,
                                       CoreConstants.RepositoryLoginsTable,
                                       SQLHelpers.CreateSafeString(name),
                                       SQLHelpers.CreateSafeDatabaseName(CoreConstants.RepositoryDatabase),
                                       CoreConstants.RepositoryLoginsTable,
                                       SQLHelpers.CreateSafeString(name)
                                       );

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public bool
           IsReportAccessible(
              string username,
              string reportname,
              SqlConnection conn
           )
        {
            bool IsReportAccessible = false;

            try
            {
                string cmdstr = String.Format("USE {0}; SELECT " +
                               "(Select [uid] from {1} ra where [uid] = (select uid from Logins where name = {2}) and ar.[rid] = ra.[rid] ) as accesscheck " +
                               "FROM {3} ar where reportname = {4}",
                               CoreConstants.RepositoryDatabase,
                               CoreConstants.RepositoryReportAccessTable,
                               SQLHelpers.CreateSafeString(username.ToLower()),
                               CoreConstants.RepositoryAuditReportsTable,
                               SQLHelpers.CreateSafeString(reportname));

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    object obj = cmd.ExecuteScalar();
                    if (obj is System.DBNull)
                    {
                        IsReportAccessible = false;
                    }
                    else
                    {
                        IsReportAccessible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // if it fails, you dont have right to do it so you are not a sysadmin
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "IsReportAccessible",
                                         ex);
            }

            return IsReportAccessible;
        }

        #endregion
    }
    //Serializable object for Availability Group of AlwaysOn
    [Serializable]
    public class RawAVGroup : ISerializable
    {
        public RawAVGroup() { }
        public string dbName;
        public string avgName;
        public string replicaServerName;
        public int replicaCount;
        public string primaryServerName;
        public bool isPrimary;

        override public string ToString() { return dbName; }

        #region Deserialization Constructor
        public RawAVGroup(
        SerializationInfo info,
        StreamingContext context)
        {
            try
            {
                dbName = info.GetString("dbName");
                avgName = info.GetString("avgName");
                replicaServerName = info.GetString("replicaServerName");
                replicaCount = info.GetInt32("replicaCount");
                isPrimary = info.GetBoolean("isPrimary");
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("dbName", dbName);
                info.AddValue("avgName", avgName);
                info.AddValue("replicaServerName", replicaServerName);
                info.AddValue("replicaCount", replicaCount);
                info.AddValue("isPrimary", isPrimary);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion
    }

    [Serializable]
    public class DatabaseAlwaysOnDetails : ISerializable
    {
        public DatabaseAlwaysOnDetails() { }
        public int dbId;
        public int srvId;
        public string srvInstance;
        public int sqlDatabaseId;
        public string dbName;
        public int isAlwaysOn;
        public string replicaServers;
        public int isPrimary;
        public string availGroupName;

        override public string ToString() { return dbName; }

        #region Deserialization Constructor
        public DatabaseAlwaysOnDetails(
        SerializationInfo info,
        StreamingContext context)
        {
            try
            {
                dbId = info.GetInt32("dbId");
                srvId = info.GetInt32("srvId");
                srvInstance = info.GetString("srvInstance");
                sqlDatabaseId = info.GetInt32("sqlDatabaseId");
                ;
                dbName = info.GetString("dbName");
                isAlwaysOn = info.GetInt32("isAlwaysOn");
                ;
                replicaServers = info.GetString("replicaServers");
                isPrimary = info.GetInt32("isPrimary");
                ;
                availGroupName = info.GetString("availGroupName");
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("dbId", dbId);
                info.AddValue("srvId", srvId);
                info.AddValue("srvInstance", srvInstance);
                info.AddValue("sqlDatabaseId", sqlDatabaseId);
                info.AddValue("dbName", dbName);
                info.AddValue("isAlwaysOn", isAlwaysOn);
                info.AddValue("replicaServers", replicaServers);
                info.AddValue("isPrimary", isPrimary);
                info.AddValue("availGroupName", availGroupName);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

        #endregion
    }


    [Serializable]
    public class SecondaryRoleDetails : ISerializable
    {
        public SecondaryRoleDetails() { }
        public int secondaryRoleAllowConnections;
        public string replicaServerName;
        public string avgName;
        public bool isPrimary;

        #region Deserialization Constructor

        public SecondaryRoleDetails(
        SerializationInfo info,
        StreamingContext context)
        {
            try
            {
                secondaryRoleAllowConnections = info.GetInt32("secondaryRoleAllowConnections");
                replicaServerName = info.GetString("replicaServerName");
                avgName = info.GetString("avgName");
                isPrimary = info.GetBoolean("isPrimary");
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowDeserializationException(e, this.GetType());
            }
        }
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                info.AddValue("secondaryRoleAllowConnections", secondaryRoleAllowConnections);
                info.AddValue("replicaServerName", replicaServerName);
                info.AddValue("avgName", avgName);
                info.AddValue("isPrimary", isPrimary);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }
        }
        #endregion

    }

}
