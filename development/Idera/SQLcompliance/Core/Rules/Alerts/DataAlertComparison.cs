using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Idera.SQLcompliance.Core.Rules.Alerts
{
   public enum ComparisonOperator
   {
      [Description("Equal To")]
      Equal,
      [Description("Not Equal To")]
      NotEqual,
      [Description("Less Than")]
      LessThan,
      [Description("Greater Than")]
      GreaterThan,
      [Description("Less Than or Equal To")]
      LessThanEqual,
      [Description("Greater Than or Equal To")]
      GreaterThanEqual
   };

   public class DataAlertComparison
   {
      private long _comparisonValue;
      private ComparisonOperator _comparisonOperator;
      private int _id;

      [XmlElement("ComparisonValue")]
      public long Value
      {
         get { return _comparisonValue; }
         set { _comparisonValue = value; }
      }

      [XmlElement("ComparisonOperator")]
      public ComparisonOperator Operator
      {
         get { return _comparisonOperator; }
         set { _comparisonOperator = value; }
      }

      [XmlElement("Id")]
      public int Id
      {
         get { return _id; }
         set { _id = value; }
      }

      public DataAlertComparison()
      {
         _id = -1;
         _comparisonValue = 5000;  //This is an arbitrary number.  It doesn't matter what it is, I just wanted a default.
         _comparisonOperator = ComparisonOperator.GreaterThan; //This is an arbitrary operator.  It doesn't matter what it is, I just wanted a default.
      }
   }
}
