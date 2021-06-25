using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Infragistics.Win;

namespace Idera.SQLcompliance.Application.GUI.Controls
{
   public partial class GroupPanel : Panel
   {
      private Color groupBoxBackColor = Color.White;
      private Color headerBackColor = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientLight;
      private Color headerBackColor2 = Office2007ColorTable.Colors.OutlookNavPaneCurrentGroupHeaderGradientDark;
      private Color borderColor = Office2007ColorTable.Colors.OutlookNavPaneBorder;
      private bool roundedBottoms = false ;

      public GroupPanel()
      {
         SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);

         InitializeComponent();
      }

      [Category("Appearance")]
      public Color BorderColor
      {
         get { return borderColor; }
         set
         {
            borderColor = value;
            Invalidate(false);
         }
      }

      [Category("Appearance")]
      public Color HeaderBackColor
      {
         get { return headerBackColor; }
         set
         {
            headerBackColor = value;
            Invalidate(false);
         }
      }

      [Category("Appearance")]
      public Color HeaderBackColor2
      {
         get { return headerBackColor2; }
         set
         {
            headerBackColor2 = value;
            Invalidate(false);
         }
      }

      [Category("Appearance")]
      public Color GroupBoxBackColor
      {
         get { return groupBoxBackColor; }
         set
         {
            groupBoxBackColor = value;
            Invalidate(false);
         }
      }

      [Category("Appearance")]
      public bool RoundedBottoms
      {
         get { return roundedBottoms; }
         set
         {
            roundedBottoms = value;
            Invalidate(false);
         }
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

         float borderArcRadius = 1;

         Rectangle borderBounds = Rectangle.Inflate(ClientRectangle, -1, -1);
         if(roundedBottoms)
            DrawRoundRectangle(e.Graphics, groupBoxBackColor, Color.Empty, borderBounds, borderArcRadius);
         else
            DrawRoundTopRectangle(e.Graphics, groupBoxBackColor, Color.Empty, borderBounds, borderArcRadius);

         Rectangle headerBounds = Rectangle.Inflate(ClientRectangle, -1, -1);
         headerBounds.Height = 25;
         DrawHeader(e.Graphics, headerBounds, borderArcRadius);

         base.OnPaint(e);
      }

      private void DrawHeader(Graphics graphics, Rectangle bounds, float radius)
      {
         if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
         {
            using (GraphicsPath gp = new GraphicsPath())
            {
               gp.AddLine(0, radius, 0, bounds.Height);
               gp.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
               gp.AddLine(radius, 0, bounds.Width - radius, 0);
               gp.AddArc(bounds.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
               gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height);
               gp.AddLine(bounds.Width, bounds.Height, 0, bounds.Height);
               gp.CloseFigure();

               RectangleF topBounds = bounds;
               using (LinearGradientBrush fillBrush = new LinearGradientBrush(topBounds, headerBackColor,
                                                                              headerBackColor2,
                                                                              LinearGradientMode.Vertical))
               {
                  graphics.FillPath(fillBrush, gp);
               }

               using (Pen pen = new Pen(borderColor, 1))
               {
                  graphics.DrawPath(pen, gp);
               }
            }
         }
      }

      private void DrawRoundRectangle(Graphics graphics, Color fillColor1, Color fillColor2,
                                      RectangleF bounds, float radius)
      {
         if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
         {
            using (GraphicsPath gp = new GraphicsPath())
            {
               // Top Line
               gp.AddLine(radius, 0, bounds.Width - (radius*2), 0);
               gp.AddArc(bounds.Width - (radius*2), 0, radius*2, radius*2, 270, 90);
               // right Line
               gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height - (radius*2));
               gp.AddArc(bounds.Width - (radius*2), bounds.Height - (radius*2), radius*2, radius*2, 0, 90);
               // Bottom Line
               gp.AddLine(bounds.Width - (radius*2), bounds.Height, radius, bounds.Height);
               gp.AddArc(0, bounds.Height - (radius*2), radius*2, radius*2, 90, 90);
               // Left Line
               gp.AddLine(0, bounds.Height - (radius*2), 0, radius);
               gp.AddArc(0, 0, radius*2, radius*2, 180, 90);
               gp.CloseFigure();

               if (fillColor1 != Color.Empty)
               {
                  if (fillColor2 == Color.Empty)
                  {
                     using (SolidBrush fillBrush = new SolidBrush(fillColor1))
                     {
                        graphics.FillPath(fillBrush, gp);
                     }
                  }
                  else
                  {
                     using (LinearGradientBrush fillBrush = new LinearGradientBrush(bounds, fillColor1,
                                                                                    fillColor2,
                                                                                    LinearGradientMode.Vertical))
                     {
                        graphics.FillPath(fillBrush, gp);
                     }
                  }
               }
               using (Pen pen = new Pen(borderColor, 1))
               {
                  graphics.DrawPath(pen, gp);
               }
            }
         }
      }

      private void DrawRoundTopRectangle(Graphics graphics, Color fillColor1, Color fillColor2,
                                      RectangleF bounds, float radius)
      {
         if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
         {
            using (GraphicsPath gp = new GraphicsPath())
            {
               // Top Line
               gp.AddLine(radius, 0, bounds.Width - (radius * 2), 0);
               gp.AddArc(bounds.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
               // right Line
               gp.AddLine(bounds.Width, radius, bounds.Width, bounds.Height - radius);
               // Bottom Line
               gp.AddLine(bounds.Width, bounds.Height, 0, bounds.Height);
               // Left Line
               gp.AddLine(0, bounds.Height, 0, radius);
               gp.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
               gp.CloseFigure();
               if (fillColor1 != Color.Empty)
               {
                  if (fillColor2 == Color.Empty)
                  {
                     using (SolidBrush fillBrush = new SolidBrush(fillColor1))
                     {
                        graphics.FillPath(fillBrush, gp);
                     }
                  }
                  else
                  {
                     using (LinearGradientBrush fillBrush = new LinearGradientBrush(bounds, fillColor1,
                                                                                    fillColor2,
                                                                                    LinearGradientMode.Vertical))
                     {
                        graphics.FillPath(fillBrush, gp);
                     }
                  }
               }
               using (Pen pen = new Pen(borderColor, 1))
               {
                  graphics.DrawPath(pen, gp);
               }
            }
         }
      }
   }
}