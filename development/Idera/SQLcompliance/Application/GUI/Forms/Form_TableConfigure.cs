using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.Properties;
using Idera.SQLcompliance.Application.GUI.SQL;
using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Configure a table for BA auditing.
    /// </summary>
    public partial class Form_TableConfigure : Form
    {
        private RawTableObject _table;
        private ICollection _columnList;
        private List<string> _selectedColumns;
        private UIConstants.Table_Column_Usage _usage;
        private Dictionary<string, IList> _dataSet;

        //-----------------------------------------------------------------------------
        // Form_TableConfigure (Constructor)
        //-----------------------------------------------------------------------------
        public Form_TableConfigure(RawTableObject table, int maxRows, ICollection columnList, ICollection selectedColumns, UIConstants.Table_Column_Usage usage, Dictionary<string, IList> dataSet = null)
        {
            InitializeComponent();
            Icon = Resources.SQLcompliance_product_ico;


            if (maxRows == -1)
                _comboMaxRows.SelectedItem = "All";
            else
                _comboMaxRows.SelectedItem = maxRows.ToString();

            _table = table;
            _columnList = columnList;
            _selectedColumns = new List<string>();
            _usage = usage;
            _dataSet = dataSet;

            switch (_usage)
            {
                case UIConstants.Table_Column_Usage.BADColumns:
                case UIConstants.Table_Column_Usage.BADTables:
                    {
                        this.Text += " for table " + table.TableName;
                        if (selectedColumns.Count > 0)
                        {
                            foreach (string col in selectedColumns)
                            {
                                _selectedColumns.Add(col);
                            }
                        }
                        else if (!table.HasBlobData)
                        {
                            foreach (RawColumnObject col in columnList)
                            {
                                _selectedColumns.Add(col.ColumnName);
                            }
                        }

                        if (table.HasBlobData)
                        {
                            _lblDescription.Text = "This table contains BLOB columns such as binary or text which cannot be audited. Specific columns must be selected to enable Before-After auditing.";
                            _radioColumnsSelect.Checked = true;
                            _radioColumnsAll.Enabled = false;
                        }
                        else
                        {
                            _lblDescription.Text = "All columns will be audited unless only specific columns are selected.";
                            _radioColumnsAll.Enabled = true;
                            _radioColumnsAll.Checked = selectedColumns.Count == 0;
                            _radioColumnsSelect.Checked = !_radioColumnsAll.Checked;
                        }
                        break;
                    }
                case UIConstants.Table_Column_Usage.SensitiveColumns:
                    {
                        this.Text += " for table " + table.DisplayName;
                        if (selectedColumns.Count > 0)
                        {
                            foreach (string col in selectedColumns)
                            {
                                _selectedColumns.Add(col);
                            }                            
                        }
                        else
                        {
                            if (dataSet != null && dataSet.Count > 0)
                            {
                                foreach (string col in columnList)
                                {
                                    _selectedColumns.Add(col);
                                }
                            }
                            else
                            {
                                foreach (RawColumnObject col in columnList)
                                {
                                    _selectedColumns.Add(col.ColumnName);
                                }
                            }
                        }
                        _radioColumnsAll.Enabled = true;
                        _radioColumnsAll.Checked = selectedColumns.Count == 0;
                        _radioColumnsSelect.Checked = !_radioColumnsAll.Checked;

                        _lblHeader.Text = "Configure Sensitive Columns";
                        _lblDescription.Text = "All columns will be audited unless only specific columns are selected.";
                        _lblRows.Visible = false;
                        _comboMaxRows.Visible = false;
                        this.Size = new System.Drawing.Size(this.Size.Width, 442);
                        _radioColumnsAll.Enabled = true;
                        _radioColumnsSelect.Enabled = true;
                        if (_dataSet != null && _dataSet.Count > 0)
                        {
                            panel1.Enabled = true;
                            if (_radioColumnsAll.Checked && listAvailable.Items.Count == 0)
                            {
                                _radioColumnsSelect.Enabled = false;
                            }
                        }
                        break;
                    }
                case UIConstants.Table_Column_Usage.Filter:
                default:
                    {
                        this.Text += " for table " + table.TableName;
                        if (selectedColumns.Count > 0)
                        {
                            foreach (string col in selectedColumns)
                            {
                                _selectedColumns.Add(col);
                            }
                        }
                        else
                        {
                            foreach (RawColumnObject col in columnList)
                            {
                                _selectedColumns.Add(col.ColumnName);
                            }
                        }
                        _lblHeader.Text = "Configure Column Auditing";
                        _lblDescription.Text = "All columns will be audited unless only specific columns are selected.";
                        _radioColumnsAll.Enabled = true;
                        _radioColumnsAll.Checked = selectedColumns.Count == 0;
                        _radioColumnsSelect.Checked = !_radioColumnsAll.Checked;
                        _lblRows.Visible = false;
                        _comboMaxRows.Visible = false;
                        this.Size = new System.Drawing.Size(this.Size.Width, 442);
                        break;
                    }
            }
        }

        public int MaximumRows
        {
            get
            {
                return GetMaxRows(_comboMaxRows.SelectedItem.ToString());
            }
        }

        public static int GetMaxRows(string s)
        {
            // this function should be the same as in Form_MaxRows, so keep the code in one place
            return Form_MaxRows.GetMaxRows(s);
        }

        public static string GetMaxRowString(int i)
        {
            // this function should be the same as in Form_MaxRows, so keep the code in one place
            return Form_MaxRows.GetMaxRowString(i);
        }

        public static string[] GetColumns(string s)
        {
            if (String.IsNullOrEmpty(s) || s.Equals(UIConstants.BAD_AllColumns) || s.Equals(UIConstants.BAD_NoColumns))
                return new string[] { };
            else
            {
                try
                {
                    return s.Split(new char[] { ',' });
                }
                catch (Exception)
                {
                    // Default
                    return new string[] { };
                }
            }
        }

        public static string GetColumnString(string[] cols)
        {
            if (cols == null || cols.Length == 0)
                return UIConstants.BAD_NoColumns;
            else
            {
                try
                {
                    return String.Join(",", cols);
                }
                catch (Exception)
                {
                    // Default
                    return UIConstants.BAD_NoColumns;
                }
            }
        }
        public bool AllColumns
        {
            get
            {
                if (_usage == UIConstants.Table_Column_Usage.SensitiveColumns)
                    return _radioColumnsAll.Checked;
                else
                    return !_table.HasBlobData && _radioColumnsAll.Checked;
            }
        }

        public List<string> SelectedColumns
        {
            get { return _selectedColumns; }
        }

        public string SelectedColumnsString
        {
            get { return GetColumnString(_selectedColumns.ToArray()); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadColumns();
        }


        #region Load Logic

        //-----------------------------------------------------------------------------
        // LoadTables
        //-----------------------------------------------------------------------------
        private void LoadColumns()
        {
            listSelected.SuspendLayout();
            listSelected.BeginUpdate();
            listAvailable.SuspendLayout();
            listAvailable.BeginUpdate();
            Cursor = Cursors.WaitCursor;

            listSelected.Items.Clear();
            listAvailable.Items.Clear();

            
            if ((_columnList != null) && (_columnList.Count != 0))
            {
                if (_usage == UIConstants.Table_Column_Usage.SensitiveColumns && _dataSet != null && _dataSet.Count > 0)
              {
                foreach (string column  in _columnList)
                {
                    ListViewItem x;
                    string columnName = column; 
                        if (_selectedColumns.Contains(columnName))
                        {
                            x = listSelected.Items.Add(columnName);
                        }
                        else
                        {
                            x = listAvailable.Items.Add(columnName);
                        }
                        x.Tag = column;
                    }
            }
            else
              { 
                foreach (RawColumnObject column  in _columnList)
                {
                    ListViewItem x;
                    //DatabaseObjectRecord dbo = new DatabaseObjectRecord(column);
                    string columnName = column.ColumnName;
                    if (_selectedColumns.Contains(columnName))
                    {
                        x = listSelected.Items.Add(columnName);
                        if (column.HasBlobData)
                        {
                            if (_usage == UIConstants.Table_Column_Usage.BADColumns ||
                                _usage == UIConstants.Table_Column_Usage.BADTables)
                            {
                                x.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    else
                    {
                        if (column.HasBlobData)
                        {
                            x = listAvailable.Items.Add(string.Format("{0} (BLOB data)", columnName));

                            if (_usage == UIConstants.Table_Column_Usage.BADColumns ||
                                _usage == UIConstants.Table_Column_Usage.BADTables)
                            {
                                x.ForeColor = System.Drawing.Color.LightGray;
                            }
                        }
                        else
                        {
                            x = listAvailable.Items.Add(columnName);
                        }
                    }
                    x.Tag = column;
                    }
                }
            }
            else if (_columnList == null)
            {
                ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
            }
            listAvailable.EndUpdate();
            listAvailable.ResumeLayout();
            listSelected.EndUpdate();
            listSelected.ResumeLayout();
            Cursor = Cursors.Default;
        }

        #endregion

        #region OK / CANCEL Logic

        //-----------------------------------------------------------------------------
        // btnCancel_Click
        //-----------------------------------------------------------------------------
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //-----------------------------------------------------------------------------
        // btnOK_Click
        //-----------------------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_radioColumnsSelect.Checked && listSelected.Items.Count == 0)
            {
                ErrorMessage.Show(this.Text, UIConstants.Error_NoColumns);
                DialogResult = DialogResult.None;
                return;
            }
            _selectedColumns.Clear();

            foreach (ListViewItem col in listSelected.Items)
            {
                if ((_usage == UIConstants.Table_Column_Usage.BADColumns ||
                     _usage == UIConstants.Table_Column_Usage.BADTables) &&
                     ((RawColumnObject)col.Tag).HasBlobData)
                {
                    ErrorMessage.Show(this.Text, UIConstants.Error_BlobColumnsNotSupported);
                    DialogResult = DialogResult.None;
                    return;
                }
                _selectedColumns.Add(col.Text);
            }
            if (_dataSet != null && _dataSet.Count > 0)
            {
                try
                {
                    bool contains = false;
                    foreach (KeyValuePair<string, IList> entry in _dataSet)
                    {
                        bool isMatched = false;
                        List<string> listValues = new List<string>();
                        foreach (RawColumnObject o in entry.Value)
                        {
                            listValues.Add(entry.Key+"."+o.ColumnName);
                        }
                        if (_selectedColumns.Intersect(listValues).Any())
                        {
                            isMatched = true;
                        }
                        contains = isMatched;
                        if (!isMatched)
                        {
                            break;
                        }
                    }

                    if (_radioColumnsAll.Checked)
                    {
                        contains = true;
                    }

                    if (!contains)
                    {
                        string errorMessage = "Atleast one column of each table in a Multi-Table DataSet must be selected.";
                        MessageBox.Show(this, errorMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #region Add & Remove Logic

        //-----------------------------------------------------------------------------
        // btnAdd_Click
        //-----------------------------------------------------------------------------
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelected();
        }

        //-----------------------------------------------------------------------------
        // AddSelected
        //-----------------------------------------------------------------------------
        private void AddSelected()
        {
            ListViewItem tmp;

            if ((_usage == UIConstants.Table_Column_Usage.BADColumns ||
                 _usage == UIConstants.Table_Column_Usage.BADTables) &&
                 ((RawColumnObject)listAvailable.SelectedItems[0].Tag).HasBlobData)
            {
                return;
            }
            listSelected.BeginUpdate();
            listAvailable.BeginUpdate();
            int ndx = listAvailable.SelectedItems[0].Index;
            listSelected.SelectedItems.Clear();

            foreach (ListViewItem itm in listAvailable.SelectedItems)
            {
                itm.Remove();
                tmp = listSelected.Items.Add(itm);
                tmp.Selected = true;
            }

            if (listAvailable.Items.Count != 0)
            {
                if (ndx >= listAvailable.Items.Count)
                {
                    listAvailable.Items[listAvailable.Items.Count - 1].Selected = true;
                }
                else
                    listAvailable.Items[ndx].Selected = true;
            }
            if (_dataSet != null && _dataSet.Count > 0)
            {
                if (listAvailable.Items.Count == 0)
                {
                    _radioColumnsAll.Checked = true;
                }
            }
            listAvailable.Focus();
            listSelected.EndUpdate();
            listAvailable.EndUpdate();
        }

        //-----------------------------------------------------------------------------
        // btnRemove_Click
        //-----------------------------------------------------------------------------
        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        //-----------------------------------------------------------------------------
        // RemoveSelected
        //-----------------------------------------------------------------------------
        private void RemoveSelected()
        {
            listSelected.BeginUpdate();
            listAvailable.BeginUpdate();

            int ndx = listSelected.SelectedItems[0].Index;
            ListViewItem tmp;

            listAvailable.SelectedItems.Clear();

            foreach (ListViewItem itm in listSelected.SelectedItems)
            {
                itm.Remove();
                tmp = listAvailable.Items.Add(itm);
                tmp.Selected = true;
            }
            if (listSelected.Items.Count != 0)
            {
                if (ndx >= listSelected.Items.Count)
                {
                    listSelected.Items[listSelected.Items.Count - 1].Selected = true;
                }
                else
                    listSelected.Items[ndx].Selected = true;
            }
            if (_dataSet != null && _dataSet.Count > 0)
            {
                if (listAvailable.Items.Count != 0)
                {
                    _radioColumnsSelect.Enabled = true;
                    _radioColumnsSelect.Checked = true;
                }
            }

            listSelected.Focus();

            listSelected.EndUpdate();
            listAvailable.EndUpdate();
        }

        //-----------------------------------------------------------------------------
        // listAvailable_SelectedIndexChanged
        //-----------------------------------------------------------------------------
        private void listAvailable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listAvailable.SelectedItems.Count == 0)
            {
                btnAdd.Enabled = false;
            }
            else
            {
                bool valid = false;
                foreach (ListViewItem lvi in listAvailable.SelectedItems)
                {
                    if ((_usage == UIConstants.Table_Column_Usage.BADColumns ||
                         _usage == UIConstants.Table_Column_Usage.BADTables) &&
                         ((RawColumnObject)lvi.Tag).HasBlobData)
                    {
                        lvi.Selected = false;
                    }
                    else
                    {
                        valid = true;
                    }
                }

                btnAdd.Enabled = valid;
            }
        }

        //-----------------------------------------------------------------------------
        // listSelected_SelectedIndexChanged
        //-----------------------------------------------------------------------------
        private void listSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listSelected.SelectedItems.Count == 0)
            {
                btnRemove.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = true;
            }
        }

        //-----------------------------------------------------------------------------
        // listAvailable_DoubleClick
        //-----------------------------------------------------------------------------
        private void listAvailable_DoubleClick(object sender, EventArgs e)
        {
            if (btnAdd.Enabled)
                AddSelected();
        }

        //-----------------------------------------------------------------------------
        // listSelected_DoubleClick
        //-----------------------------------------------------------------------------
        private void listSelected_DoubleClick(object sender, EventArgs e)
        {
            if (btnRemove.Enabled)
                RemoveSelected();
        }

        #endregion

        #region Help
        //--------------------------------------------------------------------
        // Form_TableAdd_HelpRequested - Show Context Sensitive Help
        //--------------------------------------------------------------------
        private void Form_TableAdd_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (_usage == UIConstants.Table_Column_Usage.BADColumns ||
                 _usage == UIConstants.Table_Column_Usage.BADTables)
            {
                HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_TableAdd_BAD);
            }
            else
            {
                HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_TableAdd_SC);
            }
            hlpevent.Handled = true;
        }
        #endregion

        private void _radioColumnsAll_CheckedChanged(object sender, EventArgs e)
        {
            if (_radioColumnsAll.Checked)
            {
                if ((_dataSet != null && _dataSet.Count > 0) && listAvailable.Items.Count == 0)
                {
                    _radioColumnsSelect.Enabled = false;
                }
                else
                {
                    panel1.Enabled = false;
                }
            }
        }

        private void _radioColumnsSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (_radioColumnsSelect.Checked)
            {
                panel1.Enabled = true;
                if (listAvailable.Items.Count != 0)
                {
                    listAvailable.Items[0].Selected = true;
                }
                else if (listSelected.Items.Count != 0)
                {
                    listSelected.Items[0].Selected = true;
                }
            }
        }
    }
}