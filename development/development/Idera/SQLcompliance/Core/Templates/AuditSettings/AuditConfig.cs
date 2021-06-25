using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Templates.AuditSettings
{
   public enum AccessCheckFilterOption
   {
      CapturePassedCheckActions = AccessCheckFilter.SuccessOnly,
      NoFilter = AccessCheckFilter.NoFilter,
      CaptureFailedCheckActions = AccessCheckFilter.FailureOnly
   }
   
   abstract public class AuditConfig
   {
      #region Data members
        
      Dictionary <AuditCategory, AuditCategory> _categories = new Dictionary <AuditCategory, AuditCategory>();
      AccessCheckFilterOption _checkFilter = AccessCheckFilterOption.NoFilter;
      object syncObj = new object();
      
      #endregion
      
      #region Constructors
            
      #endregion
      
      #region Properties
      
      [XmlElement( "AuditedActivity" )]
      public AuditCategory [] Categories
      {
         get 
         { 
            lock( syncObj )
            {
               List<AuditCategory> cats = new List<AuditCategory>( _categories.Count );
               foreach ( AuditCategory key in _categories.Keys )
               {
                  cats.Add( key );
               }
               return cats.ToArray();
            }
         } 
         set
         {
            if( value != null )
            {
               lock( syncObj )
               {
                  _categories.Clear();
                  for ( int i = 0; i < value.Length; i++ )
                  {
                     if( IsValidCategory( value[i] ))
                        _categories.Add( value[i],
                                         value[i] );
                  }
               }
            }
         }
      }
      
      [XmlElement( "AccessCheckFilter" )]
      public AccessCheckFilterOption AccessCheckFilter
      {
         get
         {
            return _checkFilter;
         }
         
         set
         {
            _checkFilter = value;
         }
      }
      
      #endregion
      
      #region Public Methods
      
      public void AddCategory( AuditCategory cat )
      {
         try
         {
            lock( syncObj )
               _categories.Add( cat,
                                cat );
         }
         catch
         {
         }
      }
      
      public void RemoveCategory( AuditCategory cat )
      {
         try
         {
            lock( syncObj )
               _categories.Remove( cat );
         }
         catch
         {
         }
      }
      
      public virtual void Clear()
      {
         _categories.Clear();
         _checkFilter = AccessCheckFilterOption.NoFilter;
      }

      #endregion
      
      #region Abstract Methods
      
      protected  abstract bool IsValidCategory( AuditCategory cat );
      
      #endregion
   }
}
