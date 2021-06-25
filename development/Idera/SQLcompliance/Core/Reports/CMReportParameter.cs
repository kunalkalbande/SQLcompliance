using System;
using System.Collections.Generic;
using System.Data;

namespace Idera.SQLcompliance.Core.Reports
{
   public class CMReportParameter 
   {
      private string _name ;
      private object _value ;
      private Type _valueType ;
      private string _prompt ;
      private bool _usedInQuery ;
      private bool _isList ;
      private object _defaultValue;
      private Dictionary<string, object> _valueItems;
      private System.EventHandler _onChangeEvent;
      
      #region Constructors

      public CMReportParameter()
      {
         _name = "";
         _value = null;
         _valueType = typeof(object);
         _prompt = "";
         _usedInQuery = false;
         _isList = false;
         _defaultValue = null;
         _valueItems = new Dictionary<string, object>();
      }

      #endregion
      
      #region Properties

      public System.EventHandler OnChangeEvent
      {
          get { return _onChangeEvent; }
          set { _onChangeEvent = value; }
      }

      public string Name 
      { 
         get { return _name; }
         set { _name = value; }
      }
      
      public object Value
      {
         get { return _value; }
         set { _value = value; }
      }
      
      public Type ValueType
      {
         get { return _valueType; }
         set { _valueType = value; }
      }

      public SqlDbType SqlDbType
      {
         get 
         {
            if (_valueType == typeof(bool))
               return SqlDbType.Bit;
            else if (_valueType == typeof(int))
               return SqlDbType.Int;
            else if ((_valueType == typeof(string)) || (_valueType == typeof(Char)))
                return SqlDbType.NVarChar;
            else if (_valueType == typeof(DateTime))
               return SqlDbType.DateTime;
            else
               return SqlDbType.Money;
         }
      }
      
      public string Prompt
      {
         get { return _prompt; }
         set { _prompt = value; }
      }
      
      public bool UsedInQuery
      {
         get { return _usedInQuery; }
         set { _usedInQuery = value; }
      }

      public bool IsList
      {
         get { return _isList; }
         set { _isList = value; }
      }


      public object DefaultValue
      {
         get { return _defaultValue; }
         set { _defaultValue = value; }
      }

      public Dictionary<string, object> ValueItems
      {
         get { return _valueItems; }
      }

      #endregion

      #region Public Methods

      public void ClearList()
      {
         _valueItems.Clear();
      }

      public void AddListItem(string displayName, object value)
      {
         _isList = true;
         _valueItems.Add(displayName, value);
      }

      public object GetItemValue(string displayName)
      {
         return _valueItems[displayName];
      }

      #endregion
   }
}
