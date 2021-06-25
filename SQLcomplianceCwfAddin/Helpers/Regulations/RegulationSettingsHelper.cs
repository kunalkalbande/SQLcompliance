using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RegulationSettings;

namespace SQLcomplianceCwfAddin.Helpers.Regulations
{
    public class RegulationSettingsHelper : Singleton<RegulationSettingsHelper>
    {
        #region Private Members
        private const string NEW_LINE_SPLITTER = "\r\n"; 
        #endregion

        #region Private Methods
        private string GetRegulationName(int regulationType, List<Regulation> coreRegulations)
        {
            var found = coreRegulations.Find(regulation => Convert.ToInt32(regulation.RegType) == regulationType);

            if (found == null)
            {
                return string.Empty;
            }

            return found.Name;
        }

        private string FormEventsString(ArrayList events)
        {
            if (events.Count > 0)
            {
                StringBuilder eventStr = new StringBuilder((string)events[0]);

                for (int index = 1; index < events.Count; index++)
                {
                    if (index % 2 == 0)
                        eventStr.AppendFormat(",{0}{1}", NEW_LINE_SPLITTER, events[index]);
                    else
                        eventStr.AppendFormat(", {0}", events[index]);
                }
                return eventStr.ToString();
            }

            return "None";
        }

        private string ConvertServerEvents(int serverEvents)
        {
            ArrayList events = new ArrayList();

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins)
                events.Add("Logins");

            // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts)
                events.Add("Logouts");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins)
                events.Add("Failed Logins");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges)
                events.Add("Security Changes");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition)
                events.Add("DDL");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity)
                events.Add("Administrative Actions");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined)
                events.Add("User Defined Events");

            if ((serverEvents & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers)
            {
                events.Add("Privileged Users");
                events.Add("Privileged Users Events");
            }

            return FormEventsString(events);
        }

        private string ConvertDatabaseEvents(int databaseEvents)
        {
            ArrayList events = new ArrayList();

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges)
                events.Add("Security");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition)
                events.Add("DDL");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity)
                events.Add("Administrative Actions");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification)
                events.Add("DML");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select)
                events.Add("Select");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText)
                events.Add("SQL Text");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions)
                events.Add("Transactions");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns) == (int)RegulationSettings.RegulationDatabaseCategory.SensitiveColumns)
                events.Add("Sensitive Columns");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange) == (int)RegulationSettings.RegulationDatabaseCategory.BeforeAfterDataChange)
                events.Add("Before After Data Change");

            if ((databaseEvents & (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationDatabaseCategory.PrivelegedUsers)
            {
                events.Add("Privileged Users");
                events.Add("Privileged Users Events");
            }
            return FormEventsString(events);
        } 
        #endregion

        #region Public Methods
        public Dictionary<int, RegulationSettings> GetCategoryDictionary(SqlConnection connection)
        {
            Dictionary<int, RegulationSettings> regulationCategoryDictionary = null;

            try
            {
                regulationCategoryDictionary = RegulationDAL.LoadRegulationCategories(connection);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                       ErrorLog.Level.Verbose,
                       String.Format("Failed to read regulation category settings with connection: {0}: ", connection),
                       ex,
                       ErrorLog.Severity.Warning);
            }

            return regulationCategoryDictionary;
        }

        public List<RestRegulation> GetRegulationTypeList(SqlConnection connection)
        {
            var coreRegulations = RegulationDAL.LoadRegulations(connection);
            var restRegulations = Converter.ConvertReferenceList(coreRegulations, delegate(Regulation regulation)
            {
                return new RestRegulation
                {
                    Name = regulation.Name,
                    Description = regulation.Description,
                    Type = (RestRegulationType)regulation.RegType,
                };
            });

            return restRegulations;
        }

        public Dictionary<string, List<RestRegulationSection>> GetRegulationSectionDictionary(SqlConnection connection)
        {
            var dictionary = new Dictionary<string, List<RestRegulationSection>>();
            var regSections = RegulationDAL.LoadRegulationSections(connection);
            var coreRegulations = RegulationDAL.LoadRegulations(connection);

            foreach (var regulationType in regSections.Keys)
            {
                var sectionList = regSections[regulationType];
                var restSectionList = Converter.ConvertReferenceList(sectionList, delegate(RegulationSection section)
                {
                    return new RestRegulationSection
                    {
                        Name = section.Name,
                        ServerEvents = ConvertServerEvents(section.ServerCategories),
                        DatabaseEvents = ConvertDatabaseEvents(section.DatabaseCategories),
                    };
                });

                var key = GetRegulationName(regulationType, coreRegulations);
                if (!string.IsNullOrEmpty(key))
                {
                    dictionary.Add(key, restSectionList);
                }
            }

            return dictionary;
        } 
        #endregion
    }
}
