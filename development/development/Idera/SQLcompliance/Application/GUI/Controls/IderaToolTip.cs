using System.Drawing;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Idera.SQLcompliance.Application.GUI.Properties;

namespace SQLCM_Installer.Custom_Controls
{
    class IderaToolTip : System.Windows.Forms.UserControl
    {
        Label text = new Label();
        private Panel panel1;
        private PictureBox pictureBox1;
        private Label toolTipLbl;
        PictureBox blacktriangle = new PictureBox();

        public IderaToolTip()
        {
            InitializeComponent();
        }
        

        public override string Text
        {
            get { return this.toolTipLbl.Text; }
            set { this.toolTipLbl.Text = value; }
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTipLbl = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(29)))), ((int)(((byte)(22)))));
            this.panel1.Controls.Add(this.toolTipLbl);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(170, 66);
            this.panel1.TabIndex = 0;
            // 
            // toolTipLbl
            // 
            this.toolTipLbl.ForeColor = System.Drawing.Color.White;
            this.toolTipLbl.Location = new System.Drawing.Point(4, 4);
            this.toolTipLbl.Name = "toolTipLbl";
            this.toolTipLbl.Size = new System.Drawing.Size(163, 58);
            this.toolTipLbl.TabIndex = 0;
            this.toolTipLbl.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Idera.SQLcompliance.Application.GUI.Properties.Resources.TooltipTriangle;
            this.pictureBox1.Location = new System.Drawing.Point(69, 65);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // IderaToolTip
            // 
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Name = "IderaToolTip";
            this.Size = new System.Drawing.Size(170, 85);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

