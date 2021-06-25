using System;
using System.Collections ;

namespace Idera.SQLcompliance.Core.Rules.Filters
{
	/// <summary>
	/// Summary description for FilterProcessor.
	/// </summary>
	public class FilterProcessor
	{/*
      #region Member Variables

      private ArrayList _conditionStatusList ; // master list
      private ArrayList _rulePlans ;
      private EventType _ruleType ;

      #endregion

      #region Properties

      #endregion

      #region Construction/Destruction

      public FilterProcessor()
      {
         _conditionStatusList = new ArrayList() ;
         _rulePlans = new ArrayList() ;
      }

      public void Initialize(ArrayList filters, EventType ruleType)
      {
         ConditionStatus relatedCondition ;
         ConditionLink link ;
         _ruleType = ruleType ;
         _conditionStatusList.Clear() ;
         _rulePlans.Clear() ;

         foreach(AlertRule rule in rules)
         {
            if(rule.RuleType != _ruleType || !rule.Enabled || !rule.IsValid)
               continue ;

            RulePlan plan = new RulePlan(rule) ;
            relatedCondition = null ;

            foreach(EventCondition condition in rule.Conditions)
            {
               link = ConditionLink.None ;
               foreach(ConditionStatus status in _conditionStatusList)
               {
                  link = status.GetConditionLink(condition) ;
                  if(link != ConditionLink.None)
                  {
                     relatedCondition = status ;
                     break ;
                  }
               }
               if(relatedCondition != null)
               {
                  if(link == ConditionLink.Equal)
                     plan.AddConditionStatus(relatedCondition) ;
                  else if(link == ConditionLink.Opposite)
                  {
                     ConditionStatus status = new ConditionStatus(condition) ;
                     _conditionStatusList.Add(status) ;
                     status.AddOppositeCondition(relatedCondition) ;
                     relatedCondition.AddOppositeCondition(status) ;
                     plan.AddConditionStatus(status) ;
                  }
                  else if(link == ConditionLink.Superset)
                  {
                  }
               }
               else
               {
                  ConditionStatus status = new ConditionStatus(condition) ;
                  _conditionStatusList.Add(status) ;
                  plan.AddConditionStatus(status) ;
               }
            }
            plan.FinalizePlan() ;
            _rulePlans.Add(plan) ;
         }
      }

      #endregion

      private void ResetRules(ArrayList plans)
      {
         if(plans == null)
            plans = _rulePlans ;
         foreach(RulePlan plan in plans)
            plan.Reset() ;
      }

      public AlertCollection GenerateAlerts(string sourceInstance, EventRecord[] records)
      {
         AlertCollection retVal = new AlertCollection() ;
         ArrayList plans = new ArrayList() ;

         if(_ruleType != EventType.SqlServer)
            return retVal ;

         foreach(RulePlan plan in _rulePlans)
            if(plan.Rule.Enabled && plan.TargetsInstance(sourceInstance))
               plans.Add(plan) ;
            
         foreach(EventRecord record in records)
         {
            ResetRules(plans) ;
            foreach(RulePlan plan in plans)
            {
               if(plan.EvaluateEvent(record))
                  retVal.Add(GenerateAlert(plan.Rule, sourceInstance, record)) ;
            }
         }

         return retVal ;
      }

      public AlertCollection GenerateAlerts(OperationalRecord record)
      {
         throw new NotImplementedException() ;
      }

      public AlertCollection GenerateAlerts(ConfigurationRecord record)
      {
         throw new NotImplementedException() ;
      }

      private Alert GenerateAlert(AlertRule rule, string sourceInstance, EventRecord record)
      {
         Alert retVal = new Alert() ;

         retVal.AlertType = rule.RuleType ;
         retVal.ParentRule = rule ;
         retVal.Instance = sourceInstance ;
         retVal.EventId = record.eventId ;
         retVal.Level = rule.Level ;
         retVal.EventType = (int)record.eventType ;
         retVal.EventData = record ;
         retVal.MessageData = rule.MessageData ;
         if(rule.HasLogAction)
            retVal.LogStatus = NotificationStatus.Pending ;
         if(rule.HasEmailAction)
            retVal.EmailStatus = NotificationStatus.Pending ;
         retVal.RuleName = rule.Name ;

         return retVal ;
      }*/
   }
}
