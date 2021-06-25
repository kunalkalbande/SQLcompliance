using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Idera.SQLcompliance.Core.Templates.AuditTemplates
{
   public class RegulationDAL
   {
      private const string SELECT_Regulations = "SELECT regulationId,name,description FROM {0}..{1}";
      private const string SELECT_RegulationMap = "SELECT regulationId, serverEvents, databaseEvents FROM {0}..{1}";
      private const string SELECT_RegulationSection = "SELECT regulationId, section, sectionDescription, serverEvents, databaseEvents FROM {0}..{1}";

      private static void ValidateConnection(SqlConnection connection)
      {
         if (connection == null)
            throw new ArgumentNullException("connection");
         if (connection.State != ConnectionState.Open)
            throw new Exception("SqlConnection object is not opened.");
      }

      public static List<Regulation> LoadRegulations(string connectionString)
      {
         if(connectionString == null)
            throw new ArgumentNullException("connectionString") ;
         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            return LoadRegulations(connection);
         }
      }

      public static List<Regulation> LoadRegulations(SqlConnection connection)
      {
         List<Regulation> regulations = new List<Regulation>();

         string cmdStr = String.Format(SELECT_Regulations, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryReglationTable);
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  Regulation reg = new Regulation();

                  reg.RegType = (Regulation.RegulationType)reader.GetInt32(0);
                  reg.Name = reader.GetString(1);
                  reg.Description = reader.GetString(2);
                  regulations.Add(reg);
               }
            }
         }
         return regulations;
      }

      public static Dictionary<int, RegulationSettings> LoadRegulationCategories(SqlConnection connection)
      {
         Dictionary<int, RegulationSettings> regSettings = new Dictionary<int, RegulationSettings>();
         RegulationSettings settings;

         string cmdStr = String.Format(SELECT_RegulationMap, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryRegulationMapTable);
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  Regulation.RegulationType type = (Regulation.RegulationType)reader.GetInt32(0);

                  if (regSettings.TryGetValue((int)type, out settings))
                  {
                     settings.ServerCategories = reader.GetInt32(1);
                     settings.DatabaseCategories = reader.GetInt32(2);
                  }
                  else
                  {
                     settings = new RegulationSettings();
                     settings.regulationType = type;
                     settings.ServerCategories = reader.GetInt32(1);
                     settings.DatabaseCategories = reader.GetInt32(2);
                     regSettings.Add((int)type, settings);
                  }
               }
            }
         }
         return regSettings;
      }

      public static int GetCombinedRegulationSettingsServer(Dictionary<int, RegulationSettings> regSettings, int regulations) 
      {
          RegulationSettings settings;
          int result = 0;

          if((regulations & (int)Regulation.RegulationTypeUser.CIS) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.CIS, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.DISA) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.FERPA) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.GDPR) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.HIPAA) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.NERC) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.PCI) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings);
              result = result | settings.ServerCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.SOX) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings);
              result = result | settings.ServerCategories;
          }

          return result;
      }

      public static int GetCombinedRegulationSettingsDatabase(Dictionary<int, RegulationSettings> regSettings, int regulations)
      {
          RegulationSettings settings;
          int result = 0;

          if ((regulations & (int)Regulation.RegulationTypeUser.CIS) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.CIS, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.DISA) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.DISA, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.FERPA) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.FERPA, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.GDPR) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.GDPR, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.HIPAA) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.NERC) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.NERC, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.PCI) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.PCI, out settings);
              result = result | settings.DatabaseCategories;
          }

          if ((regulations & (int)Regulation.RegulationTypeUser.SOX) > 0)
          {
              regSettings.TryGetValue((int)Regulation.RegulationType.SOX, out settings);
              result = result | settings.DatabaseCategories;
          }

          return result;
      }

      public static Dictionary<int, List<RegulationSection>> LoadRegulationSections(SqlConnection connection)
      {
         Dictionary<int, List<RegulationSection>> regSections = new Dictionary<int, List<RegulationSection>>();
         List<RegulationSection> sections;
         RegulationSection section;

         string cmdStr = String.Format(SELECT_RegulationSection, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryRegulationMapTable);
         ValidateConnection(connection);

         using (SqlCommand command = new SqlCommand(cmdStr, connection))
         {
            using (SqlDataReader reader = command.ExecuteReader())
            {
               while (reader.Read())
               {
                  Regulation.RegulationType type = (Regulation.RegulationType)reader.GetInt32(0);
                  section = new RegulationSection();

                  if (regSections.TryGetValue((int)type, out sections))
                  {
                     section.regulationType = type;
                     section.Name = reader.GetString(1);
                     section.Description = reader.GetString(2);
                     section.ServerCategories = reader.GetInt32(3);
                     section.DatabaseCategories = reader.GetInt32(4);
                     sections.Add(section);
                  }
                  else
                  {
                     section.regulationType = type;
                     section.Name = reader.GetString(1);
                     section.Description = reader.GetString(2);
                     section.ServerCategories = reader.GetInt32(3);
                     section.DatabaseCategories = reader.GetInt32(4);
                     sections = new List<RegulationSection>();
                     sections.Add(section);
                     regSections.Add((int)type, sections);
                  }
               }
            }
         }
         return regSections;
      }
   }
}
