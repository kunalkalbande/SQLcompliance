using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Data.SqlClient;
using Xceed.Compression;
using Xceed.Compression.Formats;

namespace Idera.SQLcompliance.Utility.TraceRegister
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
   public class Form1 : System.Windows.Forms.Form
   {
      //------------------------
      // Constants / Properties
      //------------------------
      public const string   CollectionService_RegKey          = @"Software\Idera\SQLcompliance\CollectionService";
      public const string   CollectionService_RegKey_CM       = @"Software\Idera\SQLCM\CollectionService";
      public const string   CollectionService_RegVal_TraceDir = "TraceDirectory";

      public string         m_collectionServer_TraceDirectory   = "";
      public SqlConnection  m_connection                        = null;
      public int            m_goodFiles                         = 0;
      public int            m_badFiles                          = 0;
      public int            m_reprocessedFiles                  = 0;
      public bool           m_processing                        = false;

      //--------------------------------------------------------
      // DevStudio Generated Properties
      //--------------------------------------------------------
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.MainMenu mainMenu1;
      private System.Windows.Forms.MenuItem menuItem1;
      private System.Windows.Forms.MenuItem menuItem3;
      private System.Windows.Forms.MenuItem menuExit;
      private System.Windows.Forms.MenuItem menuAbout;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.GroupBox grpResults;
      private System.Windows.Forms.Button btnRegister;
      private System.Windows.Forms.ListBox listResults;
      private System.Windows.Forms.TextBox textTraceDirectory;
      private System.Windows.Forms.TextBox textServer;
      private System.Windows.Forms.TextBox textCollectionServerTraceDirectory;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Button btnClose;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;

      //--------------------------------------------------------
      // Constructor
      //--------------------------------------------------------
      public Form1()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         //
         // TODO: Add any constructor code after InitializeComponent call
         //
         //Xceed.Compression.Licenser.LicenseKey         = "SCN10DKKPZTUTWAU4AA";
         Xceed.Compression.Formats.Licenser.LicenseKey = "SCN10DKKPZTUTWAU4AA";
      }

      //--------------------------------------------------------
      // OnLoad
      //--------------------------------------------------------
      protected override void OnLoad(EventArgs e)
      {
         m_collectionServer_TraceDirectory = GetCollectionServerTraceDirectory();
         if ( m_collectionServer_TraceDirectory== "" )
         {
            ShowError( "TraceRegister could not read the Collection Server trace file location in the registry. Check that the Collection Server is installed on the local machine and that the user has appropriate permissions to read the Collection Server registry. " );
            Close();
         }
         textCollectionServerTraceDirectory.Text = m_collectionServer_TraceDirectory;
      }

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if (components != null) 
            {
               components.Dispose();
            }
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code
      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.textCollectionServerTraceDirectory = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.btnBrowse = new System.Windows.Forms.Button();
         this.textTraceDirectory = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.textServer = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.mainMenu1 = new System.Windows.Forms.MainMenu();
         this.menuItem1 = new System.Windows.Forms.MenuItem();
         this.menuExit = new System.Windows.Forms.MenuItem();
         this.menuItem3 = new System.Windows.Forms.MenuItem();
         this.menuAbout = new System.Windows.Forms.MenuItem();
         this.grpResults = new System.Windows.Forms.GroupBox();
         this.listResults = new System.Windows.Forms.ListBox();
         this.btnRegister = new System.Windows.Forms.Button();
         this.btnClose = new System.Windows.Forms.Button();
         this.groupBox1.SuspendLayout();
         this.grpResults.SuspendLayout();
         this.SuspendLayout();
         // 
         // groupBox1
         // 
         this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.groupBox1.Controls.Add(this.textCollectionServerTraceDirectory);
         this.groupBox1.Controls.Add(this.label3);
         this.groupBox1.Controls.Add(this.btnBrowse);
         this.groupBox1.Controls.Add(this.textTraceDirectory);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Controls.Add(this.textServer);
         this.groupBox1.Controls.Add(this.label1);
         this.groupBox1.Location = new System.Drawing.Point(8, 8);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(608, 96);
         this.groupBox1.TabIndex = 0;
         this.groupBox1.TabStop = false;
         // 
         // textCollectionServerTraceDirectory
         // 
         this.textCollectionServerTraceDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.textCollectionServerTraceDirectory.Location = new System.Drawing.Point(168, 68);
         this.textCollectionServerTraceDirectory.Multiline = true;
         this.textCollectionServerTraceDirectory.Name = "textCollectionServerTraceDirectory";
         this.textCollectionServerTraceDirectory.ReadOnly = true;
         this.textCollectionServerTraceDirectory.Size = new System.Drawing.Size(404, 20);
         this.textCollectionServerTraceDirectory.TabIndex = 7;
         this.textCollectionServerTraceDirectory.Text = "";
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(8, 72);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(180, 16);
         this.label3.TabIndex = 6;
         this.label3.Text = "Collection Server Directory:";
         // 
         // btnBrowse
         // 
         this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnBrowse.Location = new System.Drawing.Point(576, 40);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(24, 20);
         this.btnBrowse.TabIndex = 5;
         this.btnBrowse.Text = "...";
         this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
         // 
         // textTraceDirectory
         // 
         this.textTraceDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.textTraceDirectory.Location = new System.Drawing.Point(168, 40);
         this.textTraceDirectory.Multiline = true;
         this.textTraceDirectory.Name = "textTraceDirectory";
         this.textTraceDirectory.Size = new System.Drawing.Size(404, 20);
         this.textTraceDirectory.TabIndex = 3;
         this.textTraceDirectory.Text = "";
         this.textTraceDirectory.TextChanged += new System.EventHandler(this.textTraceDirectory_TextChanged);
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 44);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(152, 16);
         this.label2.TabIndex = 2;
         this.label2.Text = "Trace File Directory (source):";
         // 
         // textServer
         // 
         this.textServer.Location = new System.Drawing.Point(168, 12);
         this.textServer.Name = "textServer";
         this.textServer.Size = new System.Drawing.Size(220, 20);
         this.textServer.TabIndex = 1;
         this.textServer.Text = "";
         this.textServer.TextChanged += new System.EventHandler(this.textServer_TextChanged);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 16);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(128, 16);
         this.label1.TabIndex = 0;
         this.label1.Text = "Repository SQL Server:";
         // 
         // mainMenu1
         // 
         this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.menuItem1,
                                                                                  this.menuItem3});
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.menuExit});
         this.menuItem1.Text = "&File";
         // 
         // menuExit
         // 
         this.menuExit.Index = 0;
         this.menuExit.Text = "E&xit";
         this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
         // 
         // menuItem3
         // 
         this.menuItem3.Index = 1;
         this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                  this.menuAbout});
         this.menuItem3.Text = "&Help";
         // 
         // menuAbout
         // 
         this.menuAbout.Index = 0;
         this.menuAbout.Text = "&About TraceRegister...";
         this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
         // 
         // grpResults
         // 
         this.grpResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.grpResults.Controls.Add(this.listResults);
         this.grpResults.Location = new System.Drawing.Point(12, 156);
         this.grpResults.Name = "grpResults";
         this.grpResults.Size = new System.Drawing.Size(612, 335);
         this.grpResults.TabIndex = 1;
         this.grpResults.TabStop = false;
         this.grpResults.Text = "Results";
         // 
         // listResults
         // 
         this.listResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.listResults.HorizontalScrollbar = true;
         this.listResults.Location = new System.Drawing.Point(8, 16);
         this.listResults.Name = "listResults";
         this.listResults.Size = new System.Drawing.Size(592, 303);
         this.listResults.TabIndex = 0;
         // 
         // btnRegister
         // 
         this.btnRegister.Anchor = System.Windows.Forms.AnchorStyles.Top;
         this.btnRegister.Enabled = false;
         this.btnRegister.Location = new System.Drawing.Point(246, 116);
         this.btnRegister.Name = "btnRegister";
         this.btnRegister.Size = new System.Drawing.Size(140, 32);
         this.btnRegister.TabIndex = 7;
         this.btnRegister.Text = "Register Trace Files";
         this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(536, 500);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(84, 24);
         this.btnClose.TabIndex = 8;
         this.btnClose.Text = "&Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // Form1
         // 
         this.AcceptButton = this.btnRegister;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(632, 533);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.btnRegister);
         this.Controls.Add(this.grpResults);
         this.Controls.Add(this.groupBox1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mainMenu1;
         this.MinimumSize = new System.Drawing.Size(400, 400);
         this.Name = "Form1";
         this.Text = "SQL Compliance Manager Trace Register Utility";
         this.groupBox1.ResumeLayout(false);
         this.grpResults.ResumeLayout(false);
         this.ResumeLayout(false);
         this.textServer.Text = this.getDefaultSQLServerInstance();

      }
      #endregion

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main() 
      {
         Application.Run(new Form1());
      }

      //--------------------------------------------------------
      // menuExit_Click
      //--------------------------------------------------------
      private void menuExit_Click(object sender, System.EventArgs e)
      {
         if ( ! m_processing ) Close();
      }

      //--------------------------------------------------------
      // menuAbout_Click
      //--------------------------------------------------------
      private void menuAbout_Click(object sender, System.EventArgs e)
      {
         Form_About frm = new Form_About();
         frm.ShowDialog();
		
      }

      //--------------------------------------------------------
      // btnRegisterClick
      //--------------------------------------------------------
      private void btnRegister_Click(object sender, System.EventArgs e)
      {
         listResults.Items.Clear();

         Cursor = Cursors.WaitCursor;
         btnRegister.Enabled = false;
         
         m_processing = true;
         

         try
         {
            //------------
            // Validation
            //------------

            // check for valid trace directory
            if ( ! Directory.Exists( textTraceDirectory.Text ) )
            {
               ShowError( "Error: The specified trace directory does not exist." );
               return;
            }
            
            // Make sure they are not importing files already in collection server trace directory
            if ( PathsMatch( textTraceDirectory.Text,
                             m_collectionServer_TraceDirectory  ) )
            {
               ShowError( "Error: You cannot register trace files already in the Collection Server trace directory." );
               return;
            }
            

            // check sql server connection (by opening connection)
            try
            {
               OpenConnection( textServer.Text );
            }
            catch ( Exception ex )
            {
               ShowError( String.Format( "Error: Cant connect to SQL Server.\n\nError: {0}",
                  ex.Message) );
               return;
            }
            
            try
            {
               // see if SQlcompliance database exists
              string cmdstr = "select name from master..sysdatabases where name='SQLcompliance'";
              using ( SqlCommand    cmd    = new SqlCommand( cmdstr, m_connection ) )
              {
                 using ( SqlDataReader reader = cmd.ExecuteReader() )
                 {
                    if ( ! reader.Read() )
                    {
                        throw new Exception("xxx" );
                    }
                 }
              }
            }
            catch
            {
               ShowError( "Error: SQL Compliance Manager Repository database not found on the specified SQL Server." );
               return;
            }

            // validate that user has rights to write to repository
            if ( ! IsCurrentUserSysadmin( m_connection ) )
            {
               ShowError( "Error: You do not have enough rights within SQL Compliance Manager to register trace files." );
               return;
            }

            //------------
            // Processing
            //------------
            m_goodFiles = 0;
            m_badFiles  = 0;
            m_reprocessedFiles = 0;
            
            ProcessTraceFiles( textTraceDirectory.Text,
                               m_collectionServer_TraceDirectory);

            listResults.Items.Add( "" );
            listResults.Items.Add( "Registered Files:   " + m_goodFiles.ToString() );
            listResults.Items.Add( "Reprocessed Files:  " + m_reprocessedFiles.ToString() );
            listResults.Items.Add( "Files with Errors:  " + m_badFiles.ToString() );
            
            listResults.SelectedIndex = listResults.Items.Count-1;
            Application.DoEvents();            
            
            MessageBox.Show( this,
                             String.Format( "Trace File Registration Complete\n" +
                                            "================================\n" +
                                            "Files registered:  {0}\n" +
                                            "Files reprocessed: {1}\n" +
                                            "Files with errors: {2}\n\n" +
                                            "Note: All successfully registered files have been " +
                                            "renamed to '<name>.extension_processed' to prevent duplicate " +
                                            "registration. The reprocessed files have been moved to CollectionServerTrace directory.",
                                            m_goodFiles,
                                            m_reprocessedFiles,
                                            m_badFiles ),
                            this.Text ); 
         }
         finally
         {
            if ( listResults.Items.Count != 0 )
            {
               listResults.SelectedIndex = listResults.Items.Count-1;
            }
            
            CloseConnection();
            btnRegister.Enabled = true;
            Cursor = Cursors.Default;
            
            m_processing = false;
         }
      }
      
      //------------------------------------------
      // PathsMatch
      //-------------------------------------------
      private bool
         PathsMatch(
            string            path1,
            string            path2
         )
      {
         string p1 = path1.Trim().ToUpper();
         string p2 = path2.Trim().ToUpper();
         
         if ( ! p1.EndsWith(@"\")) p1 += @"\";
         if ( ! p2.EndsWith(@"\")) p2 += @"\";
         
         return (p1==p2);
      }

      //--------------------------------------------------------
      // ProcessTraceFiles
      //--------------------------------------------------------
      private void
         ProcessTraceFiles(
            string           sourceDir,
            string           targetDir
         )
      {
         try
         {
            DirectoryInfo traceDirectoryInfo = new DirectoryInfo( sourceDir );
            FileInfo [] fileInfoListTRC = traceDirectoryInfo.GetFiles(  "*.trc" );
            FileInfo [] fileInfoListXEL = traceDirectoryInfo.GetFiles(  "*.xel" );

            ArrayList fileInfoList = new ArrayList();
            fileInfoList.AddRange(fileInfoListTRC);
            fileInfoList.AddRange(fileInfoListXEL);

            if ( fileInfoList == null )
            {
               listResults.Items.Add( "**** ERROR: No trace files found in the specified source trace file directory." );
               return;
            }
            
            ArrayList tmpFileList = new ArrayList( fileInfoList );
            tmpFileList.Sort( new FileInfoComparer());
            

            foreach( FileInfo file in tmpFileList )
            {
               string status;
               
               // check for .TRC extension since GetFiles works funny for 3 character extensions
               string ext = file.Extension.ToUpper();
               if ( ext == ".TRC" || ext == ".XEL")
                    {
                  if ( TraceFile.Process( m_connection,
                                          sourceDir,
                                          file.Name,
                                          m_collectionServer_TraceDirectory,
                                          out status ) )
                  {
                     m_goodFiles ++;
                     listResults.Items.Add( String.Format( "Trace File: {0} - Registered",
                                                         file.Name) );
                  }
                  else
                  {
                     m_badFiles ++;
                     listResults.Items.Add( String.Format( "Trace File: {0} - Failed\n\n***** ERROR: {1}",
                                                         file.Name,
                                                         status) );
                  }
                  
                  // force screen refresh
                  listResults.SelectedIndex = listResults.Items.Count-1;
                  Application.DoEvents();
               }
            }

            //TODO Call this method from other process
            //Call reprocess method in order to move orphaned files and register them into jobs table
            this.ReprocessFiles(sourceDir, targetDir);

            if ( m_goodFiles == 0 && m_badFiles == 0 && m_reprocessedFiles == 0)
            {
               listResults.Items.Add( "**** ERROR: No files to process found in the specified file directory." );
            }
         }
         catch ( Exception ex )
         {
            listResults.Items.Add( String.Format( "***** ERROR PROCESSING FILES: {0}",
                                                  ex.Message ) );
         }
      }

        //--------------------------------------------------------
        // Reprocess orphaned files 
        //--------------------------------------------------------
        private void
           ReprocessFiles(
              string sourceDir,
              string targetDir
           )
        {  
            try
            {
                
                DirectoryInfo traceDirectoryInfo = new DirectoryInfo(sourceDir);

                FileInfo[] fileInfoListTRZ = traceDirectoryInfo.GetFiles("*.trz");
                FileInfo[] fileInfoListTR7Z = traceDirectoryInfo.GetFiles("*.tr7z");

                ArrayList fileInfoList = new ArrayList();
                fileInfoList.AddRange(fileInfoListTRZ);
                fileInfoList.AddRange(fileInfoListTR7Z);

                if (fileInfoList.Count == 0)                {
                    
                    return;
                }

                ArrayList tmpFileList = new ArrayList(fileInfoList);
                tmpFileList.Sort(new FileInfoComparer());


                foreach (FileInfo file in tmpFileList)
                {

                    string status;

                    // check for files with .TRZ or TR7Z extensions
                    string ext = file.Extension.ToUpper();
                    if (ext == ".TRZ" || ext == ".TR7Z")
                    {
                        if (TraceFile.Reprocess(m_connection,
                                                sourceDir,
                                                file.Name,
                                                m_collectionServer_TraceDirectory,
                                                out status))
                        {
                            m_reprocessedFiles++;
                            listResults.Items.Add(String.Format("File: {0} - Registered",
                                                                file.Name));
                        }
                        else
                        {
                            m_badFiles++;
                            listResults.Items.Add(String.Format("File: {0} - Failed\n\n***** ERROR: {1}",
                                                                file.Name,
                                                                status));
                        }

                        // force screen refresh
                        listResults.SelectedIndex = listResults.Items.Count - 1;
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                listResults.Items.Add(String.Format("***** ERROR REPROCESSING FILES: {0}",
                                                      ex.Message));
            }
        }

        //--------------------------------------------------------
        // btnBrowseCLick
        //--------------------------------------------------------
        private void btnBrowse_Click(object sender, System.EventArgs e)
      {
         FolderBrowserDialog dlg = new FolderBrowserDialog();
         dlg.ShowNewFolderButton = false;

         if ( DialogResult.OK == dlg.ShowDialog() )
         {
            textTraceDirectory.Text = dlg.SelectedPath;
         }
      }

      //--------------------------------------------------------
      // textServer_TextChanged
      //--------------------------------------------------------
      private void textServer_TextChanged(object sender, System.EventArgs e)
      {
         if ( textTraceDirectory.Text.Length != 0 && textServer.Text.Length != 0 )
            btnRegister.Enabled = true;
         else
            btnRegister.Enabled = false;
      }

      //--------------------------------------------------------
      // textTraceDirectory_TextChanged
      //--------------------------------------------------------
      private void textTraceDirectory_TextChanged(object sender, System.EventArgs e)
      {
         if ( textTraceDirectory.Text.Length != 0 && textServer.Text.Length != 0 )
            btnRegister.Enabled = true;
         else
            btnRegister.Enabled = false;
      }


      //--------------------------------------------------------
      // GetCollectionServerTraceDirectory
      //--------------------------------------------------------
      private string GetCollectionServerTraceDirectory()
      {
         string      traceDir = "";
         RegistryKey rk  = null;
         RegistryKey rks = null;
      
         try
         {
            rk  = Registry.LocalMachine;
            rks = rk.CreateSubKey(CollectionService_RegKey);
            traceDir = (string)rks.GetValue( CollectionService_RegVal_TraceDir, "" );
            if (traceDir == "")
            {
                rks = rk.CreateSubKey(CollectionService_RegKey_CM);
                traceDir = (string)rks.GetValue(CollectionService_RegVal_TraceDir, "");
            }
         }
         catch
         {
            traceDir = "";
         }
         finally
         {
            if ( rks != null )rks.Close();
            if ( rk != null )rk.Close();
         }

         return traceDir;
      }

      //--------------------------------------------------------
      // ShowError
      //--------------------------------------------------------
      private void
         ShowError(
            string errMessage
         )
      {
         MessageBox.Show( this,
            errMessage,
            this.Text,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
      }

      //-----------------------------------------------------------------------------
      // OpenConnection
      //-----------------------------------------------------------------------------
      public void
         OpenConnection(
            string            serverName
         )
      {
         string strConn = String.Format( "server={0};" +
            "integrated security=SSPI;" +
            "Connect Timeout=30;" + 
            "Application Name='SQLcompliance TraceRegister';",
            serverName );

         m_connection = new SqlConnection( strConn );
         m_connection.Open();
      }
	   
      //-----------------------------------------------------------------------------
      // CloseConnection
      //-----------------------------------------------------------------------------
      public void
         CloseConnection()
      {
         if ( m_connection != null )
         {
            m_connection.Close();
            m_connection.Dispose();
            m_connection = null;
         }
      }

      //-----------------------------------------------------------------------------
      // IsCurrentUserSysadmin
      //-----------------------------------------------------------------------------
      static public bool
         IsCurrentUserSysadmin(
            SqlConnection  conn 
         )
      {
         bool isSysadmin = false;
         
         try
         {
            string cmdStr = "SELECT IS_SRVROLEMEMBER ('sysadmin')";
            using ( SqlCommand    cmd    = new SqlCommand( cmdStr, conn ) )
            {
               int sysadmin;
               object obj = cmd.ExecuteScalar();
               if( obj is System.DBNull )
               {
                  sysadmin = 0;
               }
               else
               {
                  sysadmin = (int)obj;
               }

               if ( sysadmin == 1 )
               {
                  isSysadmin = true;
               }
            }
         }
         catch
         {
            // if it fails, you dont have right to do it so you are not a sysadmin
         }
			
         return isSysadmin;
      }

      private void btnClose_Click(object sender, System.EventArgs e)
      {
         if ( ! m_processing ) Close();
      }

      //-----------------------------------------------------------------------------
      // Getting SQL Server instance from CollectionService registry key
      //-----------------------------------------------------------------------------
        private String getDefaultSQLServerInstance()
      {
          String SQLServerInstance = String.Empty;

          try
          {
              using (RegistryKey key = Registry.LocalMachine.OpenSubKey(CollectionService_RegKey_CM))
              {
                  if (key != null)
                  {
                      SQLServerInstance = (String) key.GetValue("ServerInstance");
                  }
              }
          }
          catch (Exception ex)  
          {
            ShowError(String.Format("Error: Could not get SQL Server Instance Name: {0}", ex.Message));
          }
          return SQLServerInstance;
      }
   }
}
