using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Idera.SQLcompliance.Core.Triggers
{
   public enum TriggerType : int
   {
      DML = 1, // DML is the only supported type as V 3.1
      DDL
   };
   
   public abstract class TriggerInfo
   {
   
   #region constants
   

   #endregion
   
   #region fields
   
   protected string _instance;
   protected string _database;
   protected string _name;  // Trigger name
   protected TriggerAssemblyInfo _assembly; // Assembly name
   protected DateTime _dateCreated;
   protected bool _exists = false;
   protected TriggerType _type = TriggerType.DML;
   protected string _safeInstanceName;
   protected bool _disabled = false;
   protected TriggerManager _manager = null;
   
   #endregion
   
   #region Constructor
   
   public TriggerInfo (
      string instance,
      string database
      )
   {
      _instance = instance;
      _safeInstanceName = TriggerHelpers.GetSaveInstancename( instance );
      _database = database;
      _assembly = null;
   }

   #endregion
   
   #region Properties
   
   public string Name
   {
      get
      {
         return _name;
      }
   }
   
   public TriggerAssemblyInfo AssemblyInfo
   { 
      get
      {
         return _assembly;
      } 
   }
   
   public DateTime DateCreated
   {
      get
      {
         return _dateCreated;
      }
      set
      {
         _dateCreated = value;
      }
   }
   
   public bool Exists
   {
      get
      {
         return _exists;
      }
   }
   
   public TriggerType Type
   {
      get
      {
         return _type;
      }
   }
   
   public bool Disabled
   {
      get
      {
         return _disabled;
      }
   }
   
   public TriggerManager Manager
   {
      get
      {
         return _manager;
      }
      set
      {
         _manager = value;
         if( _assembly != null )
            _assembly.Manager = _manager;
      }
   }

   #endregion
   
   #region Public Methods
   
   public abstract bool Create( bool recreate);
   public abstract bool Drop();
   
   public bool Create()
   {
      return Create( false );
   }

   #endregion
   
   #region Protected Methods
   
   protected SqlConnection GetConnection()
   {
      if( _manager != null )
         return _manager.GetConnection( _database );
      else
         return TriggerHelpers.GetConnection( _instance, _database );
   }

   #endregion
   }
}
