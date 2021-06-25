using System ;
using System.Windows.Forms ;
using Idera.SQLcompliance.Application.GUI.Properties ;

namespace Idera.SQLcompliance.Application.GUI.Forms
{
	/// <summary>
	/// Summary description for Form_DateSpan.
	/// </summary>
	public partial class Form_TimeSpan : Form
	{
      private static DateTime _startTime ;
      private static DateTime _endTime ;


      static Form_TimeSpan()
      {
         _endTime = DateTime.Now.Date.AddDays(1) ;
         _startTime = _endTime.AddDays(-90) ;
      }

	   public Form_TimeSpan()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.Icon = Resources.SQLcompliance_product_ico;
         _dtStartDate.Value = _startTime ;
         _dtStartTime.Value = _startTime ;
         _dtEndDate.Value = _endTime ;
         _dtEndTime.Value = _endTime ;
      }

      public DateTime StartDate
      {
         get 
         {
            DateTime dt1, dt2;
            DateTime date, time;

            date = _dtStartDate.Value;
            time = _dtStartTime.Value;
            dt1 = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            date = _dtEndDate.Value;
            time = _dtEndTime.Value;
            dt2 = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            if (dt1 < dt2)
               _startTime = dt1 ;
            else
               _startTime = dt2;

            return _startTime ;
         }

//         set
//         {
//            _dtStartDate.Value = value ;
//            _dtStartTime.Value = value ;
//         }
      }

      public DateTime EndDate
      {
         get
         {
            DateTime dt1, dt2;
            DateTime date, time;

            date = _dtStartDate.Value;
            time = _dtStartTime.Value;
            dt1 = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            date = _dtEndDate.Value;
            time = _dtEndTime.Value;
            dt2 = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            if (dt1 > dt2)
               _endTime = dt1;
            else
               _endTime = dt2;

            return _endTime ;
         }

//         set
//         {
//            _dtEndDate.Value = value;
//            _dtEndTime.Value = value;
//         }
      }
   }
}
