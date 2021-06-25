using System.Collections.Generic;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Templates.AuditSettings
{
   public enum AuditedObjectTypes
   {
      UserTables = 1,
      SystemTables,
      StoredProcedures,
      AllOtherTypes
   }
   
   public class DBAuditConfig : AuditConfig
   {
      #region Data members
      
      bool _keepSQL = false;
      bool _auditTrans = false;
        bool _auditDDL = false;
      string _dbname = "";
      bool _auditAllUserTables = true;
      bool _auditAllTypes = true;
      Dictionary<string, string> _auditedTables = new Dictionary<string, string>( );
      Dictionary< AuditedObjectTypes, AuditedObjectTypes> _auditedTypes 
                     = new Dictionary< AuditedObjectTypes, AuditedObjectTypes>();
      UserList _userList = null;
      List<DataChangeTableConfig> _dataChangeTables = new List<DataChangeTableConfig>();
      List<SensitiveColumnTableConfig> _sensitiveColumnTables = new List<SensitiveColumnTableConfig>();
        UserAuditConfig _userConfig = null;
         #endregion
      
      #region Constructors
      

      #endregion
      
      #region Properties
      
      [XmlAttribute( "DatabaseName" )]
      public string Database
      {
         get
         {
            return _dbname;
         }
         set
         {
            _dbname = value;
         }
      }
      
      [XmlElement( "CaptureSQL" )]
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

      [XmlElement("CaptureTransactions")]
      public bool AuditTrans
      {
         get { return _auditTrans; }
         set { _auditTrans = value; }
      }
      
        [XmlElement("CaptureDDL")]
        public bool AuditDDL
        {
            get 
            { 
                return _auditDDL; 
            }
            set 
            { 
                _auditDDL = value; 
            }
        }

      [XmlElement( "AuditAllObjectTypes" )]
      public bool AuditAllObjects
      {
         get
         {
            return _auditAllTypes;
         }
         
         set
         {
            _auditAllTypes = value;
            if( value )
            {
               AddAllTypes();
            }
         }
      }
      
      [XmlElement("AuditAllUserTables" )]
      public bool AuditAllUserTables
      {
         get
         {
            return _auditAllUserTables;
         }
         set
         {
            _auditAllUserTables = value;
            if ( value )
            {
               _auditedTables.Clear();
            }
            else
            {
               _auditAllTypes = false;
            }
         }
      }
      
      [XmlElement("AuditedUserTable")]
      public string [] AuditedTables
      {
         get
         {
            List<string> tables = new List<string>( _auditedTables.Count );
            tables.AddRange( _auditedTables.Keys );
            return tables.ToArray();
         }
         set
         {
            _auditedTables.Clear();
            if( value == null || value.Length == 0 )
            {
               _auditAllUserTables = true;
               return;
            }
            
            AuditAllUserTables = false;
                        
            for ( int i = 0; i < value.Length; i++ )
            {
               try
               {
                  _auditedTables.Add( value[i],
                                      value[i] );
               }
               catch
               {
               }
            }
         }
      }
      
      [XmlElement( "AuditedObjectType")]
      public AuditedObjectTypes [] AuditedTypes
      {
         get
         {
            List<AuditedObjectTypes> types = new List<AuditedObjectTypes>( _auditedTypes.Count );
            types.AddRange( _auditedTypes.Keys );
            return types.ToArray();
         }
         
         set
         {
            if( value.Length < 4 )
               _auditAllTypes = false;
               
            _auditedTypes.Clear();
            foreach ( AuditedObjectTypes type in value )
            {
               try
               {
                  _auditedTypes.Add(type,
                                    type);
               }
               catch
               {
               }
            }
         }
      }
      
      [XmlElement("TrustedUserList")]
      public UserList TrustedUserList 
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

      [XmlElement("DataChangeTable")]
      public DataChangeTableConfig[] DataChangeTables
      {
         get { return _dataChangeTables.ToArray(); }
         set 
         { 
            _dataChangeTables = new List<DataChangeTableConfig>();
            if (value != null)
               _dataChangeTables.AddRange(value);
         }
      }

      [XmlElement("SensitiveColumnTable")]
      public SensitiveColumnTableConfig[] SensitiveColumnTables
      {
         get { return _sensitiveColumnTables.ToArray(); }
         set
         {
            _sensitiveColumnTables = new List<SensitiveColumnTableConfig>();
            if (value != null)
               _sensitiveColumnTables.AddRange(value);
         }
      }

        [XmlElement("PrivilegedUserAuditConfig")]
        public UserAuditConfig PrivUserConfig
        {
            get
            {
                return _userConfig;
            }
            set
            {
                _userConfig = value;
            }
        }
      
      #endregion
      
      #region Public Methods

      public void SetDataChangeTables(TableConfiguration[] tables)
      {
         if (tables == null)
            return;
         foreach (TableConfiguration t in tables)
         {
            DataChangeTableConfig item = new DataChangeTableConfig() ;
            item.TableName = t.GetFullName();
            item.RowCount = t.MaxRows;
            item.Columns = t.Columns;
            _dataChangeTables.Add(item);
         }
      }

      public void SetSensitiveColumnTables(TableConfiguration[] tables)
      {
         if (tables == null)
            return;

         foreach (TableConfiguration t in tables)
         {
            SensitiveColumnTableConfig item = new SensitiveColumnTableConfig();
            item.TableName = t.GetFullName();
            item.Columns = t.Columns;
            item.Type = t.Type;
            _sensitiveColumnTables.Add(item);
         }
      }

      public void AddAuditedType( AuditedObjectTypes type )
      {
         if(!_auditedTypes.ContainsKey(type))
            _auditedTypes.Add( type, type );
      }

      public override void Clear()
      {
         AuditAllObjects = true;
         KeepSQL = false;
         AuditTrans = false;
            AuditDDL = false;
         _userList = null;
         base.Clear();
      }

      #endregion
      
      #region Private Methods
      
      void AddAllTypes()
      {
         _auditedTypes.Clear();
         _auditedTypes.Add( AuditedObjectTypes.UserTables,
                            AuditedObjectTypes.UserTables );
         _auditedTypes.Add( AuditedObjectTypes.SystemTables,
                            AuditedObjectTypes.SystemTables );
         _auditedTypes.Add( AuditedObjectTypes.StoredProcedures,
                            AuditedObjectTypes.StoredProcedures );
         _auditedTypes.Add( AuditedObjectTypes.AllOtherTypes,
                            AuditedObjectTypes.AllOtherTypes );

         _auditAllUserTables = true;
         _auditedTables.Clear();
      }

      #endregion
      
      #region Implementations of Abstract Methods

      protected override bool IsValidCategory(AuditCategory cat)
      {
         switch (cat)
         {
            case AuditCategory.UDC:
            case AuditCategory.FailedLogins:
            case AuditCategory.Logins:
            case AuditCategory.Logouts:
               return false;

            default:
               return true;
         }
      }
      #endregion
   }

   public class DataChangeTableConfig
   {
      private string _tableName ;
      private int _rowCount ;
      private string[] _columns;


      [XmlElement("TableName")]
      public string TableName
      {
         get { return _tableName; }
         set { _tableName = value; }
      }

      [XmlElement("RowCount")]
      public int RowCount
      {
         get { return _rowCount; }
         set { _rowCount = value; }
      }

      [XmlElement("Columns")]
      public string[] Columns
      {
         get { return _columns == null ? new string[0] : _columns; }
         set { _columns = value; }
      }
   }

   public class SensitiveColumnTableConfig
   {
      private string _tableName;
      private int _rowCount;
      private string[] _columns;
      private string _type;


      [XmlElement("TableName")]
      public string TableName
      {
         get { return _tableName; }
         set { _tableName = value; }
      }

      [XmlElement("Columns")]
      public string[] Columns
      {
         get { return _columns == null ? new string[0] : _columns; }
         set { _columns = value; }
      }

      [XmlElement("Type")]
      public string Type
      {
          get { return _type; }
          set { _type = value; }
      }

   }
}
