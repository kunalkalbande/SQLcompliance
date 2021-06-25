using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.TSqlParsing;
using System.Collections.Generic;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for TraceRow.
	/// </summary>
    internal class TraceRow
    {
        internal int eventClass;
        internal int eventSubclass;
        internal DateTime startTime;
        internal int spid;
        internal string applicationName;
        internal string hostName;
        internal string serverName;
        internal string loginName;
        internal int success;
        internal string databaseName;
        internal int databaseId;
        internal string dbUserName;
        internal int objectType;
        internal string objectName;
        internal int objectId;
        internal int permissions;
        internal int columnPermissions;
        internal string targetLoginName;
        internal string targetUserName;
        internal string roleName;
        internal string ownerName;
        internal string textData;
        internal int nestLevel;

        // new columns for SQL Server 2005
        internal string fileName;
        internal string linkedServerName;
        internal string parentName;
        internal int isSystem;
        internal string sessionLoginName;
        internal string providerName;
        internal long eventSequence = -1;
        internal long? rowCounts = null; // 5.5
        internal string guid;//5.5

        internal long startSequence = -1;
        internal long endSequence = -1;
        internal DateTime endTime = CoreConstants.InvalidDateTimeValue;

        internal int processed;
        internal int hash = 0;      // value for data integrity check
        internal Dictionary<int, List<ColumnTableConfiguration>> columnTablesConfig;

        //-----------------------------------------------------------------------
        // Constructor
        //-----------------------------------------------------------------------
        public TraceRow()
        {
            processed = 0;
            hash = 0;
        }

        //-----------------------------------------------------------------------
        // LoadReader - Loads class properties from result set row
        //              order matches columns from GetLoadSql()
        //-----------------------------------------------------------------------
        internal void
           LoadReader(
              SqlDataReader reader
           )
        {
            LoadReader(reader, 0);
        }

        internal void
           LoadReader(
              SqlDataReader reader,
              int readerColumn
           )
        {
            int col = readerColumn;

            eventClass = SQLHelpers.GetInt32(reader, col++);
            eventSubclass = SQLHelpers.GetInt32(reader, col++);
            startTime = SQLHelpers.GetDateTime(reader, col++);
            spid = SQLHelpers.GetInt32(reader, col++);
            applicationName = SQLHelpers.GetString(reader, col++);
            hostName = SQLHelpers.GetString(reader, col++);
            serverName = SQLHelpers.GetString(reader, col++);
            loginName = SQLHelpers.GetString(reader, col++);
            success = SQLHelpers.GetInt32(reader, col++);
            databaseName = SQLHelpers.GetString(reader, col++);
            databaseId = SQLHelpers.GetInt32(reader, col++);
            dbUserName = SQLHelpers.GetString(reader, col++);
            objectType = SQLHelpers.GetInt32(reader, col++);
            objectName = SQLHelpers.GetString(reader, col++);
            objectId = SQLHelpers.GetInt32(reader, col++);
            permissions = SQLHelpers.GetInt32(reader, col++);
            columnPermissions = SQLHelpers.GetInt32(reader, col++);
            targetLoginName = SQLHelpers.GetString(reader, col++);
            targetUserName = SQLHelpers.GetString(reader, col++);
            roleName = SQLHelpers.GetString(reader, col++);
            ownerName = SQLHelpers.GetString(reader, col++);
            textData = SQLHelpers.GetString(reader, col++);
            nestLevel = SQLHelpers.GetInt32(reader, col++);

            // sql server 2005 columns
            fileName = SQLHelpers.GetString(reader, col++);
            linkedServerName = SQLHelpers.GetString(reader, col++);
            parentName = SQLHelpers.GetString(reader, col++);
            isSystem = SQLHelpers.GetInt32(reader, col++);
            sessionLoginName = SQLHelpers.GetString(reader, col++);
            providerName = SQLHelpers.GetString(reader, col++);
            eventSequence = SQLHelpers.GetLong(reader, col++);
            //5.5
            rowCounts = SQLHelpers.GetRowCounts(reader, col++);
            guid = SQLHelpers.GetString(reader, col++);
        }

        //-----------------------------------------------------------------------
        // LoadRow
        //-----------------------------------------------------------------------
        internal void
           LoadRow(
              DataRow row
           )
        {
            LoadRow(row, 0);
        }

        internal void
           LoadRow(
              DataRow row,
              int startColumn
           )
        {
            int col = startColumn;

            eventClass = GetRowInt32(row, col++);
            eventSubclass = GetRowInt32(row, col++);
            startTime = GetRowDateTime(row, col++);
            spid = GetRowInt32(row, col++);
            applicationName = GetRowString(row, col++);
            hostName = GetRowString(row, col++);
            serverName = GetRowString(row, col++);
            loginName = GetRowString(row, col++);
            success = GetRowInt32(row, col++);
            databaseName = GetRowString(row, col++);
            databaseId = GetRowInt32(row, col++);
            dbUserName = GetRowString(row, col++);
            objectType = GetRowInt32(row, col++);
            objectName = GetRowString(row, col++);
            objectId = GetRowInt32(row, col++);

            permissions = GetRowPermissions(row, col++);

            columnPermissions = GetRowInt32(row, col++);
            targetLoginName = GetRowString(row, col++);
            targetUserName = GetRowString(row, col++);
            roleName = GetRowString(row, col++);
            ownerName = GetRowString(row, col++);
            textData = GetRowString(row, col++);
            nestLevel = GetRowInt32(row, col++);

            // SQL Server 2005 - only load when columns are present
            if (row.ItemArray.Length > col)
            {
                fileName = GetRowString(row, col++);
                linkedServerName = GetRowString(row, col++);
                parentName = GetRowString(row, col++);
                isSystem = GetRowInt32(row, col++);
                sessionLoginName = GetRowString(row, col++);
                providerName = GetRowString(row, col++);
                eventSequence = GetRowInt64(row, col++);
                int colNumber = col++;
                if (row[colNumber].Equals(DBNull.Value))
                    rowCounts = null;
                else
                    rowCounts = GetRowInt64(row, colNumber);
            }

            // 3.1 : Calculated columns for data change records
            //
            if (row.ItemArray.Length > TraceJob.ndxStartSequence)
            {
                startSequence = GetRowInt64(row, TraceJob.ndxStartSequence, -1);
                endSequence = GetRowInt64(row, TraceJob.ndxEndSequence, -1);
                endTime = GetRowDateTime(row, TraceJob.ndxEndTime);
            }

            //5.5 guid to map data change record in case of XEvent
            if (row.ItemArray.Length > TraceJob.ndxGUId)
            {
                guid = GetRowString(row, TraceJob.ndxGUId);
            }
        }

        internal string
           GetRowString(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return "";
            else
                return (string)row[colName];
        }

        internal int
           GetRowInt32(
              DataRow row,
              int colNdx
           )
        {
            try
            {
                if (row.IsNull(colNdx))
                    return 0;
                else
                {
                    return Convert.ToInt32(row[colNdx]);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return 0;
            }
        }

        internal Int64
           GetRowInt64(
           DataRow row,
           int colNdx)
        {
            return GetRowInt64(row, colNdx, 0);
        }

        internal long?
          GetRowCounts(
          DataRow row,
          int colNdx)
        {
            try
            {
                if (row.IsNull(colNdx))
                    return null;
                else
                    return Convert.ToInt64(row[colNdx]);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        internal Int64
             GetRowInt64(
             DataRow row,
             int colNdx,
             Int64 nullValue
             )
        {
            try
            {
                if (row.IsNull(colNdx))
                    return nullValue;
                else
                    return Convert.ToInt64(row[colNdx]);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return 0;
            }
        }

        internal DateTime
           GetRowDateTime(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return DateTime.MinValue;
            else
                return (DateTime)row[colNdx];
        }

        internal string
           GetRowString(
              DataRow row,
              int colNdx
           )
        {
            // Strip \0 out of strings.  It's not useful in event data
            //  and causes problems for .NET display
            if (row.IsNull(colNdx))
                return "";
            else
            {
                string retVal = (string)row[colNdx];

                if (retVal != null)
                    return retVal.Replace("\0", " ");
                else
                    return retVal;
            }
        }

        internal int
           GetRowInt32(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return 0;
            else
                return (int)row[colName];
        }

        internal DateTime
           GetRowDateTime(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return DateTime.MinValue;
            else
                return (DateTime)row[colName];
        }

        internal long
           GetRowInt64(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return 0;
            else
                return (long)row[colName];
        }

        internal int GetRowPermissions(DataRow row, int col)
        {
            int perm;
            if (row[col].GetType() == Type.GetType("System.Int64"))
            {
                var rowPermission = GetRowInt64(row, col);
                switch (rowPermission)
                {
                    case 68719476736:
                        perm = -999;
                        break;

                    case 8589934592:
                        perm = -998;
                        break;

                    default:
                        perm = (int)rowPermission;
                        break;
                }

            }
            else
            {
                perm = GetRowInt32(row, col);
            }
            return perm;

        }

        #region Copy Routines

        //-----------------------------------------------------------------------
        // CopyToRow
        //-----------------------------------------------------------------------
        internal void
           CopyToRow(
              DataRow row
           )
        {
            CopyToRow(row, 0);
        }

        internal void
           CopyToRow(
              DataRow row,
              int startColumn
           )
        {
            row[TraceJob.ndxEventClass] = eventClass;
            row[TraceJob.ndxEventSubclass] = eventSubclass;
            row[TraceJob.ndxStartTime] = startTime;
            row[TraceJob.ndxSpid] = spid;
            row[TraceJob.ndxApplicationName] = applicationName;
            row[TraceJob.ndxHostName] = hostName;
            row[TraceJob.ndxServerName] = serverName;
            row[TraceJob.ndxLoginName] = loginName;
            row[TraceJob.ndxSuccess] = success;
            row[TraceJob.ndxDatabaseName] = databaseName;
            row[TraceJob.ndxDatabaseId] = databaseId;
            row[TraceJob.ndxDbUserName] = dbUserName;
            row[TraceJob.ndxObjectType] = objectType;
            row[TraceJob.ndxObjectName] = objectName;
            row[TraceJob.ndxObjectId] = objectId;
            row[TraceJob.ndxPermissions] = permissions;
            row[TraceJob.ndxColumnPermissions] = columnPermissions;
            row[TraceJob.ndxTargetLoginName] = targetLoginName;
            row[TraceJob.ndxTargetUserName] = targetUserName;
            row[TraceJob.ndxRoleName] = roleName;
            row[TraceJob.ndxOwnerName] = ownerName;
            row[TraceJob.ndxTextData] = textData;
            row[TraceJob.ndxNestLevel] = nestLevel;

            // SQL Server 2005
            row[TraceJob.ndxFileName] = fileName;
            row[TraceJob.ndxLinkedServerName] = linkedServerName;
            row[TraceJob.ndxParentName] = parentName;
            row[TraceJob.ndxIsSystem] = isSystem;
            row[TraceJob.ndxSessionLoginName] = sessionLoginName;
            row[TraceJob.ndxProviderName] = providerName;

            // V3.1
            row[TraceJob.ndxEventSequence] = eventSequence;
            row[TraceJob.ndxStartSequence] = startSequence;
            row[TraceJob.ndxEndSequence] = endSequence;
            row[TraceJob.ndxEndTime] = endTime;

            // V5.5
            if (rowCounts == null)
            {
                row[TraceJob.ndxRowCounts] = DBNull.Value;
            }
            else
            {
                row[TraceJob.ndxRowCounts] = rowCounts;
            }
            if (row.Table.Columns.Count > TraceJob.ndxGUId)
            {
                if (guid == null)
                {
                    row[TraceJob.ndxGUId] = DBNull.Value;
                }
                else
                {
                    row[TraceJob.ndxGUId] = guid;
                }
            }
        }

        internal void
           CopyAll(
              TraceRow sourceRow
           )
        {
            CopyAll(sourceRow, true);
        }

        internal void
           CopyAll(
              TraceRow sourceRow,
              bool copySql
           )
        {
            eventClass = sourceRow.eventClass;
            eventSubclass = sourceRow.eventSubclass;
            startTime = sourceRow.startTime;
            spid = sourceRow.spid;
            applicationName = sourceRow.applicationName;
            hostName = sourceRow.hostName;
            loginName = sourceRow.loginName;
            success = sourceRow.success;
            databaseName = sourceRow.databaseName;
            databaseId = sourceRow.databaseId;
            dbUserName = sourceRow.dbUserName;
            objectType = sourceRow.objectType;
            objectName = sourceRow.objectName;
            objectId = sourceRow.objectId;
            permissions = sourceRow.permissions;
            columnPermissions = sourceRow.columnPermissions;
            targetLoginName = sourceRow.targetLoginName;
            targetUserName = sourceRow.targetUserName;
            roleName = sourceRow.roleName;
            ownerName = sourceRow.ownerName;
            if (copySql)
            {
                textData = sourceRow.textData;
            }
            nestLevel = sourceRow.nestLevel;

            // 2005 columns         
            fileName = sourceRow.fileName;
            linkedServerName = sourceRow.linkedServerName;
            parentName = sourceRow.parentName;
            isSystem = sourceRow.isSystem;
            sessionLoginName = sourceRow.sessionLoginName;
            providerName = sourceRow.providerName;
            eventSequence = sourceRow.eventSequence;

            startSequence = sourceRow.startSequence;
            endSequence = sourceRow.endSequence;
            endTime = sourceRow.endTime;

            //V5.5
            rowCounts = sourceRow.rowCounts;
        }

        internal void
           CopySQL(
              TraceRow sourceRow
           )
        {
            textData = sourceRow.textData;
        }

        internal void
           CopyDatabaseInfo(
              TraceRow sourceRow
           )
        {
            databaseName = sourceRow.databaseName;
            databaseId = sourceRow.databaseId;
        }

        internal void
           CopyObjectInfo(
              TraceRow sourceRow
           )
        {
            objectType = sourceRow.objectType;
            objectName = sourceRow.objectName;
            objectId = sourceRow.objectId;
        }

        internal void
           CopyTime(
              TraceRow sourceRow
           )
        {
            startTime = sourceRow.startTime;
        }

        internal void
           CopyAllToTraceRow(
              TraceRow targetRow,
              bool copySql
           )
        {
            targetRow.eventClass = eventClass;
            targetRow.eventSubclass = eventSubclass;
            targetRow.startTime = startTime;
            targetRow.spid = spid;
            targetRow.applicationName = applicationName;
            targetRow.hostName = hostName;
            targetRow.loginName = loginName;
            targetRow.success = success;
            targetRow.databaseName = databaseName;
            targetRow.databaseId = databaseId;
            targetRow.dbUserName = dbUserName;
            targetRow.objectType = objectType;
            targetRow.objectName = objectName;
            targetRow.objectId = objectId;
            targetRow.permissions = permissions;
            targetRow.columnPermissions = columnPermissions;
            targetRow.targetLoginName = targetLoginName;
            targetRow.targetUserName = targetUserName;
            targetRow.roleName = roleName;
            targetRow.ownerName = ownerName;
            if (copySql)
            {
                targetRow.textData = textData;
            }
            targetRow.nestLevel = nestLevel;

            // 2005 columns         
            targetRow.fileName = fileName;
            targetRow.linkedServerName = linkedServerName;
            targetRow.parentName = parentName;
            targetRow.isSystem = isSystem;
            targetRow.sessionLoginName = sessionLoginName;
            targetRow.providerName = providerName;
            targetRow.eventSequence = eventSequence;
            targetRow.startSequence = startSequence;
            targetRow.endSequence = endSequence;
            targetRow.endTime = endTime;
            targetRow.rowCounts = rowCounts;
        }

        //------------------------------------------------------------------------
        // Clear
        //------------------------------------------------------------------------
        internal void
           Clear()
        {
            eventClass = 0;
            eventSubclass = 0;
            startTime = DateTime.MinValue;
            spid = 0;
            applicationName = "";
            hostName = "";
            serverName = "";
            loginName = "";
            success = 0;
            databaseName = "";
            databaseId = 0;
            dbUserName = "";
            objectType = 0;
            objectName = "";
            objectId = 0;
            permissions = 0;
            columnPermissions = 0;
            targetLoginName = "";
            targetUserName = "";
            roleName = "";
            ownerName = "";
            textData = "";
            nestLevel = 0;

            // 2005 columns         
            fileName = "";
            linkedServerName = "";
            parentName = "";
            isSystem = 0;
            sessionLoginName = "";
            providerName = "";
            eventSequence = -1;
            startSequence = -1;
            endSequence = -1;
            endTime = CoreConstants.InvalidDateTimeValue;
        }

        #endregion

        #region SQL

        //-----------------------------------------------------------------------
        // GetSelectSQL - Select for walking temp table
        //-----------------------------------------------------------------------
        internal string
           GetSelectSQL(
              string eventsTable,
              bool is2005,
              bool supportBeforeAfterData
           )
        {
            string loadTemp = "SELECT {0} FROM [{1}];";

            return String.Format(loadTemp,
                                  GetColumnsSQL(is2005, supportBeforeAfterData),
                                  eventsTable);
        }

        //-----------------------------------------------------------------------
        // GetColumnsSQL - Select for walking temp table
        //-----------------------------------------------------------------------
        static internal string
           GetColumnsSQL(
              bool is2005,
              bool supportBeforeAfterData
           )
        {
            string SQL = "EventClass," +
                         "EventSubClass," +
                         "StartTime," +
                         "SPID," +
                         "ApplicationName," +
                         "HostName," +
                         "ServerName," +
                         "LoginName," +
                         "Success," +
                         "DatabaseName," +
                         "DatabaseID," +
                         "DBUserName," +
                         "ObjectType," +
                         "ObjectName," +
                         "ObjectID," +
                         "Permissions," +
                         "ColumnPermissions," +
                         "TargetLoginName," +
                         "TargetUserName," +
                         "RoleName," +
                         "OwnerName," +
                         "TextData," +
                         "NestLevel";

            if (is2005)
            {
                SQL += ",FileName" +
                         ",LinkedServerName" +
                         ",ParentName" +
                         ",IsSystem" +
                         ",SessionLoginName" +
                         ",ProviderName";
                if (supportBeforeAfterData)
                    SQL += ",eventSequence" +
                           ",RowCounts";
            }

            return SQL;
        }

        //-----------------------------------------------------------------------
        // GetInsertPropsSQL
        //-----------------------------------------------------------------------
        internal string
           GetInsertPropsSQL()
        {
            return TraceRow.GetColumnsSQL(true /* always get 2005 columns */,
                                           true /* always get before/after data columns */);
        }

        //-----------------------------------------------------------------------
        // GetInsertValuesSQL
        //-----------------------------------------------------------------------
        internal string
           GetInsertValuesSQL()
        {
            StringBuilder insertSql = new StringBuilder();

            insertSql.AppendFormat("{0}", eventClass);
            insertSql.AppendFormat(",{0}", eventSubclass);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeDateTimeString(startTime));
            insertSql.AppendFormat(",{0}", spid);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(applicationName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(hostName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(serverName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(loginName));
            insertSql.AppendFormat(",{0}", success);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(databaseName));
            insertSql.AppendFormat(",{0}", databaseId);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(dbUserName));
            insertSql.AppendFormat(",{0}", objectType);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(objectName));
            insertSql.AppendFormat(",{0}", objectId);
            insertSql.AppendFormat(",{0}", permissions);
            insertSql.AppendFormat(",{0}", columnPermissions);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(targetLoginName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(targetUserName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(roleName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(ownerName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(textData));
            insertSql.AppendFormat(",{0}", nestLevel);

            // 2005 columns         
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(fileName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(linkedServerName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(parentName));
            insertSql.AppendFormat(",{0}", isSystem);
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(sessionLoginName));
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(providerName));

            // new since 3.1
            insertSql.AppendFormat(",{0}", eventSequence);
            //V5.5
            if (rowCounts.HasValue)
                insertSql.AppendFormat(",{0}", rowCounts);
            else
                insertSql.AppendFormat(",{0}", "NULL");
            insertSql.AppendFormat(",{0}", SQLHelpers.CreateSafeString(guid));
            return insertSql.ToString();
        }


        //-----------------------------------------------------------------------
        // GetUpdateColumnsSQL
        //-----------------------------------------------------------------------
        internal string
           GetUpdateColumnsSQL()
        {
            StringBuilder updateSql = new StringBuilder();

            updateSql.AppendFormat("EventClass={0}", eventClass);
            updateSql.AppendFormat(",EventSubClass={0}", eventSubclass);
            updateSql.AppendFormat(",StartTime={0}", SQLHelpers.CreateSafeDateTimeString(startTime));
            updateSql.AppendFormat(",SPID={0}", spid);
            updateSql.AppendFormat(",ApplicationName={0}", SQLHelpers.CreateSafeString(applicationName));
            updateSql.AppendFormat(",HostName={0}", SQLHelpers.CreateSafeString(hostName));
            updateSql.AppendFormat(",ServerName={0}", SQLHelpers.CreateSafeString(serverName));
            updateSql.AppendFormat(",LoginName={0}", SQLHelpers.CreateSafeString(loginName));
            updateSql.AppendFormat(",Success={0}", success);
            updateSql.AppendFormat(",DatabaseName={0}", SQLHelpers.CreateSafeString(databaseName));
            updateSql.AppendFormat(",DatabaseID={0}", databaseId);
            updateSql.AppendFormat(",DBUserName={0}", SQLHelpers.CreateSafeString(dbUserName));
            updateSql.AppendFormat(",ObjectType={0}", objectType);
            updateSql.AppendFormat(",ObjectName={0}", SQLHelpers.CreateSafeString(objectName));
            updateSql.AppendFormat(",ObjectID={0}", objectId);
            updateSql.AppendFormat(",Permissions={0}", permissions);
            updateSql.AppendFormat(",ColumnPermissions={0}", columnPermissions);
            updateSql.AppendFormat(",TargetLoginName={0}", SQLHelpers.CreateSafeString(targetLoginName));
            updateSql.AppendFormat(",TargetUserName={0}", SQLHelpers.CreateSafeString(targetUserName));
            updateSql.AppendFormat(",RoleName={0}", SQLHelpers.CreateSafeString(roleName));
            updateSql.AppendFormat(",OwnerName={0}", SQLHelpers.CreateSafeString(ownerName));
            updateSql.AppendFormat(",TextData={0}", SQLHelpers.CreateSafeString(textData));
            updateSql.AppendFormat(",NestLevel={0}", nestLevel);

            // 2005 columns         
            updateSql.AppendFormat(",FileName={0}", SQLHelpers.CreateSafeString(fileName));
            updateSql.AppendFormat(",LinkedServerName={0}", SQLHelpers.CreateSafeString(linkedServerName));
            updateSql.AppendFormat(",ParentName={0}", SQLHelpers.CreateSafeString(parentName));
            updateSql.AppendFormat(",IsSystem={0}", isSystem);
            updateSql.AppendFormat(",SessionLoginName={0}", SQLHelpers.CreateSafeString(sessionLoginName));
            updateSql.AppendFormat(",ProviderName={0}", SQLHelpers.CreateSafeString(providerName));
            updateSql.AppendFormat(",eventSequence={0}", eventSequence);
            if (rowCounts.HasValue)
            {
                updateSql.AppendFormat(",RowCounts={0}", rowCounts);
            }
            else { 
                updateSql.AppendFormat(",RowCounts={0}", "NULL"); 
            }

            return updateSql.ToString();
        }


        #endregion

        # region Sensitive Column

        /// <summary>
        /// Process Senesitive Columns
        /// </summary>
        /// <param name="row"></param>
        /// <param name="tableConfigs"></param>
        /// <param name="parser"></param>
        /// <param name="accessedColumns"></param>
        /// <returns></returns>
        protected bool ProcessSensitiveColumns(DataRow row, List<ColumnTableConfiguration> tableConfigs, ColumnParser parser, out List<string> accessedColumns)
        {
            int indexIndividual = tableConfigs.FindIndex(x => x.Type.ToUpper() == "INDIVIDUAL"
                    && x.Name.ToLower().Equals(String.Format("{0}.{1}",GetRowString(row, TraceJob.ndxParentName).ToLower(), GetRowString(row, TraceJob.ndxObjectName).ToLower())));
            ColumnList columnList = null;
            accessedColumns = new List<string>();
            if (indexIndividual > -1)
            {
                ColumnTableConfiguration config = tableConfigs[indexIndividual];
                tableConfigs.RemoveAt(indexIndividual);
                if (!CreateColumnSet(row,
                                objectName,
                                true,
                                parser,
                                out columnList))
                {
                    return false;
                }
                else
                {
                    if (columnList.AllColumns)
                    {
                        if (!String.IsNullOrEmpty(config.Columns[0]))
                        {
                            accessedColumns.AddRange(config.Columns);
                        }
                        else
                        {
                            accessedColumns.Add("All Columns");
                            return true;
                        }

                    }
                    else
                    {
                        //We are auditing all the columns, so add all that were accessed
                        if (String.IsNullOrEmpty(config.Columns[0]))
                        {
                            accessedColumns = columnList.Columns;
                            return true;
                        }
                        else
                        {
                            //we are only auding a few column and we only accessed a few of them
                            //tableConfig.Columns is the list of audited columns from the bin file
                            //columnList.Columns is the list of columns from the query
                            foreach (string auditedColumn in config.Columns)
                            {
                                foreach (string selectedColumn in columnList.Columns)
                                {
                                    if (selectedColumn.ToUpper() == auditedColumn.ToUpper())
                                    {
                                        accessedColumns.Add(auditedColumn);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (ColumnTableConfiguration config in tableConfigs)
            {
                ColumnList columnListTemp = null;
                bool isValid = true;
                string[] tablesFromConfig = config.Name.Split(',');
                int intersectCount = 0;
                foreach (string tableFromConfig in tablesFromConfig)
                {
                    foreach (string tableWithSchema in parser.TableListWithSchema)
                    {
                        if(tableFromConfig.ToUpper() == tableWithSchema.ToUpper()){
                            intersectCount++;
                            break;
                        }
                    }
                }

                if (intersectCount != tablesFromConfig.Length)
                {
                    return true;
                }
                foreach (string tableName in config.tableColumnMap.Keys)
                {

                    if (tableName == objectName && columnList == null)
                    {
                        CreateColumnSet(row,
                            tableName,
                            false,
                            parser,
                            out columnList);
                        columnListTemp = columnList;
                    }
                    else if (tableName != objectName)
                    {
                        CreateColumnSet(row,
                            tableName,
                            false,
                            parser,
                            out columnListTemp);
                    }
                    else
                    {
                        columnListTemp = columnList;
                    }
                    if (columnListTemp == null)
                    {
                        isValid = false;
                        break;
                    }
                    foreach (string col in config.tableColumnMap[tableName])
                    {
                        if (!columnListTemp.AllColumns && columnListTemp.Columns.FindIndex(x => x.ToUpper() == col.Split('.')[col.Split('.').Length - 1].ToUpper()) < 0)
                        {
                            isValid = false;
                            break;
                        }
                        else if (columnListTemp.AllColumns)
                            break;
                    }
                    if (!isValid)
                        break;
                }
                if (isValid)
                    accessedColumns.AddRange(config.tableColumnMap[objectName].Select(x => x.Split('.')[x.Split('.').Length - 1]));
            }
            accessedColumns = new List<string>((new HashSet<string>(accessedColumns)));
            return true;
        }

        private bool CreateColumnSet(DataRow row,
                                    string tableName,
                                    bool IsIndividual,
                                    ColumnParser parser,
                                    out ColumnList columnList)
        {
            columnList = null;
            //only get columns for the table listed in the event.
            if (!parser.FindNoCase(tableName, out columnList))
            {
                //If it is a sensitive column only trace, quit.  Otherwise, it is 
                // sensitive column and select so there is no parsing to do.  Just 
                //Create the calcutated columns and quit
                return false;
            }
            return true;
        }

        public Dictionary<int, List<ColumnTableConfiguration>> GetDictAuditedColumns(string instanceName)
        {
            Dictionary<int, List<ColumnTableConfiguration>> tableConfig = new Dictionary<int, List<ColumnTableConfiguration>>();
            List<ColumnTableConfiguration> columnTablesConfig = null;
            List<SensitiveColumnsTableRecord> colList = new List<SensitiveColumnsTableRecord>();

            string stmt = String.Format(@"SELECT sc.srvId, db.sqlDatabaseId, st.objectId, sc.name, sc.columnId, sc.type, st.tableName, st.schemaName FROM SensitiveColumnTables st JOIN Databases db on (db.dbId = st.dbId AND db.srvInstance = '{2}') JOIN SensitiveColumnColumns sc
                                          ON st.objectId = sc.objectId and st.dbId = sc.dbId union (SELECT st.srvId,db.sqlDatabaseId,st.objectId,null,null,'Individual',st.tableName,st.schemaName from SensitiveColumnTables as st JOIN Databases db on (db.dbId = st.dbId AND db.srvInstance = '{2}') where selectedColumns = 0) ORDER BY objectId ASC",
                                         CoreConstants.RepositorySensitiveColumnColumnsTable,
                                         CoreConstants.RepositorySensitiveColumnTablesTable,
                                         instanceName);

            Repository readRepository = new Repository();
            try
            {
                readRepository.OpenConnection(CoreConstants.RepositoryDatabase);
                using (SqlCommand cmd = new SqlCommand(stmt, readRepository.connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SensitiveColumnsTableRecord col = new SensitiveColumnsTableRecord();
                            col.Load(reader);
                            //Get all columns list
                            colList.Add(col);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                readRepository.CloseConnection();
            }

            // Group by ColId on the columns list
            var groupedList = colList.FindAll(x => x.Type != null && x.Type != CoreConstants.IndividualColumnType)
                      .GroupBy(u => u.ColumnId)
                      .Select(grp => grp.ToList())
                      .ToList();
            groupedList.AddRange(colList.FindAll(x => x.Type == null || x.Type
                      == CoreConstants.IndividualColumnType).GroupBy(u => u.ObjectId)
                      .Select(grp => grp.ToList()).ToList());
            //Iterate all the groups and assign respective columns to their tables to create records
            foreach (var group in groupedList)
            {
                ColumnTableConfiguration colConfig = new ColumnTableConfiguration();
                List<string> tblNameOfColumn = new List<string>();
                List<string> columns = new List<string>();

                foreach (var user in group)
                {
                    colConfig.Type = user.Type;
                    colConfig.DbId = user.DbId;
                    colConfig.SrvId = user.SrvId;
                    colConfig.Schema = user.SchemaName;
                    tblNameOfColumn.Add(String.Format("{0}.{1}",user.SchemaName, user.TableName));
                    columns.Add(user.Name);
                    if (colConfig.tableColumnMap.ContainsKey(user.TableName))
                    {
                        colConfig.tableColumnMap[user.TableName].Add(user.Name);
                    }
                    else
                    {
                        colConfig.tableColumnMap.Add(user.TableName, new List<string>() { user.Name });
                    }
                }


                colConfig.Columns = columns.ToArray();
                //Get unique table names of columns to create an Appended string which will be the new Table name
                var unique_items = new HashSet<string>(tblNameOfColumn);

                colConfig.Name = string.Join(",", unique_items);//builder.ToString();

                //Add the records
                //columnTablesConfig[colConfig.FullTableName] = colConfig;
                if (tableConfig.ContainsKey(colConfig.DbId))
                {
                    tableConfig[colConfig.DbId].Add(colConfig);
                }
                else
                {
                    columnTablesConfig = new List<ColumnTableConfiguration>();
                    columnTablesConfig.Add(colConfig);
                    tableConfig[colConfig.DbId] = columnTablesConfig;
                }
            }

            return tableConfig;
        }

        #endregion

        public class ColumnTableConfiguration
        {
            #region Fields
            private static int srvId;
            private int dbId;
            private string schema;
            private string name;
            private string[] columns;
            private string type;
            public Dictionary<string, List<string>> tableColumnMap = new Dictionary<string, List<string>>();
            #endregion
            #region Properties
            public int SrvId
            {
                get { return srvId; }
                set { srvId = value; }
            }
            public int DbId
            {
                get { return dbId; }
                set { dbId = value; }
            }
            public string Schema
            {
                get { return schema; }
                set { schema = value; }
            }
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public string[] Columns
            {
                get { return columns; }
                set { columns = value; }
            }
            public string Type
            {
                get { return type; }
                set { type = value; }
            }
            #endregion
        }

        public class SensitiveColumnsTableRecord
        {
            #region Fields
            private int _srvId;
            private int _dbId;
            private int _objectId;
            private string _name;
            private string _type;
            private int _columnId;
            private string _tableName;
            private string _schemaName;
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
            public string TableName
            {
                get
                {
                    return _tableName;
                }
                set
                {
                    _tableName = value;
                }
            }
            public string SchemaName
            {
                get
                {
                    return _schemaName;
                }
                set
                {
                    _schemaName = value;
                }
            }
            #endregion

            #region Public Methods
            public void Load(SqlDataReader reader)
            {
                if (!reader.IsDBNull(0))
                    _srvId = reader.GetInt32(0);
                else
                    _srvId = -1;
                if (!reader.IsDBNull(1))
                    _dbId = reader.GetInt16(1);
                else
                    _dbId = -1;
                if (!reader.IsDBNull(2))
                    _objectId = reader.GetInt32(2);
                else
                    _objectId = -1;
                if (!reader.IsDBNull(3))
                    _name = reader.GetString(3);
                if (!reader.IsDBNull(4))
                    _columnId = reader.GetInt32(4);
                else
                    _columnId = -1;
                if (!reader.IsDBNull(5))
                    _type = reader.GetString(5);
                else
                    _type = CoreConstants.IndividualColumnType;
                if (!reader.IsDBNull(6))
                    _tableName = reader.GetString(6);
                if (!reader.IsDBNull(7))
                    _schemaName = reader.GetString(7);
            }
            #endregion
        }
    }      
}
