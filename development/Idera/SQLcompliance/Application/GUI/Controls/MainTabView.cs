using System;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Qios.DevSuite.Components;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class MainTabView : BaseControl
   {
      public enum Tabs
      {
         Summary = 0,
         Alerts = 1,
         Events = 2,
         Archive = 3,
         ChangeLog = 4,
         ActivityLog = 5
      }

      private object _scope ;

      public MainTabView()
      {
         InitializeComponent();
         _scope = null ;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Events].Visible = false;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive].Visible = false;
      }

      #region Properties

      public AlertingConfiguration AlertConfiguration
      {
         set
         {
            _alertViewTab.AlertConfiguration = value ; 
         }
      }

      public override bool ShowGroupBy
      {
         get
         {
            return base.ShowGroupBy ;
         }

         set
         {
            _activityLogViewTab.ShowGroupBy = value ;
            _alertViewTab.ShowGroupBy = value ;
            _changeLogViewTab.ShowGroupBy = value ;
            _eventViewTab.ShowGroupBy = value ;
            _archiveViewTab.ShowGroupBy = value;
            base.ShowGroupBy = value ;
         }
      }

      public override bool ShowBanner
      {
         get
         {
            return base.ShowBanner ;
         }

         set
         {
            _activityLogViewTab.ShowBanner = value;
            _alertViewTab.ShowBanner = value;
            _changeLogViewTab.ShowBanner = value;
            _eventViewTab.ShowBanner = value;
            _archiveViewTab.ShowBanner = value;
            base.ShowBanner = value ;
         }
      }

      #endregion Properties


      protected override void OnShowBannerChanged(ToggleChangedEventArgs e)
      {
         e.Enabled = false ;
         base.OnShowBannerChanged(e);
      }

      public void SetScope(ServerRecord s)
      {
         QTabButton activeButton = _tabControl.TabStripTop.ActiveButton ;

         _scope = s ;
         //SetTitle(String.Format("Server: {0}", s.Instance));
         _tabControl.SuspendDraw();
         if (s.IsAuditedServer)
         {
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary].Visible = true;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Alerts].Visible = true;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Events].Visible = true;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive].Visible = true;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.ChangeLog].Visible = true;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.ActivityLog].Visible = false;
            if (!activeButton.Visible)
               _tabControl.TabStripTop.ActiveButton = _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary];
            _serverSummaryTab.BringToFront();
            _serverSummaryTab.Visible = true;
            _enterpriseSummaryTab.Visible = false;
            _databaseSummaryTab.Visible = false;
            _alertViewTab.SetScope(s);
         }
         else
         {
            // Archive-only server
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary].Visible = false;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Alerts].Visible = false;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Events].Visible = false;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive].Visible = true;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.ChangeLog].Visible = false;
            _tabControl.TabStripTop.TabButtons[(int)Tabs.ActivityLog].Visible = false;
            if (!activeButton.Visible)
               _tabControl.TabStripTop.ActiveButton = _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive];
         }
//         if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary])
//         {
//            _serverSummaryTab.BringToFront() ;
//            _serverSummaryTab.Visible = true ;
//            _enterpriseSummaryTab.Visible = false ;
//            _databaseSummaryTab.Visible = false ;
//            _serverSummaryTab.RefreshView(s) ;
//         }
         _tabControl.ResumeDraw(true) ;
      }
         

      public void SetScope(DatabaseRecord d)
      {
         QTabButton activeButton = _tabControl.TabStripTop.ActiveButton;

         _scope = d;
         //SetTitle(String.Format("Server: {0}  Database: {1}", d.SrvInstance, d.Name));
         _tabControl.SuspendDraw();
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary].Visible = true;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Alerts].Visible = false;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Events].Visible = true;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive].Visible = true;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.ChangeLog].Visible = false;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.ActivityLog].Visible = false;
         if (!activeButton.Visible)
            _tabControl.TabStripTop.ActiveButton = _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary];
         _databaseSummaryTab.BringToFront();
         _databaseSummaryTab.Visible = true;
         _serverSummaryTab.Visible = false;
         _enterpriseSummaryTab.Visible = false;
//         if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary])
//         {
//            _databaseSummaryTab.BringToFront() ;
//            _databaseSummaryTab.Visible = true;
//            _serverSummaryTab.Visible = false;
//            _enterpriseSummaryTab.Visible = false;
//            _databaseSummaryTab.RefreshView(d) ;
//         }
         _tabControl.ResumeDraw(true);
      }

      public void SetScope()
      {
         QTabButton activeButton = _tabControl.TabStripTop.ActiveButton;

         _scope = null ;
         //SetTitle("Audited SQL Servers");
         _tabControl.SuspendDraw();
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary].Visible = true;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Alerts].Visible = true;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Events].Visible = false;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive].Visible = false;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.ChangeLog].Visible = true;
         _tabControl.TabStripTop.TabButtons[(int)Tabs.ActivityLog].Visible = false;
         if (!activeButton.Visible)
            _tabControl.TabStripTop.ActiveButton = _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary];
         _enterpriseSummaryTab.BringToFront();
         _enterpriseSummaryTab.Visible = true;
         _serverSummaryTab.Visible = false;
         _databaseSummaryTab.Visible = false;
         _alertViewTab.SetScope();
//         if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary])
//         {
//            _enterpriseSummaryTab.BringToFront();
//            _enterpriseSummaryTab.Visible = true;
//            _serverSummaryTab.Visible = false;
//            _databaseSummaryTab.Visible = false;
//            _enterpriseSummaryTab.RefreshView() ;
//         }
         _tabControl.ResumeDraw(true);
      }

      public void ShowTab(Tabs tab)
      {
         _tabControl.TabStripTop.ActiveButton = _tabControl.TabStripTop.TabButtons[(int)tab] ;
      }

      public override void RefreshView()
      {
         if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.ActivityLog])
         {
            _activityLogViewTab.RefreshView() ;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Alerts])
         {
            _alertViewTab.RefreshView() ;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive])
         {
            if (_scope is DatabaseRecord)
            {
               DatabaseRecord record = _scope as DatabaseRecord;
               _archiveViewTab.LoadArchiveDatabaseEvents(record);
            }
            else if (_scope is ServerRecord)
            {
               ServerRecord record = _scope as ServerRecord;
               _archiveViewTab.LoadArchiveServerEvents(record);
            }
            else
            {
            }
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Events])
         {
            if(_scope is DatabaseRecord)
            {
               DatabaseRecord record = _scope as DatabaseRecord ;
               _eventViewTab.LoadDatabaseEvents(record) ;
            }else if(_scope is ServerRecord)
            {
               ServerRecord record = _scope as ServerRecord ;
               _eventViewTab.LoadServerEvents(record) ;
            }else
            {
            }
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.ChangeLog])
         {
            _changeLogViewTab.RefreshView() ;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary])
         {
            if (_scope is DatabaseRecord)
            {
               DatabaseRecord record = _scope as DatabaseRecord;
               _databaseSummaryTab.RefreshView(record) ;
            }
            else if (_scope is ServerRecord)
            {
               ServerRecord record = _scope as ServerRecord;
               _serverSummaryTab.RefreshView(record) ;
            }
            else
            {
               _enterpriseSummaryTab.RefreshView() ;
            }
         }
      }

      public override bool GetMenuFlag(CMMenuItem item)
      {
         BaseControl ctrl = GetActiveControl() ;

         if(ctrl != null)
            return ctrl.GetMenuFlag(item) ;
         else
            return base.GetMenuFlag(item) ;
      }

      public override void Delete()
      {
         BaseControl ctrl = GetActiveControl();
         base.Delete();
         if(ctrl != null)
            ctrl.Delete() ;
      }

      public override void Properties()
      {
         base.Properties();
         BaseControl ctrl = GetActiveControl();
         if (ctrl != null)
            ctrl.Properties();
      }

      public override void CollapseAll()
      {
         base.CollapseAll();
         BaseControl ctrl = GetActiveControl();
         if (ctrl != null)
            ctrl.CollapseAll();
      }

      public override void ExpandAll()
      {
         base.ExpandAll();
         BaseControl ctrl = GetActiveControl();
         if (ctrl != null)
            ctrl.ExpandAll();
      }

      public override void HelpOnThisWindow()
      {
         base.HelpOnThisWindow();
         BaseControl ctrl = GetActiveControl();
         if (ctrl != null)
            ctrl.HelpOnThisWindow();
      }

      public override void Initialize(Form_Main2 mainForm)
      {
         base.Initialize(mainForm);
         _activityLogViewTab.Initialize(mainForm) ;
         _alertViewTab.Initialize(mainForm);
         _archiveViewTab.Initialize(mainForm);
         _eventViewTab.Initialize(mainForm);
         _changeLogViewTab.Initialize(mainForm);
         _enterpriseSummaryTab.Initialize(mainForm) ;
         _serverSummaryTab.Initialize(mainForm) ;
         _databaseSummaryTab.Initialize(mainForm) ;
      }

      private BaseControl GetActiveControl()
      {
         if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.ActivityLog])
         {
            return _activityLogViewTab ;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Alerts])
         {
            return _alertViewTab;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Archive])
         {
            return _archiveViewTab ;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Events])
         {
            return _eventViewTab ;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.ChangeLog])
         {
            return _changeLogViewTab;
         }
         else if (_tabControl.TabStripTop.ActiveButton == _tabControl.TabStripTop.TabButtons[(int)Tabs.Summary])
         {
         }
         return null ;
      }

      private void MenuFlagChanged_child(object sender, MenuFlagChangedEventArgs e)
      {
         OnMenuFlagChanged(e) ;
      }

      private void ActivePageChanged_tabControl(object sender, QTabPageChangeEventArgs e)
      {
         RefreshView();
         OnMenuFlagChanged(new MenuFlagChangedEventArgs(CMMenuItem.All, true));
      }
   }
}

