using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Idera.SQLcompliance.Cluster
{
   public partial class Form_SetAssemblyDirectory : Form
   {
      //Virtual Server name key with instance name value
      List<VirtualServer> virtualServers = new List<VirtualServer>();
     
      public List<VirtualServer> VirtualServers
      {
         get { return virtualServers; }
         set { virtualServers = value; }
      }

      public Form_SetAssemblyDirectory()
      {
         InitializeComponent();
      }

      public void AddServer(VirtualServer virtualServer)
      {
         virtualServers.Add(virtualServer);
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         if (virtualServers != null)
         {
            if (virtualServers.Count > 0)
               assemblyPathGrid.Rows.Add(virtualServers.Count);
         }
      }

      //make sure all the directory paths have been set.
      private void acceptButton_Click(object sender, EventArgs e)
      {
         foreach (VirtualServer virtualServer in virtualServers)
         {
            if (String.IsNullOrEmpty(virtualServer.TriggerAssemblyDirectory))
            {
               MessageBox.Show("The CLR trigger assembly directory must be set for all instances.", "Set Diretory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               return;
            }
            else
            {
               if (!ValidatePath(virtualServer.TriggerAssemblyDirectory))
               {
                  MessageBox.Show(String.Format("{0} is not a valid path.", virtualServer.TriggerAssemblyDirectory), "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
               }
            }
         }
         this.DialogResult = DialogResult.OK;
      }

      private bool ValidatePath(string filepath)
      {
         // make sure defined and a local path
         if (filepath.Length < 3) return false;
         if (filepath.Length > 180) return false;
         if (filepath[1] != ':') return false;
         if (filepath[2] != '\\') return false;
         if (filepath.IndexOf("..") != -1) return false;

         try
         {
            if (!Path.IsPathRooted(filepath))
               return false;

            //This will check for aall invalid filename characters.
            Path.GetFullPath(filepath);
         }
         catch (Exception)
         {
            return false;
         }
         return true;
      }

      private void cancelButton_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      private void assemblyPathGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
      {
         if (virtualServers == null || e.RowIndex > virtualServers.Count - 1)
            return;

         switch (e.ColumnIndex)
         {
            case 0: //Instance Name
               {
                  e.Value = ((VirtualServer)virtualServers[e.RowIndex]).FullInstanceName;
                  break;
               }
            case 1: //Path
               {
                  string path = ((VirtualServer)virtualServers[e.RowIndex]).TriggerAssemblyDirectory;

                  if (String.IsNullOrEmpty(path))
                     e.Value = "";
                  else
                     e.Value = path;
                  break;
               }
            default:
               {
                  e.Value = "?";
                  break;
               }
         }
      }

      private void assemblyPathGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
      {
         if (!assemblyPathGrid.Enabled && e.RowIndex > -1)
         {
            e.Graphics.FillRectangle(SystemBrushes.Control, e.CellBounds);
            e.Handled = true;
         }
      }

      private void assemblyPathGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         //only allow the path to be edited
         if (e.RowIndex < 0 || e.ColumnIndex != 1)
            return;

         ((VirtualServer)virtualServers[e.RowIndex]).TriggerAssemblyDirectory = (string)assemblyPathGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
         assemblyPathGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = assemblyPathGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
      }

      private void Form_SetAssemblyDirectory_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         HelpAlias.ShowHelp(this, HelpAlias.CLUSTERHELP_Form_Main) ;
         hlpevent.Handled = true ;
		}
   }
}