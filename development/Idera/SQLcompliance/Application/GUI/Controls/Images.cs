using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices ;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class Images : UserControl
   {
      public Images()
      {
         InitializeComponent();
      }
   }

   public static class AppIcons
   {
      private static Images _images = new Images() ;
      private static Icon[] _icons16 ;
      private static Icon[] _icons24 ;

      static AppIcons()
      {
         _icons16 = new Icon[_images.ImageList16.Images.Count];
         _icons24 = new Icon[_images.ImageList24.Images.Count];
      }

      public enum Img24
      {
      }

     //DONT CHANGE THE SEQUENCE OF THE BELOW ENUMS
      public enum Img16
      {
         Role, 
         WindowsUser,
         Server,
         AlertRules,
         EventFilters,
         ActivityLog,
         ChangeLog,
         ServerDisabled,
         Database,
         DatabaseDisabled,
         WarningServer,
         DisabledServer,
         ErrorServer,
         OkServer,
         ReportServer,
         SqlServerLogin,
         WindowsGroup,
         DefaultAuditSettings
      }

      public static Image AppImg24(Img24 type)
      {
         return _images.ImageList24.Images[(int)type] ;
      }

      public static ImageList AppImageList16()
      {
         return _images.ImageList16 ;
      }

      public static Image AppImg16(Img16 type)
      {
         return _images.ImageList16.Images[(int)type];
      }
      
      public static Icon AppIcon16(Img16 type)
      {
         if(_icons16[(int)type] == null)
         {
            IntPtr iconPtr = ImageList_GetIcon(_images.ImageList16.Handle, (int)type, (int)ILD_FLAGS.ILD_NORMAL) ;
            _icons16[(int)type] =  Icon.FromHandle(iconPtr) ;
         }
         return _icons16[(int)type] ;
      }

      [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
      static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, [MarshalAs(UnmanagedType.U4)] int flags);

      [Flags]
      public enum ILD_FLAGS : int
      {
         ILD_NORMAL = 0x00000000,
         ILD_TRANSPARENT = 0x00000001,
         ILD_BLEND25 = 0x00000002,
         ILD_FOCUS = 0x00000002,
         ILD_BLEND50 = 0x00000004,
         ILD_SELECTED = 0x00000004,
         ILD_BLEND = 0x00000004,
         ILD_MASK = 0x00000010,
         ILD_IMAGE = 0x00000020,
         ILD_ROP = 0x00000040,
         ILD_OVERLAYMASK = 0x00000F00,
         ILD_PRESERVEALPHA = 0x00001000,
         ILD_SCALE = 0x00002000,
         ILD_DPISCALE = 0x00004000,
         ILD_ASYNC = 0x00008000
      }
   }
}
