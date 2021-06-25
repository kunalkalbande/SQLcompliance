using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data.SqlClient;

namespace Idera.SQLcompliance.Core.Templates.AuditTemplates
{
   public class RegulationSettings
   {

      [Flags]
      public enum RegulationServerCategory : int
      {
         Logins               = 1,
         FailedLogins         = 2,
         SecurityChanges      = 4,
         DatabaseDefinition   = 8,
         AdminActivity        = 16,
         UserDefined          = 32,
         FilterPassedAccess   = 64,
         FilterFailedAccess   = 128,
         PrivelegedUsers      = 256,
         Logouts = 512  // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
        }

      [Flags]
      public enum RegulationDatabaseCategory : int
      {
         SecurityChanges      = 1,
         DatabaseDefinition   = 2,
         AdminActivity        = 4,
         DatabaseModification = 8,
         Select               = 16,
         FilterPassedAccess   = 32,
         FilterFailedAccess   = 64, 
         SQLText              = 128, 
         Transactions         = 256, 
            SensitiveColumns = 512,
            BeforeAfterDataChange = 1024,
            PrivelegedUsers = 2048
      }

      public Regulation.RegulationType regulationType;
      private int serverCategories = 0;
      private int databaseCategories = 0;

      public int ServerCategories
      {
         get { return serverCategories; }
         set { serverCategories |= value; }
      }

      public int DatabaseCategories
      {
         get { return databaseCategories; }
         set { databaseCategories |= value; }
      }

      public RegulationSettings()
      {
         regulationType = Regulation.RegulationType.NoRegulation;
      }


        //Fix for 5241
        //Load user applied regulations for server level
        public static int LoadUserAppliedSettingsServer(SqlConnection conn, int srvId)
        {
            int regulationSettings = 0;
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);
                
                query = string.Format("SELECT userAppliedRegulations FROM {0}..{1} WHERE srvId = {2}", 
                    CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            regulationSettings = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "LoadUserAppliedSettingsServer", query, ex);
            }

            return regulationSettings;
        }

        //Fix for 5241
        //Update regulation settings for server level
        public static void UpdateRegulationSettingsServer(SqlConnection conn, int srvId, int newValue)
        {
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = string.Format("UPDATE {0}..{1} SET userAppliedRegulations = {2} WHERE srvId = {3}", 
                    CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, newValue, srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "UpdateRegulationSettingsServer", query, ex);
            }
        }

        //Fix for 5241
        //Load user applied regulations for database level
        public static int LoadUserAppliedSettingsDatabase(SqlConnection conn, int srvId, int dbId)
        {
            int regulationSettings = 0;
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = string.Format("SELECT pci,hipaa,disa,nerc,cis,sox,ferpa,gdpr FROM {0}..{1} WHERE srvId = {2} AND dbId = {3}",
                    CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, srvId, dbId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 0) == 1)? (int)Regulation.RegulationTypeUser.PCI : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 1) == 1) ? (int)Regulation.RegulationTypeUser.HIPAA : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 2) == 1) ? (int)Regulation.RegulationTypeUser.DISA : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 3) == 1) ? (int)Regulation.RegulationTypeUser.NERC : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 4) == 1) ? (int)Regulation.RegulationTypeUser.CIS : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 5) == 1) ? (int)Regulation.RegulationTypeUser.SOX : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 6) == 1) ? (int)Regulation.RegulationTypeUser.FERPA : 0);
                            regulationSettings = regulationSettings | ((SQLHelpers.ByteToInt(reader, 7) == 1) ? (int)Regulation.RegulationTypeUser.GDPR : 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "LoadUserAppliedSettingsDatabase", query, ex);
            }

            return regulationSettings;
        }
        
    }

   public class RegulationSection
   {
      public Regulation.RegulationType regulationType;
      public string Name;
      public string Description;
      public int ServerCategories = 0;
      public int DatabaseCategories = 0;

      public RegulationSection()
      {
      }
   }

    public class Regulation
    {
        public enum RegulationType
        {
            NoRegulation,
            PCI,
            HIPAA,
            DISA,
            NERC,
            CIS,
            SOX,
            FERPA,
            GDPR
        }

        /// <summary>
        /// Fix for 5241 - Just for processing user applied regulation settings
        /// </summary>
        public enum RegulationTypeUser
        {
            NoRegulation = 0,
            PCI = 1,
            HIPAA = 2,
            DISA = 4,
            NERC = 8,
            CIS = 16,
            SOX = 32,
            FERPA = 64,
            GDPR = 128
        }

      private RegulationType regulation;
      private string name;
      private string description;

      public RegulationType RegType
      {
         get { return regulation; }
         set { regulation = value; }
      }

      public string Description
      {
         get { return description; }
         set { description = value; }
      }

      public string Name
      {
         get { return name; }
         set { name = value; }
      }

      public Regulation()
      {
         regulation = RegulationType.PCI;
      }
   }
}
