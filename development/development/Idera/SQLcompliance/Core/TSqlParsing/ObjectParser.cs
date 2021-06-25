using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Schema.ScriptDom;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLcompliance.Core;
using System.Linq;

namespace Idera.SQLcompliance.Core.TSqlParsing
{
    internal class ObjectParser
    {
        public List<string> TableListWithSchema = new List<string>();
        // SQLCM-5471 v5.6 Add Activity to Senstitive columns
        private string _sqlText;
        public string SQLText { get { return _sqlText; } set { _sqlText = value; } }
        private string _databaseName = string.Empty;
        public string DatabaseName { get { return _databaseName; } set { _databaseName = value; } }
        private string _serverInstance = string.Empty;
        public string ServerInstance { get { return _serverInstance; } set { _serverInstance = value; } }
        public ObjectParser()
        {
        }

        public static TSqlScript ParseObjects(string script, out IList<ParseError> errors)
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

        public void GetObjects(TSqlScript scriptParseTree)
        {
            foreach (TSqlBatch batch in scriptParseTree.Batches)
            {
                GetObjects(batch);
            }
        }

        public void GetObjects(TSqlBatch batchParseTree)
        {
            foreach (TSqlStatement stmt in batchParseTree.Statements)
            {
                GetObjects(stmt);
            }
        }

        public void GetObjects(TSqlStatement fragment)
        {
            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is SelectStatement)
                {
                    GetObjects((SelectStatement)frag);
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
                            }
                        }
                    }
                    //SQLCM-5471 v5.6 - Sensitive columns to handle views - END
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
                }
                // SQLCM-5471 v5.6 Add Activity to Senstitive columns - END
            }
        }

        private void GetObjects(SelectStatement select)
        {
           GetObjects(null, select);
        }

        private void GetObjects(Subquery query)
        {
           GetObjects(query, null);
        }

        private void GetObjects(Subquery query, SelectStatement select)
        {
           if (query != null)
           {
              SubquerySpecification spec = query.QueryExpression as SubquerySpecification;
              if (spec != null)
                 GetObjects(spec.SelectElements, TSqlParsingHelpers.GetTableSources(spec.FromClauses));
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
                       GetObjects(cte.Subquery);
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
                    GetObjects(spec.SelectElements, TSqlParsingHelpers.GetTableSources(select));
              }
           }
        }

        private void GetObjects(IList<TSqlFragment> selectElements, Dictionary<string, TableSource> tableSources)
        {           
           foreach (TSqlFragment element in selectElements)
           {
              if (element is SelectColumn)
              {
                 if (((SelectColumn)element).Expression is Subquery)
                 {
                    GetObjects(((SelectColumn)element).Expression as Subquery);
                 }
                 else{
                     TrySetTableNameWithSchema(tableSources);
                 }
              }
              else if (element is SelectSetVariable)
              {
                 SelectSetVariable x = (SelectSetVariable)element;

                 if (((SelectSetVariable)element).Expression is Subquery)
                 {
                    GetObjects(((SelectSetVariable)element).Expression as Subquery);
                 }
                 else{
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
                    GetObjects(first.SelectElements, TSqlParsingHelpers.GetTableSources(first.FromClauses));
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
                    GetObjects(second.SelectElements, TSqlParsingHelpers.GetTableSources(second.FromClauses));
                }
            }
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
