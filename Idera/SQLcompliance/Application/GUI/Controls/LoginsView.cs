using System;
using System.Collections;
using System.Data.SqlClient;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Cwf;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLcompliance.Core.Collector;
using System.Collections.Generic;
using System.Linq;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    /// <summary>
    /// Summary description for LoginsView.
    /// </summary>
    public partial class LoginsView : BaseControl
    {
        public LoginsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            GridHelper.ApplyAdminSettings(_grid);

            // Enable main form menus 			
            SetMenuFlag(CMMenuItem.Refresh);
            SetMenuFlag(CMMenuItem.ShowHelp);
            SetMenuFlag(CMMenuItem.NewLogin, Globals.isAdmin);
        }

        #region Base Control overrides

        //-------------------------------------------------------------
        // Properties - public function called from here and mainForm menus
        //--------------------------------------------------------------
        public override void Properties()
        {
            ShowProperties();
        }

        //-------------------------------------------------------------
        // showDelete - show Delete not Remove on main menu
        //--------------------------------------------------------------
        public override string showDelete()
        {
            return "&Delete";
        }

        //-----------------------------------------------------
        // RefreshView - Called from mainForm refresh handlers
        //-----------------------------------------------------
        public override void RefreshView()
        {
            LoadLogins();
            if (_dsLogins.Rows.Count > 0)
                _grid.Rows[0].Selected = true;
            UpdateMenuFlags();
        }

        //-------------------------------------------------------------
        // Delete - public function called from here and mainForm menus
        //--------------------------------------------------------------
        public override void Delete()
        {
            RemoveLogin();
        }

        #endregion

        #region Login - New

        public void SelectLogin(string name)
        {
            _grid.Selected.Rows.Clear();
            foreach (UltraGridRow row in _grid.Rows)
            {
                if (row.Cells["Name"].Value.ToString() == name)
                {
                    _grid.Selected.Rows.Add(row);
                    break;
                }
            }
        }

        #endregion

        #region Login - Properties

        //      private RawLoginObject GetSelectedLogin()
        //      {
        //         if(_grid.Selected.Rows.Count == 0)
        //            return null ;
        //         return (RawLoginObject)_grid.Selected.Rows[0].Tag ;
        //      }

        //-------------------------------------------------------------
        // ShowProperties
        //--------------------------------------------------------------
        private void ShowProperties()
        {
            if (_grid.Selected.Rows.Count == 0) return;
            UltraGridRow selectedRow = _grid.Selected.Rows[0];
            UltraDataRow dataRow = (UltraDataRow)selectedRow.ListObject;
            RawLoginObject selectedLogin = (RawLoginObject)dataRow.Tag;

            Cursor = Cursors.WaitCursor;

            Form_LoginProperties frm = new Form_LoginProperties(selectedLogin);

            if (frm.ShowDialog() == DialogResult.OK)
                UpdateRowValues(dataRow, frm.rawLogin);

            Cursor = Cursors.Default;
        }

        private void UpdateRowValues(UltraDataRow row, RawLoginObject login)
        {
            // icon and type of login
            if (login.isntgroup == 1)
            {
                row["Icon"] = AppIcons.AppImg16(AppIcons.Img16.WindowsGroup);
                row["Type"] = UIConstants.Login_WindowsGroup;
            }
            else if (login.isntname == 1)
            {
                row["Icon"] = AppIcons.AppImg16(AppIcons.Img16.WindowsUser);
                row["Type"] = UIConstants.Login_WindowsUser;

            }
            else
            {
                row["Icon"] = AppIcons.AppImg16(AppIcons.Img16.SqlServerLogin);
                row["Type"] = UIConstants.Login_Standard;
            }

            // web application access
            if (login.isntuser == 1)
            {
                var loginAccount = new LoginAccount(login.name);
                row["WebApplicationAccess"] = loginAccount.WebApplicationAccess;
            }
            else
                row["WebApplicationAccess"] = false;

            // access
            if (login.denylogin == 1)
            {
                row["ServerAccess"] = UIConstants.Login_Deny;
            }
            else if (login.hasaccess == 1)
            {
                row["ServerAccess"] = UIConstants.Login_Permit;
            }
            else if (login.isntname == 1)
            {
                row["ServerAccess"] = UIConstants.Login_ViaGroup;
            }
            else
            {
                row["ServerAccess"] = UIConstants.Login_Deny;
            }

            // permissions
            if (login.denylogin == 1)
            {
                row["CMAccess"] = UIConstants.Login_None;
            }
            else if (login.sysadmin == 1)
            {
                row["CMAccess"] = UIConstants.Login_CanConfigure;
            }
            else
            {
                row["CMAccess"] = UIConstants.Login_CanView;
            }
            row["Name"] = login.name;
            row.Tag = login;
        }

        #endregion

        #region Login - Remove

        //-------------------------------------------------------------
        // RemoveLogin
        //--------------------------------------------------------------
        private void RemoveLogin()
        {
            if (_grid.Selected.Rows.Count == 0) return;
            UltraGridRow selectedRow = _grid.Selected.Rows[0];
            UltraDataRow dataRow = (UltraDataRow)selectedRow.ListObject;
            RawLoginObject selectedLogin = (RawLoginObject)dataRow.Tag;

            //WARNING
            DialogResult choice =
               MessageBox.Show(UIConstants.Warning_DeletingLogin,
                               UIConstants.Title_DeletingLogin,
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Warning);
            if (choice == DialogResult.No)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                // delete account
                var loginAccount = new LoginAccount(selectedLogin.name);
                loginAccount.WebApplicationAccess = false;

                CwfHelper.Instance.SynchronizeUsersWithCwf(loginAccount);

                loginAccount.Delete();

                RawSQL.DeleteReportData(selectedLogin.name.ToLower(), Globals.Repository.Connection);


                string sql = String.Format("exec sp_revokelogin {0}",
                                           SQLHelpers.CreateSafeString(selectedLogin.name));

                using (SqlCommand cmd = new SqlCommand(sql, Globals.Repository.Connection))
                {
                    cmd.ExecuteNonQuery();

                    LogRecord.WriteLog(Globals.Repository.Connection,
                                       LogType.DeleteLogin,
                                       Globals.RepositoryServer,
                                       String.Format("Login: {0}", selectedLogin.name));
                }

                _dsLogins.Rows.Remove(dataRow);
                if (_dsLogins.Rows.Count > 0)
                    _grid.Rows[0].Selected = true;
                else
                    _grid.Selected.Rows.Clear();
                selectedRow.Delete();

                //Update audit settings bin file
                AgentManager manager = GUIRemoteObjectsProvider.AgentManager();
                manager.UpdateAuditUsers(Globals.Repository.Instance);
            }
            catch (Exception ex)
            {
                ErrorMessage.Show(this.Text, UIConstants.Error_DeletingLogin, ex.Message);
            }

            UpdateMenuFlags();
            this.Cursor = Cursors.Default;
        }

        private void UpdateMenuFlags()
        {
            SetMenuFlag(CMMenuItem.NewLogin, Globals.isAdmin);

            SetMenuFlag(CMMenuItem.Delete, _grid.Selected.Rows.Count > 0 && Globals.isAdmin && _grid.Focused);
            SetMenuFlag(CMMenuItem.Properties, _grid.Selected.Rows.Count == 1 && Globals.isAdmin && _grid.Focused);
        }

        #endregion

        #region Load list

        //-------------------------------------------------------------
        // LoadLogins
        //--------------------------------------------------------------
        public void LoadLogins()
        {
            Cursor = Cursors.WaitCursor;
            _dsLogins.Rows.Clear();

            try
            {
                List<string> rptLoginsList = RawSQL.GetReportLogins(Globals.Repository.Connection);
                ICollection loginList = RawSQL.GetServerLogins(Globals.Repository.Connection);
                List<string> newLogins = new List<string>();
                List<string> existingLogins = new List<string>();

                if ((loginList != null) && (loginList.Count != 0))
                {
                    foreach (RawLoginObject login in loginList)
                    {                        
                        if (login.isntname != 0)
                        {
                            UltraDataRow row = _dsLogins.Rows.Add();
                            UpdateRowValues(row, login);
                            if (Globals.isAdmin)
                                newLogins.Add((login.name).ToLower());                          
                        }
                    }
                    if (Globals.isAdmin)
                    {
                        var removedlogins = rptLoginsList.Except(newLogins).ToList();
                        var newlogins = newLogins.Except(rptLoginsList).ToList();

                        if (newlogins != null && newlogins.Count > 0)
                        {
                            RawSQL.InsertReportLogin(newlogins, Globals.Repository.Connection);
                        }

                        if (removedlogins != null && removedlogins.Count > 0)
                        {
                            foreach (var item in removedlogins)
                            {
                                RawSQL.DeleteReportData(item, Globals.Repository.Connection);

                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                if (UIUtils.CloseIfConnectionLost()) return;

                ErrorMessage.Show(this.Text,
                                  "An error occurred trying to load the SQL Server logins.",
                                  ex.Message);
            }

            Cursor = Cursors.Default;
        }

        #endregion

        #region Help

        //--------------------------------------------------------------------
        // LoginsView_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void LoginsView_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_LoginsView);
            hlpevent.Handled = true;
        }

        public override void HelpOnThisWindow()
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_LoginsView);
        }

        #endregion

        private void AfterSelectChange_grid(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateMenuFlags();
        }

        private void DoubleClickRow_grid(object sender, DoubleClickRowEventArgs e)
        {
            if (GetMenuFlag(CMMenuItem.Properties))
            {
                ShowProperties();
            }
        }

        private void KeyDown_grid(object sender, KeyEventArgs e)
        {
            if (_grid.Selected.Rows.Count > 0)
            {
                if (e.KeyCode == Keys.Delete)
                    RemoveLogin();
                else if (e.KeyCode == Keys.Enter)
                    ShowProperties();
            }
        }

        private void MouseDown_grid(object sender, MouseEventArgs e)
        {
            UIElement elementMain;
            UIElement elementUnderMouse;

            elementMain = _grid.DisplayLayout.UIElement;
            elementUnderMouse = elementMain.ElementFromPoint(e.Location);
            if (elementUnderMouse != null)
            {
                UltraGridCell cell = elementUnderMouse.GetContext(typeof(UltraGridCell)) as UltraGridCell;
                if (cell != null)
                {
                    if (!cell.Row.Selected)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            _grid.Selected.Rows.Clear();
                            cell.Row.Selected = true;
                            _grid.ActiveRow = cell.Row;
                        }
                    }
                }
                else
                {
                    HeaderUIElement he = elementUnderMouse.GetAncestor(typeof(HeaderUIElement)) as HeaderUIElement;
                    if (he == null)
                    {
                        _grid.Selected.Rows.Clear();
                        _grid.ActiveRow = null;
                    }
                }
            }
        }

        private void ToolClick_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "newLogin":
                    Form_LoginNew frm = new Form_LoginNew();
                    if (DialogResult.OK == frm.ShowDialog())
                    {
                        RefreshView();
                        LoadLogins();

                        // find new login to select
                        SelectLogin(frm.Name);
                    }
                    break;
                case "delete":
                    RemoveLogin();
                    break;
                case "refresh":
                    LoadLogins();
                    break;
                case "properties":
                    ShowProperties();
                    break;

            }
        }

        private void BeforeToolDropdown_toolbarsManager(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            // set menus based on flags above
            _toolbarsManager.Tools["newLogin"].SharedProps.Enabled = Globals.isAdmin;
            _toolbarsManager.Tools["delete"].SharedProps.Enabled = _grid.Selected.Rows.Count > 0 && Globals.isAdmin && _grid.Focused;
            _toolbarsManager.Tools["properties"].SharedProps.Enabled = _grid.Selected.Rows.Count == 1 && Globals.isAdmin && _grid.Focused;
        }

        private void _grid_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key.Equals("WebApplicationAccess"))
            {
                var loginObject = (RawLoginObject)((UltraDataRow)e.Cell.Row.ListObject).Tag;
                if (loginObject.isntuser != 1)
                {
                    MessageBox.Show(this,
                                    @"You can provide \ deny web application access to system users only.", Text,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Hand);
                    return;
                }

                var webAccess = !Convert.ToBoolean(e.Cell.Value);

                var login = new LoginAccount(loginObject.name);
                login.WebApplicationAccess = webAccess;
                login.Set();

                Cursor = Cursors.WaitCursor;
                var success = CwfHelper.Instance.SynchronizeUsersWithCwf(login);
                Cursor = Cursors.Default;

                if (success)
                {
                    e.Cell.Value = webAccess;
                    MessageBox.Show(this,
                                    string.Format("Web application access permission updated for user {0}.", login.Name),
                                    UIConstants.AppTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    login.WebApplicationAccess = !webAccess;
                    login.Set();

                    MessageBox.Show(this,
                                    string.Format("Failed to update Web application access permission for user {0}.", login.Name),
                                    UIConstants.AppTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}