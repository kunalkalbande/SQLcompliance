using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCM_Installer.Custom_Controls
{
    class IderaTitleLabel : Label
    {
        public IderaTitleLabel()
        {
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Constants.SourceSansProSemiBold != null)
            {
                this.Font = new System.Drawing.Font(Constants.SourceSansProSemiBold, 15F, FontStyle.Regular);
            }
        }
    }
}
