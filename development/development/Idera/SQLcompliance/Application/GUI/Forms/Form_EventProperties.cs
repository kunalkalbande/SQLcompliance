using System ;
using System.Collections.Generic;
using System.Globalization ;
using System.Text ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Controls ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event ;
using Idera.SQLcompliance.Core.TimeZoneHelper ;
using Infragistics.Win.UltraWinDataSource;
using Resources=Idera.SQLcompliance.Application.GUI.Properties.Resources;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
   /// <summary>
   /// Summary description for Form_EventProperties.
   /// </summary>
   public partial class Form_EventProperties : Form
   {
      private BaseControl _parentView;
      private string _eventDatabase;
      private int _eventId;
      private Core.TimeZoneHelper.TimeZoneInfo _tzi ;
      private ServerRecord _server;

      
      public Form_EventProperties(BaseControl inParentView, ServerRecord server,int inEventId, Core.TimeZoneHelper.TimeZoneInfo tzi)
         : this(inParentView, server.EventDatabase, server, inEventId, tzi)
      {
      }

      public Form_EventProperties(BaseControl inParentView, string eventDatabase, ServerRecord server, int inEventId, Core.TimeZoneHelper.TimeZoneInfo tzi)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
         GridHelper.ApplyAdminSettings(_gridDetails);
         GridHelper.ApplyAdminSettings(_gridBeforeAfter);
         GridHelper.ApplyAdminSettings(_gridSensitiveColumns);
         this.Icon = Resources.SQLcompliance_product_ico;

         _parentView = inParentView;
         _eventDatabase = eventDatabase;
         _server = server;
         _eventId = inEventId;
         _tzi = tzi;

         LoadEvent();
      }

      public Form_EventProperties(EventRecord theEvent, string inEventDatabase, Core.TimeZoneHelper.TimeZoneInfo tzi)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();
         _eventDatabase = inEventDatabase;
         _tzi = tzi;
         btnNext.Visible = false ;
         btnNext.Enabled = false ;
         btnPrevious.Visible = false ;
         btnPrevious.Enabled = false ;

         DisplayEvent(theEvent) ;
      }


      private void btnClose_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      private void btnPrevious_Click(object sender, EventArgs e)
      {
         int newId ;
         
         if(_parentView is AlertView)
         {
            newId = ((AlertView)_parentView).Previous(out _eventDatabase);
         }
         else if (_parentView is DataAlertView)
         {
            newId = ((DataAlertView)_parentView).Previous(out _eventDatabase);
         }
         else
            newId = _parentView.Previous();
         if(_eventId != newId)
         {
            _eventId = newId;
            LoadEvent();
         }
      }

      private void btnNext_Click(object sender, EventArgs e)
      {
         int newId ;
         
         if(_parentView is AlertView)
         {
            newId = ((AlertView)_parentView).Next(out _eventDatabase);
         }
         else if (_parentView is DataAlertView)
         {
            newId = ((DataAlertView)_parentView).Next(out _eventDatabase);
         }
         else
            newId = _parentView.Next();

         if(_eventId != newId)
         {
            _eventId = newId;
            LoadEvent();
         }
      }
      
      private void DisplayEvent(EventRecord ev)
      {
         if (Settings.Default.ShowLocalTime)
            txtTime.Text = ev.startTime.ToLocalTime().ToString();
         else
            txtTime.Text = Core.TimeZoneHelper.TimeZoneInfo.ToLocalTime(_tzi, ev.startTime).ToString();

         txtCategory.Text    = ev.eventCategoryString;
         txtType.Text        = ev.eventTypeString;
         txtApplication.Text = ev.applicationName;
               
         txtUser.Text        = ev.ResponsibleLogin ;
         txtDatabase.Text    = ev.databaseName;
         txtObject.Text      = ev.targetObject;
         txtDetails.Text     = ev.details;
         txtRowCounts.Text   = ev.rowCounts == null ? "Not Applicable" : ev.rowCounts.ToString();  

                    
         // Display Properties
         LoadList(ev);

         // Before-After Properties
         LoadBeforeAfter(ev);

         //Sensitive Column Info
         LoadSensitiveColumns(ev);
               
         // see if there is any SQL
         EventSqlRecord evs = new EventSqlRecord( Globals.Repository.Connection,
            _eventDatabase );
         if ( evs.Read( ev.eventId ) )
         {
            txtSQL.Text    = evs.sqlText;
            txtSQL.Enabled = true;
         }
         else
         {
            txtSQL.Text    = "";
            txtSQL.Enabled = false;
         }
      }

      private void LoadSensitiveColumns(EventRecord ev)
      {
         _gridSensitiveColumns.BeginUpdate();
         _dsSensitiveColumns.Rows.Clear();

         try
         {
            List<SensitiveColumnRecord> columns = SensitiveColumnRecord.GetSensitiveColumnRecords(Globals.Repository.Connection, _eventDatabase, ev);

            if (columns.Count <= 0)
            {
               if (_tabControl.Controls.Contains(_tabSensitiveColumns))
                  _tabControl.Controls.Remove(_tabSensitiveColumns);
               return;
            }
            if (!_tabControl.Controls.Contains(_tabSensitiveColumns))
               _tabControl.Controls.Add(_tabSensitiveColumns);

            foreach (SensitiveColumnRecord column in columns)
            {
               UltraDataRow row = _dsSensitiveColumns.Rows.Add();
               row[0] = column.columnName;
               row[1] = ev.rowCounts == null? "Not Applicable": ev.rowCounts.ToString();               
            }                    

         }
         catch (Exception e)
         {
            ErrorLog.Instance.Write("Error loading Before-After data", e);
         }
         finally
         {
            _gridSensitiveColumns.EndUpdate();
         }
      }

      private void LoadBeforeAfter(EventRecord ev)
      {
         // SQL Server 2005,2008
         if (_server.SqlVersion < 9)
         {
            _lblBeforeAfterNotAvailable.Text = CoreConstants.Feature_BeforeAfterNotAvailable;
            _lblBeforeAfterNotAvailable.Visible = true;
            _gridBeforeAfter.Visible = false;
            return;
         }

         _gridBeforeAfter.BeginUpdate();
         _dsBeforeAfter.Rows.Clear();
         try
         {
            if (ev == null) return;

            if (ev.eventCategory != TraceEventCategory.DML)
            {
               txtColumnsAffected.Text = "Not Applicable";
               txtRowsAffected.Text = "Not Applicable";
               if(_tabControl.Controls.Contains(_tabBeforeAfter))
                  _tabControl.Controls.Remove(_tabBeforeAfter);
               return;
            }

            if (ev.startSequence == -1 && ev.endSequence == -1)
            {
               txtColumnsAffected.Text = "Not Available";
               txtRowsAffected.Text = "Not Available";
               if (_tabControl.Controls.Contains(_tabBeforeAfter))
                  _tabControl.Controls.Remove(_tabBeforeAfter);
               return;
            }

            List<DataChangeRecord> dcRecords =
               DataChangeRecord.GetDataChangeRecords(Globals.Repository.Connection, _eventDatabase, ev);
            txtRowsAffected.Text = dcRecords.Count.ToString();
            if (dcRecords.Count == 0)
            {
               txtColumnsAffected.Text = "None";
               if (_tabControl.Controls.Contains(_tabBeforeAfter))
                  _tabControl.Controls.Remove(_tabBeforeAfter);
               return;
            }
            if (!_tabControl.Controls.Contains(_tabBeforeAfter))
               _tabControl.Controls.Add(_tabBeforeAfter);
            int i = 1;
            List<string> columnNames = new List<string>();
            foreach (DataChangeRecord rec in dcRecords)
            {
               List<ColumnChangeRecord> ccRecords = ColumnChangeRecord.GetColumnChangeRecords(Globals.Repository.Connection, _eventDatabase, rec);
               foreach (ColumnChangeRecord ccRec in ccRecords)
               {
                  // Get Columns and Get Records for grids
                  UltraDataRow row = _dsBeforeAfter.Rows.Add();

                  row[0] = i.ToString();
                  row[1] = rec.primaryKey;
                  row[2] = ccRec.columnName;
                  row[3] = ccRec.beforeValue;
                  row[4] = ccRec.afterValue;
                  if (!columnNames.Contains(ccRec.columnName))
                  {
                     columnNames.Add(ccRec.columnName);
                  }
               }
               txtColumnsAffected.Text = String.Join(",", columnNames.ToArray());
               i++;
            }
            if (_dsBeforeAfter.Rows.Count == 0)
            {
                if (_tabControl.Controls.Contains(_tabBeforeAfter))
                    _tabControl.Controls.Remove(_tabBeforeAfter);
            }
         }
         catch (Exception e)
         {
            txtColumnsAffected.Text = "Error loading Before-After data";
            txtRowsAffected.Text = "Error loading Before-After data";
            ErrorLog.Instance.Write("Error loading Before-After data", e) ;
         }
         finally
         {
            _gridBeforeAfter.EndUpdate();
         }
      }

      private void LoadEvent()
      {
         Cursor = Cursors.WaitCursor;
      
         if ( _eventId == -1 )
         {
            //foreach ( Control c in grpError.Controls )
            //{
            //   if (  c.Tag!= null && (string)c.Tag == "1" ) c.Visible = false;
            //}
            
            lblError.Text    = UIConstants.Prop_NotDataRow;
            _tabControl.Visible = false;
            pnlError.Visible = true;
            lblError.Visible = true;
            txtSQL.Text = "";
         }
         else
         {
            EventRecord ev = new EventRecord( Globals.Repository.Connection,
               _eventDatabase );
            if ( ev.Read( _eventId ) )
            {
               //foreach ( Control c in grpError.Controls )
               //{
               //   if ( c.Tag!= null && (string)c.Tag == "1" ) c.Visible = true;
               //}
               _tabControl.Visible = true;
               pnlError.Visible = false;
               lblError.Visible = false;
               DisplayEvent(ev);
            }
            else
            {
               //foreach ( Control c in grpError.Controls )
               //{
               //   if (  c.Tag!= null && (string)c.Tag == "1" ) c.Visible = false;
               //}
               _tabControl.Visible = false;
               pnlError.Visible = true;
               lblError.Visible = true;
               lblError.Text = String.Format( UIConstants.Prop_ErrorLoading,
                  EventRecord.GetLastError() );
               txtSQL.Text = "" ;
            }
         }
         
         Cursor = Cursors.Default;
      }
      
      private void LoadList(EventRecord ev)
      {
         if ( ev == null ) return;

         _gridDetails.BeginUpdate();
         _dsDetails.Rows.Clear();

         ListAdd( "Event ID", ev.eventId );
         ListAdd( "Event time",
            ev.startTime.ToLocalTime().ToString( "yyyy-MM-dd HH:mm:ss.fff",
            DateTimeFormatInfo.InvariantInfo ) );
         ListAdd( "Event type ID", (int)ev.eventType );
         ListAdd( "Event type", ev.eventTypeString );
         ListAdd( "Event category",   ev.eventCategoryString );
         ListAdd( "Event category ID", (int)ev.eventCategory );
         ListAdd( "Application name", ev.applicationName );
         ListAdd( "Target object", ev.targetObject );
         ListAdd( "Details", ev.details );
         ListAdd( "Event class", ev.eventClass );
         ListAdd( "Event subclass", ev.eventSubclass );
         ListAdd( "SPID", ev.spid );
         ListAdd( "Host name", ev.hostName );
         ListAdd( "Login name", ev.loginName );
         ListAdd( "Database name", ev.databaseName );
         ListAdd( "Database ID", ev.databaseId );
         ListAdd( "Database user name", ev.dbUserName );
         ListAdd( "Object type", ev.objectType );
         ListAdd( "Object name", ev.objectName );
         ListAdd( "Object ID", ev.objectId );
         ListAdd( "Permissions", ev.permissions );
         ListAdd( "Privileged User Event", ev.privilegedUser==1 ? "True" : "False");
         ListAdd( "Column permissions", ev.columnPermissions );
         ListAdd( "Target login name", ev.targetLoginName );
         ListAdd( "Target user name", ev.targetUserName );
         ListAdd( "Server name", ev.serverName );
         ListAdd( "Role name", ev.roleName );
         ListAdd( "Owner name", ev.ownerName );
         ListAdd( "Checksum", ev.checksum );
         ListAdd( "Hash", ev.hash );

         // new columns added for 2005
         ListAdd( "File name", ev.fileName );
         ListAdd( "Linked server name", ev.linkedServerName );
         ListAdd( "Parent name", ev.providerName );
         ListAdd( "System event", ev.isSystem==1 ? "True" : "False");
         ListAdd( "Session login name", ev.sessionLoginName );
         ListAdd( "Provider name", ev.providerName );
         
         ListAdd( "Access check", (ev.success == 1) ? "Passed" : "Failed" );
          ListAdd( "Row Counts", ev.rowCounts );

         _gridDetails.EndUpdate();
      }
      
      private void ListAdd(string propName, object value)
      {
         UltraDataRow row = _dsDetails.Rows.Add();

         row[0] = propName;
         row[1] = value;
      }
      

      #region Help
      //--------------------------------------------------------------------
      // Form_EventProperties_HelpRequested - Show Context Sensitive Help
      //--------------------------------------------------------------------
      private void Form_EventProperties_HelpRequested(object sender, HelpEventArgs hlpevent)
      {
         string helpTopic ;

         switch (_tabControl.SelectedIndex)
         {
            case 0:
               helpTopic = HelpAlias.SSHELP_Form_EventProperties_General;
               break;
            case 1:
               helpTopic = HelpAlias.SSHELP_Form_EventProperties_Details;
               break;
            case 2:
               helpTopic = HelpAlias.SSHELP_Form_EventProperties_Data;
               break;
            default:
               return;
         }

         HelpAlias.ShowHelp(this,helpTopic);
         hlpevent.Handled = true;
      }
      #endregion

      private void btnCopy_Click(object sender, EventArgs e)
      {
         StringBuilder s = new StringBuilder("",1024);
         
         s.Append ("Event time:\t"     + txtTime.Text     + "\r\n");
         s.Append ("Event category:\t" + txtCategory.Text + "\r\n");
         s.Append ("Event type:\t"     + txtType.Text     + "\r\n");
         s.Append ("Application:\t"    + txtApplication.Text     + "\r\n");
         s.Append ("User:\t\t"         + txtUser.Text     + "\r\n");
         s.Append ("Database:\t\t"     + txtDatabase.Text + "\r\n");
         s.Append ("Target:\t\t"       + txtObject.Text   + "\r\n");
         s.Append ("\r\nDetails:\r\n");

         foreach (UltraDataRow row in _dsDetails.Rows)
         {
            s.Append(row[0]);
            s.Append(":\t");
            s.Append(row[1]);
            s.Append("\r\n");
         }
         
         s.Append ("\r\nSQL Statement:\r\n");
         s.Append (txtSQL.Text);

         if(_tabControl.Controls.Contains(_tabBeforeAfter))
         {
            s.Append("\r\n\r\nBefore-After Values\r\n");
            foreach(UltraDataRow row in _dsBeforeAfter.Rows)
            {
               s.Append("Row #:\t" + row[0] + "\r\n");
               s.Append("Primary Key:\t" + row[1] + "\r\n");
               s.Append("Column:\t" + row[2] + "\r\n");
               s.Append("Before Value:\t" + row[3] + "\r\n");
               s.Append("After Value:\t" + row[4] + "\r\n\r\n");
            }
         }

         Clipboard.SetDataObject( s.ToString() );
      }
   }
}
