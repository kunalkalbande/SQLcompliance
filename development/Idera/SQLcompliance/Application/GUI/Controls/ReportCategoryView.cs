using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core.Reports;
using Infragistics.Win;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class ReportCategoryView : BaseControl
   {
      #region Data Members
      
      private List<FeatureButton> _items = new List<FeatureButton>();
      private TreeView _tree = null;
      private List<CMReport> _reports;
      private ReportsRecord _reportRecord;
      
      #endregion
      
      #region Properties
      
      public TreeView ReportsTree
      {
         get
         {
            return _tree;
         }
         set
         {
            _tree = value;
         }
      }


      public List<CMReport> Reports
      {
         get { return _reports; }
         set 
         { 
            _reports = value;
            if (_reports != null)
               UpdateViewDeployedReportsButton();
         }
      }

      #endregion
      
      public ReportCategoryView()
      {
         InitializeComponent();

         SetMenuFlag(CMMenuItem.ShowHelp);
      }

      public override void UpdateColors()
      {
         base.UpdateColors();
         _headerReportingServices.BackColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientLight;
         _headerReportingServices.BackColor2 = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientDark;
         _headerReportingServices.BorderColor = Office2007ColorTable.Colors.OutlookNavPaneBorder;
         _lblReportingServices.ForeColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderForecolor;

         BackColor = Office2007ColorTable.Colors.DockAreaGradientLight;
      }

      public override void RefreshView()
      {
         DisplayReports();
         base.RefreshView();
         _tblReports.PerformLayout() ;
      }

      #region Reports View

      private void DisplayReports()
      {
         int idx = 0;
         if (_items.Count == 0)
         {
            InitReportItemList();
         }
         _tblReports.Controls.Clear();
         if (this.ReportsTree != null && this.ReportsTree.SelectedNode.Text == "Alerts /  History")
         {
         	foreach (CMReport info in _reports)
         	{
         		if (info.RootHead == "Alerts /  History")
            	{
            		AddReportTask( _items[idx], info, idx );
            		idx++;
                }
             }
          }
          else
          {
          	foreach (CMReport info in _reports)
            {
            	if (info.RootHead == "Configuration")
                {
                	AddReportTask(_items[idx], info, idx);
                    idx++;
                }
            }
         }
      }

      private void AddReportTask(FeatureButton btn, CMReport info, int index)
      {
         _tblReports.Controls.Add(btn);
         btn.HeaderText = info.Name;
         btn.DescriptionText = info.Description;
         btn.Tag = info;
      }

      #endregion
      
      private void InitReportItemList()
      {
         for(int i = 0 ; i < _reports.Count ; i++)
         {
            FeatureButton btn = new FeatureButton() ;
            btn.Size = new Size(325, 65);
            //btn.Size = new Size(285,65);
            btn.Image = GUI.Properties.Resources.Reports_48 ;
            InitReportTask(btn) ;
         }
      }
      
      private void InitReportTask( FeatureButton btn )
      {
         //SetColors( task );
         btn.MouseClick += UpdateSelectedTreeNode ;
         _items.Add( btn );
      }

      private void UpdateSelectedTreeNode(object sender, EventArgs e)
      {
         if ( _tree == null ||
              _tree.Nodes.Count == 0 )
            return;
         

         if ( sender is FeatureButton )
         {
            FeatureButton task = (FeatureButton)sender;
            if (task.Tag is CMReport) // Show report view
            {
               CMReport info = (CMReport)task.Tag;
               foreach (TreeNode node in _tree.Nodes)
               {
                    foreach (TreeNode innerNode in node.Nodes)
                    {
                        if (innerNode.Tag == info)
                  		{
                        	_tree.SelectedNode = innerNode;
                     		return;
                        }
                  	}
               }
            }
         }
         return;
      }

      private void ReportCategoryView_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpOnThisWindow();
         hlpevent.Handled = true;
      }

      public override void HelpOnThisWindow()
      {
         HelpAlias.ShowHelp(this, HelpAlias.SSHELP_ReportsView);
      }

      private void MouseClick_btnDeployReports(object sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
         {
            using (Form_DeployReportsWizard frm = new Form_DeployReportsWizard())
            {
               frm.ShowDialog(this);
               UpdateViewDeployedReportsButton();
            }
         }
      }

      private void MouseClick_btnViewDeployedReports(object sender, MouseEventArgs e)
      {
         if (!_btnViewDeployedReports.Enabled)
            return;
         if (e.Button == MouseButtons.Left)
         {
            HelpAlias.LaunchWebBrowser(_reportRecord.GetReportManagerUrl(true));
         }
      }

      private void UpdateViewDeployedReportsButton()
      {
         _reportRecord = ReportsRecord.Read(Globals.Repository.Connection);
         _btnViewDeployedReports.Enabled = _reportRecord.ReportsDeployed;
      }
   }
}
