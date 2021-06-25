using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections;

namespace SQLcomplianceIndexUpgrade
{
    /// <summary>
    /// Summary description for SQLHelpers.
    /// </summary>
    public class SQLHelpers
    {
        public SQLHelpers()
        {
        }

        public static void
           CheckConnection(
              SqlConnection conn
           )
        {
            if (conn.State == ConnectionState.Open) return;

            if (conn.State == ConnectionState.Broken)
            {
                conn.Close();
            }

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        #region Safe SqlReader readers

        static public int ByteToInt(SqlDataReader rdr, int index)
        {
            return ByteToInt(rdr, index, -1);
        }

        static public int ByteToInt(SqlDataReader rdr, int index, int nullValue)
        {
            int retval = nullValue;

            if (!rdr.IsDBNull(index))
            {
                retval = (int)rdr.GetByte(index);
            }

            return retval;
        }

        static public bool ByteToBool(SqlDataReader rdr, int index)
        {
            bool retval = false;

            if (!rdr.IsDBNull(index))
            {
                if (rdr.GetByte(index) != 0)
                    retval = true;
                else
                    retval = false;
            }

            return retval;
        }

        static public string GetString(SqlDataReader rdr, int index)
        {
            string retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetString(index);
            }
            else
            {
                retval = "";
            }

            return retval;
        }

        static public string GetSafeString(SqlDataReader rdr, int index)
        {
            string retval;

            retval = GetString(rdr, index);
            return CreateSafeString(rdr.GetString(index));
        }

        static public DateTime GetDateTime(SqlDataReader rdr, int index)
        {
            DateTime retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetDateTime(index);
            }
            else
            {
                retval = DateTime.MinValue;
            }

            return retval;
        }

        static public long GetLong(SqlDataReader rdr, int index, long nullValue)
        {
            long retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetInt64(index);
            }
            else
            {
                retval = nullValue;
            }

            return retval;
        }

        static public long GetLong(SqlDataReader rdr, int index)
        {
            return GetLong(rdr, index, 0);
        }

        static public long? GetRowCounts(SqlDataReader rdr, int index)
        {
            long? retval = null;

            if (!rdr.IsDBNull(index))
            {
                retval = (long)rdr.GetInt64(index);
            }
            else
            {
                retval = null;
            }
            return retval;
        }

        static public ulong GetULong(SqlDataReader rdr, int index)
        {
            ulong retval;

            if (!rdr.IsDBNull(index))
            {
                retval = (ulong)rdr.GetInt64(index);
            }
            else
            {
                retval = 0;
            }

            return retval;
        }

        static public int GetInt32(SqlDataReader rdr, int index, int nullValue)
        {
            int retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetInt32(index);
            }
            else
            {
                retval = nullValue;
            }

            return retval;
        }

        static public int GetInt32(SqlDataReader rdr, int index)
        {
            return GetInt32(rdr, index, 0);
        }

        static public int GetInt16(SqlDataReader rdr, int index)
        {
            int retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetInt16(index);
            }
            else
            {
                retval = 0;
            }

            return retval;
        }

        static public bool GetBool(SqlDataReader reader, string columnName)
        {
            var retval = false;
            var colIndex = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(colIndex))
            {
                retval = (Boolean)reader[colIndex];
            }

            return retval;
        }

        static public string GetString(SqlDataReader reader, string columnName)
        {
            var retval = string.Empty;
            var colIndex = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(colIndex))
            {
                retval = (string)reader[colIndex];
            }

            return retval;
        }

        static public Int32 GetInt32(SqlDataReader reader, string columnName)
        {
            var retval = 0;
            var colIndex = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(colIndex))
            {
                retval = (Int32)reader[colIndex];
            }

            return retval;
        }

        static public int GetByteToInt(SqlDataReader reader, string columnName)
        {
            var retval = 0;
            var colIndex = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(colIndex))
            {
                retval = (int)(byte)reader[colIndex];
            }

            return retval;
        }
        #endregion

        #region Safe DataRow readers

        static public string
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

        static public int
           GetRowInt32(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return 0;
            else
                return (int)row[colNdx];
        }

        static public long
           GetRowLong(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return 0;
            else
                return (long)row[colNdx];
        }

        static public DateTime
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

        static public string
           GetRowString(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return "";
            else
                return (string)row[colNdx];
        }

        static public int
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

        static public DateTime
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

        static public long
           GetRowLong(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return 0;
            else
                return (long)row[colName];
        }
        #endregion

        #region Get SQL Server Version Routines

        //------------------------------------------------------------------
        // GetSqlVersionString - Retrieve instance's version string
        //------------------------------------------------------------------
        static public string
           GetSqlVersionString(
              SqlConnection conn
           )
        {
            string versionString = null;

            try
            {
                using (SqlCommand myCommand = new SqlCommand("SELECT @@VERSION", conn))
                {
                    versionString = (string)myCommand.ExecuteScalar();
                }
            }
            catch { }

            return versionString;
        }


        static public string GetSqlVersionName(SqlConnection conn)
        {
            var sqlVersionString = GetSqlVersionString(conn);
            var sqlVersionName = sqlVersionString.Substring(0, sqlVersionString.IndexOf("-")).Trim();
            return sqlVersionName;
        }

        static public ArrayList GetServerProperties(SqlConnection conn, params string[] propertyNames)
        {
            ArrayList values = new ArrayList();

            try
            {
                StringBuilder query = new StringBuilder("SELECT");
                foreach (string property in propertyNames)
                {
                    query.Append(string.Format(" SERVERPROPERTY('{0}'),", property));
                }

                query.Length--; //remove last comma         

                using (SqlCommand command = new SqlCommand(query.ToString(), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        for (int index = 0; index < reader.FieldCount; index++)
                        {
                            object value = reader.GetValue(index);
                            value = value is DBNull ? 0 : value;
                            values.Add(value);
                        }
                    }
                }
            }
            catch
            {
                values = null;
            }

            return values;
        }

        //------------------------------------------------------------------
        // GetSqlVersion - return integer representation of major version
        //                 8 = 2000, 9 = 2005, 10 = 2008, 11 = 2012, 12 = 2014
        //
        //  Two versions
        //  (1) Read from SQL Server and parse
        //  (2) Just parse a string for version
        //
        //   Note: Any badness in string results in us returning 0 for version
        //------------------------------------------------------------------
        // GetSqlVersion - read from SQL Server
        //------------------------------------------------------------------
        static public int
           GetSqlVersion(
              SqlConnection conn
           )
        {
            string sqlVersionString = GetSqlVersionString(conn);

            if (sqlVersionString == null) return 0;

            return GetSqlVersion(sqlVersionString);
        }

        //------------------------------------------------------------------
        // GetSqlVersion - parse version string
        //------------------------------------------------------------------
        static public int
           GetSqlVersion(
              string versionString
           )
        {
            int sqlVersion = 0;
            string sqlVersionMajor;
            int iStart, iEnd;
            try
            {
                sqlVersion = 0;
                iStart = versionString.IndexOf("- ");
                if (iStart == -1) return 0;
                iStart += 2;
                iEnd = versionString.IndexOf(".", iStart);
                if (iEnd == -1) return 0;
                sqlVersionMajor = versionString.Substring(iStart, iEnd - iStart);
                sqlVersion = int.Parse(sqlVersionMajor);
            }
            catch
            {
                sqlVersion = 0;
            }
            return sqlVersion;
        }
        #endregion

        #region Create Escaped Strings

        //-----------------------------------------------------------------------
        // CreateSafeString - creates safe string parameter includes
        //                    single quotes; used to create sql parameters
        //-----------------------------------------------------------------------
        static public string CreateSafeString(string propName)
        {
            return CreateSafeString(propName, int.MaxValue);
        }

        //-----------------------------------------------------------------------
        // CreateSafeString - creates safe string parameter includes
        //                    single quotes; used to create sql parameters with
        //                    length limit
        //-----------------------------------------------------------------------
        static public string CreateSafeString(string propName, int limit)
        {
            StringBuilder newName;
            string tmpValue;

            if (propName == null)
            {
                newName = new StringBuilder("null");
            }
            else
            {
                newName = new StringBuilder("'");
                tmpValue = propName.Replace("'", "''");
                if (tmpValue.Length > limit)
                {
                    if (tmpValue[limit - 1] == '\'')
                    {
                        limit--;
                        if (tmpValue[limit - 1] == '\'')
                            limit--;
                    }
                    tmpValue = tmpValue.Remove(limit, tmpValue.Length - limit);
                }
                newName.Append(tmpValue);
                newName.Append("'");
            }

            return newName.ToString();
        }

        static public string CreateEscapedString(string propname)
        {
            string newValue = "null";

            if (propname != null)
                newValue = propname.Replace("'", "''");

            return newValue;
        }

        //-----------------------------------------------------------------------
        // CreateSafeDateTimeString - creates safe string parameter
        //                            includes single quotes
        //                            used to create sql parameters
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDateTimeString(
              DateTime timestamp
           )
        {
            return CreateSafeDateTimeString(timestamp, true);
        }

        //-----------------------------------------------------------------------
        // CreateSafeDateTimeString - creates safe string parameter
        //                            includes single quotes
        //                            used to create sql parameters
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDateTimeString(
              DateTime timestamp,
              bool includeQuotes
           )
        {
            /* The problem with CultureInfo.CurrentCulture.DateTimeFormat is it doesn't work
             * on UK platforms.  SQL Server complains that the datetime values cannot be converted.
             * So use the SQL Server CONVERT function to explicitly convert the value from ODBC format */
            /*
            StringBuilder newName = new StringBuilder("");

            if ( timestamp == DateTime.MinValue )
            {
               newName.Append("null");
            }
            else
            {
                  if ( includeQuotes) newName = new StringBuilder("'");
               //newName.Append( timestamp.ToString( "yyyy-MM-dd HH:mm:ss.fff",
               //                                    DateTimeFormatInfo.InvariantInfo ) );
               newName.AppendFormat( timestamp.ToString( CultureInfo.CurrentCulture.DateTimeFormat ) );
                 if ( includeQuotes) newName.Append("'");
              }

               //builder.Append(String.Format("{0}-{1}-{2} {3}:{4}:{5}", filterDateTime.Year, filterDateTime.Month,
               //filterDateTime.Day, filterDateTime.Hour, filterDateTime.Minute, filterDateTime.Second)) ;
            //ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug, "CreateSafeDateTimeString() returning " + newName.ToString());


            //return newName.ToString();
            */
            return CreateSafeDateTime(timestamp);
        }

        //-----------------------------------------------------------------------
        // CreateSafeDateTime - creates a SQL Server CONVERT function call for 
        //                      DateTime Value
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDateTime(
              DateTime timestamp
           )
        {
            string newString;

            if (timestamp == DateTime.MinValue)
            {
                newString = "null";
            }
            else
            {
                newString = String.Format("CONVERT(DATETIME, '{0}-{1}-{2} {3}:{4}:{5}.{6:000}',121)", timestamp.Year,
                    timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute, timestamp.Second, timestamp.Millisecond);
                /*
                newString = String.Format( "CONVERT( DATETIME, '{0}', 20 ) ", 
                                           timestamp.ToString( "yyyy-MM-dd HH:mm:ss.fff",
                                                                            DateTimeFormatInfo.InvariantInfo ));
                                                                            */
            }
            //ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug, "CreateSafeDateTime() returning " + newString);

            return newString;
        }
        //-----------------------------------------------------------------------
        // CreateSafeDatabaseName - creates safe db name for SQL
        //-----------------------------------------------------------------------
        static public string CreateSafeDatabaseName(string dbName)
        {
            StringBuilder newName;

            newName = new StringBuilder("[");
            newName.Append(dbName.Replace("]", "]]"));
            newName.Append("]");

            return newName.ToString();
        }

        //-----------------------------------------------------------------------
        // CreateSafeDatabaseNameForConnectionString
        //
        // (1) If database begins or ends with blanks, wrap in quotes
        // (2) If contains one of ;'" then you need to espace with ' or ". Use "
        //     unless first character is single quote then use double quote
        // (3) If string contains any escaped chars enclose in quotes
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDatabaseNameForConnectionString(
              string dbName
           )
        {
            if (dbName == null || dbName.Length == 0) return dbName;

            // Use double quote as escape character unless first character is double
            // quote; then use single quote
            string doubleQuote = "\"";

            bool encloseInQuotes = false;

            // Do we need to enclose in quotes? (contains semicolon or leading or trailing spaces)
            if ((-1 != dbName.IndexOf(";")) ||
                 (dbName[0] == ' ' || dbName[dbName.Length - 1] == ' '))
            {
                encloseInQuotes = true;
            }

            if (encloseInQuotes)
            {
                // escape any double quotes
                dbName = dbName.Replace(doubleQuote, "\"\"");
                dbName = doubleQuote + dbName + doubleQuote;
            }

            return dbName;
        }

        #endregion

    }
}

