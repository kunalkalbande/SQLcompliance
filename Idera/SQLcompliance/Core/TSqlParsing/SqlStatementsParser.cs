using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Idera.SQLcompliance.Core.TSqlParsing
{
    public class SqlStatementsParser
    {
        private string _sqlText;
        public string SqlQueryText {
            get
            {
                return _sqlText;
            }
            set
            {
                _sqlText = value;
            }
        }
        private static void ParseLikePredicate(LikePredicate predicate, DataContainer container)
        {
            var initial = predicate.FirstExpression;
            ParseScalarExpression(initial, container);
            var compareTo = predicate.SecondExpression;
            ParseScalarExpression(compareTo, container);
            var escape = predicate.EscapeExpression;
            ParseScalarExpression(escape, container);
        }

        private static void ParseScalarExpression(ScalarExpression expression, DataContainer container)
        {
            if (expression is ColumnReferenceExpression)
            {
                ParseColumnReferenceExpression(expression as ColumnReferenceExpression, container, "");
            }
            else if (expression is ScalarSubquery)
            {
                ParseScalarSubquery(expression as ScalarSubquery, container);
            }
        }

        private static void ParseInPredicate(InPredicate predicate, DataContainer container)
        {
            ParseScalarExpression(predicate.Expression, container);
            foreach (var scalarExpression in predicate.Values)
            {
                ParseScalarExpression(scalarExpression, container);
            }
            if (predicate.Subquery != null)
            {
                ParseScalarSubquery(predicate.Subquery, container);
            }
        }

        private static void ParseScalarSubquery(ScalarSubquery subquery, DataContainer container)
        {
            var expression = subquery.QueryExpression as QuerySpecification;
            ParseQueryExpression(expression, container);
        }

        private static void ParseQueryExpression(QuerySpecification querySpec, DataContainer container)
        {
            if (querySpec.FromClause == null)
            {
                return;
            }
            var tables = querySpec.FromClause.TableReferences;
            foreach (var table in tables)
            {
                if (table is NamedTableReference)
                {
                    ParsedNamedTableReference(table, container);
                }
            }
            var columns = querySpec.SelectElements;
            foreach (var column in columns)
            {
                if (column is SelectStarExpression)
                {
                    continue;
                }
                if (column is SelectScalarExpression)
                {
                    var scalar = column as SelectScalarExpression;
                    var alias = scalar.ColumnName == null ? "" : scalar.ColumnName.Value;
                    var expression = scalar.Expression;
                    if (expression is ColumnReferenceExpression)
                    {
                        var columnExpression = expression as ColumnReferenceExpression;
                        ParseColumnReferenceExpression(columnExpression, container, alias);
                    }
                    else if (expression is FunctionCall)
                    {
                        var functionExpression = expression as FunctionCall;
                        ParseFunctionCall(functionExpression, container, alias);
                    }
                }
            }
            if (querySpec.WhereClause != null)
            {
                ParseWhereClause(querySpec.WhereClause, container);
            }
        }
        private static void ParseFunctionCall(FunctionCall functionExpression, DataContainer container, string alias)
        {
            var parameters = functionExpression.Parameters;
            foreach (var parameter in parameters)
            {
                if (parameter is ColumnReferenceExpression)
                {
                    var columnExpression = parameter as ColumnReferenceExpression;
                    ParseColumnReferenceExpression(columnExpression, container, alias);
                }
                else if (parameter is FunctionCall)
                {
                    var function = parameter as FunctionCall;
                    ParseFunctionCall(function, container, alias);
                }
            }
        }
        private static void ParseBooleanIsNullExpression(BooleanIsNullExpression expression, DataContainer container)
        {
            ParseScalarExpression(expression.Expression, container);
        }

        private static void ParseBooleanTernaryExpression(BooleanTernaryExpression expression, DataContainer container)
        {
            var initial = expression.FirstExpression;
            ParseScalarExpression(initial, container);
            var lower = expression.SecondExpression;
            ParseScalarExpression(lower, container);
            var higher = expression.ThirdExpression;
            ParseScalarExpression(higher, container);
        }

        private static void ParseBooleanNotExpression(BooleanNotExpression expression, DataContainer container)
        {
            ParseBooleanExpression(expression.Expression, container);
        }

        private static void ParseBooleanParenthesisExpression(BooleanParenthesisExpression expression, DataContainer container)
        {
            ParseBooleanExpression(expression.Expression, container);
        }

        private static void ParseBooleanBinaryExpression(BooleanBinaryExpression expression, DataContainer container)
        {
            var left = expression.FirstExpression;
            ParseBooleanExpression(left, container);
            var right = expression.SecondExpression;
            ParseBooleanExpression(right, container);
        }

        private static void ParseWhereClause(WhereClause clause, DataContainer container)
        {
            var condition = clause.SearchCondition;
            if (condition == null)
            {
                return;
            }
            if (condition is BooleanParenthesisExpression)
            {
                ParseBooleanParenthesisExpression(condition as BooleanParenthesisExpression, container);
            }
            else if (condition is BooleanComparisonExpression)
            {
                ParseBooleanComparisonExpression(condition as BooleanComparisonExpression, container);
            }
            else if (condition is BooleanBinaryExpression)
            {
                ParseBooleanBinaryExpression(condition as BooleanBinaryExpression, container);
            }
            else if (condition is BooleanNotExpression)
            {
                ParseBooleanNotExpression(condition as BooleanNotExpression, container);
            }
            else if (condition is BooleanTernaryExpression)
            {
                ParseBooleanTernaryExpression(condition as BooleanTernaryExpression, container);
            }
            else if (condition is BooleanIsNullExpression)
            {
                ParseBooleanIsNullExpression(condition as BooleanIsNullExpression, container);
            }
            else if (condition is InPredicate)
            {
                ParseInPredicate(condition as InPredicate, container);
            }
            else if (condition is LikePredicate)
            {
                ParseLikePredicate(condition as LikePredicate, container);
            }
        }
        private static void ParsedNamedTableReference(TableReference table, DataContainer container)
        {
            var reference = table as NamedTableReference;
            var tableName = reference.SchemaObject.BaseIdentifier.Value;
            var alias = reference.Alias == null ? "" : reference.Alias.Value;
            container.AddTable(tableName, alias);
        }

        private static void ParseBooleanComparisonExpression(BooleanComparisonExpression expression, DataContainer container)
        {
            var left = expression.FirstExpression;
            ParseScalarExpression(left, container);
            var right = expression.SecondExpression;
            ParseScalarExpression(right, container);
        }
        private static void ParseBooleanExpression(BooleanExpression expression, DataContainer container)
        {
            if (expression is BooleanComparisonExpression)
            {
                ParseBooleanComparisonExpression(expression as BooleanComparisonExpression, container);
            }
            else if (expression is BooleanBinaryExpression)
            {
                ParseBooleanBinaryExpression(expression as BooleanBinaryExpression, container);
            }
            else if (expression is BooleanParenthesisExpression)
            {
                ParseBooleanParenthesisExpression(expression as BooleanParenthesisExpression, container);
            }
            else if (expression is BooleanTernaryExpression)
            {
                ParseBooleanTernaryExpression(expression as BooleanTernaryExpression, container);
            }
            else if (expression is BooleanIsNullExpression)
            {
                ParseBooleanIsNullExpression(expression as BooleanIsNullExpression, container);
            }
            else if (expression is InPredicate)
            {
                ParseInPredicate(expression as InPredicate, container);
            }
            else if (expression is LikePredicate)
            {
                ParseLikePredicate(expression as LikePredicate, container);
            }
        }
        private static void ParseColumnReferenceExpression(ColumnReferenceExpression columnExpression, DataContainer container,
            string alias)
        {
            var identifier = columnExpression.MultiPartIdentifier;
            if (identifier.Identifiers.Count == 1)
            {
                var columnName = identifier.Identifiers.First().Value;
                var tableName = "";
                var foundTables = container.GetTables();
                if (foundTables.Count == 1)
                {
                    tableName = foundTables[0];
                }
                container.AddColumn(columnName);
            }
            else
            {
                var tableRef = identifier.Identifiers.First().Value;
                var tableName = (from x in container.GetTables()
                                 where (x.Equals(tableRef))
                                 select x).First();
                var columnName = identifier.Identifiers.Skip(1).First().Value;
                container.AddColumn(columnName);
            }
        }

        public DataContainer GetColumns()
        {
            DataContainer container = new DataContainer();
            TSqlParser sqlParser;
            IList<ParseError> errors;
            sqlParser = new TSql100Parser(false);
            var _reader = new StringReader(_sqlText);
            var statements = sqlParser.ParseStatementList(_reader, out errors);
            if (errors.Count > 0)
            {
                foreach (var err in errors)
                {
                    Console.WriteLine(err);
                }
            }
            else
            {
                foreach (var stmt in statements.Statements)
                {
                    if (stmt is UpdateStatement)
                    {
                        var updateStmt = stmt as UpdateStatement;
                        var updateSpec = updateStmt.UpdateSpecification;
                        var target = updateSpec.Target;
                        if (target is NamedTableReference)
                        {
                            ParsedNamedTableReference(target, container);
                        }
                        foreach (var setClause in updateSpec.SetClauses)
                        {
                            if (setClause is AssignmentSetClause)
                            {
                                var assignment = setClause as AssignmentSetClause;
                                ParseColumnReferenceExpression(assignment.Column, container, "");
                            }
                        }
                        if (updateSpec.WhereClause != null)
                        {
                            ParseWhereClause(updateSpec.WhereClause, container);
                        }
                    }
                    if (stmt is DeleteStatement)
                    {
                        var deleteStatement = stmt as DeleteStatement;
                        var deleteSpec = deleteStatement.DeleteSpecification;
                        if (deleteSpec.TopRowFilter != null)
                        {
                            //check for top
                            container.IsTopSelection = true;
                            container.IsTopPercent = deleteSpec.TopRowFilter.Percent;
                            var expression = deleteSpec.TopRowFilter.Expression as ParenthesisExpression;
                            if (expression != null && expression.Expression is IntegerLiteral)
                            {
                                container.TopCount = Int32.Parse((expression.Expression as IntegerLiteral).Value);
                            }
                        }
                        var target = deleteSpec.Target;
                        if (target is NamedTableReference)
                        {
                            ParsedNamedTableReference(target, container);
                        }
                        if (deleteSpec.WhereClause != null)
                        {
                            ParseWhereClause(deleteSpec.WhereClause, container);
                        }
                    }
                    if (stmt is InsertStatement)
                    {
                        var insertStatement = stmt as InsertStatement;
                        var insertSpec = insertStatement.InsertSpecification;

                        var target = insertSpec.Target;
                        if (target is NamedTableReference)
                        {
                            ParsedNamedTableReference(target, container);
                        }
                        foreach (var column in insertSpec.Columns)
                        {
                            ParseColumnReferenceExpression(column, container, "");
                        }
                    }
                    if (stmt is TruncateTableStatement)
                    {
                        var truncateStatement = stmt as TruncateTableStatement;
                        string tableName = truncateStatement.TableName.BaseIdentifier.Value;
                        container.AddTable(tableName, "");
                    }
                    if (stmt is AlterTableAlterColumnStatement)
                    {
                        var alterStatement = stmt as AlterTableAlterColumnStatement;
                        var tableName = alterStatement.SchemaObjectName.BaseIdentifier.Value;
                        container.AddTable(tableName, "");
                        string columnName = alterStatement.ColumnIdentifier.Value;
                        container.AddColumn(columnName);
                    }
                    if (stmt is AlterTableAddTableElementStatement)
                    {
                        var alterStatement = stmt as AlterTableAddTableElementStatement;
                        var tableName = alterStatement.SchemaObjectName.BaseIdentifier.Value;
                        container.AddTable(tableName, "");
                        IList<ColumnDefinition> columns = alterStatement.Definition.ColumnDefinitions;
                        foreach (ColumnDefinition col in columns)
                        {
                            string colName = col.ColumnIdentifier.Value;
                            container.AddColumn(colName);
                        }
                    }
                    if (stmt is AlterTableDropTableElementStatement)
                    {
                        var alterStatement = stmt as AlterTableDropTableElementStatement;
                        var tableName = alterStatement.SchemaObjectName.BaseIdentifier.Value;
                        container.AddTable(tableName, "");
                        IList<AlterTableDropTableElement> columns = alterStatement.AlterTableDropTableElements;
                        foreach (AlterTableDropTableElement element in columns)
                        {
                            string colName = element.Name.Value;
                            container.AddColumn(colName);
                        }
                    }
                    if (stmt is DropTableStatement)
                    {
                        var dropStratment = stmt as DropTableStatement;
                        IList<SchemaObjectName> objects = dropStratment.Objects;
                        foreach (SchemaObjectName objName in objects)
                        {
                            string tableName = objName.BaseIdentifier.Value;
                            container.AddTable(tableName, "");
                        }
                    }
                    if (stmt is DbccStatement)
                    {
                        var dbccStatment = stmt as DbccStatement;
                        IList<DbccNamedLiteral> literals = dbccStatment.Literals;
                        foreach (DbccNamedLiteral dbcc in literals)
                        {
                            ScalarExpression sc = dbcc.Value;
                            var idetifier = sc as IdentifierLiteral;
                            string tableName = idetifier.Value;
                            container.AddTable(tableName, "");

                        }
                    }
                }
            }
            return container;
        }
    }
}
