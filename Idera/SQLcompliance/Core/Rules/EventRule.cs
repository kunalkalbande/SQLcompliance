using System;
using System.Collections;
using System.Xml.Serialization;

namespace Idera.SQLcompliance.Core.Rules
{
   public enum EventType
   {
      SqlServer = 1,
      Status,
      Data
   }

	/// <summary>
	/// Summary description for EventRule.
	/// </summary>
	public class EventRule
	{
      #region Member Variables

      public const int NULL_ID = -2100000001;

      // Database columns
      protected int _id ;
      protected string _name ;
      protected string _description ;
      protected EventType _type ;
      protected string[] _targetInstances ;
      protected bool _enabled ;

      protected Hashtable _conditionMap ;
      protected ArrayList _removedConditions ;

      protected bool _isDirty ;

      #endregion

      #region Properties

      [XmlIgnore]
      public int Id
      {
         get { return _id ; }
         set { _id = value ; }
      }

      [XmlAttribute("name")]
      public string Name
      {
         get { return _name ; }
         set 
         { 
            _isDirty = true ;
            _name = value ; 
         }
      }

      [XmlAttribute("description")]
      public string Description
      {
         get { return _description ; }
         set 
         { 
            _isDirty = true ;
            _description = value ; 
         }
      }

      [XmlAttribute("ruleType")]
      public EventType RuleType
      {
         get { return _type; }
         set 
         { 
            _isDirty = true ;
            _type = value; 
         }
      }

      [XmlElement("TargetInstance")]
      public string[] TargetInstances
      {
         get { return (string[])_targetInstances.Clone() ; }
         set 
         { 
            _isDirty = true ;
            _targetInstances = value ; 
         }
      }

      [XmlIgnore]
      public string TargetInstanceList
      {
         get 
         {
            return String.Join(";", _targetInstances);
         }
         set 
         { 
            _isDirty = true ;
            if(value != null && value.Trim().Length > 0)
            {
               //If we don't find the semicolon, it is an old filter.
               if (value.IndexOf(';') == -1)
                  _targetInstances = value.Split(new char[] { ',' });
               else
                  _targetInstances = value.Split(new char[] { ';' });
            }
            else
               _targetInstances = new string[0] ;
         }
      }

      [XmlElement("Enabled")]
      public bool Enabled
      {
         get { return _enabled ; }
         set 
         { 
            _isDirty = true ;
            _enabled = value ; 
         }
      }

      [XmlElement("Condition")]
      public EventCondition[] Conditions
      {
         get 
         { 
            EventCondition[] retVal = new EventCondition[_conditionMap.Count] ;
            int i = 0 ; 
            foreach(EventCondition condition in _conditionMap.Values)
               retVal[i++] = condition ;
            return retVal ;
         }
         set // Need this for XML serialization
         {
            if( value == null )
               return;
            foreach( EventCondition condition in value )
               AddCondition( condition );
         }
      }

      [XmlElement("DeleteCondition")]
      public EventCondition[] RemovedConditions
      {
         get 
         {
            EventCondition[] retVal = new EventCondition[_removedConditions.Count] ;

            for(int i = 0 ; i < _removedConditions.Count ; i++)
               retVal[i] = (EventCondition)_removedConditions[i] ;
            return retVal ;
         }
      }

      [XmlIgnore]
      public bool IsValid
      {
         get 
         {
            if(_targetInstances.Length == 0)
               return false ;
            if(_conditionMap.Count == 0)
               return false ;
            foreach(EventCondition condition in _conditionMap.Values)
               if(!condition.IsValid)
                  return false ;
            return true ;
         }
      }

      [XmlIgnore]
      public bool Dirty
      {
         get { return _isDirty ; }
         set 
         { 
            _isDirty = value ; 
            if(!_isDirty)
            {
               foreach(EventCondition condition in _conditionMap.Values)
                  condition.Dirty = false ;
            }
         }
      }
      
      [XmlElement]
      public bool HasConditions
      {
         get { return _conditionMap.Count > 0; }
      }

      [XmlIgnore]
      public bool HasTargetInstances
      {
         get { return _targetInstances.Length > 0 ; }
      }

      #endregion

      #region Construction/Destruction

      public EventRule()
      {
         _id = NULL_ID ;
         _name = "New Rule" ;
         _description = "" ;
         _enabled = false ;
         _type = EventType.SqlServer ;
         _targetInstances = new string[] {"<ALL>"} ;
         _conditionMap = new Hashtable() ;
         _removedConditions = new ArrayList() ;
         _isDirty = false ;
      }

      public EventRule Clone()
      {
         EventRule retVal = (EventRule)MemberwiseClone() ;

         if(_targetInstances != null)
         {
            retVal._targetInstances = new string[_targetInstances.Length] ;
            for(int i = 0 ; i < _targetInstances.Length ; i++)
               retVal._targetInstances[i] = _targetInstances[i] ;
         }
         else
            retVal._targetInstances = new string[0] ;

         retVal._removedConditions = new ArrayList() ;
         retVal._conditionMap = new Hashtable() ;

         foreach(EventCondition condition in _conditionMap.Values)
            retVal.AddCondition(condition.Clone()) ;

         return retVal ;
      }

      #endregion

      public bool HasCondition(string fieldName)
      {
         return _conditionMap.ContainsKey(fieldName) ;
      }

      public EventCondition GetCondition(string fieldName)
      {
         return (EventCondition)_conditionMap[fieldName] ;
      }

      public void AddCondition(EventCondition condition)
      {
         condition.Dirty = true ;
         condition.ParentRule = this ;
         _conditionMap[condition.TargetEventField.ColumnName] = condition ;
      }

      public void RemoveCondition(string fieldName)
      {
         EventCondition condition = (EventCondition)_conditionMap[fieldName] ;

         if(condition != null && condition.Id != NULL_ID)
            _removedConditions.Add(condition) ;
         _conditionMap.Remove(fieldName) ;
      }

      public void RemoveAllConditions()
      {
         foreach(EventCondition condition in _conditionMap.Values)
         {
            if(condition.Id != NULL_ID)
               _removedConditions.Add(condition) ;
         }
         _conditionMap.Clear() ;
      }

      public void ClearAllRemovedCondiions()
      {
          _removedConditions.Clear();
      }

      public override bool Equals(object obj)
      {
         if (obj == null) return false;

         if (this.GetType() != obj.GetType()) return false;

         // safe because of the GetType check
         EventRule rule = (EventRule)obj;     

         // use this pattern to compare value members
         if(!_id.Equals(rule._id)) return false ;
         if(!_enabled.Equals(rule._enabled)) return false ;
         if(!_type.Equals(rule._type)) return false ;

         // use this pattern to compare reference members
         if(!Object.Equals(_name, rule._name)) return false ;
         if(!Object.Equals(_description, rule._description)) return false ;

         // Target Instances
         if(!_targetInstances.Length.Equals(rule._targetInstances.Length)) return false ;
         ArrayList instances = new ArrayList(_targetInstances) ;
         foreach(string s in rule._targetInstances)
         {
            if(instances.Contains(s))
               instances.Remove(s) ;
            else
               return false ;
         }

         // Conditions
         if(!_conditionMap.Count.Equals(rule._conditionMap.Count)) return false ;
         foreach(EventCondition condition in _conditionMap.Values)
         {
            EventCondition condition2 = (EventCondition)rule._conditionMap[condition.TargetEventField.ColumnName] ;

            if(!Object.Equals(condition, condition2)) return false ;
         }

         return true;
      }

      public override int GetHashCode()
      {
         return _id ;
      }

      public bool TargetsInstance(string instance)
      {
         if(_targetInstances.Length == 1 && String.Compare(_targetInstances[0], "<ALL>", true) == 0)
            return true ;
         foreach(string s in _targetInstances)
            if(String.Compare(s, instance, true) == 0)
               return true ;
         return false ;
      }

   }
}
