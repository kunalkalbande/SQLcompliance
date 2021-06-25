using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Idera.SQLcompliance.Core.TSqlParsing
{
    public class DataContainer
    {
        private IList<string> _tables;
        private IList<string> _columns;
        private IList<ParseError> _errors;
        private bool _isTopSelection;
        private bool _isTopPercent;
        private int _topCount;

        public DataContainer()
        {
            _tables = new List<string>();
            _columns = new List<string>();
        }
        public void AddTable(string tableName, string alias)
        {
            if (!_tables.Contains(tableName))
            {
                _tables.Add(tableName);
            }
        }
        public void AddColumn(string columnName)
        {
            if (!_columns.Contains(columnName))
            {
                _columns.Add(columnName);
            }
        }

        public void AddErrors(IList<ParseError> errors)
        {
            _errors = errors;
        }

        public IList<string> GetTables()
        {
            return _tables;
        }

        public IList<string> GetColumns()
        {
            return _columns;
        }

        public IList<ParseError> GetErrors()
        {
            return _errors;
        }
        public int TopCount
        {
            get
            {
                return _topCount;
            }
            set { _topCount = value; }
        }
        public bool IsTopSelection
        {
            get
            {
                return _isTopSelection;
            }
            set { _isTopSelection = value; }
        }
        public bool IsTopPercent
        {
            get
            {
                return _isTopPercent;
            }
            set { _isTopPercent = value; }
        }
    }
}
