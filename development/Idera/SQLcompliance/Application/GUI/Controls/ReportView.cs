using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Reports;
using Microsoft.Reporting.WinForms;
using PresentationControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public partial class ReportView : BaseControl
    {
        #region Data Members

        private string _instance;
        private CMReport _report = null;
        private Dictionary<string, CMReportParameter> _supportedParameters = new Dictionary<string, CMReportParameter>();
        private Dictionary<string, ParameterUIElement> _paramControlMap = new Dictionary<string, ParameterUIElement>();
        private Dictionary<string, string> _dataSetReferences = new Dictionary<string, string>();
        private bool _usesDate;

        private DateTimePicker _startDate;
        private DateTimePicker _endDate;
        private ComboBox _startMinute;
        private ComboBox _endMinute;
        private ComboBox _startHour;
        private ComboBox _endHour;
        private ComboBox _amPmStart;
        private ComboBox _amPmEnd;
        private Label _warningLabel;

        private Button _btnExecute;

        private BackgroundWorker _bgReportLoader;

        private const string DML_ACTIVITY_REPORT = "DML Activity (Before-After)";
        private const string REGULATORY_COMPLIANCE_CHECK_REPORT = "Regulatory Compliance Check"; //SQLCM-5465
        private const string AUDIT_CONTROL_CHANGES = "Audit Control Changes";

        #endregion

        #region Properties

        public CMReport Report
        {
            get { return _report; }
            set { _report = value; }
        }

        public string ServerInstance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        #endregion

        public ReportView()
        {
            InitializeComponent();

            _bgReportLoader = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            _bgReportLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_bgReportLoader);
            _bgReportLoader.DoWork += new DoWorkEventHandler(DoWork_bgReportLoader);

            _reportViewer.ShowDocumentMapButton = false;
            InitializeParameters();
            SetMenuFlag(CMMenuItem.ShowHelp);
            _warningLabel = new Label
            {
                Text = "Report not available.  No servers are currently registered.",
                AutoSize = true,
                Padding = new Padding(10, 10, 10, 10)
            };
            RemoveOptions("toolStrip1", "export", new[] { "Word" });
        }

        /// <summary>
        /// Customize the report viewer to remove the word button from the export button drop down list
        /// </summary>
        /// <param name="toolStripKey">tool strip control to search in the <see cref="ReportViewer"/></param>
        /// <param name="dropDownButton">drop down button where item needs to be removed</param>
        /// <param name="options">options to be removed</param>
        /// <remarks>
        /// SQLCM-5878 (SQLCM 5.6): Remove 'Export to Word' from export options to keep it consistent with 5.5.1
        /// </remarks>
        private void RemoveOptions(string toolStripKey, string dropDownButton, string[] options)
        {
            try
            {
                if (string.IsNullOrEmpty(toolStripKey) || string.IsNullOrEmpty(dropDownButton) || options == null || options.Length == 0)
                {
                    ErrorLog.Instance.Write(string.Format("Invalid Parameters to Remove Options from the Report Viewer Toolbar: '{0}' '{1}' '{2}'",
                                                            toolStripKey, dropDownButton, string.Join(",", options ?? new string[] { })));
                }
                Control[] toolStripControls = _reportViewer.Controls.Find(toolStripKey, true);
                if (toolStripControls != null)
                {
                    ToolStrip toolStrip = toolStripControls.First() as ToolStrip;
                    if (toolStrip != null)
                    {
                        ToolStripDropDownButton exportDropDownButton = toolStrip.Items.OfType<ToolStripDropDownButton>().First(button => button.Name == dropDownButton);
                        if (exportDropDownButton != null)
                        {
                            exportDropDownButton.DropDownOpened += (object sender, EventArgs e) =>
                            {
                                if (sender is ToolStripDropDownButton)
                                {
                                    ToolStripDropDownButton dropDownList = sender as ToolStripDropDownButton;
                                    IEnumerable<ToolStripDropDownItem> items = dropDownList.DropDownItems.OfType<ToolStripDropDownItem>().Where(item => options.Contains(item.Text));
                                    items.ToList().ForEach(item => dropDownList.DropDownItems.Remove(item));
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            string.Format("Remove Options from the Report Viewer Toolbar: '{0}' '{1}' '{2}'",
                                                            toolStripKey, dropDownButton, string.Join(",", options ?? new string[] { })),
                                            ex,
                                            ErrorLog.Severity.Error);
            }
        }

        public override void Initialize(Forms.Form_Main2 mainForm)
        {
            base.Initialize(mainForm);
            mainForm.ConnectionChanged += new EventHandler<ConnectionChangedEventArgs>(ConnectionChanged_mainForm);
            mainForm.ServerAdded += new EventHandler<ServerEventArgs>(ServerAddedRemoved_mainForm);
            mainForm.ServerRemoved += ServerAddedRemoved_mainForm;
        }

        private void ServerAddedRemoved_mainForm(object sender, ServerEventArgs e)
        {
            LoadReportComboChoices();
        }

        private void ConnectionChanged_mainForm(object sender, ConnectionChangedEventArgs e)
        {
            LoadReportComboChoices();
        }

        private void LoadReportComboChoices()
        {
            CMReportParameter p, p1, p2;
            ParameterUIElement uip;

            p = _supportedParameters["eventDatabase"];
            LoadServerInstances(p, true);
            uip = _paramControlMap["eventDatabase"];
            uip.UpdateValues(p);

            p = _supportedParameters["loginName"];
            p.ClearList();
            p.AddListItem("", "");
            uip = _paramControlMap["loginName"];
            uip.UpdateValuesCheckBoxComboBox(p);

            p = _supportedParameters["dataType"];
            uip = _paramControlMap["dataType"];
            uip.UpdateValues(p);

            p = _supportedParameters["eventDatabaseAll"];
            LoadServerInstances(p, true);
            uip = _paramControlMap["eventDatabaseAll"];
            uip.UpdateValues(p);

            p = _supportedParameters["loginNameAll"];
            p.ClearList();
            p.AddListItem("", "");
            uip = _paramControlMap["loginNameAll"];
            uip.UpdateValuesCheckBoxComboBox(p);

            p = _supportedParameters["eventDatabaseAbove2005"];
            LoadServerInstancesAbove2005(p, true);
            uip = _paramControlMap["eventDatabaseAbove2005"];
            uip.UpdateValues(p);

            p = _supportedParameters["loginNameAbove2005"];
            p.ClearList();
            p.AddListItem("", "");
            uip = _paramControlMap["loginNameAbove2005"];
            uip.UpdateValuesCheckBoxComboBox(p);

            p = _supportedParameters["loginNameForUser"];
            p.ClearList();
            p.AddListItem("", "");
            uip = _paramControlMap["loginNameForUser"];
            uip.UpdateValuesCheckBoxComboBox(p);

            p = _supportedParameters["roleNameForUser"];
            p.ClearList();
            p.AddListItem("", "");
            uip = _paramControlMap["roleNameForUser"];
            uip.UpdateValuesCheckBoxComboBox(p);

            p = _supportedParameters["instance"];
            LoadAgentInstances(p);
            uip = _paramControlMap["instance"];
            uip.UpdateValues(p);

            p = _supportedParameters["eventCategory"];
            LoadCategories(p);
            uip = _paramControlMap["eventCategory"];
            uip.UpdateValues(p);

            p = _supportedParameters["serverName"];
            LoadServerInstances(p);
            uip = _paramControlMap["serverName"];
            uip.UpdateValues(p);

            p = _supportedParameters["dbName"];
            LoadDatabases(p, 0);
            uip = _paramControlMap["dbName"];
            uip.UpdateValues(p);

            p = _supportedParameters["auditSetting"];
            LoadAuditSettings(p);
            uip = _paramControlMap["auditSetting"];
            uip.ValueControl.Width = 350;
            uip.UpdateValues(p);

            //SQLCM-5465
            p = _supportedParameters["Server"];
            LoadInstances(p);
            uip = _paramControlMap["Server"];
            uip.UpdateValues(p);

            p = _supportedParameters["Database"];
            LoadDatabasesByServerName(p, "<ALL>");
            uip = _paramControlMap["Database"];
            uip.UpdateValues(p);

            p = _supportedParameters["RegulationGuidelines"];
            LoadAllRegulationGuidelines(p);
            uip = _paramControlMap["RegulationGuidelines"];
            uip.ValueControl.Width = 350;
            uip.UpdateValues(p);

            p = _supportedParameters["AuditSettings"];
            LoadAllAuditSettings(p);
            uip = _paramControlMap["AuditSettings"];
            uip.ValueControl.Width = 350;
            uip.UpdateValues(p);

            p = _supportedParameters["Values"];
            uip = _paramControlMap["Values"];
            uip.UpdateValues(p);

            p1 = _supportedParameters["dateFrom"];
            p2 = _supportedParameters["dateTo"];
            LoadDateTime(p1, p2);

            p1 = _supportedParameters["CISServerSettings"];
            p2 = _supportedParameters["CISDatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 5);

            p1 = _supportedParameters["DISASTIGServerSettings"];
            p2 = _supportedParameters["DISASTIGDatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 3);

            p1 = _supportedParameters["FERPAServerSettings"];
            p2 = _supportedParameters["FERPADatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 7);

            p1 = _supportedParameters["GDPRServerSettings"];
            p2 = _supportedParameters["GDPRDatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 8);

            p1 = _supportedParameters["HIPAAServerSettings"];
            p2 = _supportedParameters["HIPAADatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 2);

            p1 = _supportedParameters["NERCServerSettings"];
            p2 = _supportedParameters["NERCDatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 4);

            p1 = _supportedParameters["PCIDSSServerSettings"];
            p2 = _supportedParameters["PCIDSSDatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 1);

            p1 = _supportedParameters["SOXServerSettings"];
            p2 = _supportedParameters["SOXDatabaseSettings"];
            LoadSDConfigurationSettings(p1, p2, 6);

            BuildDataSetReferences();
        }

        public override void RefreshView()
        {
            if (_bgReportLoader.IsBusy)
            {
                _bgReportLoader.CancelAsync();
                return;
            }

            SetLoginComboValues();
            _reportViewer.Reset();
            if (_report != null)
            {
                try
                {
                    BuildParameterView();
                    _pnlColors.Size = _pnlParams.PreferredSize;

                    if (_report.Name == "Table-Data Access by Rowcount")
                    {
                        CMReportParameter p;
                        ParameterUIElement uip;
                        p = _supportedParameters["eventDatabaseAbove2005"];
                        LoadServerInstancesAbove2005(p, true);
                        uip = _paramControlMap["eventDatabaseAbove2005"];
                        uip.UpdateValues(p);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Report parameter view");
                }
            }
            base.RefreshView();
        }

        public void SetLoginComboValues()
        {
            ParameterUIElement uiparam;
            CMReportParameter paramEventDatabase = _supportedParameters["eventDatabase"];

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {

                if (_report.GetParameters(conn, true).Contains("@loginNameForUser"))
                {
                    CMReportParameter paramLogin = _supportedParameters["loginNameForUser"];
                    LoadLoginsForUser(paramLogin);
                    uiparam = _paramControlMap["loginNameForUser"];
                    uiparam.UpdateValuesCheckBoxComboBox(paramLogin);

                    CMReportParameter paramRole = _supportedParameters["roleNameForUser"];
                    LoadRolesForUser(paramRole);
                    uiparam = _paramControlMap["roleNameForUser"];
                    uiparam.UpdateValuesCheckBoxComboBox(paramRole);
                }

                else if (_report.GetParameters(conn, true).Contains("@eventDatabase"))
                {
                    CMReportParameter paramLogin = _supportedParameters["loginName"];
                    foreach (KeyValuePair<string, object> eventDatabase in paramEventDatabase.ValueItems)
                    {
                        LoadLogins(paramLogin, (string)eventDatabase.Value);
                        uiparam = _paramControlMap["loginName"];
                        uiparam.UpdateValuesCheckBoxComboBox(paramLogin);
                        break;
                    }
                }
                else if (_report.GetParameters(conn, true).Contains("@eventDatabaseAll"))
                {
                    CMReportParameter paramLogin = _supportedParameters["loginNameAll"];
                    foreach (KeyValuePair<string, object> eventDatabase in paramEventDatabase.ValueItems)
                    {
                        LoadLogins(paramLogin, (string)eventDatabase.Value);
                        uiparam = _paramControlMap["loginNameAll"];
                        uiparam.UpdateValuesCheckBoxComboBox(paramLogin);
                        break;
                    }
                }
                else if (_report.GetParameters(conn, true).Contains("@eventDatabaseAbove2005"))
                {
                    CMReportParameter paramLogin = _supportedParameters["loginNameAbove2005"];
                    foreach (KeyValuePair<string, object> eventDatabase in paramEventDatabase.ValueItems)
                    {
                        LoadLogins(paramLogin, (string)eventDatabase.Value);
                        uiparam = _paramControlMap["loginNameAbove2005"];
                        uiparam.UpdateValuesCheckBoxComboBox(paramLogin);
                        break;
                    }
                }
            }
        }

        #region Helper Functions

        private void BuildParameterView()
        {
            int i = 0;
            bool invalidReport = false;
            _pnlParams.SuspendLayout();
            _pnlParams.Controls.Clear();
            _pnlParams.Enabled = true;

            _usesDate = false;

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                foreach (string s in _report.GetParameters(conn, true))
                {
                    string paramName;

                    if (s.StartsWith("@"))
                    {
                        paramName = s.Substring(1);
                    }
                    else
                    {
                        paramName = s;
                    }

                    if ((_report.Name == "Configuration Check" && paramName == "dbName") ||
                        (_report.Name == "Table-Data Access by Rowcount" && paramName == "databaseNameAll"))
                    {
                        continue;
                    }
                    if (_paramControlMap.ContainsKey(paramName))
                    {
                        ParameterUIElement element = _paramControlMap[paramName];

                        if (paramName.Equals("eventDatabase"))
                        {
                            if (((ComboBox)element.ValueControl).Items.Count == 0)
                            {
                                _pnlParams.Controls.Clear();
                                _pnlParams.Controls.Add(_warningLabel, 0, 0);
                                invalidReport = true;
                                break;
                            }
                        }
                        else if (paramName.Equals("eventDatabaseAll"))
                        {
                            if (((ComboBox)element.ValueControl).Items.Count == 0)
                            {
                                _pnlParams.Controls.Clear();
                                _pnlParams.Controls.Add(_warningLabel, 0, 0);
                                invalidReport = true;
                                break;
                            }
                        }
                        else if (paramName.Equals("startDate") || paramName.Equals("endDate"))
                        {
                            _usesDate = true;
                        }

                        _pnlParams.Controls.Add(element.Prompt, i % 5, i / 5);
                        element.Prompt.TabIndex = i;
                        i++;
                        _pnlParams.Controls.Add(element.ValueControl, i % 5, i / 5);
                        element.ValueControl.TabIndex = i;
                        element.SetToDefaultValue();
                        i++;

                        // Increasing event database drop-down size
                        if (paramName.Equals("eventDatabase") || paramName.Equals("eventDatabaseAll") || paramName.Equals("eventDatabaseAbove2005") || paramName.Equals("instance") || paramName.Equals("serverName") || paramName.Equals("Server")
                            || paramName.Equals("databaseName") || paramName.Equals("dbName") || paramName.Equals("Database") || paramName.Equals("databaseNameAll")
                            || paramName.Equals("loginName") || paramName.Equals("loginNameAll") || paramName.Equals("loginNameAbove2005") || paramName.Equals("loginNameForUser")
                            || paramName.Equals("tableName")
                            || paramName.Equals("auditSetting") || paramName.Equals("AuditSettings")
                            || paramName.Equals("RegulationGuidelines")
                            )
                        {
                            element.ValueControl.Width = 530;
                            _pnlParams.SetColumnSpan(element.ValueControl, 5);
                            i += 3;
                        }
                        // Skip the middle column that is used for spacing
                        if (i % 5 == 2)
                        {
                            i++;
                        }
                    }
                }
            }

            // Add the execute button on a new last row
            if (!invalidReport)
            {
                if (i % 5 > 0)
                {
                    i += (i % 5);
                }

                _pnlParams.Controls.Add(_btnExecute, 4, i / 5);
                _btnExecute.TabIndex = i;
            }

            foreach (RowStyle style in _pnlParams.RowStyles)
            {
                style.SizeType = SizeType.AutoSize;
            }

            _pnlParams.ResumeLayout(true);
        }

        private void serverComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox serverComboBox = (ComboBox)sender;
            string selectedServer = null;
            selectedServer = (string)serverComboBox.SelectedItem;

            if (_supportedParameters.ContainsKey("dbName") && selectedServer != null)
            {
                CMReportParameter param = _supportedParameters["dbName"];
                ParameterUIElement parameterUIElement = _paramControlMap["dbName"];

                if (selectedServer != "All")
                {
                    int srvId = 0;
                    if (_supportedParameters.ContainsKey("serverName") && selectedServer != null)
                    {
                        CMReportParameter selectedParam = _supportedParameters["serverName"];
                        srvId = Convert.ToInt32(selectedParam.GetItemValue(selectedServer));
                    }
                    param = _supportedParameters["dbName"];
                    LoadDatabases(param, srvId);

                }
                else
                {
                    param.ClearList();
                    param.AddListItem("All", 0);
                }
                parameterUIElement = _paramControlMap["dbName"];
                parameterUIElement.UpdateValues(param);
                parameterUIElement.SetToDefaultValue();
            }
        }

        private void serverComboBox_SelectedIndexChanged1(object sender, EventArgs e)
        {
            ComboBox serverComboBox = (ComboBox)sender;
            string selectedServer = null;
            selectedServer = (string)serverComboBox.SelectedItem;

            if (_supportedParameters.ContainsKey("Database") && selectedServer != null)
            {
                CMReportParameter param = _supportedParameters["Database"];
                ParameterUIElement parameterUIElement = _paramControlMap["Database"];

                if (selectedServer != "<ALL>")
                {
                    string srvName = "<ALL>";
                    if (_supportedParameters.ContainsKey("Server") && selectedServer != null)
                    {
                        CMReportParameter selectedParam = _supportedParameters["Server"];
                        srvName = (string)selectedParam.GetItemValue(selectedServer);
                    }
                    param = _supportedParameters["Database"];
                    LoadDatabasesByServerName(param, srvName);

                }
                else
                {
                    param.ClearList();
                    param.AddListItem("<ALL>", "<ALL>");
                }
                parameterUIElement = _paramControlMap["Database"];
                parameterUIElement.UpdateValues(param);
                parameterUIElement.SetToDefaultValue();
            }
        }

        private void rowCountServerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox serverComboBox = (ComboBox)sender;
            string selectedServer = null;
            selectedServer = (string)serverComboBox.SelectedItem;
            if (selectedServer != "placeholder")
            {
                CMReportParameter eventDatabaseparam = _supportedParameters["eventDatabaseAbove2005"];
                string eventDatabaseForSelectedServer = (string)eventDatabaseparam.GetItemValue(selectedServer);
                CMReportParameter paramLogin = _supportedParameters["loginNameAbove2005"];
                ParameterUIElement parameterLoginUIElement = _paramControlMap["loginNameAbove2005"];
                bool isInitialLoad = true;
                Dictionary<string, object> allLogins = paramLogin.ValueItems;
                foreach (KeyValuePair<string, object> login in allLogins)
                {
                    if (login.Key != "placeholder")
                    {
                        isInitialLoad = false;
                        break;
                    }
                }
                if (selectedServer == "<ALL>" && !isInitialLoad)
                {
                    LoadLogins(paramLogin, "<All>");
                    if (paramLogin.ValueItems.Count > 1)
                    {
                        parameterLoginUIElement.UpdateValuesCheckBoxComboBox(paramLogin);
                    }
                }
                else if (!isInitialLoad)
                {
                    LoadLogins(paramLogin, eventDatabaseForSelectedServer);
                    if (paramLogin.ValueItems.Count > 1)
                    {
                        parameterLoginUIElement.UpdateValuesCheckBoxComboBox(paramLogin);
                    }
                }
            }
        }

        private void serverComboBox_SelectedIndexChanged2(object sender, EventArgs e)
        {
            ComboBox serverComboBox = (ComboBox)sender;
            string selectedServer = null;
            selectedServer = (string)serverComboBox.SelectedItem;
            if (selectedServer != "placeholder")
            {
                CMReportParameter eventDatabaseparam = _supportedParameters["eventDatabase"];
                string eventDatabaseForSelectedServer = (string)eventDatabaseparam.GetItemValue(selectedServer);
                CMReportParameter param = _supportedParameters["loginName"];
                ParameterUIElement parameterUIElement = _paramControlMap["loginName"];
                bool isInitialLoad = true;
                Dictionary<string, object> allLogins = param.ValueItems;
                foreach (KeyValuePair<string, object> login in allLogins)
                {
                    if (login.Key != "placeholder")
                    {
                        isInitialLoad = false;
                        break;
                    }
                }
                if (selectedServer == "<ALL>" && !isInitialLoad)
                {
                    LoadLogins(param, "<All>");
                    if (param.ValueItems.Count > 1)
                    {
                        parameterUIElement.UpdateValuesCheckBoxComboBox(param);
                    }
                }
                else if (!isInitialLoad)
                {
                    LoadLogins(param, eventDatabaseForSelectedServer);
                    if (param.ValueItems.Count > 1)
                    {
                        parameterUIElement.UpdateValuesCheckBoxComboBox(param);
                    }
                }
            }
        }

        private void serverComboBox_SelectedIndexChanged3(object sender, EventArgs e)
        {
            ComboBox serverComboBox = (ComboBox)sender;
            string selectedServer = null;
            selectedServer = (string)serverComboBox.SelectedItem;
            if (selectedServer != "placeholder")
            {
                CMReportParameter eventDatabaseparam = _supportedParameters["eventDatabaseAll"];
                string eventDatabaseForSelectedServer = (string)eventDatabaseparam.GetItemValue(selectedServer);
                CMReportParameter param = _supportedParameters["loginNameAll"];
                ParameterUIElement parameterUIElement = _paramControlMap["loginNameAll"];
                bool isInitialLoad = true;
                Dictionary<string, object> allLogins = param.ValueItems;
                foreach (KeyValuePair<string, object> login in allLogins)
                {
                    if (login.Key != "placeholder")
                    {
                        isInitialLoad = false;
                        break;
                    }
                }
                if (selectedServer == "<ALL>" && !isInitialLoad)
                {
                    LoadLogins(param, "<All>");
                    if (param.ValueItems.Count > 1)
                    {
                        parameterUIElement.UpdateValuesCheckBoxComboBox(param);
                    }
                }
                else if (!isInitialLoad)
                {
                    LoadLogins(param, eventDatabaseForSelectedServer);
                    if (param.ValueItems.Count > 1)
                    {
                        parameterUIElement.UpdateValuesCheckBoxComboBox(param);
                    }
                }
            }
        }

        private void InitializeParameters()
        {
            _supportedParameters = new Dictionary<string, CMReportParameter>();

            CMReportParameter param;
            ParameterUIElement element;
            //added new code for SQLCM-6276
            param = new CMReportParameter
            {
                Name = "fromConsole",
                ValueType = typeof(bool),
            };
            param.DefaultValue = true;
            _supportedParameters.Add(param.Name, param);
            param = new CMReportParameter
            {
                Name = "eventDatabase",
                ValueType = typeof(string),
                Prompt = "Server Instance:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            param.OnChangeEvent = new EventHandler(serverComboBox_SelectedIndexChanged2);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "eventDatabaseAll",
                ValueType = typeof(string),
                Prompt = "Server Instance:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            param.OnChangeEvent = new EventHandler(serverComboBox_SelectedIndexChanged3);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "eventDatabaseAbove2005",
                ValueType = typeof(string),
                Prompt = "Server Instance:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            System.EventHandler eventHandler = new System.EventHandler(rowCountServerComboBox_SelectedIndexChanged);
            param.OnChangeEvent = eventHandler;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "databaseNameAll",
                ValueType = typeof(string),
                Prompt = "Database:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "instance",
                ValueType = typeof(string),
                Prompt = "Server Instance:",
                UsedInQuery = true
            };
            param.AddListItem("placeholder", "Placeholder");
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "alertLevel",
                ValueType = typeof(int),
                Prompt = "Alert Level:",
                UsedInQuery = true
            };
            param.AddListItem("<ALL>", 0);
            param.AddListItem("Severe", 4);
            param.AddListItem("High", 3);
            param.AddListItem("Medium", 2);
            param.AddListItem("Low", 1);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "applicationName",
                ValueType = typeof(string),
                Prompt = "Application:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "databaseName",
                ValueType = typeof(string),
                Prompt = "Database:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "hostName",
                ValueType = typeof(string),
                Prompt = "Host:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "loginName",
                ValueType = typeof(string),
                Prompt = "Login:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            //param.DefaultValue = "*";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "loginNameAll",
                ValueType = typeof(string),
                Prompt = "Login:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "loginNameAbove2005",
                ValueType = typeof(string),
                Prompt = "Login:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "loginNameForUser",
                ValueType = typeof(string),
                Prompt = "Login:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "roleNameForUser",
                ValueType = typeof(string),
                Prompt = "Role:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "objectName",
                ValueType = typeof(string),
                Prompt = "Target Object:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "primaryKey",
                ValueType = typeof(string),
                Prompt = "Primary Key:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "tableName",
                ValueType = typeof(string),
                Prompt = "Table Name:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "schemaName",
                ValueType = typeof(string),
                Prompt = "Schema:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            #region 5.7 PRD- 5.2.4.5
            param = new CMReportParameter
            {
                Name = "agent",
                ValueType = typeof(string),
                Prompt = "Agent:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "event",
                ValueType = typeof(string),
                Prompt = "Event:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            #endregion 5.7 5.2.4.5

            param = new CMReportParameter
            {
                Name = "eventCategory",
                ValueType = typeof(int),
                Prompt = "Category:",
                UsedInQuery = true
            };
            param.AddListItem("placeholder", "Placeholder");
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "loginStatus",
                ValueType = typeof(int),
                Prompt = "Login Status:",
                UsedInQuery = true
            };
            param.AddListItem("Both", 0);
            param.AddListItem("Login", 1);
            param.AddListItem("Login Failed", 2);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "userType",
                ValueType = typeof(string),
                Prompt = "User Type:",
                UsedInQuery = true
            };
            param.AddListItem("Both", "Both");
            param.AddListItem("Trusted", "Trusted");
            param.AddListItem("Privileged", "Privileged");
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "dataType",
                ValueType = typeof(string),
                Prompt = "Data Type:",
                UsedInQuery = true
            };
            param.AddListItem("Both", "Both");
            param.AddListItem("Sensitive Column", "Sensitive Column");
            param.AddListItem("Before After Data", "Before After Data");
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "startDate",
                ValueType = typeof(DateTime),
                Prompt = "Start Date:",
                UsedInQuery = true,
                DefaultValue = DateTime.Today.AddDays(-7)
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _startDate = (DateTimePicker)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "endDate",
                ValueType = typeof(DateTime),
                Prompt = "End Date:",
                UsedInQuery = true,
                DefaultValue = DateTime.Today//.AddDays(1);
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _endDate = (DateTimePicker)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "privilegedUserOnly",
                ValueType = typeof(bool),
                Prompt = "Privileged Users Only:",
                UsedInQuery = true
            };
            param.AddListItem("False", false);
            param.AddListItem("True", true);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "showSqlText",
                ValueType = typeof(bool),
                Prompt = "Show SQL:",
                UsedInQuery = true
            };
            param.AddListItem("False", false);
            param.AddListItem("True", true);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "rowCountThreshold",
                ValueType = typeof(int),
                Prompt = "Row Count Threshold:",
                UsedInQuery = true,
                DefaultValue = Settings.Default.RowCountThreshold
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "columnName",
                ValueType = typeof(string),
                Prompt = "Column Name:",
                UsedInQuery = true,
                DefaultValue = Settings.Default.ColumnName
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "privilegedUsers",
                ValueType = typeof(string),
                Prompt = "Privileged User:",
                UsedInQuery = true,
                DefaultValue = Settings.Default.PrivilegedUsers
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "sortColumn",
                ValueType = typeof(string),
                Prompt = "",
                UsedInQuery = true,
                DefaultValue = "date"
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "rowCount",
                ValueType = typeof(int),
                Prompt = "",
                UsedInQuery = true,
                DefaultValue = Settings.Default.ReportRowCount
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "ruleName",
                ValueType = typeof(string),
                Prompt = "Rule:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "serverInstance",
                ValueType = typeof(string),
                Prompt = "Server Instance:",
                UsedInQuery = true,
                DefaultValue = "*"
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "logMessage",
                ValueType = typeof(string),
                Prompt = "Event Log:",
                UsedInQuery = true
            };
            param.AddListItem("All", -1);
            param.AddListItem("Yes", 1);
            param.AddListItem("No", 0);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "emailMessage",
                ValueType = typeof(string),
                Prompt = "Email:",
                UsedInQuery = true
            };
            param.AddListItem("All", -1);
            param.AddListItem("Yes", 1);
            param.AddListItem("No", 0);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "ruleType",
                ValueType = typeof(string),
                Prompt = "Rule Type:",
                UsedInQuery = true
            };
            param.AddListItem("All", 0);
            param.AddListItem("Event Rules", 1);
            param.AddListItem("Status Rules", 2);
            param.AddListItem("Data Rules", 3);
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "dateType",
                ValueType = typeof(int),
                Prompt = "",
                UsedInQuery = true,
                DefaultValue = 0
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "serverName",
                ValueType = typeof(int),
                Prompt = "Server Instance:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            System.EventHandler eventHandler1 = new System.EventHandler(serverComboBox_SelectedIndexChanged);
            param.OnChangeEvent = eventHandler1;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "dbName",
                ValueType = typeof(int),
                Prompt = "Database:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "auditSetting",
                ValueType = typeof(int),
                Prompt = "Audit Setting:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = false;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "status",
                ValueType = typeof(int),
                Prompt = "Default Status:"
            };
            param.AddListItem("All", 0);
            param.AddListItem("Same", 2);
            param.AddListItem("Different", 1);
            param.UsedInQuery = false;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);


            param = new CMReportParameter
            {
                Name = "flag",
                ValueType = typeof(bool),
                Prompt = "",
                UsedInQuery = true
            };
            _supportedParameters.Add(param.Name, param);


            //SQLCM-5465
            param = new CMReportParameter
            {
                Name = "Server",
                ValueType = typeof(string),
                Prompt = "Server Instance:",
                UsedInQuery = true
            };
            //param.IsList = true;
            param.AddListItem("placeholder", "Placeholder");
            param.OnChangeEvent = new System.EventHandler(serverComboBox_SelectedIndexChanged1);
            param.DefaultValue = "<ALL>";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "Database",
                ValueType = typeof(string),
                Prompt = "Database:"
            };
            param.AddListItem("placeholder", "Placeholder");
            param.UsedInQuery = true;
            //param.IsList = true;
            param.DefaultValue = "<ALL>";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "RegulationGuidelines",
                ValueType = typeof(string)
            };
            param.AddListItem("placeholder", "Placeholder");
            param.Prompt = "Regulatory Guidelines:";
            param.DefaultValue = "<ALL>";
            param.UsedInQuery = true;
            //param.IsList = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "AuditSettings",
                ValueType = typeof(string)
            };
            param.AddListItem("placeholder", "Placeholder");
            param.Prompt = "Audit Settings:";
            param.DefaultValue = "<ALL>";
            param.UsedInQuery = true;
            //param.IsList = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "Values",
                ValueType = typeof(string),
                Prompt = "Values:"
            };
            param.AddListItem("Selected", "Selected");
            param.AddListItem("Varies", "Varies");
            param.AddListItem("Deselected", "Deselected");
            param.AddListItem("N/A", "N/A");
            param.UsedInQuery = true;
            param.DefaultValue = "Selected";
            //param.IsList = true;
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "dateFrom",
                ValueType = typeof(DateTime),
                Prompt = "Date From:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "dateTo",
                ValueType = typeof(DateTime),
                Prompt = "Date To:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "CISServerSettings",
                ValueType = typeof(int),
                Prompt = "CIS Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "CISDatabaseSettings",
                ValueType = typeof(int),
                Prompt = "CIS Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "DISASTIGServerSettings",
                ValueType = typeof(int),
                Prompt = "DISASTIG Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "DISASTIGDatabaseSettings",
                ValueType = typeof(int),
                Prompt = "DISASTIG Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "FERPAServerSettings",
                ValueType = typeof(int),
                Prompt = "FERPA Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "FERPADatabaseSettings",
                ValueType = typeof(int),
                Prompt = "FERPA Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "GDPRServerSettings",
                ValueType = typeof(int),
                Prompt = "GDPR Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "GDPRDatabaseSettings",
                ValueType = typeof(int),
                Prompt = "GDPR Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "HIPAAServerSettings",
                ValueType = typeof(int),
                Prompt = "HIPAA Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "HIPAADatabaseSettings",
                ValueType = typeof(int),
                Prompt = "HIPAA Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "NERCServerSettings",
                ValueType = typeof(int),
                Prompt = "NERC Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "NERCDatabaseSettings",
                ValueType = typeof(int),
                Prompt = "NERC Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "PCIDSSServerSettings",
                ValueType = typeof(int),
                Prompt = "PCIDSS Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "PCIDSSDatabaseSettings",
                ValueType = typeof(int),
                Prompt = "PCIDSS Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "SOXServerSettings",
                ValueType = typeof(int),
                Prompt = "SOX Server Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "SOXDatabaseSettings",
                ValueType = typeof(int),
                Prompt = "SOX Database Settings:",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);

            param = new CMReportParameter
            {
                Name = "serverActivityCategory",
                ValueType = typeof(string),
                Prompt = "Server Activity:"
            };
            param.AddListItem("Event Alerts", "4");
            param.AddListItem("Logins", "16");
            param.AddListItem("Logouts", "23");
            param.AddListItem("Failed Logins", "6");
            param.AddListItem("Security", "10");
            param.AddListItem("DDL", "9");
            param.AddListItem("Privileged User", "5");
            param.AddListItem("Overall Activity", "21");
            param.UsedInQuery = false;
            param.DefaultValue = "4";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            #region 5.2.4.4
            // Using type as Char to distinguish the non display controls (labels)
            param = new CMReportParameter
            {
                Name = "StartTimeofDay",
                ValueType = typeof(char),
                Prompt = "Start Time for Each Day",
                UsedInQuery = false,
                DefaultValue = ""
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "EndTimeofDay",
                ValueType = typeof(char),
                Prompt = "End Time for Each Day",
                UsedInQuery = false,
                DefaultValue = ""
            };
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);

            param = new CMReportParameter
            {
                Name = "hourOfStartTime",
                ValueType = typeof(string),
                Prompt = "Start Time - Hour:"
            };
            param.AddListItem("1", "1");
            param.AddListItem("2", "2");
            param.AddListItem("3", "3");
            param.AddListItem("4", "4");
            param.AddListItem("5", "5");
            param.AddListItem("6", "6");
            param.AddListItem("7", "7");
            param.AddListItem("8", "8");
            param.AddListItem("9", "9");
            param.AddListItem("10", "10");
            param.AddListItem("11", "11");
            param.AddListItem("12", "12");
            param.UsedInQuery = false;
            param.DefaultValue = "12";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _startHour = (ComboBox)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "hourOfEndTime",
                ValueType = typeof(string),
                Prompt = "End Time - Hour:"
            };
            param.AddListItem("1", "1");
            param.AddListItem("2", "2");
            param.AddListItem("3", "3");
            param.AddListItem("4", "4");
            param.AddListItem("5", "5");
            param.AddListItem("6", "6");
            param.AddListItem("7", "7");
            param.AddListItem("8", "8");
            param.AddListItem("9", "9");
            param.AddListItem("10", "10");
            param.AddListItem("11", "11");
            param.AddListItem("12", "12");
            param.UsedInQuery = false;
            param.DefaultValue = "11";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _endHour = (ComboBox)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "minuteOfStartTime",
                ValueType = typeof(string),
                Prompt = "Start Time - Min:"
            };
            param.AddListItem("00", "00");
            param.AddListItem("01", "01");
            param.AddListItem("02", "02");
            param.AddListItem("03", "03");
            param.AddListItem("04", "04");
            param.AddListItem("05", "05");
            param.AddListItem("06", "06");
            param.AddListItem("07", "07");
            param.AddListItem("08", "08");
            param.AddListItem("09", "09");
            param.AddListItem("10", "10");
            param.AddListItem("11", "11");
            param.AddListItem("12", "12");
            param.AddListItem("13", "13");
            param.AddListItem("14", "14");
            param.AddListItem("15", "15");
            param.AddListItem("16", "16");
            param.AddListItem("17", "17");
            param.AddListItem("18", "18");
            param.AddListItem("19", "19");
            param.AddListItem("20", "20");
            param.AddListItem("21", "21");
            param.AddListItem("22", "22");
            param.AddListItem("23", "23");
            param.AddListItem("24", "24");
            param.AddListItem("25", "25");
            param.AddListItem("26", "26");
            param.AddListItem("27", "27");
            param.AddListItem("28", "28");
            param.AddListItem("29", "29");
            param.AddListItem("30", "30");
            param.AddListItem("31", "31");
            param.AddListItem("32", "32");
            param.AddListItem("33", "33");
            param.AddListItem("34", "34");
            param.AddListItem("35", "35");
            param.AddListItem("36", "36");
            param.AddListItem("37", "37");
            param.AddListItem("38", "38");
            param.AddListItem("39", "39");
            param.AddListItem("40", "40");
            param.AddListItem("41", "41");
            param.AddListItem("42", "42");
            param.AddListItem("43", "43");
            param.AddListItem("44", "44");
            param.AddListItem("45", "45");
            param.AddListItem("46", "46");
            param.AddListItem("47", "47");
            param.AddListItem("48", "48");
            param.AddListItem("49", "49");
            param.AddListItem("50", "50");
            param.AddListItem("51", "51");
            param.AddListItem("52", "52");
            param.AddListItem("53", "53");
            param.AddListItem("54", "54");
            param.AddListItem("55", "55");
            param.AddListItem("56", "56");
            param.AddListItem("57", "57");
            param.AddListItem("58", "58");
            param.AddListItem("59", "59");
            param.UsedInQuery = false;
            param.DefaultValue = "00";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _startMinute = (ComboBox)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "minuteOfEndTime",
                ValueType = typeof(string),
                Prompt = "End Time - Min:"
            };
            param.AddListItem("00", "00");
            param.AddListItem("01", "01");
            param.AddListItem("02", "02");
            param.AddListItem("03", "03");
            param.AddListItem("04", "04");
            param.AddListItem("05", "05");
            param.AddListItem("06", "06");
            param.AddListItem("07", "07");
            param.AddListItem("08", "08");
            param.AddListItem("09", "09");
            param.AddListItem("10", "10");
            param.AddListItem("11", "11");
            param.AddListItem("12", "12");
            param.AddListItem("13", "13");
            param.AddListItem("14", "14");
            param.AddListItem("15", "15");
            param.AddListItem("16", "16");
            param.AddListItem("17", "17");
            param.AddListItem("18", "18");
            param.AddListItem("19", "19");
            param.AddListItem("20", "20");
            param.AddListItem("21", "21");
            param.AddListItem("22", "22");
            param.AddListItem("23", "23");
            param.AddListItem("24", "24");
            param.AddListItem("25", "25");
            param.AddListItem("26", "26");
            param.AddListItem("27", "27");
            param.AddListItem("28", "28");
            param.AddListItem("29", "29");
            param.AddListItem("30", "30");
            param.AddListItem("31", "31");
            param.AddListItem("32", "32");
            param.AddListItem("33", "33");
            param.AddListItem("34", "34");
            param.AddListItem("35", "35");
            param.AddListItem("36", "36");
            param.AddListItem("37", "37");
            param.AddListItem("38", "38");
            param.AddListItem("39", "39");
            param.AddListItem("40", "40");
            param.AddListItem("41", "41");
            param.AddListItem("42", "42");
            param.AddListItem("43", "43");
            param.AddListItem("44", "44");
            param.AddListItem("45", "45");
            param.AddListItem("46", "46");
            param.AddListItem("47", "47");
            param.AddListItem("48", "48");
            param.AddListItem("49", "49");
            param.AddListItem("50", "50");
            param.AddListItem("51", "51");
            param.AddListItem("52", "52");
            param.AddListItem("53", "53");
            param.AddListItem("54", "54");
            param.AddListItem("55", "55");
            param.AddListItem("56", "56");
            param.AddListItem("57", "57");
            param.AddListItem("58", "58");
            param.AddListItem("59", "59");
            param.UsedInQuery = false;
            param.DefaultValue = "59";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _endMinute = (ComboBox)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "amPmOfStartTime",
                ValueType = typeof(string),
                Prompt = "Start Time - AM/PM:"
            };
            param.AddListItem("AM", "AM");
            param.AddListItem("PM", "PM");
            param.UsedInQuery = false;
            param.DefaultValue = "AM";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _amPmStart = (ComboBox)element.ValueControl;

            param = new CMReportParameter
            {
                Name = "amPmOfEndTime",
                ValueType = typeof(string),
                Prompt = "End Time - AM/PM:"
            };
            param.AddListItem("AM", "AM");
            param.AddListItem("PM", "PM");
            param.UsedInQuery = false;
            param.DefaultValue = "PM";
            _supportedParameters.Add(param.Name, param);
            element = new ParameterUIElement(param);
            _paramControlMap.Add(param.Name, element);
            _amPmEnd = (ComboBox)element.ValueControl;

            #endregion

            param = new CMReportParameter
            {
                Name = "generatedFrom",
                ValueType = typeof(string),
                Prompt = "Generated From:",
                DefaultValue = "Windows",
                UsedInQuery = false
            };
            _supportedParameters.Add(param.Name, param);
            _btnExecute = new Button
            {
                Text = "Run Report"
            };
            _btnExecute.Click += Click_btnExecute;
        }

        private void LoadServerInstancesAbove2005(CMReportParameter param, bool includeAll)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetEventDatabasesAbove2005", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    if (includeAll)
                    {
                        SqlParameter sqlParam = new SqlParameter("includeAll", SqlDbType.Bit)
                        {
                            Value = 1
                        };
                        cmd.Parameters.Add(sqlParam);
                    }

                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            try
                            {
                                param.AddListItem(reader.GetString(1), reader.GetString(0));
                            }
                            catch (Exception e)
                            {
                                ErrorLog.Instance.Write(string.Format("Unable to add {0} to the list of instances.", reader.GetString(1)), e, ErrorLog.Severity.Error);
                            }
                        }
                    }
                }
            }
        }

        private void LoadServerInstances(CMReportParameter param, bool includeAll)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetEventDatabases", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    if (includeAll)
                    {
                        SqlParameter sqlParam = new SqlParameter("includeAll", SqlDbType.Bit)
                        {
                            Value = 1
                        };
                        cmd.Parameters.Add(sqlParam);
                    }

                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                param.AddListItem(reader.GetString(1), reader.GetString(0));
                            }
                            catch (Exception e)
                            {
                                ErrorLog.Instance.Write(string.Format("Unable to add {0} to the list of instances.", reader.GetString(1)), e, ErrorLog.Severity.Error);
                            }
                        }
                    }
                }
            }
        }

        private void LoadDatabases(CMReportParameter param, int srvId)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetDatabases", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    SqlParameter sqlParam = new SqlParameter("serverid", SqlDbType.Int)
                    {
                        Value = srvId
                    };
                    cmd.Parameters.Add(sqlParam);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            param.AddListItem(reader.GetString(4), (reader.GetInt32(3)).ToString());
                        }
                    }
                }
            }
        }

        private void LoadDatabasesByServerName(CMReportParameter param, string srvName)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetDatabasesByServerName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    SqlParameter sqlParam = new SqlParameter("serverName", SqlDbType.NVarChar)
                    {
                        Value = srvName
                    };
                    cmd.Parameters.Add(sqlParam);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dbName = reader.GetString(0);
                            param.AddListItem(dbName, dbName);
                        }
                    }
                }
            }
        }

        private void LoadAuditSettings(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetAuditSettings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            param.AddListItem(reader.GetString(1), (reader.GetInt32(0)).ToString());
                        }
                    }
                }
            }
        }

        private void LoadServerInstances(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetServerInstances", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            param.AddListItem(reader.GetString(1), (reader.GetInt32(0)).ToString());
                        }
                    }
                }
            }
        }

        private void LoadInstances(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetInstances", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string serverName = reader.GetString(0);
                            param.AddListItem(serverName, serverName);
                        }
                    }
                }
            }
        }

        private void LoadAllRegulationGuidelines(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetAllRegulationGuidelines", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            param.AddListItem(reader.GetString(1), reader.GetInt32(0));
                        }
                    }
                }
            }
        }

        private void LoadAllAuditSettings(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetAllAuditSettings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            param.AddListItem(reader.GetString(1), reader.GetInt32(0));
                        }
                    }
                }
            }
        }

        private void LoadDateTime(CMReportParameter param1, CMReportParameter param2)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT TOP (1) timeCreated AS dateFrom, GETDATE() AS dateTo FROM {0}", CoreConstants.RepositoryServerTable), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param1.ClearList();
                    param2.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            param1.AddListItem(reader.GetDateTime(0).ToString(), param1.DefaultValue = reader.GetDateTime(0));

                            param2.AddListItem(reader.GetDateTime(1).ToString(), param2.DefaultValue = reader.GetDateTime(1));
                        }
                    }
                }
            }
        }

        private void LoadSDConfigurationSettings(CMReportParameter param1, CMReportParameter param2, int regulationId)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetStandardRegulationSettings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param1.ClearList();
                    param2.ClearList();
                    SqlParameter sqlParam = new SqlParameter("regulationId", SqlDbType.Int)
                    {
                        Value = regulationId
                    };
                    cmd.Parameters.Add(sqlParam);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            param1.AddListItem(reader.GetInt32(0).ToString(), param1.DefaultValue = reader.GetInt32(0));
                            param2.AddListItem(reader.GetInt32(1).ToString(), param2.DefaultValue = reader.GetInt32(1));
                        }
                    }
                }
            }
        }

        private void LoadAgentInstances(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetInstances", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string s = reader.GetString(0);
                            param.AddListItem(s, s);
                        }
                    }
                }
            }
        }

        private void LoadCategories(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetCategories", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            param.AddListItem(reader.GetString(1), reader.GetInt32(0));
                        }
                    }
                }
            }
        }

        private void LoadLogins(CMReportParameter param, string eventDatabaseName)
        {
            CMReportParameter param1 = new CMReportParameter();
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetAllLogins", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    SqlParameter sqlParam = new SqlParameter("eventDatabaseName", SqlDbType.NVarChar)
                    {
                        Value = eventDatabaseName
                    };
                    SqlParameter sqlParam2 = new SqlParameter("requestedFrom", SqlDbType.NVarChar)
                    {
                        Value = "Windows"
                    };
                    cmd.Parameters.Add(sqlParam);
                    cmd.Parameters.Add(sqlParam2);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string loginName = reader.GetString(0);
                            var chars = "~`>!?;:|{}[]@#$%^*()+=\"";
                            if (loginName != "" && loginName != null)
                            {
                                if (chars.Any(x => loginName.StartsWith(x.ToString()))) //(loginName.StartsWith() || loginName.StartsWith("*") || loginName.StartsWith("{") || loginName.StartsWith("["))
                                {
                                    param1.AddListItem(loginName, loginName);
                                }
                                else
                                {
                                    param.AddListItem(loginName, loginName);
                                }
                            }
                        }
                    }
                    foreach (var item in param1.ValueItems)
                    {
                        param.AddListItem(item.Value.ToString(),item.Value.ToString());
                    }
                }
            }
        }

        private void LoadLoginsForUser(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                //using (SqlCommand cmd = new SqlCommand(string.Format("SELECT '<ALL>' as name, 0 as sid UNION SELECT name, sid FROM master..syslogins WHERE sid <> 0"), conn))
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT '<ALL>' as name, 0 as sid UNION SELECT distinct u.loginName,l.uid FROM Users u join Logins l on u.loginName = l.name where u.loginName is not NULL"),conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string loginName = reader.GetString(0);
                            if (loginName != "" && loginName != null)
                            {
                                param.AddListItem(loginName, loginName);
                            }
                        }
                    }
                }
            }
        }

        private void LoadRolesForUser(CMReportParameter param)
        {
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT '<ALL>' as name, 0 as number UNION SELECT v2.name, v1.number FROM master..spt_values v1, master..spt_values v2 where v1.low = 0 and v1.type = 'SRV' and v2.low = -1 and v2.type = 'SRV' and v1.number = v2.number UNION select role.name,role.principal_id from sys.server_principals role where role.name not in (SELECT v1.name FROM master..spt_values v1, master..spt_values v2 where v1.low = 0 and v1.type = 'SRV' and v2.low = -1 and v2.type = 'SRV' and v1.number = v2.number) and role.type = 'R' and role.name <> 'public'"), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    param.ClearList();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string roleName = reader.GetString(0);
                            if (roleName != "" && roleName != null)
                            {
                                param.AddListItem(roleName, roleName);
                            }
                        }
                    }
                }
            }
        }

        public void ShowInfo()
        {
            foreach (string s in _reportViewer.LocalReport.GetDataSourceNames())
            {
                switch (s)
                {
                    case "Servers":
                        _reportViewer.LocalReport.DataSources.Add(BuildServersDataSource());
                        break;
                    case "Categories":
                        _reportViewer.LocalReport.DataSources.Add(BuildCategoriesDataSource());
                        break;
                    case "ReportOutput":
                        _reportViewer.LocalReport.DataSources.Add(BuildOutputDataSource());
                        break;
                    case "Regulations":
                        _reportViewer.LocalReport.DataSources.Add(BuildRegulationsDataSource());
                        break;
                    case "ServerConfiguration":
                        _reportViewer.LocalReport.DataSources.Add(BuildServerConfigurationDataSources());
                        break;
                    case "DatabaseConfiguration":
                        _reportViewer.LocalReport.DataSources.Add(BuildDatabaseConfigurationDataSources());
                        break;
                    case "ParamValues":
                        _reportViewer.LocalReport.DataSources.Add(BuildParamValuesDataSources());
                        break;
                    case "ServerAuditSettings":
                        _reportViewer.LocalReport.DataSources.Add(BuildServerAuditDataSources());
                        break;
                    case "DatabaseAuditSettings":
                        _reportViewer.LocalReport.DataSources.Add(BuildDatabaseAuditDataSources());
                        break;
                    case "DefaultServerSettings":
                        _reportViewer.LocalReport.DataSources.Add(BuildDefaultServerDataSources());
                        break;
                    case "DefaultDatabaseSettings":
                        _reportViewer.LocalReport.DataSources.Add(BuildDefaultDatabaseDataSources());
                        break;
                    case "IntegrityCheck":
                        _reportViewer.LocalReport.DataSources.Add(BuildDefaultIntegrityCheckDataSources());
                        break;
                    case "ServerActivity":
                        _reportViewer.LocalReport.DataSources.Add(BuildServerActivityDataSources());
                        break;

                }
            }
        }

        private ReportDataSource BuildDefaultDatabaseDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("DefaultDatabaseSettings");
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("select auditAdmin,auditCaptureDDL,auditCaptureSQL,auditCaptureTrans,auditDDL,auditDML," +
                "auditFailures,auditPrivUsersList,auditSecurity,auditSELECT, auditUserAdmin, auditUserAll, auditUserCaptureDDL, auditUserCaptureSQL," +
                " auditUserCaptureTrans, auditUserDDL, auditUserDML, auditUserFailedLogins,auditUserFailures, auditUserLogins, auditUserLogouts," +
                " auditUserSecurity, auditUserSELECT, auditUsersList, auditUserUDE from DefaultDatabasePropertise", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildDefaultIntegrityCheckDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("IntegrityCheck");
            _report.StoredProcedure = "sp_cmreport_GetIntegrityCheckStatus";
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        SqlParameter sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true);
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        if (paramName == "eventDatabase")
                        {
                            cmd.Parameters.Add(sqlParam);
                        }
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            _report.StoredProcedure = "sp_cmreport_GetIntegrityCheck";
            return retVal;
        }

        private ReportDataSource BuildServerActivityDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("ServerActivity");
            _report.StoredProcedure = "sp_cmreport_GetServerActivityGraph";
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        SqlParameter sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true);
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        if (paramName == "flag")
                        {
                            sqlParam.Value = 0;
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            _report.StoredProcedure = "sp_cmreport_GetServerActivity";
            return retVal;
        }

        private ReportDataSource BuildDefaultServerDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("DefaultServerSettings");
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("select auditAdmin, auditCaptureSQLXE, auditDDL, auditFailedLogins, auditFailures, auditLogins," +
                "auditLogouts, auditSecurity, auditTrace, auditTrustedUsersList, auditUDE,auditUserAdmin, auditUserAll, auditUserCaptureDDL," +
                " auditUserCaptureSQL, auditUserCaptureTrans, auditUserDDL, auditUserDML, auditUserFailedLogins, auditUserFailures,auditUserLogins," +
                " auditUserLogouts, auditUserSecurity, auditUserSELECT, auditUsersList, auditUserUDE, defaultAccess, isAuditLogEnabled," +
                " maxSqlLength from DefaultServerPropertise", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private bool HasValidDates()
        {
            if (_usesDate)
            {
                return _startDate.Value <= _endDate.Value;
            }

            return true;
        }

        private bool HasValidTimeFrame()
        {
            if (_usesDate)
            {
                string startHour = _startHour.SelectedItem.ToString();
                string startMin = _startMinute.SelectedItem.ToString();
                string amPmStart = _amPmStart.SelectedItem.ToString();
                string endHour = _endHour.SelectedItem.ToString();
                string endMin = _endMinute.SelectedItem.ToString();
                string amPmEnd = _amPmEnd.SelectedItem.ToString();

                string startdatestr = startHour + ":" + startMin + amPmStart;
                string enddatestr = endHour + ":" + endMin + amPmEnd;

                DateTime startdt = DateTime.Parse(startdatestr);
                DateTime enddt = DateTime.Parse(enddatestr);
                if (enddt > startdt)
                {
                    TimeSpan timespan = enddt - startdt;
                    return (timespan.TotalMinutes > 4);
                }
            }
            return true;
        }

        private bool HasValidTime()
        {
            if (_usesDate)
            {
                string startHour = _startHour.SelectedItem.ToString();
                string startMin = _startMinute.SelectedItem.ToString();
                string amPmStart = _amPmStart.SelectedItem.ToString();
                string endHour = _endHour.SelectedItem.ToString();
                string endMin = _endMinute.SelectedItem.ToString();
                string amPmEnd = _amPmEnd.SelectedItem.ToString();

                string startdatestr = startHour + ":" + startMin + amPmStart;
                string enddatestr = endHour + ":" + endMin + amPmEnd;

                DateTime startdt = DateTime.Parse(startdatestr);
                DateTime enddt = DateTime.Parse(enddatestr);

                return (startdt <= enddt);
            }
            return true;
        }

        private void Click_btnExecute(object sender, EventArgs e)
        {
            try
            {
                if (!HasValidDates())
                {
                    MessageBox.Show(this, "The Start Date must be less than or equal to the End Date.", "Invalid Dates");
                    return;
                }

                //if (!HasValidTime())
                //{
                //    MessageBox.Show(this, "Your End Time must be greater than your Start Time.", "Invalid Times");
                //    return;

                if (!HasValidTimeFrame())
                {
                    MessageBox.Show(this, "Your Start Time and End Time are less than 5 minutes apart.", "Warning");
                    return;
                }

                if (_bgReportLoader.IsBusy)
                {
                    return;
                }

                _btnExecute.Enabled = false;
                _reportViewer.Reset();

                Cursor = Cursors.WaitCursor;
                _mainForm.UseWaitCursor = true;
                _mainForm.Cursor = Cursors.WaitCursor;
                using (Stream s = ReportXmlHelper.ReplaceDataSetReferences(_report.FileName, _dataSetReferences))
                {
                    _reportViewer.LocalReport.LoadReportDefinition(s);
                    _reportViewer.LocalReport.EnableHyperlinks = true;
                }

                _bgReportLoader.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append(ex.Message);
                if (ex.InnerException != null)
                {
                    msg.AppendFormat("\n\t{0}", ex.InnerException.Message);
                    if (ex.InnerException.InnerException != null)
                    {
                        msg.AppendFormat("\n\t\t{0}", ex.InnerException.InnerException.Message);
                    }
                }
                MessageBox.Show(this, msg.ToString(), "An error occurred executing the report.");
            }
        }

        private void DoWork_bgReportLoader(object sender, DoWorkEventArgs e)
        {
            try
            {
                ShowInfo();
                if ((sender as BackgroundWorker).CancellationPending)
                {
                    e.Cancel = true;
                }
            }
            catch(Exception ex)
            {
                Cursor = Cursors.Default;
                _mainForm.UseWaitCursor = false;
                _mainForm.Cursor = Cursors.Default;
                _btnExecute.Enabled = true;
            }
        }

        private void RunWorkerCompleted_bgReportLoader(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Cursor = Cursors.Default;
                _mainForm.UseWaitCursor = false;
                _mainForm.Cursor = Cursors.Default;
                _btnExecute.Enabled = true;
                RefreshView();
            }
            else
            {
                BuildReportParameters();
                _reportViewer.RefreshReport();
                Cursor = Cursors.Default;
                _mainForm.UseWaitCursor = false;
                _mainForm.Cursor = Cursors.Default;
                _btnExecute.Enabled = true;
            }
        }

        #endregion

        //
        // BuildReportParameters(): Use the input values in the input controls to build report
        //                          parameters.
        private void BuildReportParameters()
        {
            List<ReportParameter> rpList = new List<ReportParameter>();

            foreach (ReportParameterInfo rp in _reportViewer.LocalReport.GetParameters())
            {
                CMReportParameter cmrp = _supportedParameters[rp.Name];
                object paramValue;
                if (_paramControlMap.ContainsKey(rp.Name))
                {
                    ParameterUIElement uiElement = _paramControlMap[rp.Name];
                    if (rp.Name == "eventDatabase" || rp.Name == "eventDatabaseAll" || rp.Name == "eventCategory")
                    {
                        paramValue = uiElement.ExtractValue(false, true);
                    }
                    else
                    {
                        paramValue = uiElement.ExtractValue(false);
                    }


                    if (rp.Name == "rowCountThreshold")
                    {
                        int temp = 0;
                        Int32.TryParse((paramValue).ToString(), out temp);
                        paramValue = temp;
                        if (temp == 0)
                        {
                            uiElement.SetToDefaultValue();
                        }
                    }
                }
                else
                {
                    paramValue = cmrp.DefaultValue;
                }
                //if (paramValue != null && paramValue.ToString().Length > 512)
                //    throw new Exception("Value too long");

                rpList.Add(new ReportParameter(rp.Name, paramValue.ToString()));
            }
            _reportViewer.LocalReport.SetParameters(rpList);
        }

        private ReportDataSource BuildOutputDataSource()
        {
            ReportDataSource retVal = new ReportDataSource("ReportOutput");

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        SqlParameter sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true);
                            if (paramName == "rowCountThreshold")
                            {
                                int temp = 0;
                                Int32.TryParse((sqlParam.Value).ToString(), out temp);
                                sqlParam.Value = temp;
                            }
                            if (_report.Name == AUDIT_CONTROL_CHANGES && paramName == "eventDatabase")
                            {
                                sqlParam.Value = uiElement.ExtractValue(true, true);
                            }
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }

                        // Special case for login name custom control dropdown to remove extra space
                        if (sqlParam.Value.ToString() == "<ALL> ")
                        {
                            sqlParam.Value = sqlParam.Value.ToString().TrimEnd();
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    if (Report.Name == DML_ACTIVITY_REPORT)
                    {
                        InsertSchemaNameIfExists(cmd.Parameters);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private void InsertSchemaNameIfExists(SqlParameterCollection parameters)
        {
            string objectName = (string)parameters["@objectName"].Value;

            if (objectName.Contains("."))
            {
                parameters["@schemaName"].Value = objectName.Split('.')[0];
                parameters["@objectName"].Value = objectName.Split('.')[1];
            }
        }

        private ReportDataSource BuildServersDataSource()
        {
            ReportDataSource retVal = new ReportDataSource("Servers");

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetEventDatabases", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildServerConfigurationDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("ServerConfiguration");
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        SqlParameter sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true);
                            if (paramName == "dbName")
                            {
                                sqlParam.Value = 9999;
                            }
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        if (paramName == "flag")
                        {
                            sqlParam.Value = 0;
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildServerAuditDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("ServerAuditSettings");
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    SqlParameter sqlParam;
                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        if (paramName == "getServerSettings")
                        {
                            sqlParam = new SqlParameter("getServerSettings", SqlDbType.Int)
                            {
                                Value = 1
                            };
                            cmd.Parameters.Add(sqlParam);
                            continue;
                        }

                        if (paramName == "serverName")
                        {
                            paramName = "Server";
                        }
                        else if (paramName == "dbName")
                        {
                            paramName = "Database";
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true, (_report.Name == REGULATORY_COMPLIANCE_CHECK_REPORT));
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildDatabaseAuditDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("DatabaseAuditSettings");
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    SqlParameter sqlParam;
                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        if (paramName == "getServerSettings")
                        {
                            sqlParam = new SqlParameter("getServerSettings", SqlDbType.Int)
                            {
                                Value = 0
                            };
                            cmd.Parameters.Add(sqlParam);
                            continue;
                        }

                        if (paramName == "serverName")
                        {
                            paramName = "Server";
                        }
                        else if (paramName == "dbName")
                        {
                            paramName = "Database";
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true, (_report.Name == REGULATORY_COMPLIANCE_CHECK_REPORT));
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildParamValuesDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("ParamValues");
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetParameterName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        SqlParameter sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true);
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        if (paramName == "flag")
                        {
                            sqlParam.Value = 0;
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }
        private ReportDataSource BuildDatabaseConfigurationDataSources()
        {
            ReportDataSource retVal = new ReportDataSource("DatabaseConfiguration");

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand(_report.StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    foreach (string spParamName in _report.GetParameters(conn))
                    {
                        string paramName;

                        if (spParamName.StartsWith("@"))
                        {
                            paramName = spParamName.Substring(1);
                        }
                        else
                        {
                            paramName = spParamName;
                        }

                        CMReportParameter param = _supportedParameters[paramName];
                        SqlParameter sqlParam = new SqlParameter(spParamName, param.SqlDbType);
                        if (_paramControlMap.ContainsKey(paramName))
                        {
                            ParameterUIElement uiElement = _paramControlMap[paramName];
                            sqlParam.Value = uiElement.ExtractValue(true);
                            if (paramName == "dbName")
                            {
                                sqlParam.Value = 9999;
                            }
                        }
                        else
                        {
                            sqlParam.Value = param.DefaultValue;
                        }
                        if (paramName == "flag")
                        {
                            sqlParam.Value = 1;
                        }
                        cmd.Parameters.Add(sqlParam);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildRegulationsDataSource()
        {
            ReportDataSource retVal = new ReportDataSource("Regulations");

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("p_cmreport_GetRegulationGuidelines", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private ReportDataSource BuildCategoriesDataSource()
        {
            ReportDataSource retVal = new ReportDataSource("Categories");

            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_cmreport_GetCategories", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    retVal.Value = ds.Tables[0];
                }
            }
            return retVal;
        }

        private void BuildDataSetReferences()
        {
            List<ValuePair> pairs = new List<ValuePair>();
            _dataSetReferences.Clear();
            using (SqlConnection conn = Globals.Repository.GetPooledConnection())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    cmd.CommandText = "sp_cmreport_GetEventDatabases";
                    SqlParameter sqlParam = new SqlParameter("includeAll", SqlDbType.Bit)
                    {
                        Value = 0
                    };
                    cmd.Parameters.Add(sqlParam);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetString(0)));
                        }
                    }
                    AddDataSetReference("eventDatabase", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetEventDatabases";
                    sqlParam = new SqlParameter("includeAll", SqlDbType.Bit)
                    {
                        Value = 1
                    };
                    cmd.Parameters.Add(sqlParam);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetString(0)));
                        }
                    }
                    AddDataSetReference("eventDatabaseAll", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetCategories";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetInt32(0)));
                        }
                    }
                    AddDataSetReference("eventCategory", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetInstances";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(0), reader.GetString(0)));
                        }
                    }
                    AddDataSetReference("AllServers", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetDatabasesByServerName";
                    sqlParam = new SqlParameter("serverName", SqlDbType.NVarChar)
                    {
                        Value = "<ALL>"
                    };
                    cmd.Parameters.Add(sqlParam);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(0), reader.GetString(0)));
                        }
                    }
                    AddDataSetReference("AllDatabases", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetAllRegulationGuidelines";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetInt32(0)));
                        }
                    }
                    AddDataSetReference("AllRegulationSettings", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetAllAuditSettings";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetInt32(0)));
                        }
                    }
                    AddDataSetReference("AllAuditSettings", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetServerInstances";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetInt32(0)));
                        }
                    }
                    AddDataSetReference("Servers", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetDatabases";
                    sqlParam = new SqlParameter("serverid", SqlDbType.Int)
                    {
                        Value = 0
                    };
                    cmd.Parameters.Add(sqlParam);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(4), reader.GetInt32(3)));
                        }
                    }
                    AddDataSetReference("Databases", pairs);

                    pairs.Clear();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "sp_cmreport_GetAuditSettings";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new ValuePair(reader.GetString(1), reader.GetInt32(0)));
                        }
                    }
                    AddDataSetReference("AuditSettings", pairs);
                }
            }
        }

        private void AddDataSetReference(string name, List<ValuePair> items)
        {
            StringBuilder str = new StringBuilder();

            str.Append("<ParameterValues>");
            foreach (ValuePair p in items)
            {
                str.AppendFormat("<ParameterValue><Value>{0}</Value><Label>{1}</Label></ParameterValue>",
                   ValuePair.XmlFormat(p.Value.ToString()), ValuePair.XmlFormat(p.DisplayName));
            }
            str.Append("</ParameterValues>");

            _dataSetReferences.Add(name, str.ToString());

        }

        private void ReportView_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            HelpOnThisWindow();
            hlpevent.Handled = true;
        }

        public override void HelpOnThisWindow()
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ReportsView);
        }
    }

    internal class ParameterUIElement
    {
        public Control ValueControl;
        public Label Prompt;
        private CMReportParameter _param;

        public ParameterUIElement(CMReportParameter param)
        {
            _param = param;
            Prompt = new Label
            {
                Margin = new Padding(3),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = 21,
                Width = 150,
                Text = param.Prompt
            };

            if (param.Name.StartsWith("loginName") ||
                param.Name.StartsWith("roleName"))
            {
                CheckBoxComboBox cb = CheckBoxDropDown();
                ValueControl = cb;
                if (param.OnChangeEvent != null)
                {
                    cb.SelectedIndexChanged += param.OnChangeEvent;
                }
                UpdateValuesCheckBoxComboBox(param);
            }
            else if (param.IsList)
            {
                ComboBox cb = BuildComboBox();
                ValueControl = cb;
                if (param.OnChangeEvent != null)
                {
                    cb.SelectedIndexChanged += param.OnChangeEvent;
                }
                UpdateValues(param);
            }
            else if (param.ValueType == typeof(DateTime))
            {
                ValueControl = BuildDateTimePicker();
            }
            else if (param.ValueType == typeof(char))
            {
                Label lbl = new Label();
                ValueControl = lbl;
            }
            else
            {
                TextBox tb = BuildTextBox();
                ValueControl = tb;
            }

        }

        public void UpdateValues(CMReportParameter param)
        {
            if (param.IsList)
            {
                ComboBox cb = (ComboBox)ValueControl;
                cb.Items.Clear();
                foreach (string key in param.ValueItems.Keys)
                {
                    cb.Items.Add(key);
                }
                if (cb.Items.Count > 0)
                {
                    cb.SelectedIndex = 0;
                }
            }
        }

        public void UpdateValuesCheckBoxComboBox(CMReportParameter param)
        {
            if (param.IsList)
            {
                CheckBoxComboBox cb = (CheckBoxComboBox)ValueControl;
                cb.Items.Clear();
                foreach (string key in param.ValueItems.Keys)
                {
                    cb.Items.Add(key);
                }
                if (cb.Items.Count > 1)
                {
                    cb.CheckBoxItems[1].Checked = true;
                }
            }
        }

        public void SetToDefaultValue()
        {
            if (ValueControl is ComboBox)
            {
                ComboBox cb = (ComboBox)ValueControl;
                if (_param.DefaultValue != null)
                {
                    cb.SelectedItem = _param.DefaultValue;
                }
                else if (cb.Items.Count > 0)
                {
                    cb.SelectedIndex = 0;
                }
            }
            else if (ValueControl is TextBox)
            {
                if (_param.DefaultValue != null)
                {
                    TextBox tb = (TextBox)ValueControl;
                    tb.Text = _param.DefaultValue.ToString();
                }
            }
            else if (ValueControl is DateTimePicker)
            {
                if (_param.DefaultValue != null && _param.DefaultValue is DateTime)
                {
                    DateTimePicker dtp = (DateTimePicker)ValueControl;
                    dtp.Value = (DateTime)_param.DefaultValue;
                }
            }
            else if (ValueControl is Label)
            {
                if (_param.DefaultValue != null)
                {
                    Label lb = (Label)ValueControl;
                    lb.Text = _param.DefaultValue.ToString();
                }
            }
        }

        public object ExtractValue(bool adjustTime, bool isRCC = false)
        {
            if (ValueControl is ComboBox && !(ValueControl is PresentationControls.CheckBoxComboBox))
            {
                ComboBox cb = (ComboBox)ValueControl;
                string selectedValue = cb.SelectedItem.ToString();
                if (isRCC)
                {
                    return selectedValue;
                }
                return _param.GetItemValue(selectedValue);
            }
            else if (ValueControl is PresentationControls.CheckBoxComboBox)
            {
                CheckBoxComboBox cb = (CheckBoxComboBox)ValueControl;
                string selectedValue;
                if (cb.SelectedItem == null)
                {
                    selectedValue = "";
                }
                else
                {
                    selectedValue = cb.SelectedItem.ToString();
                }
                return selectedValue;
            }
            else if (ValueControl is TextBox)
            {
                TextBox tb = (TextBox)ValueControl;
                string retVal = tb.Text;
                if (retVal.Length > 512)
                {
                    string message = string.Format(UIConstants.Error_ReportParameterValueTooLong,
                                                    _param.Prompt.TrimEnd(": ".ToCharArray()));
                    throw new Exception(message);
                }
                return retVal;
            }
            else if (ValueControl is Label)
            {
                Label lb = (Label)ValueControl;
                string retVal = lb.Text;
                if (retVal.Length > 512)
                {
                    string message = string.Format(UIConstants.Error_ReportParameterValueTooLong,
                                                    _param.Prompt.TrimEnd(": ".ToCharArray()));
                    throw new Exception(message);
                }
                return retVal;
            }
            else if (ValueControl is DateTimePicker)
            {
                DateTime retVal;
                DateTimePicker dtp = (DateTimePicker)ValueControl;
                if (adjustTime)
                {
                    retVal = dtp.Value.ToUniversalTime();
                }
                else
                {
                    retVal = dtp.Value;
                }
                // Special handling for endDate
                if (_param.Name == "endDate")
                {
                    retVal = retVal.AddDays(1);
                    retVal = retVal.AddSeconds(-1);
                }
                return retVal;
            }
            return null;
        }

        private static ComboBox BuildComboBox()
        {
            ComboBox retVal = new ComboBox
            {
                Height = 21,
                Width = 175,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            return retVal;
        }

        private static CheckBoxComboBox CheckBoxDropDown()
        {
            CheckBoxComboBox retVal = new CheckBoxComboBox
            {
                Height = 21,
                Width = 175,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            return retVal;
        }

        private static TextBox BuildTextBox()
        {
            TextBox retVal = new TextBox
            {
                Height = 21,
                Width = 175
            };
            return retVal;
        }

        private static DateTimePicker BuildDateTimePicker()
        {
            DateTimePicker retVal = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Height = 21,
                Width = 175
            };
            return retVal;
        }
    }

    internal class ValuePair
    {
        private string _displayName;
        private object _value;

        public ValuePair(string name, object pairValue)
        {
            _displayName = name;
            _value = pairValue;
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public static string XmlFormat(string s)
        {
            string retVal = s.Replace("<", "&lt;");
            retVal = retVal.Replace(">", "&gt;");
            return retVal;
        }
    }
}
