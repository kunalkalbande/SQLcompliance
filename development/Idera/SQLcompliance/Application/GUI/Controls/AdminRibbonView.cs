using System;
using System.Collections.Generic;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;
using Infragistics.Win.UltraWinToolbars;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class AdminRibbonView : BaseControl
   {          
      public enum Tabs
      {
         RegisteredServers = 0,
         AlertRules = 1,
         EventFilters = 2,
         SqlLogins = 3,
         ActivityLog = 4,
         ChangeLog = 5,
         //start sqlcm 5.6 - 5467
         DefaultSettings=6
         //end sqlcm 5.6 - 5467
      }

      // Change Log
      private CustomTime _changeLogCustomTime ;
      private ChangeLogViewSettings _changeLogViewSettings;
      private ComboBoxTool _comboChangeLogTime;
      private StateButtonTool _checkChangeLogGroupBy;
      private ButtonTool _btnChangeLogExpand;
      private ButtonTool _btnChangeLogCollapse;
      private int _changeLogTimeFilterLastIndex ;

      // Activity Log
      private CustomTime _activityLogCustomTime ;
      private ActivityLogViewSettings _activityLogViewSettings;
      private ComboBoxTool _comboActivityLogTime;
      private StateButtonTool _checkActivityLogGroupBy;
      private ButtonTool _btnActivityLogExpand;
      private ButtonTool _btnActivityLogCollapse;
      private int _activityLogTimeFilterLastIndex ;

      private BaseControl _activeControl ;


      private uint _internalUpdate = 0;


      public AdminRibbonView()
      {
         InitializeComponent();
         _activeControl = _serverView;

         // Changelog View Setup
         _comboChangeLogTime = (ComboBoxTool)_toolbarsManager.Tools["changeLogFilterTime"];
         _checkChangeLogGroupBy = (StateButtonTool)_toolbarsManager.Tools["changeLogShowGroupBy"];
         _btnChangeLogExpand = (ButtonTool)_toolbarsManager.Tools["changeLogExpandAll"];
         _btnChangeLogCollapse = (ButtonTool)_toolbarsManager.Tools["changeLogCollapseAll"];
         InitializeChangeLogView();

         // ActivityLog View Setup
         _comboActivityLogTime = (ComboBoxTool)_toolbarsManager.Tools["activityLogFiltersTime"];
         _checkActivityLogGroupBy = (StateButtonTool)_toolbarsManager.Tools["activityLogShowGroupBy"];
         _btnActivityLogExpand = (ButtonTool)_toolbarsManager.Tools["activityLogExpandAll"];
         _btnActivityLogCollapse = (ButtonTool)_toolbarsManager.Tools["activityLogCollapseAll"];
         InitializeActivityLogView();

         InitializeServerRibbon() ;
         InitializeAlertRuleRibbon() ;
         InitializeEventFilterRibbon() ;
         InitializeLoginRibbon() ;
         InitializeChangeLogRibbon() ;
         InitializeActivityLogRibbon() ;

            //start sqlcm 5.6 - 5467
            InitializeApplyDefaultSettingsRibbon();
            _defaultSettings.SetAdminRibbon(this);
            _toolbarsManager.Tools["applyDatabaseDefault"].ToolClick += ApplyDatabaseSettings;
            _toolbarsManager.Tools["editDatabaseDefault"].ToolClick += EditDatabaseSettings;
            _toolbarsManager.Tools["applyServerDefault"].ToolClick += ApplyServerDefaultPropertise;
            _toolbarsManager.Tools["editServerDefault"].ToolClick += EditServerDefaultPropertise;
            //end sqlcm 5.6 - 5467
        }
        //start sqlcm 5.6 - 5467
        public void ApplyDatabaseSettings(object sender , EventArgs e)
        {
            Form_ApplyDatabaseDefaultAuditSettings frm = new Form_ApplyDatabaseDefaultAuditSettings(_defaultSettings);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }
        public void EditDatabaseSettings(object sender, EventArgs e)
        {
            Form_DefaultDatabaseProperties frm = new Form_DefaultDatabaseProperties();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        public void EditServerDefaultPropertise(object sender, EventArgs e)
        {
            Form_ServerDefaultPropertise frm = new Form_ServerDefaultPropertise();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        public void ApplyServerDefaultPropertise(object sender, EventArgs e)
        {
            Form_ApplyServerDefaultAuditSettings frm = new Form_ApplyServerDefaultAuditSettings(_defaultSettings);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }
        public void DisableApplyDefaultServerSettingsButton()
        {
            _toolbarsManager.Tools["applyServerDefault"].SharedProps.Enabled = false;
        }
        public void EnableApplyDefaultServerSettingsButton()
        {
            _toolbarsManager.Tools["applyServerDefault"].SharedProps.Enabled = true;
        }
        public void DisableApplyDefaultDatabaseSettingsButton()
        {
            _toolbarsManager.Tools["applyDatabaseDefault"].SharedProps.Enabled = false;
        }
        public void EnableApplyDefaultDatabaseSettingsButton()
        {
            _toolbarsManager.Tools["applyDatabaseDefault"].SharedProps.Enabled = true;
        }
        //end sqlcm 5.6 -5467
        public Tabs GetActiveTab()
      {
         if (_activeControl == _serverView)
            return Tabs.RegisteredServers;
         else if (_activeControl == _alertRulesView)
            return Tabs.AlertRules;
         else if (_activeControl == _eventFiltersView)
            return Tabs.EventFilters;
         else if (_activeControl == _loginsView)
            return Tabs.SqlLogins;
         else if (_activeControl == _activityLogView)
            return Tabs.ActivityLog;
         else if (_activeControl == _changeLogView)
            return Tabs.ChangeLog;
         return Tabs.RegisteredServers;
      }

      //
      //  If the AfterRibbonTabSelected event is fired due to programatically setting
      //  the selected tab property, the SelectedTab.Index will not be correct until later.
      //
      private void AfterRibbonTabSelected_toolbarsManager(object sender, RibbonTabEventArgs e)
      {
         if (_internalUpdate > 0)
            return;
         UpdateSelectedTab(e.Tab.Index);
         foreach(CMMenuItem i in Enum.GetValues(typeof(CMMenuItem)))
            SetMenuFlag(i, _activeControl.GetMenuFlag(i)) ;
         _mainForm.NavigateToAdminNode((Tabs)e.Tab.Index, false) ;
      }

      private void UpdateSelectedTab(int tabIndex)
      {
         BaseControl previousControl = _activeControl ;
         BaseControl newControl ;
         switch ((Tabs)tabIndex)
         {
            case Tabs.RegisteredServers:
               newControl = _serverView;
               break;
            case Tabs.AlertRules:
               newControl = _alertRulesView ;
               break;
            case Tabs.EventFilters:
               newControl = _eventFiltersView ;
               break;
            case Tabs.SqlLogins:
               newControl = _loginsView;
               break;
            case Tabs.ChangeLog:
               newControl = _changeLogView;
               break;
            case Tabs.ActivityLog:
               newControl = _activityLogView;
               break;
           //start sqlcm 5.6 - 5467
                case Tabs.DefaultSettings:
                    newControl = _defaultSettings;
                    _defaultSettings.ResetCheckBox();
                    _defaultSettings.UncheckAllDatabases();
                    DisableApplyDefaultDatabaseSettingsButton();
                    DisableApplyDefaultServerSettingsButton();
                    break;
           //end sqlcm 5.6 - 5467
                default:
               return ;
         }
         newControl.Enabled = true ;
         newControl.BringToFront() ;
         if(previousControl != null && previousControl != newControl)
            previousControl.Enabled = false ;
         _activeControl = newControl ;
         _activeControl.Focus() ;
         RefreshView();
      }
        //start sqlcm 5.6 - 5467
        public void ResetCheckBox()
        {
            _defaultSettings.ResetCheckBox();
        }
        public void RefreshDefaultAuditServersDatabases()
        {
            _defaultSettings.GetServers();
            _defaultSettings.InitializeServersOnUI();
            _defaultSettings.InitializeDatabasesOnUI();
        }
        //end sqlcm 5.6 - 5467
      public override void Initialize(Form_Main2 mainForm)
      {
         base.Initialize(mainForm);
         _serverView.Initialize(mainForm) ;
         _alertRulesView.Initialize(mainForm) ;
         _eventFiltersView.Initialize(mainForm) ;
         _loginsView.Initialize(mainForm) ;
         _changeLogView.Initialize(mainForm) ;
         _activityLogView.Initialize(mainForm) ;
            //start sqlcm 5.6 - 5467
         _defaultSettings.Initialize(mainForm);
            //end sqlcm 5.6 - 5467
         mainForm.ConnectionChanged += ConnectionChanged_mainForm;
      }

      public override void UpgradeAgent()
      {
         if (_activeControl != null)
            _activeControl.UpgradeAgent();
      }

      public override void DeployAgent()
      {
         if (_activeControl != null)
            _activeControl.DeployAgent();
      }

      public override void AgentTraceDirectory()
      {
         if(_activeControl != null)
            _activeControl.AgentTraceDirectory() ;
      }

      public override void CheckAgent()
      {
         if (_activeControl != null)
            _activeControl.CheckAgent();
      }

      public override void Enable(bool flag)
      {
         if (_activeControl != null)
            _activeControl.Enable(flag);
      }

      public override void UpdateNow()
      {
         if (_activeControl != null)
            _activeControl.UpdateNow();
      }

      void ConnectionChanged_mainForm(object sender, ConnectionChangedEventArgs e)
      {
         if (e.Repository != null)
            SetButtonPermissions();
      }

      private void SetButtonPermissions()
      {
         // Server View
         _toolbarsManager.Tools["New"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["enableAuditing"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["disableAuditing"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["updateNow"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["importAuditSettings"].SharedProps.Enabled = Globals.isAdmin;

         // Alert Rules
         _toolbarsManager.Tools["alertNew"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["alertTemplate"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["alertEnable"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["alertDisable"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["importAlertRule"].SharedProps.Enabled = Globals.isAdmin;

         // Event Filters
         _toolbarsManager.Tools["filterNew"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["filterTemplate"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["filterEnable"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["filterDisable"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["importFilter"].SharedProps.Enabled = Globals.isAdmin;

         // SQL Logins
         _toolbarsManager.Tools["loginNew"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["loginProperties"].SharedProps.Enabled = Globals.isAdmin;
            //start sqlcm 5.6 - 5467
            _toolbarsManager.Tools["applyServerDefault"].SharedProps.Enabled = false;
            _toolbarsManager.Tools["applyDatabaseDefault"].SharedProps.Enabled = false;
            _toolbarsManager.Tools["editServerDefault"].SharedProps.Enabled = true;
            _toolbarsManager.Tools["editDatabaseDefault"].SharedProps.Enabled =true;
            //end sqlcm 5.6 - 5467
        }

      public override void UpdateColors()
      {
         base.UpdateColors();
         _serverView.UpdateColors() ;
         _alertRulesView.UpdateColors() ;
         _eventFiltersView.UpdateColors() ;
         _loginsView.UpdateColors() ;
         _changeLogView.UpdateColors() ;
         _activityLogView.UpdateColors() ;
         //start sqlcm 5.6 - 5467
         _defaultSettings.UpdateColors();
         //end sqlcm 5.6 - 5467
      }

      public override void Delete()
      {
         BaseControl ctrl = _activeControl ;
         base.Delete();
         if (ctrl != null)
            ctrl.Delete();
      }

      public override void Properties()
      {
         base.Properties();
         BaseControl ctrl = _activeControl;
         if (ctrl != null)
            ctrl.Properties();
      }

      public override void AgentProperties()
      {
         base.AgentProperties();
         BaseControl ctrl = _activeControl ;
         if (ctrl != null)
            ctrl.AgentProperties();
      }

      public override void HelpOnThisWindow()
      {
         base.HelpOnThisWindow();
         BaseControl ctrl = _activeControl;
         if (ctrl != null)
            ctrl.HelpOnThisWindow();
      }

      private void InitializeServerRibbon()
      {
         _toolbarsManager.Tools["New"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["enableAuditing"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.EnableAuditing);
         _toolbarsManager.Tools["disableAuditing"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.DisableAuditing);
         _toolbarsManager.Tools["serverSettings"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.ServerAuditSettings);
         _toolbarsManager.Tools["privilegedUsers"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.PrivilegedUserSettings);
         _toolbarsManager.Tools["databaseSettings"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.DatabaseAuditSettings);
         _toolbarsManager.Tools["trustedUsers"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.TrustedUserSettings);
         _toolbarsManager.Tools["updateNow"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.UpdateAuditSettings);
         _toolbarsManager.Tools["exportAuditSettings"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.ExportAuditSettings);
         _toolbarsManager.Tools["importAuditSettings"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["agentProperties"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.AgentProperties);
         _toolbarsManager.Tools["agentStatus"].SharedProps.Enabled = _serverView.GetMenuFlag(CMMenuItem.CheckAgent);
      }

      private void InitializeAlertRuleRibbon()
      {
         _toolbarsManager.Tools["alertNew"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["alertTemplate"].SharedProps.Enabled = Globals.isAdmin && _alertRulesView.GetMenuFlag(CMMenuItem.Properties);
         _toolbarsManager.Tools["alertEnable"].SharedProps.Enabled = _alertRulesView.GetMenuFlag(CMMenuItem.EnableRule);
         _toolbarsManager.Tools["alertDisable"].SharedProps.Enabled = _alertRulesView.GetMenuFlag(CMMenuItem.DisableRule);
         _toolbarsManager.Tools["importAlertRule"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["exportAlertRule"].SharedProps.Enabled = true ;
         _toolbarsManager.Tools["alertDetails"].SharedProps.Enabled = _alertRulesView.GetMenuFlag(CMMenuItem.Properties);
         _toolbarsManager.Tools["alertDetails"].SharedProps.Caption = Globals.isAdmin ? "Edit Details" : "View Details";
         _toolbarsManager.Tools["deleteRule"].SharedProps.Enabled = _alertRulesView.GetMenuFlag(CMMenuItem.Delete);
      }

      private void InitializeEventFilterRibbon()
      {
         _toolbarsManager.Tools["filterNew"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["filterTemplate"].SharedProps.Enabled = Globals.isAdmin && _eventFiltersView.GetMenuFlag(CMMenuItem.Properties);
         _toolbarsManager.Tools["filterEnable"].SharedProps.Enabled = _eventFiltersView.GetMenuFlag(CMMenuItem.EnableRule);
         _toolbarsManager.Tools["filterDisable"].SharedProps.Enabled = _eventFiltersView.GetMenuFlag(CMMenuItem.DisableRule);
         _toolbarsManager.Tools["importFilter"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["exportFilter"].SharedProps.Enabled = true;
         _toolbarsManager.Tools["filterDetails"].SharedProps.Enabled = _eventFiltersView.GetMenuFlag(CMMenuItem.Properties);
      }

      private void InitializeLoginRibbon()
      {
         _toolbarsManager.Tools["loginNew"].SharedProps.Enabled = Globals.isAdmin;
         _toolbarsManager.Tools["loginProperties"].SharedProps.Enabled = _loginsView.GetMenuFlag(CMMenuItem.Properties);
      }
        //start sqlcm 5.6 -5467
        private void InitializeApplyDefaultSettingsRibbon()
        {
            _toolbarsManager.Tools["applyServerDefault"].SharedProps.Enabled = false;
            _toolbarsManager.Tools["applyDatabaseDefault"].SharedProps.Enabled = false;
            _toolbarsManager.Tools["editServerDefault"].SharedProps.Enabled = true;
            _toolbarsManager.Tools["editDatabaseDefault"].SharedProps.Enabled = true;
     
        }
        //end sqlcm 5.6 - 5467
        private void InitializeChangeLogRibbon()
      {
         ((StateButtonTool)_toolbarsManager.Tools["changeLogShowGroupBy"]).Checked = false;
         _toolbarsManager.Tools["changeLogExpandAll"].SharedProps.Enabled = false;
         _toolbarsManager.Tools["changeLogCollapseAll"].SharedProps.Enabled = false ;
      }

      private void InitializeActivityLogRibbon()
      {
         ((StateButtonTool)_toolbarsManager.Tools["activityLogShowGroupBy"]).Checked = false;
         _toolbarsManager.Tools["activityLogExpandAll"].SharedProps.Enabled = false;
         _toolbarsManager.Tools["activityLogCollapseAll"].SharedProps.Enabled = false;
      }

      public override bool GetMenuFlag(CMMenuItem item)
      {
         BaseControl ctrl = _activeControl;

         if (ctrl != null)
            return ctrl.GetMenuFlag(item);
         else
            return base.GetMenuFlag(item);
      }


      private void MenuFlagChanged_child(object sender, MenuFlagChangedEventArgs e)
      {
         OnMenuFlagChanged(e);
      }

      protected override void OnMenuFlagChanged(MenuFlagChangedEventArgs e)
      {
         base.OnMenuFlagChanged(e);
         switch (e.TargetMenuItem)
         {
            case CMMenuItem.All:
               break;
            case CMMenuItem.Refresh:
               break;
            case CMMenuItem.Cut:
               break;
            case CMMenuItem.Copy:
               break;
            case CMMenuItem.Paste:
               break;
            case CMMenuItem.Delete:
               if (_activeControl == _alertRulesView)
               {
                  _toolbarsManager.Tools["deleteRule"].SharedProps.Enabled = e.Enabled;
               }
               break;
            case CMMenuItem.Rename:
               break;
            case CMMenuItem.Properties:
               if(_activeControl == _alertRulesView)
               {
                  _toolbarsManager.Tools["alertDetails"].SharedProps.Enabled = e.Enabled;
               }
               else if (_activeControl == _eventFiltersView)
               {
                  _toolbarsManager.Tools["filterDetails"].SharedProps.Enabled = e.Enabled;
               }
               else if (_activeControl == _loginsView)
               {
                  _toolbarsManager.Tools["loginProperties"].SharedProps.Enabled = e.Enabled;
               }
               break;
            case CMMenuItem.EnableAuditing:
               _toolbarsManager.Tools["enableAuditing"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.DisableAuditing:
               _toolbarsManager.Tools["disableAuditing"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.SetFilter:
               break;
            case CMMenuItem.Print:
               break;
            case CMMenuItem.PrintPreview:
               break;
            case CMMenuItem.PageSetup:
               break;
            case CMMenuItem.NewServer:
               _toolbarsManager.Tools["newServer"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.NewDatabase:
               _toolbarsManager.Tools["newDatabase"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.NewAlertRule:
               _toolbarsManager.Tools["alertNew"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.NewStatusAlertRule:
               _toolbarsManager.Tools["addStatusRule"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.NewDataAlertRule:
               _toolbarsManager.Tools["newDataAlert"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.NewLogin:
               _toolbarsManager.Tools["loginNew"].SharedProps.Enabled = e.Enabled;
               break;
                //start sqlcm 5.6 - 5467
               case CMMenuItem.DefaultServerSettings:
                    _toolbarsManager.Tools["applyServerDefault"].SharedProps.Enabled = false;
                    break;
                case CMMenuItem.DefaultDatabaseSettings:
                    _toolbarsManager.Tools["applyDatabaseDefault"].SharedProps.Enabled =false;
                    break;
                case CMMenuItem.EditServerSettings:
                    _toolbarsManager.Tools["editServerDefault"].SharedProps.Enabled = e.Enabled;
                    break;
                case CMMenuItem.EditDatabaseSettings:
                    _toolbarsManager.Tools["editDatabaseDefault"].SharedProps.Enabled = e.Enabled;
                    break;
                //end sqlcm 5.6 - 5467
                case CMMenuItem.Collapse:
               break;
            case CMMenuItem.Expand:
               break;
            case CMMenuItem.GroupByColumn:
               break;
            case CMMenuItem.ServerAuditSettings:
               _toolbarsManager.Tools["Server Settings"].SharedProps.Enabled = e.Enabled &&
                  _toolbarsManager.Tools["privilegedUsers"].SharedProps.Enabled ;
               _toolbarsManager.Tools["serverSettings"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.PrivilegedUserSettings:
               _toolbarsManager.Tools["Server Settings"].SharedProps.Enabled = e.Enabled &&
                  _toolbarsManager.Tools["serverSettings"].SharedProps.Enabled;
               _toolbarsManager.Tools["privilegedUsers"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.DatabaseAuditSettings:
               _toolbarsManager.Tools["Database Settings"].SharedProps.Enabled = e.Enabled &&
                  _toolbarsManager.Tools["trustedUsers"].SharedProps.Enabled;
               _toolbarsManager.Tools["databaseSettings"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.TrustedUserSettings:
               _toolbarsManager.Tools["Database Settings"].SharedProps.Enabled = e.Enabled &&
                  _toolbarsManager.Tools["databaseSettings"].SharedProps.Enabled;
               _toolbarsManager.Tools["trustedUsers"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.DeployAgent:
               break;
            case CMMenuItem.CheckAgent:
               _toolbarsManager.Tools["agentStatus"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.UpdateAuditSettings:
               _toolbarsManager.Tools["updateNow"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.Groom:
               break;
            case CMMenuItem.Snapshot:
               break;
            case CMMenuItem.AgentProperties:
               _toolbarsManager.Tools["agentProperties"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ShowHelp:
               break;
            case CMMenuItem.ForceCollection:
               break;
            case CMMenuItem.UpgradeAgent:
               break;
            case CMMenuItem.ChangeAgentTraceDir:
               break;
            case CMMenuItem.ViewTasks:
               break;
            case CMMenuItem.NewEventFilter:
               _toolbarsManager.Tools["filterNew"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.AttachArchive:
               break;
            case CMMenuItem.DetachArchive:
               break;
            case CMMenuItem.ExportAuditSettings:
               _toolbarsManager.Tools["exportAuditSettings"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ImportAuditSettings:
               _toolbarsManager.Tools["importAuditSettings"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.LoadReport:
               break;
            case CMMenuItem.UseRuleAsTemplate:
               if(_activeControl == _alertRulesView)
                  _toolbarsManager.Tools["alertTemplate"].SharedProps.Enabled = e.Enabled;
               else if (_activeControl == _eventFiltersView)
                  _toolbarsManager.Tools["filterTemplate"].SharedProps.Enabled = e.Enabled;
               break; 
            case CMMenuItem.EnableRule:
               if (_activeControl == _alertRulesView)
                  _toolbarsManager.Tools["alertEnable"].SharedProps.Enabled = e.Enabled;
               else if (_activeControl == _eventFiltersView)
                  _toolbarsManager.Tools["filterEnable"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.DisableRule:
               if (_activeControl == _alertRulesView)
                  _toolbarsManager.Tools["alertDisable"].SharedProps.Enabled = e.Enabled;
               else if (_activeControl == _eventFiltersView)
                  _toolbarsManager.Tools["filterDisable"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ExportAlertRules:
               _toolbarsManager.Tools["exportAlertRule"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ImportAlertRules:
               _toolbarsManager.Tools["importAlertRule"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ExportEventFilters:
               _toolbarsManager.Tools["exportFilter"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ImportEventFilters:
               _toolbarsManager.Tools["importFilter"].SharedProps.Enabled = e.Enabled;
               break;
            case CMMenuItem.ImportCSV:
               _toolbarsManager.Tools["importCSV"].SharedProps.Enabled = e.Enabled;
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public override void RefreshView()
      {
         if(_activeControl != null)
         {
            _activeControl.Focus() ;
            _activeControl.RefreshView();
         }
         return ;
      }

      public void ShowTab(Tabs tab)
      {
         _toolbarsManager.Ribbon.SelectedTab = _toolbarsManager.Ribbon.Tabs[(int)tab];
      }

      public void SetAlertingConfiguration(AlertingConfiguration config)
      {
         _alertRulesView.Configuration = config ;
      }

      public void SetFiltersConfiguration(FiltersConfiguration config)
      {
         _eventFiltersView.Configuration = config ;
      }

      private void ToolClick_toolbarsManager(object sender, ToolClickEventArgs e)
      {
         switch (e.Tool.Key)
         {
            case "newServer":
               _mainForm.AddServerAction();
               break;
            case "newDatabase":
               _mainForm.AddDatabaseAction(_serverView.GetSelectedServer());
               break;
            case "enableAuditing":
               _serverView.Enable(true) ;
               break;
            case "disableAuditing":
               _serverView.Enable(false);
               break;
            case "updateNow":
               _serverView.UpdateNow();
               break;
            case "exportAuditSettings":
               _serverView.ExportAuditSettings() ;
               break;
            case "importAuditSettings":
               _mainForm.ImportAuditSettingsAction() ;
               break;
            case "collectNow":
               break ;
            case "agentDeploy":
               _serverView.DeployAgent();
               break;
            case "agentUpgrade":
               _serverView.UpgradeAgent();
               break;
            case "agentStatus":
               _serverView.CheckAgent();
               break;
            case "agentTraceDirectory":
               _serverView.AgentTraceDirectory() ;
               break;
            case "agentProperties":
               AgentProperties() ;
               break;
            case "changeLogShowGroupBy":
               _changeLogView.ShowGroupBy = _checkChangeLogGroupBy.Checked;
               _btnChangeLogCollapse.SharedProps.Enabled = _checkChangeLogGroupBy.Checked;
               _btnChangeLogExpand.SharedProps.Enabled = _checkChangeLogGroupBy.Checked;
               break;
            case "changeLogCollapseAll":
               _changeLogView.CollapseAll();
               break;
            case "changeLogExpandAll":
               _changeLogView.ExpandAll();
               break;
            case "activityLogShowGroupBy":
               _activityLogView.ShowGroupBy = _checkActivityLogGroupBy.Checked;
               _btnActivityLogCollapse.SharedProps.Enabled = _checkActivityLogGroupBy.Checked;
               _btnActivityLogExpand.SharedProps.Enabled = _checkActivityLogGroupBy.Checked;
               break;
            case "activityLogCollapseAll":
               _activityLogView.CollapseAll();
               break;
            case "activityLogExpandAll":
               _activityLogView.ExpandAll();
               break;
            case "alertNew":
               _alertRulesView.NewAlertRule() ;
               break ;
            case "alertTemplate":
               _alertRulesView.NewAlertRuleFromTemplate() ;
               break;
            case "alertEnable":
               _alertRulesView.Enable(true) ;
               break;
            case "alertDisable":
               _alertRulesView.Enable(false);
               break;
            case "deleteRule":
               _alertRulesView.Delete();
               break;
            case "addStatusRule":
               _alertRulesView.NewStatusAlertRule();
               break;
            case "newDataAlert":
               _alertRulesView.NewDataAlertRule();
               break;
            case "alertDetails":
               _alertRulesView.Properties();
               break;
            case "eventRulesFilter":
               _alertRulesView.DisplayEventRules = ((StateButtonTool)_toolbarsManager.Tools["eventRulesFilter"]).Checked;
               _alertRulesView.RefreshView();
               break;
            case "statusRulesFilter":
               _alertRulesView.DisplayStatusRules = ((StateButtonTool)_toolbarsManager.Tools["statusRulesFilter"]).Checked;
               _alertRulesView.RefreshView();
               break;
            case "dataRulesFilter":
               _alertRulesView.DisplayDataRules = ((StateButtonTool)_toolbarsManager.Tools["dataRulesFilter"]).Checked;
               _alertRulesView.RefreshView();
               break;
            case "filterNew":
               _eventFiltersView.NewEventFilter() ;
               break;
            case "filterTemplate":
               _eventFiltersView.NewEventFilterFromTemplate();
               break;
            case "filterEnable":
               _eventFiltersView.Enable(true);
               break;
            case "filterDisable":
               _eventFiltersView.Enable(false);
               break;
            case "filterDetails":
               _eventFiltersView.Properties();
               break;
            case "loginNew":
               _mainForm.NewLoginAction() ;
               break;
            case "privilegedUsers":
               _serverView.UserProperties() ;
               break;
            case "trustedUsers":
               _serverView.UserProperties() ;
               break;
            case "serverSettings":
               _serverView.Properties() ;
               break;
            case "databaseSettings":
               _serverView.Properties();
               break;
            case "importAlertRule":
               _mainForm.ImportAlertRulesAction() ;
               break;
            case "exportAlertRule":
               _mainForm.ExportAlertRulesAction() ;
               break;
            case "importFilter":
               _mainForm.ImportEventFiltersAction() ;
               break;
            case "exportFilter":
               _mainForm.ExportEventFiltersAction() ;
               break;
            case "loginProperties":
               _loginsView.Properties() ;
               break;
         }
      }


      private void ToolValueChanged_toolbarsManager(object sender, ToolEventArgs e)
      {
         switch(e.Tool.Key)
         {
            case "changeLogFilterTime":
               ValueChanged_ChangeLogTime();
               break;
            case "activityLogFiltersTime":
               ValueChanged_ActivityLogTime();
               break;
         }
      }

      #region Change Log Ribbon

      private void InitializeChangeLogView()
      {
         _changeLogCustomTime = new CustomTime() ;

         List<ChangeLogViewSettings> views = Settings.Default.ChangeLogViews;
         if (views.Count == 0)
         {
            views.Add(Settings.Default.AppDefaultChangeLogView);
         }
         _changeLogViewSettings = views[0] ;
         ApplyChangeLogView(_changeLogViewSettings);
      }

      private void ValueChanged_ChangeLogTime()
      {
         if (_internalUpdate > 0)
            return;
         ChangeLogViewFilter filter = _changeLogView.Filter;

         switch (_comboChangeLogTime.SelectedIndex)
         {
            case 0:
               filter.DateLimitType = DateFilterType.Unlimited;
               break;
            case 1:
               filter.DateLimitType = DateFilterType.Today;
               break;
            case 2:
               filter.DateLimitType = DateFilterType.NumberDays;
               filter.Days = 7;
               break;
            case 3:
               filter.DateLimitType = DateFilterType.NumberDays;
               filter.Days = 30;
               break;
            case 4:
               _changeLogCustomTime.GetCustomTime(_comboChangeLogTime, _changeLogTimeFilterLastIndex) ;
               break;
            default:
               {
                  // custom
                  int index = _comboChangeLogTime.SelectedIndex - 5;
                  filter.DateLimitType = DateFilterType.DateRange;
                  filter.StartDate = _changeLogCustomTime.StartTime[index].Value;
                  filter.EndDate = _changeLogCustomTime.EndTime[index].Value;
               }
               break;
         }
         _changeLogTimeFilterLastIndex = _comboChangeLogTime.SelectedIndex ;
         RefreshView();
      }

      private void ApplyChangeLogView(ChangeLogViewSettings settings)
      {
         if(settings == null)
            return ;
         _internalUpdate++;
         try
         {
            _changeLogView.Filter = settings.Filter;
            _changeLogView.ShowGroupBy = settings.ShowGroupBy;
         }
         finally
         {
            _internalUpdate--;
         }
         UpdateChangeLogFilterDisplay();
      }

      private void UpdateChangeLogFilterDisplay()
      {
         _internalUpdate++;

         try
         {
            switch (_changeLogView.Filter.DateLimitType)
            {
               case DateFilterType.Unlimited:
                  _comboChangeLogTime.SelectedIndex = 0;
                  break;
               case DateFilterType.NumberDays:
                  if (_changeLogView.Filter.Days == 7)
                     _comboChangeLogTime.SelectedIndex = 2;
                  else if (_changeLogView.Filter.Days == 30)
                     _comboChangeLogTime.SelectedIndex = 3;
                  else
                     _comboChangeLogTime.SelectedIndex = 4;
                  break;
               case DateFilterType.DateRange:
                  _changeLogCustomTime.AddCustomRange(_comboChangeLogTime,
                     _changeLogView.Filter.StartDate, _changeLogView.Filter.EndDate) ;
                  _comboChangeLogTime.SelectedIndex = 5 ;
                  break;
               case DateFilterType.Today:
                  _comboChangeLogTime.SelectedIndex = 1;
                  break;
            }
            _changeLogTimeFilterLastIndex = _comboChangeLogTime.SelectedIndex ;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("UpdateFilterDisplay", e);
         }
         _internalUpdate--;
      }

      private void ShowGroupByChanged_changeLogView(object sender, ToggleChangedEventArgs e)
      {
         if (e.Enabled != _checkChangeLogGroupBy.Checked)
         {
            _changeLogViewSettings.ShowGroupBy = e.Enabled ;
            _checkChangeLogGroupBy.InitializeChecked(e.Enabled);
            _btnChangeLogExpand.SharedProps.Enabled = e.Enabled;
            _btnChangeLogCollapse.SharedProps.Enabled = e.Enabled;
            SetMenuFlag(CMMenuItem.Collapse, e.Enabled);
            SetMenuFlag(CMMenuItem.Expand, e.Enabled);
         }
      }

      #endregion Change Log Ribbon

      #region Activity Log Ribbon

      private void InitializeActivityLogView()
      {
         _activityLogCustomTime = new CustomTime() ;

         List<ActivityLogViewSettings> views = Settings.Default.ActivityLogViews;
         if (views.Count == 0)
         {
            views.Add(Settings.Default.AppDefaultActivityLogView);
         }
         _activityLogViewSettings = views[0];
         ApplyActivityLogView(_activityLogViewSettings);
      }

      private void ValueChanged_ActivityLogTime()
      {
         if (_internalUpdate > 0)
            return;
         ActivityLogViewFilter filter = _activityLogView.Filter;

         switch (_comboActivityLogTime.SelectedIndex)
         {
            case 0:
               filter.DateLimitType = DateFilterType.Unlimited;
               break;
            case 1:
               filter.DateLimitType = DateFilterType.Today;
               break;
            case 2:
               filter.DateLimitType = DateFilterType.NumberDays;
               filter.Days = 7;
               break;
            case 3:
               filter.DateLimitType = DateFilterType.NumberDays;
               filter.Days = 30;
               break;
            case 4:
               _activityLogCustomTime.GetCustomTime(_comboActivityLogTime, _activityLogTimeFilterLastIndex) ;
               break;
            default:
               {
                  // custom
                  int index = _comboActivityLogTime.SelectedIndex - 5;
                  filter.DateLimitType = DateFilterType.DateRange;
                  filter.StartDate = _activityLogCustomTime.StartTime[index].Value;
                  filter.EndDate = _activityLogCustomTime.EndTime[index].Value;
               }
               break;
         }
         _activityLogTimeFilterLastIndex = _comboActivityLogTime.SelectedIndex ;
         RefreshView();
      }

      private void ApplyActivityLogView(ActivityLogViewSettings settings)
      {
         if (settings == null)
            return;
         _internalUpdate++;
         try
         {
            _activityLogView.Filter = settings.Filter;
            _activityLogView.ShowGroupBy = settings.ShowGroupBy;
         }
         finally
         {
            _internalUpdate--;
         }
         UpdateActivityLogFilterDisplay();
      }

      private void UpdateActivityLogFilterDisplay()
      {
         _internalUpdate++;

         try
         {
            switch (_activityLogView.Filter.DateLimitType)
            {
               case DateFilterType.Unlimited:
                  _comboActivityLogTime.SelectedIndex = 0;
                  break;
               case DateFilterType.NumberDays:
                  if (_activityLogView.Filter.Days == 7)
                     _comboActivityLogTime.SelectedIndex = 2;
                  else if (_activityLogView.Filter.Days == 30)
                     _comboActivityLogTime.SelectedIndex = 3;
                  else
                     _comboActivityLogTime.SelectedIndex = 4;
                  break;
               case DateFilterType.DateRange:
                  _activityLogCustomTime.AddCustomRange(_comboActivityLogTime, _activityLogView.Filter.StartDate,
                     _activityLogView.Filter.EndDate);
                  _comboActivityLogTime.SelectedIndex = 5;
                  break;
               case DateFilterType.Today:
                  _comboActivityLogTime.SelectedIndex = 1;
                  break;
            }
            _activityLogTimeFilterLastIndex = _comboActivityLogTime.SelectedIndex ;
         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("UpdateFilterDisplay", e);
         }
         _internalUpdate--;
      }

      private void ShowGroupByChanged_activityLogView(object sender, ToggleChangedEventArgs e)
      {
         if (e.Enabled != _checkActivityLogGroupBy.Checked)
         {
            _activityLogViewSettings.ShowGroupBy = e.Enabled;
            _checkActivityLogGroupBy.InitializeChecked(e.Enabled);
            _btnActivityLogExpand.SharedProps.Enabled = e.Enabled;
            _btnActivityLogCollapse.SharedProps.Enabled = e.Enabled;
            SetMenuFlag(CMMenuItem.Collapse, e.Enabled);
            SetMenuFlag(CMMenuItem.Expand, e.Enabled);
         }
      }

      #endregion Change Log Ribbon

      public ServerRecord GetSelectedServer()
      {
         if(_activeControl == _serverView)
            return _serverView.GetSelectedServer() ;
         else
            return null ;
      }

      private void BeforeToolbarListDropdown_toolbarsManager(object sender, BeforeToolbarListDropdownEventArgs e)
      {
         e.ShowQuickAccessToolbarAddRemoveMenuItem = false;
         e.ShowQuickAccessToolbarPositionMenuItem = false;
      }
   }
}
