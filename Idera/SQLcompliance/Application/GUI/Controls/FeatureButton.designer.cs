namespace Idera.SQLcompliance.Application.GUI.Controls
{
    partial class FeatureButton
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
           this.featureImagePictureBox = new System.Windows.Forms.PictureBox();
           this.featureDescriptionLabel = new System.Windows.Forms.Label();
           this.featureHeaderLabel = new System.Windows.Forms.Label();
           ((System.ComponentModel.ISupportInitialize)(this.featureImagePictureBox)).BeginInit();
           this.SuspendLayout();
           // 
           // featureImagePictureBox
           // 
           this.featureImagePictureBox.BackColor = System.Drawing.Color.Transparent;
           this.featureImagePictureBox.Location = new System.Drawing.Point(10, 10);
           this.featureImagePictureBox.Name = "featureImagePictureBox";
           this.featureImagePictureBox.Size = new System.Drawing.Size(32, 32);
           this.featureImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
           this.featureImagePictureBox.TabIndex = 0;
           this.featureImagePictureBox.TabStop = false;
           this.featureImagePictureBox.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
           this.featureImagePictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
           this.featureImagePictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
           this.featureImagePictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
           this.featureImagePictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
           this.featureImagePictureBox.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
           // 
           // featureDescriptionLabel
           // 
           this.featureDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                       | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.featureDescriptionLabel.AutoEllipsis = true;
           this.featureDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
           this.featureDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           this.featureDescriptionLabel.Location = new System.Drawing.Point(48, 28);
           this.featureDescriptionLabel.Name = "featureDescriptionLabel";
           this.featureDescriptionLabel.Size = new System.Drawing.Size(253, 16);
           this.featureDescriptionLabel.TabIndex = 2;
           this.featureDescriptionLabel.Text = "<description>";
           this.featureDescriptionLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
           this.featureDescriptionLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
           this.featureDescriptionLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
           this.featureDescriptionLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
           this.featureDescriptionLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
           this.featureDescriptionLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
           // 
           // featureHeaderLabel
           // 
           this.featureHeaderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.featureHeaderLabel.BackColor = System.Drawing.Color.Transparent;
           this.featureHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           this.featureHeaderLabel.ForeColor = System.Drawing.Color.Red;
           this.featureHeaderLabel.Location = new System.Drawing.Point(48, 8);
           this.featureHeaderLabel.Name = "featureHeaderLabel";
           this.featureHeaderLabel.Size = new System.Drawing.Size(255, 18);
           this.featureHeaderLabel.TabIndex = 1;
           this.featureHeaderLabel.Text = "<header>";
           this.featureHeaderLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
           this.featureHeaderLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
           this.featureHeaderLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
           this.featureHeaderLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
           this.featureHeaderLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
           this.featureHeaderLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
           // 
           // FeatureButton
           // 
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
           this.Controls.Add(this.featureImagePictureBox);
           this.Controls.Add(this.featureDescriptionLabel);
           this.Controls.Add(this.featureHeaderLabel);
           this.MinimumSize = new System.Drawing.Size(0, 40);
           this.Name = "FeatureButton";
           this.Size = new System.Drawing.Size(306, 50);
           ((System.ComponentModel.ISupportInitialize)(this.featureImagePictureBox)).EndInit();
           this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox featureImagePictureBox;
        private System.Windows.Forms.Label featureDescriptionLabel;
        private System.Windows.Forms.Label featureHeaderLabel;
    }
}
