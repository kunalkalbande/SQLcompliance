using System ;
using System.Collections ;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.EnterpriseServices;
using System.Linq;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Helper ;
using Idera.SQLcompliance.Application.GUI.Properties ;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
    /// <summary>
    /// Summary description for Form_TableAdd.
    /// </summary>
    public partial class Form_TableAdd : Form
    {
        private ICollection _tableList;
        private List<string> _selectedTables;
        private bool _supportsSchema;
        private UIConstants.Table_Column_Usage _usage;
        private bool _forBad = false;
        private static int NRPP;
        private static int totalPages;
        private static int totalRec;
        private static int page = 1;
        private static int RecStart;
        private static int RecEnd;
        private int lastUpdatedPagingValue = 0;
        public List<string> _allTableList = new List<string>();
        //Sqlcm 5.6 Start 
        private List<String> duplicates;//Contains duplicate table values
        
        public List<string> Duplicates
        {
            get
            {
                return duplicates;
            }
            set
            {
                duplicates = value;
            }
        }
        //Sqlcm 5.6  End

        public Form_TableAdd(ICollection tableList, List<string> selectedTables, bool supportsSchema, UIConstants.Table_Column_Usage usage)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            _supportsSchema = supportsSchema;
            this.Icon = Resources.SQLcompliance_product_ico;

            _selectedTables = selectedTables;
            _tableList = tableList;
            _usage = usage;
            _lblHeader.Text = UIConstants.AddTableHeader;
            _forBad = ((usage == UIConstants.Table_Column_Usage.BADTables) || (usage == UIConstants.Table_Column_Usage.BADColumns));
            
            switch (_usage)
            {
                case UIConstants.Table_Column_Usage.BADTables:
                    this.Text = string.Format(UIConstants.ADDTableTitle, UIConstants.ADDTableTitle_BAD);
                    _lblDescription.Text = UIConstants.AddTableDescription_BAD;
                    break;
                case UIConstants.Table_Column_Usage.BADColumns:
                    this.Text = string.Format(UIConstants.ADDTableTitle, UIConstants.ADDTableTitle_BAD);
                    _lblDescription.Text = UIConstants.AddTableDescription_BAD_Cols;
                    break;
                case UIConstants.Table_Column_Usage.Filter:
                    this.Text = string.Format(UIConstants.ADDTableTitle, UIConstants.ADDTableTitle_DML);
                    _lblDescription.Text = UIConstants.AddTableDescription;
                    _lblInvalidCharDesc.Visible = false;
                    break;
                case UIConstants.Table_Column_Usage.SensitiveColumns:
                    this.Text = string.Format(UIConstants.ADDTableTitle, UIConstants.ADDTableTilte_SC);
                    _lblDescription.Text = UIConstants.AddTableDescription_SC;
                    _lblInvalidCharDesc.Visible = false;
                    break;
            }
        }

        public List<string> SelectedTables
        {
            get { return _selectedTables; }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadTables();

            if (listAvailable.Items.Count != 0)
            {
                listAvailable.Items[0].Selected = true;
            }
        }

        #region Load Logic

        //-----------------------------------------------------------------------------
        // LoadTables
        //-----------------------------------------------------------------------------
        private void LoadTables()
        {
            listSelected.BeginUpdate();
            listAvailable.BeginUpdate();
            Cursor = Cursors.WaitCursor;

            listSelected.Items.Clear();
            listAvailable.Items.Clear();
            _allTableList = new List<string>();

            if ((_tableList != null) && (_tableList.Count != 0))
            {
                foreach (RawTableObject table in _tableList)
                {
                    // Ignore our tables
                    if (CoreConstants.Agent_BeforeAfter_TableName.Equals(table.TableName))
                        continue;

                    //do not include tables with invalid filename characters in the name.
                    if (table.TableName.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' }) >= 0)
                        continue;

                    DatabaseObjectRecord dbo = new DatabaseObjectRecord(table);
                    string tableName = _supportsSchema ? table.FullTableName : table.TableName;
                    _allTableList.Add(tableName);
                    ListViewItem lvi = new ListViewItem(tableName);

                    if (_forBad && table.HasBlobData)
                    {
                        lvi.Font = new System.Drawing.Font(lvi.Font, System.Drawing.FontStyle.Bold);
                        _lblBadDescription.Visible = true;
                    }

                    if (_forBad && table.IsMemoryOptimized)
                        lvi.ForeColor = Color.Red;

                    if (_selectedTables.Contains(tableName))
                    {
                        listSelected.Items.Add(lvi);
                    }

                    lvi.Tag = dbo;
                }
            }
            else if (_tableList == null)
            {
                ErrorMessage.Show(this.Text, UIConstants.Error_CantLoadTables, SQLDirect.GetLastError());
            }

            LoadPaging(_allTableList);
            listAvailable.EndUpdate();
            listSelected.EndUpdate();
            Cursor = Cursors.Default;
        }

        private void LoadPaging(List<string> tables)
        {
            var tableList = GetAllAvailableTablesForSelecting(tables);

            listAvailable.Items.Clear();
            if ((tableList != null) && (tableList.Count != 0))
            {
                totalRec = tableList.Count;
                NRPP = Convert.ToInt32(numericUpDown1.Value);
                //page = 1;

                totalPages = totalRec / NRPP;

                if (totalRec % NRPP > 0)
                {
                    totalPages++;
                }

                if (page > totalPages) { page = totalPages; }


                int l, k;

                l = (page - 1) * NRPP;
                k = ((page) * NRPP);

                RecStart = l + 1;
                if (k > totalRec)
                {
                    RecEnd = totalRec;
                }
                else
                {
                    RecEnd = k;
                }

                for (; l < k; l++)
                {
                    if (l >= totalRec)
                    {
                        break;
                    }

                    var tableName = tableList[l];
                    listAvailable.Items.Add(tableName.ToString());
                }
                tbPageNo.Text = String.Format("{0}/{1}", page, totalPages);
            }
            else
                tbPageNo.Text = String.Format("0/0");

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
            _selectedTables.Clear();

            foreach (ListViewItem table in listSelected.Items)
            {
                _selectedTables.Add(table.Text);
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
            
            
            listSelected.BeginUpdate();
            listAvailable.BeginUpdate();

            int ndx = listAvailable.SelectedItems[0].Index;
            ListViewItem tmp;
       
            listSelected.SelectedItems.Clear();
            //start sqlcm 5.6 - 5635
            String schema="";
            foreach(RawTableObject rto in _tableList)
            {
                schema = rto.SchemaName;
                break;
            }
           
            
            bool showDuplicateTablePopup = false;
            List<string> duplicateTableList = new List<string>();

            for(int i=0;i<listAvailable.SelectedItems.Count;i++)
            {
                for(int j=i+1;j<listAvailable.SelectedItems.Count;j++)
                {
                    String x = listAvailable.SelectedItems[i].Text;
                    String y = listAvailable.SelectedItems[j].Text;
                    x = x.Substring(x.LastIndexOf(schema + ".") + 1);
                    y = y.Substring(y.LastIndexOf(schema + ".") + 1);
                    if(x.Equals(y))
                    {
                        if(!duplicateTableList.Contains(listAvailable.SelectedItems[i].Text))
                        duplicateTableList.Add(listAvailable.SelectedItems[i].Text);
                        if (!duplicateTableList.Contains(listAvailable.SelectedItems[j].Text))
                            duplicateTableList.Add(listAvailable.SelectedItems[j].Text);
                        showDuplicateTablePopup = true;
                    }
                }
            }
            if(!showDuplicateTablePopup)
            {
                for(int i=0;i<listAvailable.SelectedItems.Count;i++)
                {
                    for(int j=0;j<listSelected.Items.Count;j++)
                    {
                        String x = listAvailable.SelectedItems[i].Text;
                        String y = listSelected.Items[j].Text;
                        x = x.Substring(x.LastIndexOf(schema + ".") + 1);
                        y = y.Substring(y.LastIndexOf(schema + ".") + 1);
                        if(x.Equals(y))
                        {
                            if (!duplicateTableList.Contains(listAvailable.SelectedItems[i].Text))
                                duplicateTableList.Add(listAvailable.SelectedItems[i].Text);
                            if (!duplicateTableList.Contains(listSelected.Items[j].Text))
                                duplicateTableList.Add(listSelected.Items[j].Text);
                            showDuplicateTablePopup = true;
                        }
                    }
                }
            }
          

            if (showDuplicateTablePopup)
            {
                String list = "";
                foreach (String item in duplicateTableList)
                {
                    list = list + item + Environment.NewLine;
                }
                if(Text == string.Format(UIConstants.ADDTableTitle, UIConstants.ADDTableTitle_BAD))
                MessageBox.Show("In the selected database, there are two or more tables with the same name. This may cause confusion with your Before - After Data auditing. Please select only one of these tables to proceed:\n" + list);
                else
                    MessageBox.Show("In the selected database, there are two or more tables with the same name. This may cause confusion with your Sensitive Columns auditing. Please select only one of these tables to proceed:\n" + list);
            }
            else
            {
                //end sqlcm 5.6 - 5635
                foreach (ListViewItem itm in listAvailable.SelectedItems)
                {



                    if (itm.ForeColor == Color.Red)
                    {
                        MessageBox.Show(UIConstants.Error_InMemoryTablesNotSupported, Text);
                        continue;
                    }

                    itm.Remove();
                    tmp = listSelected.Items.Add(itm);
                    tmp.Selected = true;

                }

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
                btnAdd.Enabled = true;
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
            switch (_usage)
            {
                case UIConstants.Table_Column_Usage.BADColumns:
                case UIConstants.Table_Column_Usage.BADTables:
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_TableAdd_BAD);
                    break;
                case UIConstants.Table_Column_Usage.Filter:
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_TableAdd_DML);
                    break;
                case UIConstants.Table_Column_Usage.SensitiveColumns:
                    HelpAlias.ShowHelp(this, HelpAlias.SSHELP_Form_TableAdd_SC);
                    break;
            }
            hlpevent.Handled = true;
        }
        #endregion

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdatePaging();
        }

        private void numericUpDown1_ValueChanged(object sender, MouseEventArgs e)
        {
            if (!IsPagingValueChanged()) return;

            UpdatePaging();
            SaveLastUpdatedPagingValue();
        }

        private int GetCurrentPagingValue()
        {
            return Convert.ToInt32(numericUpDown1.Value);
        }

        private bool IsPagingValueChanged()
        {
            var currentPagingValue = GetCurrentPagingValue();
            return lastUpdatedPagingValue != currentPagingValue;
        }

        private void SaveLastUpdatedPagingValue()
        {
            lastUpdatedPagingValue = GetCurrentPagingValue();
        }

        private void UpdatePaging()
        {
            if (numericUpDown1.Value != 0)
                NRPP = Convert.ToInt32(numericUpDown1.Value);
            else
                numericUpDown1.Value = 1;

            if (listAvailable.Items.Count > numericUpDown1.Value)
                totalPages = totalRec / NRPP;
            else
                totalPages = 1;

            LoadPaging(_allTableList);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            page = 1;
            LoadPaging(_allTableList);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (page > 1)
                page--;
            LoadPaging(_allTableList);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (page < totalPages)
                page++;

            LoadPaging(_allTableList);
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            page = totalPages;
            LoadPaging(_allTableList);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Use the Select method to find all rows matching the filter.
            List<string> foundTables = _allTableList.Where(item => item.IndexOf(tbSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(item => item).ToList();
            page = 1;
            LoadPaging(foundTables);
            listAvailable_SelectedIndexChanged(null, null);
        }

        private bool WasTabledAlreadySelected(string fullTableName)
        {
            foreach (ListViewItem item in listSelected.Items)
            {
                string selectedTableName = item.Text;
                if (selectedTableName == fullTableName)
                {
                    return true;
                }
            }

            return false;
        }
        
        private List<string> GetAllAvailableTablesForSelecting(List<string> availableTables)
        {
            var avaiableTable = new List<string>();

            foreach (string tableName in availableTables)
            {
                if (!WasTabledAlreadySelected(tableName))
                {
                    avaiableTable.Add(tableName);
                }
            }

            return avaiableTable;
        }
       
    }
}
