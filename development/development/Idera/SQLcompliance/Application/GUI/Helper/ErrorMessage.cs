using System;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
	/// <summary>
	/// Summary description for ErrorMessage.
	/// </summary>
	public class ErrorMessage
	{
		public ErrorMessage()
		{
		}
		
		static public DialogResult
		   Show(
		      string            caption,
		      string            ErrMsg
         )
      {
         return Show( caption, ErrMsg, "" );
      }

		static public DialogResult
		   Show(
		      string            caption,
		      string            ErrMsg,
            string            ExtraInfo
         )
      {
         string dispStr = String.Format( "{0}{1}{2}",
                                         ErrMsg,
                                         (ExtraInfo!="") ? UIConstants.ErrorLabel : "",
                                         ExtraInfo );
         return MessageBox.Show( dispStr,
                                 caption,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Stop );
      }
      
		static public DialogResult
		   Show(
		      string            caption,
		      string            ErrMsg,
            string            ExtraInfo,
            MessageBoxIcon    icon
         )
      {
         string dispStr = String.Format( "{0}{1}{2}",
                                         ErrMsg,
                                         (ExtraInfo!="") ? UIConstants.ErrorLabel : "",
                                         ExtraInfo );
         return MessageBox.Show( dispStr,
                                 caption,
                                 MessageBoxButtons.OK,
                                 icon);
      }
	}
}
