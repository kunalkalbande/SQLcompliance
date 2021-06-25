using System;
using System.Windows.Forms;
using Idera.SQLcompliance.Application.GUI.Forms;

namespace Idera.SQLcompliance.Application.GUI
{
   internal class SQLcmApplicationContext : ApplicationContext
   {
      internal SQLcmApplicationContext(SplashScreen splashForm) : base(splashForm)
      {
      }

      protected override void OnMainFormClosed(object sender, EventArgs e)
      {
         if(sender is SplashScreen)
         {
            SplashScreen splash = (SplashScreen)sender ;
            if (splash.IsConnected)
            {
               splash.MainForm2.FireConnected() ;
               MainForm = splash.MainForm2;
               MainForm.Show();
            }else
               base.OnMainFormClosed(sender, e) ;
         }
         else if(sender is Form_Main2)
         {
            base.OnMainFormClosed(sender, e) ;
         }
      }
   }
}