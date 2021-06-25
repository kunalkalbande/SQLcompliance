using System;
using System.Collections ;
using System.Collections.Generic;
using System.Data ;
using Idera.SQLcompliance.Core.Event ;
using Idera.SQLcompliance.Core.Rules.Alerts ;
using Idera.SQLcompliance.Core.TraceProcessing ;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core.Rules.Filters;

namespace Idera.SQLcompliance.Core.Rules
{

	public class SupportedEventColumns
	{
		public const int Unknown = 0 ; 

		public const int Sql_StartTime = 1 ;
		public const int Sql_EventType = 2 ;
		public const int Sql_EventClass = 3 ;
		public const int Sql_EventSubclass = 4 ;
		public const int Sql_Spid = 5 ;
		public const int Sql_ApplicationName = 6 ;
		public const int Sql_HostName = 7 ;
		public const int Sql_ServerName = 8 ;
		public const int Sql_LoginName = 9 ;
		public const int Sql_Success = 10 ;
		public const int Sql_DatabaseName = 11 ;
		public const int Sql_DatabaseId = 12 ;
		public const int Sql_DbUserName = 13 ;
		public const int Sql_ObjectType = 14 ;
		public const int Sql_ObjectName = 15 ;
		public const int Sql_ObjectId = 16 ;
		public const int Sql_Permissions = 17 ;
		public const int Sql_ColumnPermissions = 18 ;
		public const int Sql_TargetLoginName = 19 ;
		public const int Sql_TargetUserName = 20 ;
		public const int Sql_RoleName = 21 ;
		public const int Sql_OwnerName = 22 ;
		public const int Sql_TargetObject = 23 ;
		public const int Sql_Details = 24 ;
		public const int Sql_EventCategory = 25 ;
		public const int Sql_PrivilegedUser = 26 ;
      public const int Sql_SessionLogin = 27;
        public const int Sql_PrivilegedUsers = 28;
        public const int Sql_Rowcount = 29;

		public static int GetColumnId(EventType alertType, string columnName)
		{
         /*
			if(alertType == AlertType.Configuration)
			{
			}
			else if(alertType == AlertType.Operational)
			{
			}
			else*/
			{
				switch(columnName)
				{
					case "startTime":
						return Sql_StartTime ;
					case "eventType":
						return Sql_EventType ;
					case "eventClass":
						return Sql_EventClass ;
					case "eventSubclass":
						return Sql_EventSubclass ;
					case "spid":
						return Sql_Spid ;
					case "applicationName":
						return Sql_ApplicationName ;
					case "hostName":
						return Sql_HostName ;
					case "serverName":
						return Sql_ServerName ;
					case "loginName":
						return Sql_LoginName ;
					case "success":
						return Sql_Success ;
					case "databaseName":
						return Sql_DatabaseName ;
					case "databaseId":
						return Sql_DatabaseId ;
					case "dbUserName":
						return Sql_DbUserName ;
					case "objectType":
						return Sql_ObjectType ;
					case "objectName":
						return Sql_ObjectName ;
					case "objectId":
						return Sql_ObjectId ;
					case "permissions":
						return Sql_Permissions ;
					case "columnPermissions":
						return Sql_ColumnPermissions ;
					case "targetLoginName":
						return Sql_TargetLoginName ;
					case "targetUserName":
						return Sql_TargetUserName ;
					case "roleName":
						return Sql_RoleName ;
					case "ownerName":
						return Sql_OwnerName ;
					case "targetObject":
						return Sql_TargetObject ;
					case "details":
						return Sql_Details ;
					case "eventCategory":
						return Sql_EventCategory ;
					case "privilegedUser":
						return Sql_PrivilegedUser ;
               case "sessionLoginName":
                  return Sql_SessionLogin;
                    case "privilegedUsers":
                        return Sql_PrivilegedUsers;
                    case "rowcount":
                        return Sql_Rowcount;
				}
			}
			return Unknown ;
		}

		public static object LookupColumn(int columnId, ConfigurationRecord record)
		{
			throw new NotImplementedException() ;
		}

		public static object LookupColumn(int columnId, OperationalRecord record)
		{
			throw new NotImplementedException() ;
		}

		public static object LookupColumn(int columnId, EventRecord record)
		{
			switch(columnId)
			{
				case SupportedEventColumns.Sql_StartTime:
					return record.startTime ;
				case SupportedEventColumns.Sql_EventType:
					return (Int32)record.eventType ;
				case SupportedEventColumns.Sql_EventClass:
					return record.eventClass ;
				case SupportedEventColumns.Sql_EventSubclass:
					return record.eventSubclass ;
				case SupportedEventColumns.Sql_Spid:
					return record.spid ;
				case SupportedEventColumns.Sql_ApplicationName:
					return record.applicationName ;
				case SupportedEventColumns.Sql_HostName:
					return record.hostName ;
				case SupportedEventColumns.Sql_ServerName:
					return record.serverName ;
				case SupportedEventColumns.Sql_LoginName:
					return record.loginName ;
				case SupportedEventColumns.Sql_Success:
					return (record.success != 0) ;
				case SupportedEventColumns.Sql_DatabaseName:
					return record.databaseName ;
				case SupportedEventColumns.Sql_DatabaseId:
					return record.databaseId ;
				case SupportedEventColumns.Sql_DbUserName:
					return record.dbUserName ;
				case SupportedEventColumns.Sql_ObjectType:
					return record.objectType ;
				case SupportedEventColumns.Sql_ObjectName:
					return record.objectName ;
				case SupportedEventColumns.Sql_ObjectId:
					return record.objectId ;
				case SupportedEventColumns.Sql_Permissions:
					return record.permissions ;
				case SupportedEventColumns.Sql_ColumnPermissions:
					return record.columnPermissions ;
				case SupportedEventColumns.Sql_TargetLoginName:
					return record.targetLoginName ;
				case SupportedEventColumns.Sql_TargetUserName:
					return record.targetUserName ;
				case SupportedEventColumns.Sql_RoleName:
					return record.roleName ;
				case SupportedEventColumns.Sql_OwnerName:
					return record.ownerName ;
				case SupportedEventColumns.Sql_TargetObject:
					return record.targetObject ;
				case SupportedEventColumns.Sql_Details:
					return record.details ;
				case SupportedEventColumns.Sql_EventCategory:
					return (int)record.eventCategory ;
				case SupportedEventColumns.Sql_PrivilegedUser:
					return (record.privilegedUser != 0) ;
            case SupportedEventColumns.Sql_SessionLogin:
               return record.sessionLoginName;
                case SupportedEventColumns.Sql_PrivilegedUsers:
                    return record.privilegedUsers;
                case SupportedEventColumns.Sql_Rowcount:
                    return record.rowcount;
			}
			return null ;
		}
	} ;

   public class RuleProcessor
   {
      #region Member Variables

      private string _name ;
      private string _instance ;
      private EventRule[] _rules ;  // All Rules
      private EventCondition[] _conditions ;  // All conditions
      private EventCondition[] _finalConditions ;  // Minimal coverage list of conditions
      private int[] _finalColumnIds ;  // The column id for each corresponding condition
      private int[] _oppositeConditions ;  // List of opposite conditions
      private Hashtable _mapConditionIdToRuleIds ;  // conditionId to ArrayList or ruleId (indexes)
      private Hashtable _mapRuleIdToConditionIds ;  // ruleId to ArrayList of conditionId (indexes)

      // State Variables - these must be reallocated in a clone to avoid threading issues
      private bool[] _conditionEvaluated ;  // Has a condition been evaluated
      private bool[] _conditionMatched ;  //  If evaluated, is it a match for the event
      private int[] _rulesRemaining ; // The number of rules that remain for processing for this condition
      private int[] _conditionsRemaining ; // the number of conditions taht remain for a rule to be fully evaluated
      private ArrayList _matchedRules ; // The rules that have been matched by the current EventRecord

      #endregion

      #region Properties

      public bool ContainsRules
      {
         get { return _finalConditions.Length > 0 ; }
      }

      #endregion

      #region Construction/Destruction

      public RuleProcessor(string sName, string targetInstance, IList rules)
      {
         _name = String.Format("{0} for {1}", sName, targetInstance) ;
         _instance = targetInstance ;
         _mapConditionIdToRuleIds = new Hashtable() ;
         _mapRuleIdToConditionIds = new Hashtable() ;
         _matchedRules = new ArrayList() ;
         ArrayList conditions = new ArrayList() ;
         ArrayList finalConditions = new ArrayList() ;
         ArrayList oppositeConditions = new ArrayList() ;
         ArrayList tempList ;

         // Save the original rules so we can reference them as needed
         //  But we only want to save the ones we intend to use
         tempList = new ArrayList() ;
         foreach(EventRule rule in rules)
         {
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                DayOfWeek currentDay = DateTime.Now.DayOfWeek;
                bool isRuleActiveToday = false;
                
                if (rule.IsValid && rule.Enabled && rule.TargetsInstance(targetInstance))
                {
                    tempList.Add(rule);
                    if (rule.GetType() != typeof(EventFilter) && ((AlertRule)rule).IsAlertRuleTimeFrameEnabled)
                    {
                        foreach(char ch in ((AlertRule)rule).AlertRuleTimeFrameDaysOfWeek)
                        {
                            switch (ch)
                            {
                                case '1':
                                    if(currentDay == DayOfWeek.Sunday)
                                        isRuleActiveToday = true;
                                    break;
                                case '2':
                                    if (currentDay == DayOfWeek.Monday)
                                        isRuleActiveToday = true;
                                    break;
                                case '3':
                                    if (currentDay == DayOfWeek.Tuesday)
                                        isRuleActiveToday = true;
                                    break;
                                case '4':
                                    if (currentDay == DayOfWeek.Wednesday)
                                        isRuleActiveToday = true;
                                    break;
                                case '5':
                                    if (currentDay == DayOfWeek.Thursday)
                                        isRuleActiveToday = true;
                                    break;
                                case '6':
                                    if (currentDay == DayOfWeek.Friday)
                                        isRuleActiveToday = true;
                                    break;
                                case '7':
                                    if (currentDay == DayOfWeek.Saturday)
                                        isRuleActiveToday = true;
                                    break;
                            }

                            if(isRuleActiveToday)
                                break;
                        }
                        if (!isRuleActiveToday)
                        {
                            tempList.Remove(rule);
                        }
                        else
                        {
                            if (Convert.ToDateTime(currentTime) < Convert.ToDateTime(((AlertRule)rule).AlertRuleTimeFrameStartTime)
                                ||
                                Convert.ToDateTime(currentTime) > Convert.ToDateTime(((AlertRule)rule).AlertRuleTimeFrameEndTime))
                            {
                                tempList.Remove(rule);
                            }
                        }
                    }
                }
         }
         _rules = new EventRule[tempList.Count] ;
         //_evaluatedRules = new bool[rulesArray.Count] ;
         for(int i = 0 ; i < tempList.Count ; i++)
         {
            _rules[i] = (EventRule)tempList[i] ;
            conditions.AddRange(_rules[i].Conditions) ;
         }

         // Sort the conditions by type for efficiency
         conditions.Sort(new ConditionComparer());

         // The ruleId is simply its index in the array.
         //  Iterate through the conditions in order and build minimal coverage conditions list.
         //  To accomplish this, we remove duplicate conditions and link up opposite conditions
         //  so that you only perform an evaluation once.
         // Check each condition to see if it is linked (equal or opposite)
         //  to a condition we have already seen and processed.
         foreach(EventCondition condition in conditions)
         {
            int ruleId = GetRuleId(condition) ;
            int relatedConditionId = -1 ;
            int conditionId = -1 ;
            ConditionLink link = ConditionLink.None ;

            // Check the already processed conditions to see if there
            //  is a related condition
            for(int i = 0 ; i < finalConditions.Count ; i++)
            {
               ConditionLink tempLink = GetConditionLink(condition, (EventCondition)finalConditions[i]) ;
               if(tempLink == ConditionLink.Equal)
               {
                  // Equal we can break asap
                  relatedConditionId = i ;
                  link = ConditionLink.Equal ;
                  break ;
               }
               else if(tempLink == ConditionLink.Opposite)
               {
                  // Opposite we store state; however, we may find a subsequent equal
                  //  and that will take precedent.
                  relatedConditionId = i ;
                  link = ConditionLink.Opposite ;
               }
            }
            // None - the condition will be used in rule processing.
            // Equal - use the existing condition
            // Opposite - use the new condition, but mark the link so the actual
            //   condition will only be processed once.  After processing, the opposite
            //   condition will be marked accordingly.
            switch(link)
            {
               case ConditionLink.None:
                  conditionId = finalConditions.Add(condition) ;
                  oppositeConditions.Add(-1) ;
                  break ;
               case ConditionLink.Equal:
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, _name,
                     String.Format("Equal Conditions:  {0},{1}", condition.Id, 
                     ((EventCondition)finalConditions[relatedConditionId]).Id)) ;
                  conditionId = relatedConditionId ;
                  oppositeConditions.Add(-1) ;
                  break ;
               case ConditionLink.Opposite:
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug, _name,
                     String.Format("Opposite Conditions:  {0},{1}", condition.Id, 
                     ((EventCondition)finalConditions[relatedConditionId]).Id)) ;
                  conditionId = finalConditions.Add(condition) ;
                  // On Opposite conditions, you need to make sure both are
                  //  aware of each other.  This allows for any order of condition
                  //  evaluation to take advantage of the information
                  oppositeConditions.Add(relatedConditionId) ;
                  oppositeConditions[relatedConditionId] = conditionId ;
                  break ;
            }
            // Maintain the parent rules that use each condition.  With sharing,
            //  a condition may be used by several rules.  We need to be aware
            //  of when all rules have been evaluated (this can happen prior to all
            //  conditions being evaluated).
            tempList = (ArrayList)_mapConditionIdToRuleIds[conditionId] ;
            if(tempList == null)
            {
               tempList = new ArrayList() ;
               _mapConditionIdToRuleIds[conditionId] = tempList ;
            }
            if(!tempList.Contains(ruleId))
               tempList.Add(ruleId) ;

            // Maintain the conditions that correspond to a rule
            tempList = (ArrayList)_mapRuleIdToConditionIds[ruleId] ;
            if(tempList == null)
            {
               tempList = new ArrayList() ;
               _mapRuleIdToConditionIds[ruleId] = tempList ;
            }
            if(!tempList.Contains(conditionId))
               tempList.Add(conditionId) ;
         }
         // Now that all condition processing is complete, organize the data into
         //  the final structures to be used during event processings.
         _conditions = new EventCondition[conditions.Count] ;
         _oppositeConditions = new int[finalConditions.Count] ;
         _finalColumnIds = new int[finalConditions.Count] ;
         _finalConditions = new EventCondition[finalConditions.Count] ;
         // State variables.  Just need sizes
         _conditionEvaluated = new bool[finalConditions.Count] ;
         _conditionMatched = new bool[finalConditions.Count] ;
         _rulesRemaining = new int[finalConditions.Count] ;
         _conditionsRemaining = new int[rules.Count] ;

         // Build master condition array
         for(int i = 0 ; i < conditions.Count ; i++)
            _conditions[i] = (EventCondition)conditions[i] ;

         // Build final condition array - these are the conditions that are actually evaluated
         for(int i = 0 ; i < finalConditions.Count ; i++)
         {
            _finalConditions[i] = (EventCondition)finalConditions[i] ;
            _oppositeConditions[i] = (int)oppositeConditions[i] ;
            _finalColumnIds[i] = SupportedEventColumns.GetColumnId(_finalConditions[i].ParentRule.RuleType, _finalConditions[i].TargetEventField.ColumnName) ;
         }
         // Debug Info
         if(ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
         {
            string sMessage = "" ;
            string[] tempString = new string[_rules.Length];

            for(int i = 0 ; i < _rules.Length ; i++)
               tempString[i] = _rules[i].Id.ToString() ;

            sMessage += String.Format("{0} Rules Included:  {1}\n", _rules.Length, String.Join(",", tempString)) ;
            sMessage += String.Format("{0} conditions total\n", _conditions.Length) ;
            sMessage += String.Format("{0} conditions actually evaluated\n", _finalConditions.Length) ;
            int oppositeCount = 0 ;
            for(int i = 0 ; i < _oppositeConditions.Length ; i++)
               if(_oppositeConditions[i] != -1)
                  oppositeCount++ ;
            sMessage += String.Format("{0} opposite conditions total\n", oppositeCount) ;
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, _name, sMessage, ErrorLog.Severity.Warning) ;
         }
      }

      #endregion

      public RuleProcessor Clone()
      {
         RuleProcessor clone = (RuleProcessor)MemberwiseClone() ;

         clone._conditionEvaluated = new bool[_finalConditions.Length] ;
         clone._conditionMatched = new bool[_finalConditions.Length] ;
         clone._rulesRemaining = new int[_finalConditions.Length] ;
         clone._conditionsRemaining = new int[_rules.Length] ;
         clone._matchedRules = new ArrayList() ;

         return clone ;
      }

      private ConditionLink GetConditionLink(EventCondition lhs, EventCondition rhs)
      {
         if(EqualLink(lhs, rhs))
            return ConditionLink.Equal ;
         else if(OppositeLink(lhs, rhs))
            return ConditionLink.Opposite ;
         else
            return ConditionLink.None ;
      }

      private bool EqualLink(EventCondition lhs, EventCondition rhs)
      {
         if(!lhs.FieldId.Equals(rhs.FieldId)) return false ;
         if(!Object.Equals(lhs.MatchString, rhs.MatchString)) return false ;
         return true ;
      }

        /// <summary>
        /// Verifies that the connection object is not null and that the connection is
        /// open.
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="ArgumentNullException">Thrown when connection is null</exception>
        /// <exception cref="Exception">Thrown when the connection is not open.</exception>
        private static void ValidateConnection(SqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (connection.State != ConnectionState.Open)
                throw new Exception("SqlConnection object is not opened.");
        }
      private bool OppositeLink(EventCondition lhs, EventCondition rhs)
      {
         if(!lhs.FieldId.Equals(rhs.FieldId)) return false ;
         if(lhs.TargetEventField.DataFormat != MatchType.Bool)
            return false ;
         return (lhs.BooleanValue != rhs.BooleanValue) ;
      }

      private void ResetEvaluation()
      {
         // Update our rules remaining counter per condition
         for(int i = 0 ; i < _finalConditions.Length ; i++)
         {
            _rulesRemaining[i] = ((ArrayList)_mapConditionIdToRuleIds[i]).Count ;
            _conditionEvaluated[i] = false ;
         }
         for(int i = 0 ; i < _rules.Length ; i++)
            _conditionsRemaining[i] = _rules[i].Conditions.Length ;
         _matchedRules.Clear() ;
      }

      private int GetRuleId(EventCondition condition)
      {
         for(int i = 0 ; i < _rules.Length ; i++)
         {
            foreach(EventCondition other in _rules[i].Conditions)
            {
               if(condition.Equals(other))
                  return i ;
            }
         }
         return -1 ;
      }

      private void MatchedRuleDump(EventRecord record)
      {
         MatchedRuleDump(record, (EventRule)_matchedRules[0]) ;
      }
      
      private void MatchedRuleDump(EventRecord record, EventRule rule)
      {
         // Debug code
         if(ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
         {
            string[] sValues ;
            string sMessage = "Filtered Event Information\n" ;

            sMessage += String.Format("Rule Name:  {0}\n", rule.Name) ; 
            sMessage += String.Format("Rule Id:  {0}\n", rule.Id) ; 
            foreach(EventCondition condition in rule.Conditions)
            {
               int columnId = SupportedEventColumns.GetColumnId(condition.ParentRule.RuleType, condition.TargetEventField.ColumnName) ;
               object columnValue = SupportedEventColumns.LookupColumn(columnId, record) ;
               sMessage += String.Format("Condition Id:  {0}\n", condition.Id) ;
               switch(condition.ConditionType)
               {
                  case MatchType.String:
                     sMessage += String.Format("  Looking for:  {0} {1}\n", 
                                               condition.Inclusive ? "" : "All except",
                                               String.Join(",", condition.StringArray)) ;
                     sMessage += String.Format("  Found:  {0}\n", columnValue) ;
                     break ;
                  case MatchType.Bool:
                     sMessage += String.Format("  Looking for:  {0}\n", condition.BooleanValue) ;
                     sMessage += String.Format("  Found:  {0}\n", columnValue) ;
                     break ;
                  case MatchType.Integer:
                     sValues = new string[condition.IntegerArray.Length] ;
                     for(int i = 0 ; i < sValues.Length ; i++)
                        sValues[i] = condition.IntegerArray[i].ToString() ;
                     sMessage += String.Format("  Looking for:  {0} {1}\n", 
                                               condition.Inclusive ? "" : "All except",
                                               String.Join(",", sValues)) ;
                     sMessage += String.Format("  Found:  {0}\n", columnValue) ;
                     break ;
               }
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, _name, sMessage, ErrorLog.Severity.Warning) ;
         }
      }

      private void SetMatch(int conditionIndex, bool matchValue)
      {
         // Avoid loops from the following logic
         if(_conditionEvaluated[conditionIndex])
            return ;

         _conditionMatched[conditionIndex] = matchValue ;
         _conditionEvaluated[conditionIndex] = true ;

         ArrayList parentRuleIds = (ArrayList)_mapConditionIdToRuleIds[conditionIndex] ;

         // Propogate a false match to parent rules.  
         // One false condition renders the rule a no-match
         //  Also remove the rule from our return list
         foreach(int i in parentRuleIds)
         {
            // Some rules are done already - skip them.
            if(_conditionsRemaining[i] == 0)
               continue ;

            if(!matchValue)
            {
               // On False, no more conditions need to be evaluated for the corresponding
               //  rule.  Also, update all corresponding condtions for that rule.
               _conditionsRemaining[i] = 0 ;
               ArrayList conditionIds = (ArrayList)_mapRuleIdToConditionIds[i] ;
               foreach(int j in conditionIds)
               {
                  if(!_conditionEvaluated[j])
                  {
                     //_conditionEvaluated[j] = true ;
                     _rulesRemaining[j]-- ;
                  }
               }
            }
            else
            {
               _conditionsRemaining[i]-- ;
               // At this point, we have a rule that matched the condition
               if(_conditionsRemaining[i] == 0)
               {
                  if(!_matchedRules.Contains(_rules[i]))
                     _matchedRules.Add(_rules[i]) ;
               }
            }
         }
         // Update the state of the opposite condition, if present
         int oppositeIndex = _oppositeConditions[conditionIndex] ;
         if(oppositeIndex != -1)
            SetMatch(oppositeIndex, !matchValue) ;
      }

      private bool CheckForMatch(int conditionIndex, object o)
      {
         bool retVal ;
         EventCondition condition = _finalConditions[conditionIndex] ;

         retVal = CheckForMatch(condition, o) ;
         SetMatch(conditionIndex, retVal) ;
         return retVal ;
      }

      private bool CheckForMatch(EventCondition condition, object o)
      {
         bool retVal = false ;

            if (condition.FieldId == (int)AlertableEventFields.rowCount)
            {
                if (!(o is Int64))
                    retVal = false;
                else
                    retVal = condition.MatchRowCount((Int64)o);
            }
            else
         switch(condition.TargetEventField.DataFormat)
         {
            case MatchType.String:
               if(!(o is String))
                  retVal = false ;
               else
                  retVal = condition.Matches(o as String) ;
               break ;
            case MatchType.Bool:
               if(!(o is Boolean))
                  retVal = false ;
               else
                  retVal = condition.Matches((Boolean)o) ;
               break ;
            case MatchType.Integer:
               if(!(o is Int32))
                  retVal = false ;
               else
                  retVal = condition.Matches((Int32)o) ;
               break ;
         }
         return retVal ;
      }

      #region Alert Rule Support

        public List<Alert> GenerateAlerts(string sourceInstance, EventRecord[] records, string serverDbName, string connectionString)
        {
            if (serverDbName == null)
                throw new ArgumentNullException("serverDbName");
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (CoreConstants.optimizeRules)
                    return GenerateAlertsOptimized(records, connection, serverDbName);
                else
                    return GenerateAlertsBruteForce(records, connection, serverDbName);
            }
        }

      private List<Alert> GenerateAlertsBruteForce(EventRecord[] records, SqlConnection connection, String serverDbName)
      {
         List<Alert> retVal = new List<Alert>();
         foreach(EventRecord record in records)
         {
            foreach(AlertRule rule in _rules)
            {
               bool bConditionMatches = true ;

               if(rule.TargetsInstance(_instance))
               {
                  foreach(EventCondition condition in rule.Conditions)
                  {
                     int columnId = SupportedEventColumns.GetColumnId(condition.ParentRule.RuleType, condition.TargetEventField.ColumnName) ;
                     object columnValue = SupportedEventColumns.LookupColumn(columnId, record) ;
                   
                    if (condition.FieldId == (int)AlertableEventFields.rowCount && !String.IsNullOrEmpty(condition.IntegerTimeFrame))
                    {
                        if (record.rowcount > 0 || (condition.CbOprtr == "<" || condition.CbOprtr == "<=") || (condition.CbOprtr == ">=" && condition.IntegerRowcount == "0") || (condition.CbOprtr == "=" && condition.IntegerRowcount == "0"))
                        {
                            if (!CheckBruteForceRowCountForMatch(record, connection, rule.Conditions, condition, serverDbName))
                            {
                                bConditionMatches = false;
                                break;
                            }
                        }
                    }
                    else if (!CheckForMatch(condition, columnValue))
                     {
                        bConditionMatches = false ;
                        break ;
                     }
                  }
                  if(bConditionMatches)
                  {
                     retVal.Add(GenerateAlert(rule, record)) ;
                  }
               }
            }
         }
         return retVal ;
      }

      private List<Alert> GenerateAlertsOptimized(EventRecord[] records, SqlConnection connection, String serverDbName)
      {
         List<Alert> retVal = new List<Alert>();
         ArrayList matchingRules = new ArrayList() ;

         foreach(EventRecord record in records)
         {
            matchingRules.Clear() ;
            int conditionIndex = 0 ;
            ResetEvaluation() ;

            // We process all conditions.  If it is evaluated already, it is skipped
            while(conditionIndex < _finalConditions.Length)
            {
               if(!_conditionEvaluated[conditionIndex])
               {
                   if (_finalConditions[conditionIndex].FieldId == (int)AlertableEventFields.rowCount && !String.IsNullOrEmpty(_finalConditions[conditionIndex].IntegerTimeFrame))
                   {
                       if (record.rowcount > 0 || (_finalConditions[conditionIndex].CbOprtr == "<" || _finalConditions[conditionIndex].CbOprtr == "<=") || (_finalConditions[conditionIndex].CbOprtr == ">=" && _finalConditions[conditionIndex].IntegerRowcount == "0") || (_finalConditions[conditionIndex].CbOprtr == "=" && _finalConditions[conditionIndex].IntegerRowcount == "0"))
                           CheckOptimizedRowCountForMatch(record, connection, conditionIndex, serverDbName);
                   }
                   else
                   {
                       // All state information is managed in the SetMatch function
                       //  Therefore, we just need to check for a match with all conditions.
                       //  Once this is done, the _matchingRules will contain our rules
                       CheckForMatch(conditionIndex, SupportedEventColumns.LookupColumn(_finalColumnIds[conditionIndex], record));
                   }
             }
               conditionIndex++ ;
            }
            // Generate the alerts
            foreach(AlertRule rule in _matchedRules)
            {
               retVal.Add(GenerateAlert(rule, record)) ;
            }
         }
         return retVal ;
        }

      private bool CheckBruteForceRowCountForMatch(EventRecord record, SqlConnection connection, EventCondition[] eventConditions, EventCondition eventCondition, string serverDbName)
        {
            DateTime startTime = record.startTime;
            startTime = startTime.AddHours(-Double.Parse(eventCondition.IntegerTimeFrame));
            ValidateConnection(connection);
            string cmdStr = "SELECT SUM(rowCounts) FROM {0}..{1} WHERE startTime >= '{2}' AND eventId <= {3}";

            string whereClauseString = "";
            cmdStr = String.Format(cmdStr,
                                   SQLHelpers.CreateSafeDatabaseName(serverDbName),
                                   CoreConstants.RepositoryEventsTable,
                                   startTime,
                                   record.eventId);

            whereClauseString = ParseConditions(eventConditions, eventCondition.RuleId);

            if (!String.IsNullOrEmpty(whereClauseString))
                cmdStr += " AND " + whereClauseString;

            using (SqlCommand command = new SqlCommand(cmdStr, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(0) != DBNull.Value)
                            return CheckForMatch(eventCondition, (object)reader.GetValue(0));
                    }
                }
            }
            return false;
        }

        private void CheckOptimizedRowCountForMatch(EventRecord record, SqlConnection connection, int conditionIndex, string serverDbName)
        {
            DateTime startTime = record.startTime;
            startTime = startTime.AddHours(-Double.Parse(_finalConditions[conditionIndex].IntegerTimeFrame));
            ValidateConnection(connection);
            string cmdStr = "SELECT SUM(rowCounts) FROM {0}..{1} WHERE startTime >= '{2}' AND eventId <= {3}";
            string whereClauseString = "";
            cmdStr = String.Format(cmdStr, 
                                   SQLHelpers.CreateSafeDatabaseName(serverDbName), 
                                   CoreConstants.RepositoryEventsTable, 
                                   startTime, 
                                   record.eventId);
            whereClauseString = ParseConditions(_finalConditions, _finalConditions[conditionIndex].RuleId);

            if (!String.IsNullOrEmpty(whereClauseString))
                cmdStr += " AND " + whereClauseString;

            using (SqlCommand command = new SqlCommand(cmdStr, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(0) != DBNull.Value)
                            CheckForMatch(conditionIndex, (object)reader.GetValue(0));
                    }
                }
            }
        }

        private string ParseConditions(EventCondition[] eventConditions, int ruleId)
        {
            string whereClauseString = "";
            int conditionIndex = 0;
            while (conditionIndex < eventConditions.Length)
            {
                EventCondition eventCondition = eventConditions[conditionIndex];

                if (eventCondition.RuleId == ruleId
                    &&
                eventCondition.FieldId != (int)AlertableEventFields.rowCount
                    &&
                eventCondition.FieldId != (int)AlertableEventFields.privilegedUsers)
                {
                    if (!String.IsNullOrEmpty(whereClauseString))
                    {
                        whereClauseString += " AND ";
                    }
                    switch (eventCondition.TargetEventField.DataFormat)
                    {
                        case MatchType.String:
                            string[] condtionValues = eventCondition.StringArray;
                            string columnName = eventCondition.TargetEventField.ColumnName;
                            if (condtionValues.Length == 1)
                            {
                                string condtionValue = eventCondition.StringArray[0];
                                if (condtionValue.Contains("*") || condtionValue.Contains("?") || condtionValue.Contains("#"))
                                {
                                    if (eventCondition.Inclusive)
                                        whereClauseString = String.Format(whereClauseString + " " + columnName + " {0}", GenarateWildCardsString(condtionValue));
                                    else
                                        whereClauseString = String.Format(whereClauseString + " " + columnName + " NOT {0}", GenarateWildCardsString(condtionValue));
                                }
                                else
                                {
                                    if (eventCondition.Inclusive)
                                    {
                                        whereClauseString = String.Format(whereClauseString + " " + columnName + " = '{0}'", eventCondition.StringArray[0]);
                                    }
                                    else
                                    {
                                        whereClauseString = String.Format(whereClauseString + " " + columnName + " <> '{0}'", eventCondition.StringArray[0]);
                                    }
                                }
                            }
                            else if (condtionValues.Length > 1)
                            {
                                string values = "";
                                string wildcardFilters = "";
                                whereClauseString += " (";
                                foreach (string value in condtionValues)
                                {
                                    if (value.Contains("*") || value.Contains("?") || value.Contains("#"))
                                    {
                                        if (!String.IsNullOrEmpty(wildcardFilters))
                                        {
                                            if (eventCondition.Inclusive)
                                                wildcardFilters += " OR "; 
                                            else
                                                wildcardFilters += " AND ";
                                        }
                                        wildcardFilters += " " + columnName;
                                        if (!eventCondition.Inclusive)                                            
                                            wildcardFilters += " NOT";
                                        wildcardFilters += GenarateWildCardsString(value);
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(values))
                                        {
                                            values += ", ";
                                        }
                                        values = values + "'" + value + "'";

                                    }
                                }
                                if (!String.IsNullOrEmpty(values))
                                {
                                    whereClauseString += " " + columnName;
                                    if (eventCondition.Inclusive)
                                    {
                                        whereClauseString = String.Format(whereClauseString + " IN ({0})", values);
                                    }
                                    else
                                    {
                                        whereClauseString = String.Format(whereClauseString + " NOT IN ({0})", values);
                                    }
                                }
                                if (!String.IsNullOrEmpty(values) && !String.IsNullOrEmpty(wildcardFilters))
                                {
                                    if (eventCondition.Inclusive)
                                        wildcardFilters += " OR ";
                                    else
                                        wildcardFilters += " AND ";
                                }
                                if (!String.IsNullOrEmpty(wildcardFilters))
                                {
                                    whereClauseString += wildcardFilters;
                                }
                                whereClauseString += ") ";
                            }
                            break;
                        case MatchType.Bool:
                            if (eventCondition.BooleanValue)
                            {
                                whereClauseString = whereClauseString + eventCondition.TargetEventField.ColumnName + " = 1";
                            }
                            else
                            {
                                whereClauseString = whereClauseString + eventCondition.TargetEventField.ColumnName + " = 0";
                            }
                            break;
                        case MatchType.Integer:
                            if (eventCondition.IntegerArray.Length == 1)
                            {
                                if (eventCondition.Inclusive)
                                {
                                    whereClauseString = String.Format(whereClauseString + eventCondition.TargetEventField.ColumnName + " = {0}", eventCondition.IntegerArray[0]);
                                }
                                else
                                {
                                    whereClauseString = String.Format(whereClauseString + eventCondition.TargetEventField.ColumnName + " <> {0}", eventCondition.IntegerArray[0]);
                                }
                            }
                            else if (eventCondition.IntegerArray.Length > 1)
                            {
                                string values = "";
                                foreach (int vaule in eventCondition.IntegerArray)
                                {
                                    if (!String.IsNullOrEmpty(values))
                                    {
                                        values += ",";
                                    }
                                    values = values + vaule;
                                }
                                if (eventCondition.Inclusive)
                                {
                                    whereClauseString = String.Format(whereClauseString + eventCondition.TargetEventField.ColumnName + " IN ({0})", values);
                                }
                                else
                                {
                                    whereClauseString = String.Format(whereClauseString + eventCondition.TargetEventField.ColumnName + " NOT IN ({0})", values);
                                }
                            }
                            break;
                    }
                }
                conditionIndex++;
            }
            return whereClauseString;
        }
        private string GenarateWildCardsString(string rawString)
        {
            string wildcardsFilters = "";
            wildcardsFilters += " LIKE '" + ConvertWildCardsString(rawString) + "'";
            return wildcardsFilters;
        }
        private string ConvertWildCardsString(String rawString)
        {
            string cmdStr = "";
            if (rawString == null)
                throw new ArgumentNullException("rawString");

            for (int i = 0; i < rawString.Length; i++)
            {
                char currentChar, nextChar;
                bool bLastChar;

                currentChar = rawString[i];
                if (rawString.Length - 1 == i)
                {
                    bLastChar = true;
                    nextChar = '\0';
                }
                else
                {
                    bLastChar = false;
                    nextChar = rawString[i + 1];
                }

                switch (currentChar)
                {
                    case '*':
                        if (bLastChar || (nextChar != '?' && nextChar != '#'))
                            cmdStr = cmdStr + "%";
                        else
                        {
                            if (nextChar == '#')
                                cmdStr += "%[0-9]%";
                            else if (nextChar == '?')
                                cmdStr += "%_%";
                            else
                                cmdStr += nextChar;
                            i++;
                        }
                        break;
                    case '?':
                        cmdStr += "_";
                        break;
                    case '#':
                        cmdStr += "[0-9]";
                        break;
                    default:
                        cmdStr = cmdStr + currentChar;
                        break;
                }
            }
            return cmdStr;
      }

      public List<Alert> GenerateAlerts(OperationalRecord record)
      {
         throw new NotImplementedException() ;
      }

      public List<Alert> GenerateAlerts(ConfigurationRecord record)
      {
         throw new NotImplementedException() ;
      }

      private Alert GenerateAlert(AlertRule rule, EventRecord record)
      {
         Alert retVal = new Alert() ;

         retVal.AlertType = rule.RuleType ;
         retVal.ParentRule = rule ;
         retVal.Instance = _instance ;
         retVal.EventId = record.eventId ;
         retVal.Level = rule.Level ;
         retVal.EventType = (int)record.eventType ;
         retVal.EventData = record ;
         retVal.MessageData = rule.MessageData ;
         if(rule.HasLogAction)
            retVal.LogStatus = NotificationStatus.Pending ;
         if(rule.HasEmailAction)
            retVal.EmailStatus = NotificationStatus.Pending ;
        if (rule.HasEmailSummaryAction)
            retVal.EmailStatus = NotificationStatus.None;
        if (rule.HasSnmpTrapAction)
            retVal.SnmpTrapStatus = NotificationStatus.Pending;
         retVal.RuleName = rule.Name ;
         retVal.ParentRuleId = rule.Id ;

         ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                  String.Format("An alert is generated for instance {0}:\n" +
                                                 "Rule: {1}\n" +
                                                 "Type: {2}\n" +
                                                 "Event ID: {3}\n" +
                                                 "Event type: {4}\n" +
                                                 "Event time{5}\n",
                                                 _instance,
                                                 rule.Name,
                                                 rule.RuleType,
                                                 record.eventId,
                                                 record.eventType,
                                                 record.startTime));

         return retVal ;
      }

      #endregion // Alert Rule Suport

      #region Event Filter Support

      //
      // This function is optimized to break on the first successful match
      //
      public bool IsFiltered(EventRecord record)
      {
         if(CoreConstants.optimizeRules)
            return IsFilteredOptimized(record) ;
         else
            return IsFilteredBruteForce(record) ;
      }

      private bool IsFilteredOptimized(EventRecord record)
      {
         int conditionIndex = 0 ;
         ResetEvaluation() ;


         // We continue processing as long as there are rules left to process
         //  _nRulesRemaining is only updated for false conditions.  That is why
         //  we use both of the following conditions to control the loop
         while(conditionIndex < _finalConditions.Length)
         {
            if(!_conditionEvaluated[conditionIndex])
            {
               CheckForMatch(conditionIndex, SupportedEventColumns.LookupColumn(_finalColumnIds[conditionIndex], record)) ;
               if(_matchedRules.Count > 0)
               {
                  // Debug code
                  MatchedRuleDump(record) ;
                  return true ;
               }
            }
            conditionIndex++ ;
         }
         return false ;
      }

      private bool IsFilteredBruteForce(EventRecord record)
      {
         foreach(EventRule rule in _rules)
         {
            bool bConditionMatches = true ;

            if(rule.TargetsInstance(_instance))
            {
               foreach(EventCondition condition in rule.Conditions)
               {
                  int columnId = SupportedEventColumns.GetColumnId(condition.ParentRule.RuleType, condition.TargetEventField.ColumnName) ;
                  object columnValue = SupportedEventColumns.LookupColumn(columnId, record) ;
                  if(!CheckForMatch(condition, columnValue))
                  {
                     bConditionMatches = false ;
                     break ;
                  }
               }
               if(bConditionMatches)
               {
                  MatchedRuleDump(record, rule) ;
                  return true ;
               }
            }
         }
         return false ;
      }

      public static EventRecord RowToRecord(DataRow row)
      {
         EventRecord retVal = new EventRecord() ;

         retVal.eventClass = GetRowInt32(row, TraceJob.ndxEventClass) ;
         retVal.eventSubclass = GetRowInt32(row, TraceJob.ndxEventSubclass) ;
         retVal.applicationName = GetRowString(row, TraceJob.ndxApplicationName) ;
         retVal.hostName = GetRowString(row, TraceJob.ndxHostName) ;
         retVal.serverName = GetRowString(row, TraceJob.ndxServerName) ;
         retVal.loginName = GetRowString(row, TraceJob.ndxLoginName) ;
         retVal.success = GetRowInt32(row, TraceJob.ndxSuccess) ;
         retVal.databaseName = GetRowString(row, TraceJob.ndxDatabaseName) ;
         retVal.dbUserName = GetRowString(row, TraceJob.ndxDbUserName) ;
         retVal.objectType = GetRowInt32(row, TraceJob.ndxObjectType) ;
         retVal.objectName = GetRowString(row, TraceJob.ndxObjectName) ;
         retVal.permissions = GetRowInt32(row, TraceJob.ndxPermissions) ;
         retVal.columnPermissions = GetRowInt32(row, TraceJob.ndxColumnPermissions) ;
         retVal.targetLoginName = GetRowString(row, TraceJob.ndxTargetLoginName) ;
         retVal.targetUserName = GetRowString(row, TraceJob.ndxTargetUserName) ;
         retVal.roleName = GetRowString(row, TraceJob.ndxRoleName) ;
         retVal.ownerName = GetRowString(row, TraceJob.ndxOwnerName) ;
         retVal.isSystem = GetRowInt32(row, TraceJob.ndxIsSystem) ;
         retVal.sessionLoginName = GetRowString(row, TraceJob.ndxSessionLoginName) ;
         retVal.providerName = GetRowString(row, TraceJob.ndxProviderName) ;
         retVal.eventType = (TraceEventType)GetRowInt32(row, TraceJob.ndxEventType) ;
         retVal.eventCategory = (TraceEventCategory)GetRowInt32(row, TraceJob.ndxEventCategory) ;
         retVal.privilegedUser = GetRowInt32(row, TraceJob.ndxPrivilegedUser) ;

         return retVal ;
      }

      private static int GetRowInt32(DataRow row, int column)
      {
         if ( row.IsNull(column) )
            return 0;
         else
            return Convert.ToInt32(row[column]);
      }
      
      private static string GetRowString(DataRow row, int column)
      {
         if ( row.IsNull(column) )
            return "";
         else
            return (string)row[column];
      }


      #endregion // Event Filter Support
   }

   internal class ConditionComparer : IComparer
   {
      #region IComparer Members

      public int Compare(object x, object y)
      {
         if(x is EventCondition && y is EventCondition)
         {
            EventCondition condition1 = (EventCondition)x ;
            EventCondition condition2 = (EventCondition)y ;
            int value1 = 2, value2 = 2 ;

            switch(condition1.ConditionType)
            {
               case MatchType.String:
                  value1 = 4 ;
                  break ;
               case MatchType.Bool:
                  value1 = 2 ;
                  break ;
               case MatchType.Integer:
                  value1 = 3 ;
                  break ;
            }
            switch(condition2.ConditionType)
            {
               case MatchType.String:
                  value2 = 4 ;
                  break ;
               case MatchType.Bool:
                  value2 = 2 ;
                  break ;
               case MatchType.Integer:
                  value2 = 3 ;
                  break ;
            }

            return Comparer.Default.Compare(value1, value2) ;
         }
         return 0;
      }

      #endregion
   }

}
