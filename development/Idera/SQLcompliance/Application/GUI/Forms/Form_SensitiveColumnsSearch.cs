using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Infragistics.Win;
using Infragistics.Win.UltraMessageBox;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinEditors;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    public partial class Form_SensitiveColumnsSearch : Form
    {
        private Timer _instanceTypingTimer;

        private readonly SensitiveColumnSearchHelper _helper;
        private readonly SensitiveColumnProfileHelper _profileHelper;
        private readonly string _serverName;
        private readonly string _databaseName;
        private bool _isInitialLoading;
        private string _activeProfile;

        public Form_SensitiveColumnsSearch(string serverName = null, string databaseName = null)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _helper = new SensitiveColumnSearchHelper();
            _profileHelper = new SensitiveColumnProfileHelper();
            InitializeComponent();
        }

        private void Form_SearchSensitiveColumns_Load(object sender, EventArgs e)
        {
            ClearDatabases();
            _isInitialLoading = true;
            RefreshInstances();
            _isInitialLoading = false;
            CheckActiveProfile();
            UpdateSearchState();
            UpdateCounter(0);
        }

        private void CheckActiveProfile()
        {
            _activeProfile = GetProfileName();
            ulProfile.Text = _activeProfile ?? "None Selected";
            UpdateSearchState();
        }

        private string GetInstanceName()
        {
            return ucbInstances.Text;
        }

        private string GetDatabaseName()
        {
            return ucbDatabases.SelectedIndex == 0 ? null : ucbDatabases.Text;
        }

        private string GetTableName()
        {
            return ucbTables.SelectedIndex == 0 ? null : ucbTables.Text;
        }

        private string GetProfileName()
        {
            var profileName = _profileHelper.GetActiveProfile();
            return string.IsNullOrEmpty(profileName) ? null : profileName;
        }

        private void ubtnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(GetInstanceName()))
            {
                ClearResults();
                ExecuteAction(() =>
                {
                    var databaseName = GetDatabaseName();

                    var databases = new List<string>();
                    if (string.IsNullOrWhiteSpace(databaseName))
                    {
                        databases.AddRange(_helper.GetDatabases(GetInstanceName()));
                    }
                    else
                    {
                        databases.Add(databaseName);
                    }


                    var counter = 0;
                    foreach (var database in databases)
                    {
                        var tableName = GetTableName();

                        var tables = new List<string>();
                        if (string.IsNullOrWhiteSpace(tableName))
                        {
                            tables.AddRange(_helper.GetTables(GetInstanceName(), database));
                        }
                        else
                        {
                            tables.Add(tableName);
                        }

                        foreach (var rawTableDetails in _helper.SearchTables(
                            GetInstanceName(),
                            database,
                            tables,
                            GetProfileName()
                        ))
                        {
                            ultraDataSourceTables.Rows.Add(true, new object[]
                            {
                                rawTableDetails.DatabaseName,
                                rawTableDetails.TableName,
                                rawTableDetails.Size,
                                rawTableDetails.RowCount,
                                rawTableDetails.ColumnsIdentified
                            });
                            counter += rawTableDetails.ColumnsIdentified;
                        }
                    }
                    UpdateCounter(counter);
                });
            }
        }

        private void ClearResults()
        {
            ultraDataSourceTables.Rows.Clear();
            ultraDataSourceColumns.Rows.Clear();
            UpdateCounter(0);
        }

        private void ubtnRefreshInstances_Click(object sender, EventArgs e)
        {
            RefreshInstances();
        }

        private void ubtnRefreshDatabases_Click(object sender, EventArgs e)
        {
            RefreshDatabases();
        }

        private void ubtnRefreshTables_Click(object sender, EventArgs e)
        {
            RefreshTables();
        }

        private void ubtnExportReport_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            dlg.FileName = GenerateReportName();
            dlg.Title = "Export Sensitive Columns Report";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    ExecuteAction(() => ExportToCsv(dlg.FileName));
                    UltraMessageBoxManager.Show("Export finished successfully",
                        "SQL Compliance Manager",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowError(new Exception(string.Format("Unable to write to '{0}'", dlg.FileName), ex));
                }
            }
        }

        private void ubtnAddToAudit_Click(object sender, EventArgs e)
        {
            IEnumerable<RawColumnDetails> sensitiveColumns = null;
            ExecuteAction(() => sensitiveColumns = LoadAllSensitiveColumnsFromResult());
            var databaseName = ultraGridTables.ActiveRow.Cells["Database"].Value.ToString();
            var tableName = ultraGridTables.ActiveRow.Cells["Schema.Table"].Value.ToString();
            using (var form = new Form_SensitiveColumnsAddToAudit(GetInstanceName(), sensitiveColumns, databaseName, tableName))
            {
                form.ShowDialog(this);
            }
        }

        private void ubtnConfigure_Click(object sender, EventArgs e)
        {
            using (var form = new Form_SensitiveColumnsProfiles())
            {
                form.ShowDialog(this);
                CheckActiveProfile();
            }
        }

        private void ucbInstances_ValueChanged(object sender, EventArgs e)
        {
            if (_isInitialLoading)
            {
                HandleInstanceTypingTimerTimeout(null, null);
                return;
            }
            if (_instanceTypingTimer == null)
            {
                _instanceTypingTimer = new Timer();
                _instanceTypingTimer.Interval = 800;

                _instanceTypingTimer.Tick += HandleInstanceTypingTimerTimeout;
            }
            _instanceTypingTimer.Stop();
            _instanceTypingTimer.Start();
        }

        private void HandleInstanceTypingTimerTimeout(object sender, EventArgs e)
        {
            var timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
            }

            RefreshDatabases();
            UpdateSearchState();
        }

        private void ucbDatabases_ValueChanged(object sender, EventArgs e)
        {
            RefreshTables();
        }

        private void ultraGridTables_AfterRowActivate(object sender, EventArgs e)
        {
            var databaseName = ultraGridTables.ActiveRow.Cells["Database"].Value.ToString();
            var tableName = ultraGridTables.ActiveRow.Cells["Schema.Table"].Value.ToString();
            var profileName = GetProfileName();

            ExecuteAction(() =>
            {
                ultraDataSourceColumns.Rows.Clear();

                foreach (var rawColumnDetails in _helper.SearchColumns(
                    GetInstanceName(),
                    databaseName,
                    new[] { tableName },
                    profileName))
                {
                    ultraDataSourceColumns.Rows.Add(true, new object[]
                    {
                        rawColumnDetails.FieldName,
                        rawColumnDetails.DataType,
                        rawColumnDetails.LengthSize
                    });
                }
            });
        }

        private void RefreshInstances()
        {
            ExecuteAction(() =>
            {
                ucbInstances.Items.Clear();
                FillComboEditor(
                    ucbInstances,
                    _helper.GetSQLServerInstances(_isInitialLoading),
                    _isInitialLoading ? _serverName : null);
            });
        }

        private void RefreshDatabases()
        {
            var instance = GetInstanceName();
            ClearDatabases();
            if (!string.IsNullOrWhiteSpace(instance))
            {
                ExecuteAction(() =>
                {
                    FillComboEditor(
                        ucbDatabases,
                        _helper.GetDatabases(instance),
                        _isInitialLoading ? _databaseName : null);
                });
            }
        }

        private void RefreshTables()
        {
            var instance = GetInstanceName();
            var database = GetDatabaseName();
            ClearTables();
            if (!string.IsNullOrWhiteSpace(instance) && !string.IsNullOrWhiteSpace(database))
            {
                ExecuteAction(() =>
                {
                    FillComboEditor(
                        ucbTables,
                        _helper.GetTables(instance, database));
                });
            }
        }

        private void FillComboEditor(
            UltraComboEditor comboEditor,
            IEnumerable<string> items,
            string preselectedItem = null)
        {
            foreach (var item in items)
            {
                var listItem = new ValueListItem(item);
                comboEditor.Items.Add(listItem);
                if (string.Equals(item, preselectedItem, StringComparison.InvariantCultureIgnoreCase))
                {
                    comboEditor.SelectedItem = listItem;
                }
            }
        }

        private void ExecuteAction(Action action)
        {
            Focus();
            System.Windows.Forms.Application.UseWaitCursor = true;
            System.Windows.Forms.Application.DoEvents();
            try
            {
                action();
                System.Windows.Forms.Application.UseWaitCursor = false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.Application.UseWaitCursor = false;
                ShowError(ex);
            }
        }

        private string GenerateReportName()
        {
            var safeInstanceName = string.Join(string.Empty, GetInstanceName().Split(Path.GetInvalidFileNameChars()));
            return string.Format("{0}-Export-{1:yyyMMdd}-{2:HHmmss}", safeInstanceName, DateTime.Now, DateTime.Now);
        }

        private void ExportToCsv(string fileName)
        {
            using (FileStream fileStream = File.Create(fileName))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.WriteLine("DatabaseName, Schema.Table, Columns Matched, Column Type, Max Length, String Matched");
                foreach (var rawColumnDetails in LoadAllSensitiveColumnsFromResult())
                {
                    writer.WriteLine(
                        string.Format("{0}, {1}, {2}, {3}, {4}, {5}", rawColumnDetails.DatabaseName,
                            rawColumnDetails.TableName, rawColumnDetails.FieldName, rawColumnDetails.DataType,
                            rawColumnDetails.LengthSize, rawColumnDetails.MatchingStr));
                }
            }
        }

        private IEnumerable<RawColumnDetails> LoadAllSensitiveColumnsFromResult()
        {
            var tablesByDatabases = new Dictionary<string, List<string>>();
            foreach (UltraDataRow tableRow in ultraDataSourceTables.Rows)
            {
                var databaseName = tableRow["Database"].ToString();
                var tableName = tableRow["Schema.Table"].ToString();
                if (!tablesByDatabases.ContainsKey(databaseName))
                {
                    tablesByDatabases[databaseName] = new List<string>();
                }
                tablesByDatabases[databaseName].Add(tableName);
            }

            var result = new List<RawColumnDetails>();
            var profileName = GetProfileName();

            foreach (var tables in tablesByDatabases)
            {
                result.AddRange(_helper.SearchColumns(
                    GetInstanceName(),
                    tables.Key,
                    tables.Value,
                    profileName));
            }

            return result;
        }
        private void UpdateCounter(int count)
        {
            ulColumnCount.Text = count.ToString();
            ubtnAddToAudit.Enabled = count > 0;
            ubtnExportReport.Enabled = count > 0;
        }

        private void UpdateSearchState()
        {
            ubtnSearch.Enabled = _activeProfile != null
                                 && !string.IsNullOrWhiteSpace(GetInstanceName());
        }

        private void ShowError(Exception ex, string message = null)
        {
            UltraMessageBoxManager.Show(message ?? ex.Message,
                "SQL Compliance Manager",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ClearTables()
        {
            ClearCombobox(ucbTables, "table");
            ClearResults();
        }

        private void ClearDatabases()
        {
            ClearCombobox(ucbDatabases, "database");
            ClearTables();
        }

        private void ClearCombobox(UltraComboEditor comboEditor, string entityName)
        {
            comboEditor.Clear();
            comboEditor.Items.Clear();
            comboEditor.Items.Add(string.Format("Select a {0} (blank for all)", entityName));
            comboEditor.SelectedIndex = 0;
        }

        private void llHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpAlias.ShowHelp(this, HelpAlias.SSHELP_SensitiveColumnSearch);
        }
    }
}
