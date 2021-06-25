using System;
using System.Collections ;
using Idera.SQLcompliance.Core.Event ;

namespace Idera.SQLcompliance.Core.Rules
{
	/// <summary>
	/// Summary description for RulePlan.
	/// </summary>
	public class RulePlan
	{
		#region Member Variables

		private EventRule _rule ;
		private bool _evaluated ;
		private bool _match ;
		private ArrayList _conditionStatusList ;

		#endregion

		#region Properties

		public bool Evaluated
		{
			get { return _evaluated ; }
		}

		public bool Match
		{
			get { return _match ; }
			set 
			{
				_match = value ; 
				_evaluated = true ;
			}
		}

		public EventRule Rule
		{
			get { return _rule ; }
		}

		#endregion

		#region Construction/Destruction

		public RulePlan(EventRule rule)
		{
			if(rule == null)
				throw new ArgumentNullException("rule") ;

			_rule = rule ;
			_conditionStatusList = new ArrayList() ;
		}

		#endregion

		public void AddConditionStatus(ConditionStatus status)
		{
			status.RegisterParent(this) ;
			_conditionStatusList.Add(status) ;
		}

		public void FinalizePlan()
		{
			_conditionStatusList.Sort() ;
		}

		public void Reset()
		{
			_evaluated = false ;
			foreach(ConditionStatus status in _conditionStatusList)
				status.Reset() ;
		}

		public bool EvaluateEvent(EventRecord record)
		{
			if(_evaluated)
			{
				return _match ;
			}

			foreach(ConditionStatus status in _conditionStatusList)
			{
            // Skip conditions that have been evaluated
            if(status.Evaluated && status.Match)
               continue ;
				if(!status.Matches(SupportedEventColumns.LookupColumn(status.ColumnId, record)))
				{
					this.Match = false ;
					return false ;
				}
			}
			this.Match = true ;
			return true ;
		}

		public bool EvaluateEvent(OperationalRecord record)
		{
			throw new NotImplementedException() ;
		}

		public bool EvaluateEvent(ConfigurationRecord record)
		{
			throw new NotImplementedException() ;
		}

		public bool TargetsInstance(string instance)
		{
			string[] instances = _rule.TargetInstances ;

			if(instances.Length == 1 && String.Compare(instances[0], "<ALL>", true) == 0)
				return true ;
			foreach(string s in instances)
				if(String.Compare(s, instance, true) == 0)
					return true ;
			return false ;
		}
	}

	public enum ConditionLink
	{
		None,
		Equal,
		Opposite
	}

	public class ConditionStatus : IComparable
	{
		private ArrayList _parentRulePlans ;
		private ArrayList _oppositeConditions ;
		private EventCondition _condition ;
		private int _columnId ;
		private bool _evaluated ;
		private bool _match ;
		private double _weight ;

		public bool Evaluated
		{
			get { return _evaluated ; }
		}

		public bool Match
		{
			get { return _match ; }
			set 
			{ 
				// Avoid loops from the following logic
				if(_evaluated)
					return ;

				_match = value ; 
            _evaluated = true ;
            // Propogate a false match to parent rules.  
				// One false condition renders the rule a no-match
				if(_match == false)
				{
					foreach(RulePlan plan in _parentRulePlans)
					{
						plan.Match = false ;
					}
				}
				// For opposites conditions will be the opposite
				foreach(ConditionStatus status in _oppositeConditions)
				{
					status.Match = !value ;
				}
			}
		}

		public double Weight
		{
			get { return _weight ; }
			set { _weight = value ; }
		}

		public int ColumnId
		{
			get { return _columnId ; }
		}

		public MatchType ConditionType
		{
			get { return _condition.ConditionType ; }
		}

		public ConditionStatus(EventCondition condition)
		{
			if(condition == null)
				throw new ArgumentNullException("condition") ;
			_parentRulePlans = new ArrayList() ;
			_oppositeConditions = new ArrayList() ;
			_condition = condition ;
			_columnId = SupportedEventColumns.GetColumnId(condition.ParentRule.RuleType, condition.TargetEventField.ColumnName) ;
			switch(condition.ConditionType)
			{
            /*
				case MatchType.Date:
					_weight = 3.0 ;
					break ;
               */
				case MatchType.String:
					_weight = 4.0 ;
					break ;
				case MatchType.Bool:
					_weight = 1.0 ;
					break ;
				case MatchType.Integer:
					_weight = 2.0 ;
					break ;
			}
		}

		public ConditionLink GetConditionLink(EventCondition condition)
		{
			if(EqualLink(condition))
				return ConditionLink.Equal ;
			else if(OppositeLink(condition))
				return ConditionLink.Opposite ;
			else
				return ConditionLink.None ;
		}

		private bool EqualLink(EventCondition condition)
		{
			if(!_condition.FieldId.Equals(condition.FieldId)) return false ;
			if(!Object.Equals(_condition.MatchString, condition.MatchString)) return false ;
			return true ;
		}

		private bool OppositeLink(EventCondition condition)
		{
			if(!_condition.FieldId.Equals(condition.FieldId)) return false ;
			if(_condition.TargetEventField.DataFormat != MatchType.Bool)
				return false ;
			return (_condition.BooleanValue != condition.BooleanValue) ;
		}

		public void RegisterParent(RulePlan parent)
		{
			if(!_parentRulePlans.Contains(parent))
				_parentRulePlans.Add(parent) ;
		}

		public void Reset()
		{
			_evaluated = false ;
		}

		public void AddOppositeCondition(ConditionStatus status)
		{
			_oppositeConditions.Add(status) ;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj == null) return -1 ;

			if (this.GetType() != obj.GetType()) return -1 ;

			ConditionStatus other = (ConditionStatus)obj ;

			return _weight.CompareTo(other.Weight) ;
		}

		#endregion

		public bool Matches(object o)
		{
			bool retVal = false ;

			switch(_condition.TargetEventField.DataFormat)
			{
               /*
				case MatchType.Date:
					if(!(o is DateTime))
						retVal = false ;
					else
						retVal = _condition.Matches((DateTime)o) ;
					break ;
               */
				case MatchType.String:
					if(!(o is String))
						retVal = false ;
					else
						retVal = _condition.Matches(o as String) ;
					break ;
				case MatchType.Bool:
					if(!(o is Boolean))
						retVal = false ;
					else
						retVal = _condition.Matches((Boolean)o) ;
					break ;
				case MatchType.Integer:
					if(!(o is Int32))
						retVal = false ;
					else
						retVal = _condition.Matches((Int32)o) ;
					break ;
			}

			this.Match = retVal ;
			return retVal ;
		}
	}
}
