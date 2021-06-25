using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core.Stats;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    public class SensitiveColumnProfileHelper
    {
        public IEnumerable<string> GetProfiles()
        {
            return ProfilerObject.GetProfiles(Globals.Repository.Connection);
        }

        public string GetActiveProfile()
        {
            return ProfilerObject.GetActiveProfile(Globals.Repository.Connection);
        }

        public void SetActiveProfile(string profileName)
        {
            new ProfilerObject().ActivateProfile(Globals.Repository.Connection, profileName);
        }
        
        public IEnumerable<ProfilerObject> GetActiveProfileDetails()
        {
            var profileDetails = ProfilerObject.GetProfileDetails(Globals.Repository.Connection).GroupBy(p => new { p.ProfileName, p.IsProfileActive} );
            var activeProfile = profileDetails.SingleOrDefault(g => g.Key.IsProfileActive);
            if (activeProfile != null)
            {
                return activeProfile;
            }

            var emptyProfile = profileDetails.SingleOrDefault(g => string.IsNullOrWhiteSpace(g.Key.ProfileName));
            if (emptyProfile != null)
            {
                return emptyProfile;
            }

            return profileDetails.First().Select(po => new ProfilerObject()
            {
                Category = po.Category,
                SearchStringName = po.SearchStringName,
                Definition = po.Definition
            });
        }

        public void CreateNewProfile(string profileName, IEnumerable<ProfilerObject> items)
        {
            foreach (var profilerObject in items)
            {
                profilerObject.ProfileName = profileName;
            }
            new ProfilerObject().InsertNewProfile(Globals.Repository.Connection, items.ToList());
        }

        public void UpdateProfile(IEnumerable<ProfilerObject> items)
        {
            new ProfilerObject().UpdateCurrentProfile(Globals.Repository.Connection, items.ToList());
        }
        
        public void DeleteProfile(string profileName)
        {
            new ProfilerObject().DeleteProfile(Globals.Repository.Connection, profileName);
        }

        public void DeleteAllProfiles()
        {
            new ProfilerObject().ResetData(Globals.Repository.Connection);
        }

        public void SaveOrUpdateSearchString(ProfilerObject newString, ProfilerObject oldString = null)
        {
            if (oldString == null)
            {
                new ProfilerObject().InsertString(Globals.Repository.Connection, newString);
            }
            else
            {
                new ProfilerObject().UpdateString(Globals.Repository.Connection, new List<ProfilerObject>()
                {
                    oldString,
                    newString
                });
            }
        }

        public void DeleteSearchString(ProfilerObject stringToDelete)
        {
            new ProfilerObject().DeleteString(Globals.Repository.Connection, stringToDelete);
        }
    }
}
