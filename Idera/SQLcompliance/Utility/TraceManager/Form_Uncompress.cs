using System ;
using System.ComponentModel ;
using System.IO ;
using System.Windows.Forms ;
using Xceed.Compression.Formats ;
using Licenser=Xceed.Compression.Licenser;

namespace Idera.SQLcompliance.Utility.TraceManager
{
	/// <summary>
	/// Summary description for Form_Uncompress.
	/// </summary>
	public class Form_Uncompress : Form
	{
      private Button btnClose;
      private Label label3;
      private GroupBox groupBox1;
      private Label label2;
      private Button btnBrowse;
      private Label label1;
      private Button btnUncompress;
      private TextBox txtTarget;
      private ListBox listResults;
      private Button btnChecksum;
      private System.Windows.Forms.ListBox _listFiles;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public Form_Uncompress()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			
		   Licenser.LicenseKey = "SCN10DKKPZTUTWAU4AA";
		   Xceed.Compression.Formats.Licenser.LicenseKey = "SCN10DKKPZTUTWAU4AA";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
         System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form_Uncompress));
         this.btnClose = new System.Windows.Forms.Button();
         this.listResults = new System.Windows.Forms.ListBox();
         this.label3 = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.btnChecksum = new System.Windows.Forms.Button();
         this.btnUncompress = new System.Windows.Forms.Button();
         this.txtTarget = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.btnBrowse = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this._listFiles = new System.Windows.Forms.ListBox();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnClose
         // 
         this.btnClose.Location = new System.Drawing.Point(468, 416);
         this.btnClose.Name = "btnClose";
         this.btnClose.TabIndex = 3;
         this.btnClose.Text = "Close";
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // listResults
         // 
         this.listResults.HorizontalScrollbar = true;
         this.listResults.Location = new System.Drawing.Point(8, 284);
         this.listResults.Name = "listResults";
         this.listResults.ScrollAlwaysVisible = true;
         this.listResults.Size = new System.Drawing.Size(536, 121);
         this.listResults.TabIndex = 7;
         // 
         // label3
         // 
         this.label3.Location = new System.Drawing.Point(8, 272);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(48, 12);
         this.label3.TabIndex = 8;
         this.label3.Text = "Results:";
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this._listFiles);
         this.groupBox1.Controls.Add(this.btnChecksum);
         this.groupBox1.Controls.Add(this.btnUncompress);
         this.groupBox1.Controls.Add(this.txtTarget);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Controls.Add(this.btnBrowse);
         this.groupBox1.Controls.Add(this.label1);
         this.groupBox1.Location = new System.Drawing.Point(8, 8);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(536, 256);
         this.groupBox1.TabIndex = 10;
         this.groupBox1.TabStop = false;
         // 
         // btnChecksum
         // 
         this.btnChecksum.Enabled = false;
         this.btnChecksum.Location = new System.Drawing.Point(424, 224);
         this.btnChecksum.Name = "btnChecksum";
         this.btnChecksum.TabIndex = 14;
         this.btnChecksum.Text = "Checksum";
         this.btnChecksum.Click += new System.EventHandler(this.btnChecksum_Click);
         // 
         // btnUncompress
         // 
         this.btnUncompress.Enabled = false;
         this.btnUncompress.Location = new System.Drawing.Point(200, 224);
         this.btnUncompress.Name = "btnUncompress";
         this.btnUncompress.Size = new System.Drawing.Size(136, 23);
         this.btnUncompress.TabIndex = 13;
         this.btnUncompress.Text = "Uncompress Now";
         this.btnUncompress.Click += new System.EventHandler(this.btnUncompress_Click);
         // 
         // txtTarget
         // 
         this.txtTarget.Location = new System.Drawing.Point(112, 188);
         this.txtTarget.Name = "txtTarget";
         this.txtTarget.Size = new System.Drawing.Size(384, 20);
         this.txtTarget.TabIndex = 12;
         this.txtTarget.Text = "";
         // 
         // label2
         // 
         this.label2.Location = new System.Drawing.Point(8, 192);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(104, 16);
         this.label2.TabIndex = 11;
         this.label2.Text = "Uncompressed file:";
         // 
         // btnBrowse
         // 
         this.btnBrowse.Location = new System.Drawing.Point(504, 16);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(24, 20);
         this.btnBrowse.TabIndex = 9;
         this.btnBrowse.Text = "...";
         this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 20);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(96, 16);
         this.label1.TabIndex = 7;
         this.label1.Text = "Compressed files:";
         // 
         // _listFiles
         // 
         this._listFiles.Location = new System.Drawing.Point(112, 20);
         this._listFiles.Name = "_listFiles";
         this._listFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this._listFiles.Size = new System.Drawing.Size(384, 160);
         this._listFiles.TabIndex = 15;
         this._listFiles.SelectedIndexChanged += new System.EventHandler(this._listFiles_SelectedIndexChanged);
         // 
         // Form_Uncompress
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(554, 448);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.listResults);
         this.Controls.Add(this.btnClose);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form_Uncompress";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Uncompress files";
         this.groupBox1.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

      private void btnBrowse_Click(object sender, EventArgs e)
      {
         OpenFileDialog frm = new OpenFileDialog();
         frm.Multiselect = true ;
         frm.Filter = "Compressed Trace Files (*.trz)|*.trz|Compressed Settings Files(*.biz)|*.biz|All Files(*.*)|*.*";
                     
         if(DialogResult.OK == frm.ShowDialog())
         {
            _listFiles.Items.Clear() ;
            _listFiles.Items.AddRange(frm.FileNames) ;
         }
      }

      private void btnUncompress_Click(object sender, EventArgs e)
      {
         if(_listFiles.SelectedIndices.Count == 1)
         {
            DecompressFile((_listFiles.Items[_listFiles.SelectedIndex]).ToString(), txtTarget.Text) ;
         }
         else
         {
            foreach(string s in _listFiles.SelectedItems)
               DecompressFile(s, RenameCompressedFile(s)) ;
         }
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         this.Close();
      }

    // ------------------------------------------------------------------------------------
    // Function that perform the actual Decompression of a source file to a destination file
    // ------------------------------------------------------------------------------------
    private bool DecompressFile( string sourceFileName, string decompressedFileName )
    {
      bool decompressedFile = false;
      
      try
      {
        
        this.Cursor = Cursors.WaitCursor;

        using( Stream sourceFile = new FileStream( sourceFileName, FileMode.Open ) )
        {
          using( Stream destinationFile = new FileStream( decompressedFileName, FileMode.Create ) )
          {
            byte[] buffer = new byte[ 32768 ];
            int read;

            using( XceedCompressedStream standard = new XceedCompressedStream( sourceFile ) )
            {
            while( ( read = standard.Read( buffer, 0, buffer.Length ) ) > 0 )
            {
               destinationFile.Write( buffer, 0, read );
            }
            }
          }
        }
      
        decompressedFile = true;
        listResults.Items.Add( sourceFileName + " successfully decompressed.");
      }
      catch( Exception exception )
      {
        listResults.Items.Add( "Error decompressing file: " + sourceFileName); 
        listResults.Items.Add( "      " + exception.Message );
      }
      finally
      {
        this.Cursor = Cursors.Default;
      }

      return decompressedFile;
    }

      private void btnChecksum_Click(object sender, EventArgs e)
      {
         foreach(string s in _listFiles.SelectedItems)
         {
            try
            {
               int chksum = CalculateChecksum( s );
               listResults.Items.Add( String.Format( "Checksum: {0}  File: {1}", chksum, s ));
            }
            catch ( Exception ex )
            {
               listResults.Items.Add( "Error calculating checksum of " + s); 
               listResults.Items.Add( "      " + ex.Message );
            }
         }
      }
      
		//-----------------------------------------------------------------------
		// CalculateChecksum - Padded with random seed 
		//-----------------------------------------------------------------------
		static public int
		   CalculateChecksum(
		      string                fileName
		   )
		{
         int crc = 3662;
		   using(Stream sourceFile = new FileStream( fileName, FileMode.Open ))
		   {
           
			   byte[] buffer = new byte[ 32768 ];
			   int read;
           
			   while( ( read = sourceFile.Read( buffer, 0, buffer.Length ) ) > 0 )
			   {
				   crc = ChecksumStream.CalculateCrc32( buffer,0,read,crc );
			   }
			   sourceFile.Close() ;
		   }
         
         return crc;
		}

      private void _listFiles_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if(_listFiles.SelectedIndices.Count > 1)
         {
            txtTarget.Enabled = false ;
            btnChecksum.Enabled = true ;
            btnUncompress.Enabled = true ;
         }
         else if(_listFiles.SelectedIndices.Count == 0)
         {
            txtTarget.Enabled = false ;
            btnChecksum.Enabled = false ;
            btnUncompress.Enabled = false ;
         }
         else if(_listFiles.SelectedIndices.Count == 1)
         {
            txtTarget.Enabled = true ;
            txtTarget.Text = RenameCompressedFile((_listFiles.Items[_listFiles.SelectedIndex]).ToString()) ;
            btnChecksum.Enabled = true ;
            btnUncompress.Enabled = true ;
         }
   }

      private string RenameCompressedFile(string sFilename)
      {
         string retVal = "" ;
         // create target name - replace trz with trc or xml with xml
         if ( sFilename.Length > 4 )
         {
            int pos = sFilename.Length - 4;
            if ( sFilename.Substring(pos,4).ToUpper() ==  ".TRZ" )
            {
               retVal = sFilename.Substring( 0,pos);
               retVal += ".trc";
            }
            else if ( sFilename.Substring(pos,4).ToUpper() ==  ".BIZ" )
            {
               retVal = sFilename.Substring( 0,pos);
               retVal += ".bin";
            }
         }
         return retVal ;
      }
      
	}
}
