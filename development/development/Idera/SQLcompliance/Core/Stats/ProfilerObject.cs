using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using System.Text;

namespace Idera.SQLcompliance.Core.Stats
{
    [DataContract(Name = "profilerObject")]
    public class ProfilerObject
    {
        private const string SELECT_SensitiveColumnProfilerDetails = "SELECT profileName,category,searchStringName,definition,isStringChecked,isProfileActive FROM {0}..SensitiveColumnProfiler order by profileName,category,searchStringName,definition";
        private const string SELECT_SensitiveColumnStringsDetails = "SELECT category,searchStringName,definition,isStringChecked FROM {0}..SensitiveColumnStrings order by category,searchStringName,definition";
        private const string SELECT_SensitiveColumnActiveProfile = "SELECT distinct profileName from {0}..SensitiveColumnProfiler where isProfileActive = '1'";
        private const string SELECT_SensitiveColumnProfiles = "SELECT distinct profileName from {0}..SensitiveColumnProfiler";
        private const string DELETE_String = "DELETE from {0}..SensitiveColumnProfiler WHERE category={1} AND searchStringName={2} AND definition={3}";
        private const string DELETE_SensitiveColumnStrings = "DELETE from {0}..SensitiveColumnStrings WHERE category={1} AND searchStringName={2} AND definition={3}";
        private const string UPDATE_String = "UPDATE {0}..SensitiveColumnProfiler SET category={1},searchStringName={2},definition={3} WHERE category={4} AND searchStringName={5} AND definition={6}";
        private const string UPDATE_SensitiveColumnStrings = "UPDATE {0}..SensitiveColumnStrings SET category={1},searchStringName={2},definition={3} WHERE category={4} AND searchStringName={5} AND definition={6}";
        private const string UPDATE_DeactivateOldProfile = "UPDATE {0}..SensitiveColumnProfiler SET isProfileActive='0'";
        private const string UPDATE_ActivateProfile = "UPDATE {0}..SensitiveColumnProfiler SET isProfileActive='1' where profileName={1}";
        private const string DELETE_ResetData = "DELETE from {0}..SensitiveColumnProfiler";
        private const string DELETE_Profile = "DELETE from {0}..SensitiveColumnProfiler where profileName={1}";
        private const string INSERT_NewProfile = "INSERT INTO {0}..SensitiveColumnProfiler (profileName, category, searchStringName, definition, isStringChecked, isProfileActive) VALUES ({1},{2},{3},{4},{5},'1')";
        private const string UPDATE_IsUpdated = "UPDATE {0}..WebRefreshDuration SET isUpdated={1}";
        private const string SELECT_IsUpdated = "SELECT isUpdated from {0}..WebRefreshDuration";
        [DataMember(Order = 1, Name = "profileName")]
        public string ProfileName;

        [DataMember(Order = 2, Name = "category")]
        public string Category;

        [DataMember(Order = 3, Name = "searchStringName")]
        public string SearchStringName;

        [DataMember(Order = 4, Name = "definition")]
        public string Definition;

        [DataMember(Order = 5, Name = "isStringChecked")]
        public bool IsStringChecked;

        [DataMember(Order = 6, Name = "isProfileActive")]
        public bool IsProfileActive;

        public static List<ProfilerObject> GetProfileDetails(SqlConnection conn)
        {
            List<ProfilerObject> retVal = new List<ProfilerObject>();
            if (GetProfiles(conn).Count < 1)
            {
                string query = String.Format(SELECT_SensitiveColumnStringsDetails, CoreConstants.RepositoryDatabase);
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int i = 0;
                            ProfilerObject record = new ProfilerObject();
                            record.Category = SQLHelpers.GetString(reader, i++);
                            record.SearchStringName = SQLHelpers.GetString(reader, i++);
                            record.Definition = SQLHelpers.GetString(reader, i++);
                            record.IsStringChecked = ((SQLHelpers.GetString(reader, i++) == "1") ? true : false);
                            retVal.Add(record);
                        }
                    }
                }
            }
            else
            {
                string query = String.Format(SELECT_SensitiveColumnProfilerDetails, CoreConstants.RepositoryDatabase);
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int i = 0;
                            ProfilerObject record = new ProfilerObject();
                            record.ProfileName = SQLHelpers.GetString(reader, i++);
                            record.Category = SQLHelpers.GetString(reader, i++);
                            record.SearchStringName = SQLHelpers.GetString(reader, i++);
                            record.Definition = SQLHelpers.GetString(reader, i++);
                            record.IsStringChecked = ((SQLHelpers.GetString(reader, i++) == "1") ? true : false);
                            record.IsProfileActive = ((SQLHelpers.GetString(reader, i++) == "1") ? true : false);
                            retVal.Add(record);
                        }
                    }
                }
            }
            return retVal;
        }

        public static List<string> GetProfiles(SqlConnection conn)
        {
            List<string> record = new List<string>{};
            string query = String.Format(SELECT_SensitiveColumnProfiles, CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int i = 0;
                        record.Add(SQLHelpers.GetString(reader, i++));
                    }
                }
            }
            return record;
        }

        public static string GetActiveProfile(SqlConnection conn)
        {
            string record = "";
            string query = String.Format(SELECT_SensitiveColumnActiveProfile, CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int i = 0;
                        record = SQLHelpers.GetString(reader, i++);
                    }
                }
            }
            return record;
        }

        public void DeleteString(SqlConnection conn, ProfilerObject selectedList)
        {
            StringBuilder category = new StringBuilder("'");
            StringBuilder searchStringName = new StringBuilder("'");
            StringBuilder definition = new StringBuilder("'");

            category.Append(selectedList.Category);
            searchStringName.Append(selectedList.SearchStringName);
            definition.Append(selectedList.Definition);

            category.Append("'");
            searchStringName.Append("'");
            definition.Append("'");

            string query = String.Format(DELETE_String, CoreConstants.RepositoryDatabase, category,
                searchStringName, definition);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteScalar();
            }

            query = String.Format(DELETE_SensitiveColumnStrings, CoreConstants.RepositoryDatabase, category,
                searchStringName, definition);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteScalar();
            }
        }

        public void InsertString(SqlConnection conn, ProfilerObject newString)
        {
            string category = newString.Category;
            string searchStringName = newString.SearchStringName;
            string definition = newString.Definition;
            string activeProfile = GetActiveProfile(conn);
            List<string> profileList = GetProfiles(conn);
            if(profileList != null && profileList.Count != 0)
            foreach(string profile in profileList)
            {
                StringBuilder queryBuilder = new StringBuilder("");
                queryBuilder.Append("INSERT INTO {0}..SensitiveColumnProfiler (profileName,category,searchStringName,definition,isStringChecked,isProfileActive) VALUES ('");
                queryBuilder.Append(profile + "','" + newString.Category + "','" + newString.SearchStringName + "','" + newString.Definition + "','0','");
                if(profile.Equals(activeProfile))
                {
                    queryBuilder.Append("1')");
                }
                else
                {
                    queryBuilder.Append("0')");
                }
                string query = String.Format(queryBuilder.ToString(), CoreConstants.RepositoryDatabase);
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteScalar();
                }
            }
            StringBuilder queryBuilderSensitiveColumnStrings = new StringBuilder("");
            queryBuilderSensitiveColumnStrings.Append("INSERT INTO {0}..SensitiveColumnStrings (category,searchStringName,definition,isStringChecked) VALUES ('");
            queryBuilderSensitiveColumnStrings.Append(newString.Category + "','" + newString.SearchStringName + "','" + newString.Definition + "','0')");
            string querySensitiveColumnStrings = String.Format(queryBuilderSensitiveColumnStrings.ToString(), CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(querySensitiveColumnStrings, conn))
            {
                cmd.ExecuteScalar();
            }
        }

        public void UpdateString(SqlConnection conn, List<ProfilerObject> updateStrings)
        {
            ProfilerObject oldString = new ProfilerObject();
            ProfilerObject newString = new ProfilerObject();
            oldString = updateStrings[0];
            newString = updateStrings[1];
            StringBuilder oldCategory = new StringBuilder("'");
            StringBuilder oldSearchStringName = new StringBuilder("'");
            StringBuilder oldDefinition = new StringBuilder("'");
            StringBuilder newCategory = new StringBuilder("'");
            StringBuilder newSearchStringName = new StringBuilder("'");
            StringBuilder newDefinition = new StringBuilder("'");

            oldCategory.Append(oldString.Category);
            oldSearchStringName.Append(oldString.SearchStringName);
            oldDefinition.Append(oldString.Definition);
            newCategory.Append(newString.Category);
            newSearchStringName.Append(newString.SearchStringName);
            newDefinition.Append(newString.Definition);

            oldCategory.Append("'");
            oldSearchStringName.Append("'");
            oldDefinition.Append("'");
            newCategory.Append("'");
            newSearchStringName.Append("'");
            newDefinition.Append("'");

            string query = String.Format(UPDATE_String, CoreConstants.RepositoryDatabase, newCategory,
                newSearchStringName, newDefinition, oldCategory, oldSearchStringName, oldDefinition);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteScalar();
            }

            query = String.Format(UPDATE_SensitiveColumnStrings, CoreConstants.RepositoryDatabase, newCategory,
                newSearchStringName, newDefinition, oldCategory, oldSearchStringName, oldDefinition);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteScalar();
            }
        }

        public void ActivateProfile(SqlConnection conn, string profileName)
        {
            StringBuilder activateProfileName = new StringBuilder("'" + profileName + "'");
            string queryToDeactivate = String.Format(UPDATE_DeactivateOldProfile, CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(queryToDeactivate, conn))
            {
                cmd.ExecuteScalar();
            }
            string queryToActivate = String.Format(UPDATE_ActivateProfile, CoreConstants.RepositoryDatabase, activateProfileName);
            using (SqlCommand cmd = new SqlCommand(queryToActivate, conn))
            {
                cmd.ExecuteScalar();
            }
        }

        public void ResetData(SqlConnection conn)
        {
            string query = String.Format(DELETE_ResetData, CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteScalar();
            }
        }

        public void InsertNewProfile(SqlConnection conn, List<ProfilerObject> newProfileData)
        {
            string queryToDeactivate = String.Format(UPDATE_DeactivateOldProfile, CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(queryToDeactivate, conn))
            {
                cmd.ExecuteScalar();
            }
            foreach (ProfilerObject profile in newProfileData)
            {
                StringBuilder profileName = new StringBuilder("'");
                StringBuilder category = new StringBuilder("'");
                StringBuilder searchStringName = new StringBuilder("'");
                StringBuilder definition = new StringBuilder("'");
                StringBuilder isStringChecked = new StringBuilder("'");
                profileName.Append(profile.ProfileName + "'");
                category.Append(profile.Category + "'");
                searchStringName.Append(profile.SearchStringName + "'");
                definition.Append(profile.Definition + "'");
                isStringChecked.Append(profile.IsStringChecked?'1':'0');
                isStringChecked.Append("'");
                string query = String.Format(INSERT_NewProfile, CoreConstants.RepositoryDatabase, profileName, category,
                    searchStringName, definition, isStringChecked);
                using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteScalar();
                    }
            }
        }

        public void UpdateCurrentProfile(SqlConnection conn, List<ProfilerObject> updatedProfileDetails)
        {
            StringBuilder currentProfileName = new StringBuilder("'" + updatedProfileDetails[0].ProfileName + "'");
            string queryToDelete = String.Format(DELETE_Profile, CoreConstants.RepositoryDatabase, currentProfileName);
            using (SqlCommand cmd = new SqlCommand(queryToDelete, conn))
            {
                cmd.ExecuteScalar();
            }
            foreach (ProfilerObject profile in updatedProfileDetails)
            {
                StringBuilder profileName = new StringBuilder("'");
                StringBuilder category = new StringBuilder("'");
                StringBuilder searchStringName = new StringBuilder("'");
                StringBuilder definition = new StringBuilder("'");
                StringBuilder isStringChecked = new StringBuilder("'");
                profileName.Append(profile.ProfileName + "'");
                category.Append(profile.Category + "'");
                searchStringName.Append(profile.SearchStringName + "'");
                definition.Append(profile.Definition + "'");
                isStringChecked.Append(profile.IsStringChecked ? '1' : '0');
                isStringChecked.Append("'");
                string query = String.Format(INSERT_NewProfile, CoreConstants.RepositoryDatabase, profileName, category,
                    searchStringName, definition, isStringChecked);
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteScalar();
                }
            }
        }

        public void DeleteProfile(SqlConnection conn, string profileName)
        {
            StringBuilder deleteProfileName = new StringBuilder("'" + profileName + "'");
            string query = String.Format(DELETE_Profile, CoreConstants.RepositoryDatabase, deleteProfileName);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteScalar();
            }
        }


        public void UpdateIsUpdated(SqlConnection conn, string value)
        {
            StringBuilder isUpdated = new StringBuilder("'" + value + "'");
            string queryToActivate = String.Format(UPDATE_IsUpdated, CoreConstants.RepositoryDatabase, isUpdated);
            using (SqlCommand cmd = new SqlCommand(queryToActivate, conn))
            {
                cmd.ExecuteScalar();
            }
        }

        public string GetIsUpdated(SqlConnection conn)
        {
            string record;
            string query = String.Format(SELECT_IsUpdated, CoreConstants.RepositoryDatabase);
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                record = (string)cmd.ExecuteScalar();
            }
            return record;
        }

    }
}
