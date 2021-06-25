using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
    public class OptimizedListView: ListView
    {
        public OptimizedListView()
        {
            //Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out Windows messages before they get to the form's WndProc
            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message message)
        {
            //Filter out the WM_ERASEBKGND message
            if (message.Msg != 0x14)
            {
                base.OnNotifyMessage(message);
            }
        }
    }
}
