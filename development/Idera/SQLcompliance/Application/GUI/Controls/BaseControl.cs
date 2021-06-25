using System ;
using System.ComponentModel;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Forms ;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public enum CMMenuItem
   {
      All = 0,
      Refresh,
      Cut,
      Copy,
      Paste,
      Delete,
      Rename,
      Properties,
      EnableAuditing,
      DisableAuditing,
      SetFilter,
      Print,
      PrintPreview,
      PageSetup,
      NewServer,
      NewDatabase,
      NewAlertRule,
      NewStatusAlertRule,
      NewDataAlertRule,
      UseRuleAsTemplate,
      NewLogin,
      Collapse,
      Expand,
      GroupByColumn,
      ServerAuditSettings,
      PrivilegedUserSettings,
      DatabaseAuditSettings,
      TrustedUserSettings,
      DeployAgent,
      CheckAgent,
      UpdateAuditSettings,
      Groom,
      Snapshot,
      AgentProperties,
      ShowHelp,
      ForceCollection,
      UpgradeAgent,
      ChangeAgentTraceDir,
      ViewTasks,
      NewEventFilter,
      AttachArchive,
      DetachArchive,
      EnableRule,
      DisableRule,
      ExportAuditSettings,
      ImportAuditSettings,
      ExportAlertRules,
      ImportAlertRules,
      ExportEventFilters,
      ImportEventFilters,
      LoadReport,
      ImportCSV,
      //start sqlcm 5.6 - 5467
      DefaultServerSettings,
      DefaultDatabaseSettings,
      EditServerSettings,
      EditDatabaseSettings
      //end sqlcm 5.6 - 5467
   } ;

   public delegate void ShowBannerChangedEventHandler(Object sender, ToggleChangedEventArgs e) ;
   public delegate void ShowGroupByChangedEventHandler(Object sender, ToggleChangedEventArgs e) ;
   public delegate void MenuFlagChangedEventHandler(Object sender, MenuFlagChangedEventArgs e) ;

   public partial class BaseControl : UserControl, IMenuFlags
   {
      private bool?[] _flags;
      private bool? _bShowGroupBy;
      protected Form_Main2 _mainForm;
       protected Form_ImportSensitiveColumnsFromCSV _scForm;

      public event ShowGroupByChangedEventHandler ShowGroupByChanged;
      public event MenuFlagChangedEventHandler MenuFlagChanged;

      public BaseControl()
      {
         InitializeComponent() ;
         _flags = new bool?[Enum.GetValues(typeof(CMMenuItem)).Length];
         _bShowGroupBy = null;
         //ResetMenuFlags();
      }

      public virtual void Initialize(Form_Main2 mainForm)
      {
         _mainForm = mainForm;
      }
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public virtual bool ShowGroupBy
      {
         get { return _bShowGroupBy.GetValueOrDefault(false); }
         set
         {
            if (_bShowGroupBy == null || value != _bShowGroupBy)
            {
               _bShowGroupBy = value;
               OnShowGroupByChanged(new ToggleChangedEventArgs(value));
            }
         }
      }

      protected virtual void OnShowGroupByChanged(ToggleChangedEventArgs e)
      {
         ShowGroupByChangedEventHandler temp = ShowGroupByChanged;
         if (temp != null)
         {
            ShowGroupByChanged(this, e);
         }
      }

      protected virtual void OnMenuFlagChanged(MenuFlagChangedEventArgs e)
      {
         MenuFlagChangedEventHandler temp = MenuFlagChanged;
         if (temp != null)
         {
            MenuFlagChanged(this, e);
         }
      }

      private void ResetMenuFlags()
      {
         foreach (CMMenuItem item in Enum.GetValues(typeof(CMMenuItem)))
            SetMenuFlag(item, false);
      }

      public virtual bool GetMenuFlag(CMMenuItem item)
      {
         return _flags[(int)item].GetValueOrDefault(false) ;
      }

      public virtual void SetMenuFlag(CMMenuItem item)
      {
         SetMenuFlag(item, true);
      }

      public virtual void SetMenuFlag(CMMenuItem item, bool flag)
      {
         if (_flags[(int)item] != flag)
         {
            _flags[(int)item] = flag;
            OnMenuFlagChanged(new MenuFlagChangedEventArgs(item, flag));
         }
      }

      // Each control that inherits from this class will optionally
      // override these default commands called from the mainForm
      // hosting the controls
      //virtual public void ShowBanner( bool on ){}

      // View Menu Commands
      public virtual void RefreshView() { }

      // Clear View
      //public virtual void Reset() {}

      // Edit Menu Commands
      public virtual void Cut() { }
      public virtual void Copy() { }
      public virtual void Paste() { }
      public virtual void Delete() { }
      public virtual void Rename() { }
      public virtual void Properties() { }
      public virtual void Enable(bool flag) { }
      public virtual void SetFilter() { }
      public virtual void PageSetup() { }
      public virtual void Print() { }
      public virtual void PrintPreview() { }
      public virtual void CollapseAll() { }
      public virtual void ExpandAll() { }
      public virtual void DeployAgent() { }
      public virtual void UpgradeAgent() { }
      public virtual void CheckAgent() { }
      public virtual void UpdateNow() { }
      public virtual void Groom(string instance) { }
      public virtual void DoSnapshot(string instance) { }
      public virtual void AgentProperties() { }
      public virtual void AgentTraceDirectory() { }
      public virtual void HelpOnThisWindow() { }
      public virtual void UpdateColors(){ }

      // for views andproperty windows
      public virtual int Previous()
      {
         return -1;
      }

      public virtual int Next()
      {
         return -1;
      }

      public virtual string showDelete()
      {
         return "Remo&ve";
      }

      public void SetGlobalWaitCursor(bool value)
      {
         if(_mainForm == null)
            return ;
         if(value)
            _mainForm.Cursor = Cursors.WaitCursor ;
         else
            _mainForm.Cursor = Cursors.Default ;

      }
   }

   public class ToggleChangedEventArgs
   {
      private bool _value ;

      public ToggleChangedEventArgs(bool newValue)
      {
         _value = newValue ;
      }

      public bool Enabled
      {
         get { return _value ; }
         set { _value = value ; }
      }
   }

   public class MenuFlagChangedEventArgs
   {
      CMMenuItem _menuItem ;
      private bool _value ;

      public MenuFlagChangedEventArgs(CMMenuItem item, bool newValue)
      {
         _menuItem = item; 
         _value = newValue ;
      }

      public bool Enabled
      {
         get { return _value ; }
      }
      
      public CMMenuItem TargetMenuItem
      {
         get { return _menuItem ; }
      }
   }

   public interface IMenuFlags
   {
      bool GetMenuFlag(CMMenuItem item);
      void SetMenuFlag(CMMenuItem item);
      void SetMenuFlag(CMMenuItem item, bool flag);
   }

   public class BaseFlags : IMenuFlags
   {
      private bool[] _flags;

      public event MenuFlagChangedEventHandler MenuFlagChanged;

      public BaseFlags()
      {
         _flags = new bool[Enum.GetValues(typeof(CMMenuItem)).Length];
         ResetMenuFlags();
      }

      protected virtual void OnMenuFlagChanged(MenuFlagChangedEventArgs e)
      {
         MenuFlagChangedEventHandler temp = MenuFlagChanged;
         if (temp != null)
         {
            MenuFlagChanged(this, e);
         }
      }

      private void ResetMenuFlags()
      {
         foreach (CMMenuItem item in Enum.GetValues(typeof(CMMenuItem)))
            SetMenuFlag(item, false);
      }

      public bool GetMenuFlag(CMMenuItem item)
      {
         return _flags[(int)item];
      }

      public void SetMenuFlag(CMMenuItem item)
      {
         SetMenuFlag(item, true);
      }

      public void SetMenuFlag(CMMenuItem item, bool flag)
      {
         if (_flags[(int)item] != flag)
         {
            _flags[(int)item] = flag;
            OnMenuFlagChanged(new MenuFlagChangedEventArgs(item, flag));
         }
      }
   }

}
