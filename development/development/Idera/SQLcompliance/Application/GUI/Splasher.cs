using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using Idera.SQLcompliance.Application.GUI.Forms;

namespace Idera.SQLcompliance.Application.GUI
{
   /*
	/// <summary>
	/// Summary description for Splasher.
	/// </summary>
    public class Splasher
    {
        public static bool  visible      = false;
        static SplashScreen splashScreen = null;
        static Thread       splashThread = null;

        //	internally used as a thread function - showing the form and
        //	starting the messageloop for it
        static void ShowThread()
        {
            splashScreen = new SplashScreen();
            System.Windows.Forms.Application.Run(splashScreen);
        }

        //	public Method to show the StartupForm
        static public void Start()
        {
            if (splashThread != null)
                return;
            
            visible = true; 
            splashThread = new Thread(new ThreadStart(Splasher.ShowThread));
            splashThread.IsBackground = true;
            splashThread.ApartmentState = ApartmentState.STA;
            splashThread.Start();
        }

        //	public Method to hide the StartupForm
        static public void Close()
        {
            visible = false;
           
            if (splashThread == null) return;
            if (splashScreen == null) return;

            try
            {
                splashScreen.Invoke(new MethodInvoker(splashScreen.Close));
            }
            catch (Exception)
            {
            }
            splashThread = null;
            splashScreen = null;
        }

        //	public Method to set or get the loading Status
        static public string Status
        {
            set
            {
                if (splashScreen == null)
                {
                    return;
                }

                splashScreen.Status = value;
            }
            get
            {
                if (splashScreen == null)
                {
                    Debug.WriteLine( "Splash screen not on screen" );
                    return "";
                }
                return splashScreen.Status;
            }
        }
    }
    */
}




