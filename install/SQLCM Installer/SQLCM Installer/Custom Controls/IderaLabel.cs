using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Controls
{
    class IderaLabel : Label
    {
        bool isFontBold = false;
        bool _isDisabled = false;

        public IderaLabel(bool boldFont)
        {
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.FromArgb(72, 62, 47);
            this.AutoSize = true;
            isFontBold = boldFont;
        }

        public IderaLabel()
        {
            this.BackColor = Color.Transparent;
            this.ForeColor = Color.FromArgb(72, 62, 47);
            this.AutoSize = true;
        }

        public bool Disabled
        {
            get
            {
                return _isDisabled;
            }
            set
            {
                if (value)
                {
                    this.ForeColor = Color.FromArgb(146, 136, 117);
                }
                else
                {
                    this.ForeColor = Color.FromArgb(72, 62, 47);
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (isFontBold)
            {
                if (Constants.SourceSansProBold != null)
                {
                    this.Font = new System.Drawing.Font(Constants.SourceSansProBold, 10.5F, FontStyle.Bold);
                }
            }
            else
            {
                if (Constants.SourceSansProRegular != null)
                {
                    this.Font = new System.Drawing.Font(Constants.SourceSansProRegular, 10.5F, FontStyle.Regular);
                }
            }
        }
    }
}
