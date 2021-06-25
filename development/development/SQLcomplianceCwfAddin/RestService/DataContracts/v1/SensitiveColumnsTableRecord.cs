using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQLcomplianceCwfAddin.RestService.DataContracts.v1
{
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
                _type = "Individual";
            if (!reader.IsDBNull(6))
                _tableName = reader.GetString(6);
            if (!reader.IsDBNull(7))
                _schemaName = reader.GetString(7);
        }
        #endregion
    }
}
