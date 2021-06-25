using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
   public class StatusAlertThreshold
   {
      private int _value;
      private int _id;

      [XmlElement("Value")]
      public int Value
      {
         get { return _value; }
         set { _value = value; }
      }

      [XmlElement("Id")]
      public int Id
      {
         get { return _id; }
         set { _id = value; }
      }

      public StatusAlertThreshold()
      {
         _id = -1;
         _value = 0;
      }
   }
}
