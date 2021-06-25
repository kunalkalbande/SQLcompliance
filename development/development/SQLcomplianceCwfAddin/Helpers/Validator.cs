using System;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings;

namespace SQLcomplianceCwfAddin.Helpers
{
    internal class Validator
    {
        #region members

        private static Validator _instance;

        #endregion

        #region constructor\destructor

        private Validator() { }

        #endregion

        #region properties

        public static Validator Instance
        {
            get { return _instance ?? (_instance = new Validator()); }
        }

        #endregion

        public void AssertIsValid(UserSettingsModel userSettings)
        {
            if (!string.IsNullOrEmpty(userSettings.Email) && !EmailHelper.IsValidEmail(userSettings.Email)) 
                throw new Exception("Invalid email address.");

            if (userSettings.SessionTimout.HasValue)
            {
                if (userSettings.SessionTimout.Value < TimeSpan.FromSeconds(30).TotalMilliseconds || userSettings.SessionTimout.Value> TimeSpan.FromDays(1).TotalMilliseconds)
                throw new Exception("SessionTimout should be in range from 30 seconds to 23:59 hours.");
            }

            if (userSettings.Subscribed && string.IsNullOrEmpty(userSettings.Email))
                throw new Exception("Email should be specified  if user is subscribed.");
        }
    }
}
