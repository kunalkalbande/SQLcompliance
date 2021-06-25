using System ;
using System.Collections.Generic ;
using System.Configuration ;
using System.Xml ;
using System.Xml.Serialization ;
using System.Xml.XPath ;
using Idera.SQLcompliance.Application.GUI.Helper ;

namespace Idera.SQLcompliance.Application.GUI
{
   // This class allows you to handle specific events on the settings class:
   //  The SettingChanging event is raised before a setting's value is changed.
   //  The PropertyChanged event is raised after a setting's value is changed.
   //  The SettingsLoaded event is raised after the setting values are loaded.
   //  The SettingsSaving event is raised before the setting values are saved.
   internal sealed partial class Settings
   {
      public Settings()
      {
         // // To add event handlers for saving and changing settings, uncomment the lines below:
         //
         // this.SettingChanging += this.SettingChangingEventHandler;
         //
         // this.SettingsSaving += this.SettingsSavingEventHandler;
         //
      }

      private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
      {
         // Add code to handle the SettingChangingEvent event here.
      }

      private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
      {
         // Add code to handle the SettingsSaving event here.
      }

      [ApplicationScopedSetting]
      public EventViewSettings AppDefaultEventView
      {
         get {
            object o = ConfigurationManager.GetSection("AppDefaultEventView");
            return (EventViewSettings)o;
         }
      }

      [UserScopedSetting]
      public List<EventViewSettings> EventViews
      {
         get 
         {
            List<EventViewSettings> retVal = (List<EventViewSettings>)this["EventViews"];

            if (retVal == null)
            {
               retVal = new List<EventViewSettings>() ;
               this["EventViews"] = retVal;
            }
            return retVal;
         }

         set { this["EventViews"] = value; }
      }

      [ApplicationScopedSetting]
      public AlertViewSettings AppDefaultAlertView
      {
         get
         {
            object o = ConfigurationManager.GetSection("AppDefaultAlertView");
            return (AlertViewSettings)o;
         }
      }

      [UserScopedSetting]
      public List<AlertViewSettings> AlertViews
      {
         get
         {
            List<AlertViewSettings> retVal = (List<AlertViewSettings>)this["AlertViews"];

            if (retVal == null)
            {
               retVal = new List<AlertViewSettings>();
               this["AlertViews"] = retVal;
            }
            return retVal;
         }

         set { this["AlertViews"] = value; }
      }

      [ApplicationScopedSetting]
      public DataAlertViewSettings AppDefaultDataAlertView
      {
         get
         {
            object o = ConfigurationManager.GetSection("AppDefaultDataAlertView");
            return (DataAlertViewSettings)o;
         }
      }

      [UserScopedSetting]
      public List<DataAlertViewSettings> DataAlertViews
      {
         get
         {
            List<DataAlertViewSettings> retVal = (List<DataAlertViewSettings>)this["DataAlertViews"];

            if (retVal == null)
            {
               retVal = new List<DataAlertViewSettings>();
               this["DataAlertViews"] = retVal;
            }
            return retVal;
         }
         set { this["DataAlertViews"] = value; }
      }



      [ApplicationScopedSetting]
      public StatusAlertViewSettings AppDefaultStatusAlertView
      {
         get
         {
            object o = ConfigurationManager.GetSection("AppDefaultStatusAlertView");
            return (StatusAlertViewSettings)o;
         }
      }

      [UserScopedSetting]
      public List<StatusAlertViewSettings> StatusAlertViews
      {
         get 
         {
            List<StatusAlertViewSettings> retVal = (List<StatusAlertViewSettings>)this["StatusAlertViews"];

            if (retVal == null)
            {
               retVal = new List<StatusAlertViewSettings>();
               this["StatusAlertViews"] = retVal;
            }
            return retVal;
         }
         set { this["StatusAlertViews"] = value; }
      }

      [ApplicationScopedSetting]
      public ChangeLogViewSettings AppDefaultChangeLogView
      {
         get
         {
            object o = ConfigurationManager.GetSection("AppDefaultChangeLogView");
            return (ChangeLogViewSettings)o;
         }
      }

      [UserScopedSetting]
      public List<ChangeLogViewSettings> ChangeLogViews
      {
         get
         {
            List<ChangeLogViewSettings> retVal = (List<ChangeLogViewSettings>)this["ChangeLogViews"];

            if (retVal == null)
            {
               retVal = new List<ChangeLogViewSettings>();
               this["ChangeLogViews"] = retVal;
            }
            return retVal;
         }

         set { this["ChangeLogViews"] = value; }
      }

      [ApplicationScopedSetting]
      public ActivityLogViewSettings AppDefaultActivityLogView
      {
         get
         {
            object o = ConfigurationManager.GetSection("AppDefaultActivityLogView");
            return (ActivityLogViewSettings)o;
         }
      }

      [UserScopedSetting]
      public List<ActivityLogViewSettings> ActivityLogViews
      {
         get
         {
            List<ActivityLogViewSettings> retVal = (List<ActivityLogViewSettings>)this["ActivityLogViews"];

            if (retVal == null)
            {
               retVal = new List<ActivityLogViewSettings>();
               this["ActivityLogViews"] = retVal;
            }
            return retVal;
         }

         set { this["ActivityLogViews"] = value; }
      }

      [UserScopedSetting]
      public List<string> RecentServers
      {
         get 
         { 
            List<string> retVal = (List<string>)this["RecentServers"] ;
            if(retVal == null)
            {
               retVal = new List<string>() ;
               this["RecentServers"] = retVal ;
            }
            return retVal ;
         }

         set { this["RecentServers"] = value; }
      }
   }

   public class ConfigSectionHandler : IConfigurationSectionHandler
   {
      public object Create(object parent, object configContext, System.Xml.XmlNode section)
      {
         XPathNavigator xNav = section.CreateNavigator();
         string typeOfObject = (string)xNav.Evaluate("string(@type)");
         Type t = Type.GetType(typeOfObject);
         XmlSerializer ser = new XmlSerializer(t);
         XmlNodeReader xNodeReader = new XmlNodeReader(section.FirstChild);
         return ser.Deserialize(xNodeReader);
      }
   }
}