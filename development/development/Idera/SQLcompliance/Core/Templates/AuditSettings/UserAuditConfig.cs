using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Templates.AuditSettings
{
   public class UserAuditConfig : AuditConfig
   {
      #region Data Members
      
      UserList _userList = null;
      bool _keepSQL = false;
      bool _captureTrans = false;
      bool _captureDDL = false;
      bool _keepSQLXE = false; //5.4_4.1.1_Extended Events

      
      #endregion 
      
      #region Properties
      
      [XmlElement("UserList")]
      public UserList UserList
      {
         get
         {
            return _userList;
         }
         set
         {
            _userList = value;
         }
      }
         
      [XmlElement("CaptureSQL")]
      public bool KeepSQL
      {
         get
         {
            return _keepSQL;
         }
         set
         {
            _keepSQL = value;
         }
      }

      [XmlElement("CaptureSQLXE")] //5.4_4.1.1_Extended Events
      public bool KeepSQLXE
      {
          get { return _keepSQLXE; }
          set { _keepSQLXE = value; }
      }
      

      [XmlElement("CaptureTrans")]
      public bool CaptureTrans
      {
         get { return _captureTrans; }
         set { _captureTrans = value; }
      }

      [XmlElement("CaptureDDL")]
      public bool CaptureDDL
      {
          get 
          { 
              return _captureDDL; 
          }
          set 
          { 
              _captureDDL = value; 
          }
      }
      #endregion
      
      
      #region Implementations of Abstract Methods

      protected override bool IsValidCategory(AuditCategory cat)
      {
         return true;
      }
      
      #endregion

      
   }
}
