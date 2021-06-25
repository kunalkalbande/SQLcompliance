using System;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class EventRow
    {
        private int _categoryId;
        private int _eventTypeId;
        private DateTime _startTime;
        private string _loginName;
        private string _databaseName;
        private string _targetObjectName;
        private string _details;
        private int _eventId;
        private int _spid;
        private string _applicationName;
        private string _hostName;
        private string _serverName;
        private bool _success;
        private string _dbUserName;
        private string _objectName;
        private string _targetLoginName;
        private string _targetUserName;
        private string _roleName;
        private string _ownerName;
        private bool _privilegedUser;
        private string _sessionLoginName;
        private long _startSequence;
        private long _endSequence;
        private DateTime _endTime;
        private string _parentName;
        private bool _sensitiveColumns;
        private string _guid;

        public string Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        public int EventTypeId
        {
            get { return _eventTypeId; }
            set { _eventTypeId = value; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public string TargetObjectName
        {
            get { return _targetObjectName; }
            set { _targetObjectName = value; }
        }

        public string Details
        {
            get { return _details; }
            set { _details = value; }
        }

        public int EventId
        {
            get { return _eventId; }
            set { _eventId = value; }
        }

        public int Spid
        {
            get { return _spid; }
            set { _spid = value; }
        }

        public string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        public string DbUserName
        {
            get { return _dbUserName; }
            set { _dbUserName = value; }
        }

        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value; }
        }

        public string TargetLoginName
        {
            get { return _targetLoginName; }
            set { _targetLoginName = value; }
        }

        public string TargetUserName
        {
            get { return _targetUserName; }
            set { _targetUserName = value; }
        }

        public string RoleName
        {
            get { return _roleName; }
            set { _roleName = value; }
        }

        public string OwnerName
        {
            get { return _ownerName; }
            set { _ownerName = value; }
        }

        public bool PrivilegedUser
        {
            get { return _privilegedUser; }
            set { _privilegedUser = value; }
        }

        public string SessionLoginName
        {
            get { return _sessionLoginName; }
            set { _sessionLoginName = value; }
        }


        public long StartSequence
        {
            get { return _startSequence; }
            set { _startSequence = value; }
        }

        public long EndSequence
        {
            get { return _endSequence; }
            set { _endSequence = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public string ParentName
        {
            get { return _parentName; }
            set { _parentName = value; }
        }

        public bool HasSensitiveColumns
        {
            get { return _sensitiveColumns; }
            set { _sensitiveColumns = value; }
        }

        public bool HasBeforeAfterData
        {
            get
            {
                if (_endSequence == -1 ||
                   _startSequence + 1 == _endSequence ||
                   _startSequence == _endSequence)
                    return false;
                else
                    return true;
            }
        }

        public static EventRow ReadRow(SqlDataReader reader)
        {
            int i = 0;
            return ReadRow(reader, ref i);
        }

        public static EventRow ReadRow(SqlDataReader reader, ref int readerColumn)
        {
            EventRow row = new EventRow();
            row.CategoryId = SQLHelpers.GetInt32(reader, readerColumn++);
            row.EventTypeId = SQLHelpers.GetInt32(reader, readerColumn++);
            row.StartTime = SQLHelpers.GetDateTime(reader, readerColumn++);
            row.LoginName = SQLHelpers.GetString(reader, readerColumn++);
            row.DatabaseName = SQLHelpers.GetString(reader, readerColumn++);
            row.TargetObjectName = SQLHelpers.GetString(reader, readerColumn++);
            row.Details = SQLHelpers.GetString(reader, readerColumn++);
            row.EventId = SQLHelpers.GetInt32(reader, readerColumn++);
            row.Spid = SQLHelpers.GetInt32(reader, readerColumn++);
            row.ApplicationName = SQLHelpers.GetString(reader, readerColumn++);
            row.HostName = SQLHelpers.GetString(reader, readerColumn++);
            row.ServerName = SQLHelpers.GetString(reader, readerColumn++);
            row.Success = SQLHelpers.GetInt32(reader, readerColumn++) == 1 ? true : false;
            row.DbUserName = SQLHelpers.GetString(reader, readerColumn++);
            row.ObjectName = SQLHelpers.GetString(reader, readerColumn++);
            row.TargetLoginName = SQLHelpers.GetString(reader, readerColumn++);
            row.TargetUserName = SQLHelpers.GetString(reader, readerColumn++);
            row.RoleName = SQLHelpers.GetString(reader, readerColumn++);
            row.OwnerName = SQLHelpers.GetString(reader, readerColumn++);
            row.PrivilegedUser = SQLHelpers.GetInt32(reader, readerColumn++) == 1 ? true : false;
            row.SessionLoginName = SQLHelpers.GetString(reader, readerColumn++);
            row.StartSequence = SQLHelpers.GetLong(reader, readerColumn++, -1);
            row.EndSequence = SQLHelpers.GetLong(reader, readerColumn++, -1);
            row.EndTime = SQLHelpers.GetDateTime(reader, readerColumn++);
            row.ParentName = SQLHelpers.GetString(reader, readerColumn++);
            row.HasSensitiveColumns = ((int)SQLHelpers.GetInt32(reader, readerColumn++) > 0);
            row.Guid = (string)SQLHelpers.GetString(reader, "guid");


            return row;
        }
    }

    public class DataChangeRow
    {
        private DateTime _startTime;
        private long _sequenceNumber;
        private int _spid;
        private int _databaseId;
        private int _actionType;
        private string _tableName;
        private int _recordNumber;
        private string _userName;
        private int _changedColumns;
        private string _primaryKey;
        private int _hashcode;
        private int _totalChanges;

        public DataChangeRow()
        {
        }

        public DataChangeRow(DataChangeRecord record)
        {
            _startTime = record.startTime;
            _sequenceNumber = record.eventSequence;
            _spid = record.spid;
            _databaseId = record.databaseId;
            _actionType = record.actionType;
            _tableName = record.tableName;
            _recordNumber = record.recordNumber;
            _userName = record.user;
            _changedColumns = record.changedColumns;
            _primaryKey = record.primaryKey;
            _hashcode = record.hashcode;
            _totalChanges = record.totalChanges;
        }


        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public long SequenceNumber
        {
            get { return _sequenceNumber; }
            set { _sequenceNumber = value; }
        }

        public int Spid
        {
            get { return _spid; }
            set { _spid = value; }
        }

        public int DatabaseId
        {
            get { return _databaseId; }
            set { _databaseId = value; }
        }

        public int ActionType
        {
            get { return _actionType; }
            set { _actionType = value; }
        }

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public int RecordNumber
        {
            get { return _recordNumber; }
            set { _recordNumber = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public int ChangedColumns
        {
            get { return _changedColumns; }
            set { _changedColumns = value; }
        }

        public string PrimaryKey
        {
            get { return _primaryKey; }
            set { _primaryKey = value; }
        }

        public int Hashcode
        {
            get { return _hashcode; }
            set { _hashcode = value; }
        }

        public int TotalChanges
        {
            get { return _totalChanges; }
            set { _totalChanges = value; }
        }

        public static DataChangeRow ReadRow(SqlDataReader reader)
        {
            int i = 0;
            return ReadRow(reader, ref i);
        }

        public static DataChangeRow ReadRow(SqlDataReader reader, ref int readerColumn)
        {
            DataChangeRow row = new DataChangeRow();
            row.StartTime = SQLHelpers.GetDateTime(reader, readerColumn++);
            row.SequenceNumber = SQLHelpers.GetLong(reader, readerColumn++);
            row.Spid = SQLHelpers.GetInt32(reader, readerColumn++);
            row.DatabaseId = SQLHelpers.GetInt32(reader, readerColumn++);
            row.ActionType = SQLHelpers.GetInt32(reader, readerColumn++);
            row.TableName = SQLHelpers.GetString(reader, readerColumn++);
            row.RecordNumber = SQLHelpers.GetInt32(reader, readerColumn++);
            row.UserName = SQLHelpers.GetString(reader, readerColumn++);
            row.ChangedColumns = SQLHelpers.GetInt32(reader, readerColumn++);
            row.PrimaryKey = SQLHelpers.GetString(reader, readerColumn++);
            row.Hashcode = SQLHelpers.GetInt32(reader, readerColumn++);

            if (reader.IsDBNull(readerColumn))
            {
                row.TotalChanges = row.ChangedColumns;
                readerColumn++;
            }
            else
                row.TotalChanges = SQLHelpers.GetInt32(reader, readerColumn++);
            return row;
        }
    }

    public class ChangeColumnRow
    {
        private DateTime _startTime;
        private long _sequenceNumber;
        private int _spid;
        private string _columnName;
        private string _beforeValue;
        private string _afterValue;
        private int _hashcode;

        public ChangeColumnRow()
        {
        }

        public ChangeColumnRow(ColumnChangeRecord record)
        {
            _startTime = record.startTime;
            _sequenceNumber = record.eventSequence;
            _spid = record.spid;
            _columnName = record.columnName;
            _beforeValue = record.beforeValue;
            _afterValue = record.afterValue;
            _hashcode = record.hashcode;
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public long SequenceNumber
        {
            get { return _sequenceNumber; }
            set { _sequenceNumber = value; }
        }

        public int Spid
        {
            get { return _spid; }
            set { _spid = value; }
        }

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        public string BeforeValue
        {
            get { return _beforeValue; }
            set { _beforeValue = value; }
        }

        public string AfterValue
        {
            get { return _afterValue; }
            set { _afterValue = value; }
        }

        public int Hashcode
        {
            get { return _hashcode; }
            set { _hashcode = value; }
        }

        public static ChangeColumnRow ReadRow(SqlDataReader reader)
        {
            int i = 0;
            return ReadRow(reader, ref i);
        }

        public static ChangeColumnRow ReadRow(SqlDataReader reader, ref int readerColumn)
        {
            ChangeColumnRow row = new ChangeColumnRow();
            row.StartTime = SQLHelpers.GetDateTime(reader, readerColumn++);
            row.SequenceNumber = SQLHelpers.GetLong(reader, readerColumn++);
            row.Spid = SQLHelpers.GetInt32(reader, readerColumn++);
            row.ColumnName = SQLHelpers.GetString(reader, readerColumn++);
            row.BeforeValue = SQLHelpers.GetString(reader, readerColumn++);
            row.AfterValue = SQLHelpers.GetString(reader, readerColumn++);
            row.Hashcode = SQLHelpers.GetInt32(reader, readerColumn++);

            return row;
        }
    }

    public class FlatEventRow
    {
        public EventRow EventData;
        public DataChangeRow RowData;
        public ChangeColumnRow ColumnData;

        public static FlatEventRow ReadRow(SqlDataReader reader)
        {
            int i = 0;
            FlatEventRow row = new FlatEventRow();
            row.EventData = EventRow.ReadRow(reader, ref i);
            row.RowData = DataChangeRow.ReadRow(reader, ref i);
            row.ColumnData = ChangeColumnRow.ReadRow(reader, ref i);

            return row;
        }
    }
}
