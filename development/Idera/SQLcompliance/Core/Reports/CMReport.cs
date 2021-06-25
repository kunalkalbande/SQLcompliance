using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Idera.SQLcompliance.Core.Reports
{
   public class CMReport : IComparable<CMReport>
   {
      #region Constants
      
#if DEBUG
      public const string RelativeReportsPath = @"..\..\..\..\Report\Reports\Supported";
#else
      public const string RelativeReportsPath = @"Reports";
#endif
      public const string RDLXmlFileName = "rdl.xml";
      #endregion
      
      #region Data Members
      
      private string _name;
      private string _description;
      private string _sproc;
      private string _fileName;
      private string _rootHead;
      private List<string> _params;
      
      #endregion
      
      #region Constructors
      
      public CMReport( )
      {
         _params = null;
      }
     
      #endregion
      
      #region Properties
      
      public string Name
      {
         get { return _name;}
         set { _name = value; }
      }
      
      public string Description
      {
         get { return _description; }
         set { _description = value; }
      }
      
      public string StoredProcedure
      {
         get { return _sproc; }
         set { _sproc = value; }
      }

      public string FileName
      {
         get { return _fileName; }
         set { _fileName = value; }
      }

      public string RootHead
      {
         get { return _rootHead; }
         set { _rootHead = value; }
      }
      #endregion
      
      public List<string> GetParameters(SqlConnection conn, bool useHardcodedValues = false)
      {
         if (_params != null && Name != "Regulatory Compliance Check")
            return _params;
         _params = new List<string>();
        
         using(SqlCommand cmd = new SqlCommand(_sproc, conn))
         {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(cmd);
            foreach (SqlParameter p in cmd.Parameters)
            {
               if (p.Direction == ParameterDirection.Input ||
                  p.Direction == ParameterDirection.InputOutput)
               {
                  _params.Add(p.ParameterName);
               }
            }
         }

         if(Name == "Regulatory Compliance Check" && useHardcodedValues)
            {
                _params.Clear();
                _params.Add("Server");
                _params.Add("Database");
                _params.Add("RegulationGuidelines");
                _params.Add("AuditSettings");
                _params.Add("Values");
            }

         return _params;
      }

      public int CompareTo(CMReport other)
      {
         if (other == null)
            return -1;
         else
            return _name.CompareTo(other._name);
      }

   }
}
