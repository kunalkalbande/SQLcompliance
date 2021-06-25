using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Idera.SQLcompliance.Core.Properties 
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings 
    {
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

       [ApplicationScopedSetting]
       public Dictionary<string, string> AssemblyDirectories
       {
          get
          {
             Dictionary<string, string> retVal = (Dictionary<string, string>)this["AssemblyDirectories"];

             if (retVal == null)
             {
                //if the values doesn't exist in the config file, create an empty one.
                retVal = new Dictionary<string, string>();
                this["AssemblyDirectories"] = retVal;
             }
             return retVal;
          }
          set { this["AssemblyDirectories"] = value; }
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
