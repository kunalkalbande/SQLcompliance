using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLcompliance.Core;
using System.Linq;

namespace Idera.SQLcompliance.Core.TSqlParsing
{
    internal class ColumnList
    {
        private string table;
        private bool allColumns;
        private List<string> columns;

        public List<string> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public bool AllColumns
        {
            get { return allColumns; }
            set { allColumns = value; }
        }

        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        public ColumnList()
        {
            allColumns = false;
        }

        public void AddColumn(string column)
        {
            if (columns == null)
                columns = new List<string>();

             //loop through the list and only add it if it is not found.
             foreach (string col in columns)
             {
                if (col.ToUpper() == column.ToUpper())
                   return;
             }
             columns.Add(column);
        }

        public void ClearColumns()
        {
           if (columns != null)
               columns.Clear();
        }
    }

    internal class ColumnParser
    {
        public Dictionary<string, ColumnList> Columns = new Dictionary<string, ColumnList>();
        public List<string> TableListWithSchema = new List<string>();
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns
        private string _sqlText = string.Empty;
        private string _databaseName = string.Empty;
        private string _serverInstance = string.Empty;
        public string SQLText { get { return _sqlText; } set { _sqlText = value; } }
        public string DatabaseName { get { return _databaseName; } set { _databaseName = value; } }
        public string ServerInstance { get { return _serverInstance; } set { _serverInstance = value; } }
        public ColumnParser()
        {
        }

        public static TSqlScript ParseColumns(string script, out IList<ParseError> errors)
        {
            TSqlScript result;
            using (TextReader reader = new StringReader(script))
            {
                TSqlParser parser = new TSql100Parser(true);
                result = parser.Parse(reader, out errors) as TSqlScript;

                if (errors.Count == 0) 
                    return result;

                /*
                 * SPECIAL CASE
                 * When their are two consecutive SQL statements starting with 'IF' then parse fails when body of first IF is missing \ not captured.
                 * We need to ignore the parse error with identifier TSP0029.
                 * example statement: IF EXISTS(SELECT 1 FROM client WHERE client = @client AND master_client_id IN (1007, 1008))
                 */
                for (int index = errors.Count - 1; index >= 0; index -= 1)
                    if (errors[index].Identifier.Equals("TSP0029", StringComparison.InvariantCultureIgnoreCase) &&
                        script.StartsWith("IF ", StringComparison.InvariantCultureIgnoreCase))
                        errors.RemoveAt(index);
            }

            return result;
        }

        public void GetColumns(TSqlScript scriptParseTree)
        {
            foreach (TSqlBatch batch in scriptParseTree.Batches)
            {
                GetColumns(batch);
            }
        }

        public void GetColumns(TSqlBatch batchParseTree)
        {
            foreach (TSqlStatement stmt in batchParseTree.Statements)
            {
                GetColumns(stmt);
            }
        }

        public void GetColumns(TSqlStatement fragment)
        {
            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is SelectStatement)
                {
                    GetColumns((SelectStatement)frag);
                    //SQLCM-5471 v5.6 - Sensitive columns to handle views
                    string tableName = string.Empty;
                    foreach (string tn in TableListWithSchema)
                    {
                        tableName = tn.Split('.')[1];
                    }
                    bool isObjectView = CoreHelpers.IsObjectView(_databaseName, tableName, _serverInstance);
                    if (isObjectView)
                    {
                        IList<KeyValuePair<string, string>> columnsList = CoreHelpers.GetColumnsUsedInView(_databaseName, tableName, _serverInstance);
                        ColumnList columnList;
                        if (columnsList.Count > 0)
                        {
                            TableListWithSchema.Clear();
                            foreach (KeyValuePair<string, string> column in columnsList)
                            {
                                string tableWithSchema = String.Format("dbo.{0}", column.Key);
                                if (!TableListWithSchema.Contains(tableWithSchema))
                                {
                                    TableListWithSchema.Add(tableWithSchema);
                                }

                                if (FindNoCase(column.Key, out columnList))
                                {
                                    columnList.Table = column.Key;
                                    columnList.AddColumn(column.Value);
                                }
                                else
                                {
                                    columnList = new ColumnList();
                                    columnList.Table = column.Key;
                                    columnList.AddColumn(column.Value);
                                    Columns.Add(column.Key, columnList);
                                }
                            }
                        }                       
                    }
                }
                // SQLCM-5471 v5.6 Add Activity to Senstitive columns
                else
                {
                    SqlStatementsParser parser = new SqlStatementsParser();
                    parser.SqlQueryText = _sqlText;
                    DataContainer dc = parser.GetColumns();
                    IList<string> tables = dc.GetTables();
                    foreach (string table in tables)
                    {
                        string tableWithSchema = String.Format("dbo.{0}", table);
                        if (!TableListWithSchema.Contains(tableWithSchema))
                        {
                            TableListWithSchema.Add(tableWithSchema);
                        }
                    }

                    ColumnList columnList;
                    if (frag is DeleteStatement|| frag is DropTableStatement || frag is DbccStatement || frag is TruncateTableStatement)
                    {
                        foreach (string table in tables)
                        {
                            if (FindNoCase(table, out columnList))
                            {
                                columnList.ClearColumns();
                                columnList.Table = table;
                                columnList.AllColumns = true;
                            }
                            else
                            {
                                columnList = new ColumnList();
                                columnList.Table = table;
                                columnList.AllColumns = true;
                                Columns.Add(table, columnList);
                            }
                        }
                    }
                    else
                    {
                        foreach (string table in tables)
                        {
                            IList<string> columns = dc.GetColumns();
                            if (columns.Count > 0)
                            {
                                foreach (string column in columns)
                                {
                                    if (FindNoCase(table, out columnList))
                                    {
                                        columnList.Table = table;
                                        columnList.AddColumn(column);
                                    }
                                    else
                                    {
                                        columnList = new ColumnList();
                                        columnList.Table = table;
                                        columnList.AddColumn(column);
                                        Columns.Add(table, columnList);
                                    }
                                }
                            }
                            else
                            {
                                if (FindNoCase(table, out columnList))
                                {
                                    columnList.ClearColumns();
                                    columnList.Table = table;
                                    columnList.AllColumns = true;
                                }
                                else
                                {
                                    columnList = new ColumnList();
                                    columnList.Table = table;
                                    columnList.AllColumns = true;
                                    Columns.Add(table, columnList);
                                }
                            }
                        }
                    }
                }
                // SQLCM-5471 v5.6 Add Activity to Senstitive columns -END
            }
        }

        private void GetColumns(SelectStatement select)
        {
           GetColumns(null, select);
        }

        private void GetColumns(Subquery query)
        {
           GetColumns(query, null);
        }

        private void GetColumns(Subquery query, SelectStatement select)
        {
           if (query != null)
           {
              SubquerySpecification spec = query.QueryExpression as SubquerySpecification;
              if (spec != null)
                 GetColumns(spec.SelectElements, TSqlParsingHelpers.GetTableSources(spec.FromClauses));
           }
           else
           {
              if (select.WithCommonTableExpressionsAndXmlNamespaces != null)
              {
                 WithCommonTableExpressionsAndXmlNamespaces ctes = select.WithCommonTableExpressionsAndXmlNamespaces;

                 foreach (CommonTableExpression cte in ctes.CommonTableExpressions)
                 {
                    if (cte.Subquery != null)
                    {
                       GetColumns(cte.Subquery);
                    }
                 }
              }
              else if (select.QueryExpression is BinaryQueryExpression)
              {
                 ProcessBinaryQueryExpression(select.QueryExpression as BinaryQueryExpression);
              }
              else
              {
                 QuerySpecification spec = select.QueryExpression as QuerySpecification;
                 if (spec != null)
                    GetColumns(spec.SelectElements, TSqlParsingHelpers.GetTableSources(select));
              }
           }
        }

        private void GetColumns(IList<TSqlFragment> selectElements, Dictionary<string, TableSource> tableSources)
        {
           foreach (TSqlFragment element in selectElements)
           {
              if (element is SelectColumn)
              {
                 if (((SelectColumn)element).Expression is Subquery)
                 {
                    GetColumns(((SelectColumn)element).Expression as Subquery);
                 }
                 else if (((SelectColumn)element).Expression is ParenthesisExpression)
                 {
                    ParenthesisExpression pe = ((SelectColumn)element).Expression as ParenthesisExpression;

                    if (pe != null)
                    {
                       if (pe.Expression is BinaryExpression)
                       {
                          BinaryExpression be = pe.Expression as BinaryExpression;

                          if (be != null)
                          {
                             Column column;
                             column = be.FirstExpression as Column;
                             ProcessColumn(column, tableSources);

                             column = be.SecondExpression as Column;
                             ProcessColumn(column, tableSources);
                          }
                       }
                       else
                       {
                          if (pe.Expression is Column)
                          {
                             Column column;
                             column = pe.Expression as Column;
                             ProcessColumn(column, tableSources);
                          }
                       }
                    }
                 }
                 else if (((SelectColumn)element).Expression is CaseExpression)
                 {
                    CaseExpression exp = ((SelectColumn)element).Expression as CaseExpression;
                    Column col = null;

                    if (exp.InputExpression == null)
                    {
                       //if the input column on the case literal is blank, we need to get a column name. get it from the first when clause.
                       if (exp.WhenClauses[0].WhenExpression is UnaryExpression)
                          col = (exp.WhenClauses[0].WhenExpression as UnaryExpression).Expression as Column;
                       else if (exp.WhenClauses[0].WhenExpression is BinaryExpression)
                       {
                          BinaryExpression binExp = exp.WhenClauses[0].WhenExpression as BinaryExpression;
                          if (binExp.FirstExpression is Column)
                             col = binExp.FirstExpression as Column;
                          else
                             col = binExp.SecondExpression as Column;
                       }
                    }
                    else
                    {
                       col = exp.InputExpression as Column;
                    }
                    ProcessColumn(col, tableSources);

                 }
                 else if (((SelectColumn)element).Expression is BinaryExpression)
                 {
                    BinaryExpression expression = ((SelectColumn)element).Expression as BinaryExpression;

                    if (expression.FirstExpression != null)
                    {
                       if (expression.FirstExpression is Column)
                          ProcessColumn((expression.FirstExpression as Column), tableSources);
                    }

                    if (expression.SecondExpression != null)
                    {
                       if (expression.SecondExpression is Column)
                          ProcessColumn((expression.SecondExpression as Column), tableSources);
                    }
                 }
                 else if (((SelectColumn)element).Expression is CastCall)
                 {
                    CastCall cast = ((SelectColumn)element).Expression as CastCall;
                    ProcessColumn(cast.Parameter as Column, tableSources);
                 }
                 else if (((SelectColumn)element).Expression is ConvertCall)
                 {
                    ConvertCall convert = ((SelectColumn)element).Expression as ConvertCall;
                    ProcessColumn(convert.Parameter as Column, tableSources);
                 }
                 else if (((SelectColumn)element).Expression is FunctionCall)
                 {
                    FunctionCall func = ((SelectColumn)element).Expression as FunctionCall;

                    for (int i = 0; i < func.Parameters.Count; i++)
                    {
                       ProcessColumn(func.Parameters[i] as Column, tableSources);
                    }
                 }
                 else
                 {
                    ProcessColumn(((SelectColumn)element).Expression as Column, tableSources);
                 }
                 if (!(((SelectColumn)element).Expression is Subquery))
                 {
                     TrySetTableNameWithSchema(tableSources);
                 }
              }
              else if (element is SelectSetVariable)
              {
                 SelectSetVariable x = (SelectSetVariable)element;

                 if (((SelectSetVariable)element).Expression is Subquery)
                 {
                    GetColumns(((SelectSetVariable)element).Expression as Subquery);
                 }
                 else if (((SelectSetVariable)element).Expression is ParenthesisExpression)
                 {
                    ParenthesisExpression pe = ((SelectSetVariable)element).Expression as ParenthesisExpression;

                    if (pe != null)
                    {
                       if (pe.Expression is BinaryExpression)
                       {
                          BinaryExpression be = pe.Expression as BinaryExpression;

                          if (be != null)
                          {
                             Column column;
                             column = be.FirstExpression as Column;
                             ProcessColumn(column, tableSources);

                             column = be.SecondExpression as Column;
                             ProcessColumn(column, tableSources);
                          }
                       }
                       else
                       {
                          if (pe.Expression is Column)
                          {
                             Column column;
                             column = pe.Expression as Column;
                             ProcessColumn(column, tableSources);
                          }
                       }
                    }
                 }
                 else if (((SelectSetVariable)element).Expression is CaseExpression)
                 {
                    CaseExpression exp = ((SelectSetVariable)element).Expression as CaseExpression;
                    Column col = null;

                    if (exp.InputExpression == null)
                    {
                       //if the input column on the case literal is blank, we need to get a column name. get it from the first when clause.
                       if (exp.WhenClauses[0].WhenExpression is UnaryExpression)
                          col = (exp.WhenClauses[0].WhenExpression as UnaryExpression).Expression as Column;
                       else if (exp.WhenClauses[0].WhenExpression is BinaryExpression)
                       {
                          BinaryExpression binExp = exp.WhenClauses[0].WhenExpression as BinaryExpression;
                          if (binExp.FirstExpression is Column)
                             col = binExp.FirstExpression as Column;
                          else
                             col = binExp.SecondExpression as Column;
                       }
                    }
                    else
                    {
                       col = exp.InputExpression as Column;
                    }
                    ProcessColumn(col, tableSources);

                 }
                 else if (((SelectSetVariable)element).Expression is BinaryExpression)
                 {
                    BinaryExpression expression = ((SelectSetVariable)element).Expression as BinaryExpression;

                    if (expression.FirstExpression != null)
                    {
                       if (expression.FirstExpression is Column)
                          ProcessColumn((expression.FirstExpression as Column), tableSources);
                    }

                    if (expression.SecondExpression != null)
                    {
                       if (expression.SecondExpression is Column)
                          ProcessColumn((expression.SecondExpression as Column), tableSources);
                    }
                 }
                 else if (((SelectSetVariable)element).Expression is CastCall)
                 {
                    CastCall cast = ((SelectSetVariable)element).Expression as CastCall;
                    ProcessColumn(cast.Parameter as Column, tableSources);
                 }
                 else if (((SelectSetVariable)element).Expression is ConvertCall)
                 {
                    ConvertCall convert = ((SelectSetVariable)element).Expression as ConvertCall;
                    ProcessColumn(convert.Parameter as Column, tableSources);
                 }
                 else if (((SelectSetVariable)element).Expression is FunctionCall)
                 {
                    FunctionCall func = ((SelectSetVariable)element).Expression as FunctionCall;

                    for (int i = 0; i < func.Parameters.Count; i++)
                    {
                       ProcessColumn(func.Parameters[i] as Column, tableSources);
                    }
                 }
                 else
                 {
                    ProcessColumn(((SelectSetVariable)element).Expression as Column, tableSources);
                 }
                 if (!(((SelectSetVariable)element).Expression is Subquery))
                 {
                     TrySetTableNameWithSchema(tableSources);
                 }
              }
           }
        }

       private void ProcessBinaryQueryExpression(BinaryQueryExpression spec)
       {
          if (spec == null)
             return;

          if (spec.FirstQueryExpression != null)
          {
             if (spec.FirstQueryExpression is BinaryQueryExpression)
             {
                ProcessBinaryQueryExpression(spec.FirstQueryExpression as BinaryQueryExpression);
             }
             else if (spec.FirstQueryExpression is QueryParenthesis)
             {
                QueryParenthesis first = spec.FirstQueryExpression as QueryParenthesis;

                if (first.QueryExpression is BinaryQueryExpression)
                {
                   ProcessBinaryQueryExpression(first.QueryExpression as BinaryQueryExpression);
                }
             }
             else if (spec.FirstQueryExpression is QuerySpecification)
             {
                QuerySpecification first = spec.FirstQueryExpression as QuerySpecification;
                GetColumns(first.SelectElements, TSqlParsingHelpers.GetTableSources(first.FromClauses));
             }
          }

          if (spec.SecondQueryExpression != null)
          {
             if (spec.SecondQueryExpression is BinaryQueryExpression)
             {
                ProcessBinaryQueryExpression(spec.SecondQueryExpression as BinaryQueryExpression);
             }
             else if (spec.FirstQueryExpression is QueryParenthesis)
             {
                QueryParenthesis second = spec.FirstQueryExpression as QueryParenthesis;

                if (second.QueryExpression is BinaryQueryExpression)
                {
                   ProcessBinaryQueryExpression(second.QueryExpression as BinaryQueryExpression);
                }
             }
             else if (spec.SecondQueryExpression is QuerySpecification)
             {
                QuerySpecification second = spec.SecondQueryExpression as QuerySpecification;
                GetColumns(second.SelectElements, TSqlParsingHelpers.GetTableSources(second.FromClauses));
             }
          }
       }

        private void ProcessColumn(Column c, Dictionary<string, TableSource> tableSources)
        {
           string tableName;

           if (c != null)
           {
              //This is for columns that use aliases
              if (c.Identifiers.Count - 2 >= 0)
              {
                 if (c.Identifiers[c.Identifiers.Count - 2] != null)
                 {
                    if (c.Identifiers[c.Identifiers.Count - 2].Value != null)
                    {
                       tableName = TSqlParsingHelpers.TryGetTableName(c.Identifiers[c.Identifiers.Count - 2].Value, tableSources);
                       
                       if (!String.IsNullOrEmpty(tableName))
                          AddColumn(c, tableName);
                       else
                       {
                          //check to see if it from is a derived table.
                          TableSource source;
                          if (tableSources.TryGetValue(c.Identifiers[c.Identifiers.Count - 2].Value.ToLower(), out source))
                          {
                             //parse the contents of the derived table
                             if (source is QueryDerivedTable)
                             {
                                GetColumns(((QueryDerivedTable)source).Subquery);
                             }
                          }
                       }
                    }
                 }
              }
              else
              {
                 tableName = "";
                 //this is for joins that do not use any table aliases
                  if (c.Identifiers.Count == 1)
                  {
                      if (c.Identifiers[c.Identifiers.Count - 1] != null)
                      {
                          if (c.Identifiers[c.Identifiers.Count - 1].Value != null)
                          {
                              tableName = TSqlParsingHelpers.TryGetTableName(c.Identifiers[c.Identifiers.Count - 1].Value, tableSources);
                          }
                      }
                  }
                 List<string> tables = GetTables(tableSources);
                 foreach (string table in tables)
                 {
                     if (string.IsNullOrEmpty(tableName) || tableName.ToUpper() == table.ToUpper())
                     {
                         AddColumn(c, table);
                     }
                 }
              }
           }
        }

        private List<string> GetTables(Dictionary<string, TableSource> sources)
        {
           List<string> tables = new List<string>();

           foreach (TableSource source in sources.Values)
           {
              if (source is VariableTableSource || source is SchemaObjectTableSource)
              {
                 if (source is VariableTableSource)
                    tables.Add(((VariableTableSource)source).Name.Value);
                 else if (source is SchemaObjectTableSource)
                    tables.Add(((SchemaObjectTableSource)source).SchemaObject.BaseIdentifier.Value);
              }
           }
           return tables;
        }

        private void AddColumn(Column c, string tableName)
        {
           if (c.ColumnType == ColumnType.Wildcard)
           {
              if (!String.IsNullOrEmpty(tableName))
              {
                 AddAllColumns(tableName);
              }
           }
           else
           {
              if (!String.IsNullOrEmpty(tableName))
              {
                  if (c.Identifiers[c.Identifiers.Count - 1] != null)
                     AddColumn(tableName, c.Identifiers[c.Identifiers.Count - 1].Value);
              }
           }
        }

        private void AddAllColumns(string table)
        {
            ColumnList columnList;

            if (FindNoCase(table, out columnList))
            {
                columnList.ClearColumns();
                columnList.Table = table;
                columnList.AllColumns = true;
            }
            else
            {
                columnList = new ColumnList();
                columnList.Table = table;
                columnList.AllColumns = true;
                Columns.Add(table, columnList);
            }
        }

        private void AddColumn(string table, string column)
        {
            ColumnList columnList;

            if (FindNoCase(table, out columnList))
            {
                columnList.Table = table;
                columnList.AddColumn(column);
            }
            else
            {
                columnList = new ColumnList();
                columnList.Table = table;
                columnList.AddColumn(column);
                Columns.Add(table, columnList);
            }
        }

       public bool FindNoCase(string table, out ColumnList columnList)
       {
          foreach (KeyValuePair<string, ColumnList> kvp in Columns)
          {
             if (table.ToUpper() == ((string)kvp.Key).ToUpper())
             {
                columnList = kvp.Value;
                return true;
             }
          }
          columnList = null;
          return false;
       }

        private bool ReturnChildren(TSqlFragment fragment)
        {
            if (fragment is FunctionCall)
            {
                switch (((FunctionCall)fragment).FunctionName.Value.ToLower())
                {
                    case "count":
                        return false;
                }
            }
            else if (fragment is ExistsPredicate)
                return false;
            else if (fragment is Subquery)
                return false;
            else if (fragment is SelectStatement)
                return false;

            return true;
        }
        public void TrySetTableNameWithSchema(Dictionary<string, TableSource> tableMap)
        {
            List<TableSource> tableSources = tableMap.Values.ToList().FindAll(x => x is SchemaObjectTableSource);
            if (tableSources != null)
            {
                foreach (SchemaObjectTableSource source in tableSources)
                {
                    if (source.SchemaObject.SchemaIdentifier != null)
                    {
                        string tableWithSchema = String.Format("{0}.{1}", source.SchemaObject.SchemaIdentifier.Value, source.SchemaObject.BaseIdentifier.Value);
                        if (!TableListWithSchema.Contains(tableWithSchema))
                        {
                            TableListWithSchema.Add(tableWithSchema);
                        }
                    }
                    else
                    {
                        string tableWithSchema = String.Format("dbo.{0}", source.SchemaObject.BaseIdentifier.Value);
                        if (!TableListWithSchema.Contains(tableWithSchema))
                        {
                            TableListWithSchema.Add(tableWithSchema);
                        }
                    }
                }
            }
            tableSources = tableMap.Values.ToList().FindAll(x => x is VariableTableSource);
            if (tableSources != null)
            {
                foreach (VariableTableSource source in tableSources)
                {
                    string tableWithSchema = String.Format("dbo.{0}", source.Name.Value);
                    if (!TableListWithSchema.Contains(tableWithSchema))
                    {
                        TableListWithSchema.Add(tableWithSchema);
                    }
                }
            }
        }
    }
}
