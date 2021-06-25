using System;
using System.Text;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;

namespace Idera.SQLcompliance.Application.GUI
{
	/// <summary>
	/// Summary description for FilterUIHelper.
	/// </summary>
	public class FilterUIHelper
	{
		private FilterUIHelper()
		{
		}

      public static string GenerateFilterDescription(EventFilter filter, FiltersConfiguration config, out LinkString link)
      {
         StringBuilder builderCodes = new StringBuilder(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}{\colortbl ;\red0\green0\blue255;\red255\green0\blue0;\red0\green128\blue0;}\viewkind4\uc1\pard\f0\fs17 ");

         link = new LinkString(80) ;

         builderCodes.Append("Filter ") ;
         link.Append("Filter ") ;

         bool bEventTypeFound = false ;

         foreach(EventCondition condition in filter.Conditions)
         {
            if(condition.FieldId == (int)AlertableEventFields.eventType)
            {
               CMEventType evType = config.LookupEventType(condition.IntegerArray[0], EventType.SqlServer) ;
               builderCodes.AppendFormat(@"the \ul\cf1 {0}\cf0\ulnone  event\line", RtfEscape(evType.Name)) ;
               link.Append("the ") ;
               link.AppendLink(evType.Name, condition) ;
               link.Append(" event") ;
               bEventTypeFound = true ;
            }
            else if(condition.FieldId == (int)AlertableEventFields.eventCategory)
            {
               CMEventCategory evCategory = config.LookupCategory(condition.IntegerArray[0]) ;
               builderCodes.AppendFormat(@"all \ul\cf1 {0}\cf0\ulnone  events\line", RtfEscape(evCategory.Name)) ;
               link.Append("all ") ;
               link.AppendLink(evCategory.Name, condition) ;
               link.Append(" events") ;
               bEventTypeFound = true ;
            }
         }
         if(!bEventTypeFound)
         {
            builderCodes.Append(@"\b All Events\b0\line") ;
            link.Append("All Events") ;
         }

         GenerateScopeString(filter, builderCodes, link) ;
         GenerateFiltersString(filter, builderCodes, link, config) ;

         // This helps with selection of the last character (need pointNext)
         builderCodes.Append("  ") ;
         link.Append("  ") ;

         builderCodes.Append(@"\par}") ;
         return builderCodes.ToString() ;
      }

      private static void GenerateScopeString(EventFilter filter, StringBuilder builderCodes, LinkString link)
      {
         EventCondition dbLevel = null, objectLevel = null ;

         foreach(EventCondition condition in filter.Conditions)
         {
            if(condition.FieldId == (int)AlertableEventFields.databaseName)
               dbLevel = condition ;
            else if(condition.FieldId == (int)AlertableEventFields.objectName)
               objectLevel = condition ;
         }

         builderCodes.Append(" on ") ;
         link.Append(" on ") ;

         // If null, we match any instance (ALL)
         if(filter.TargetInstances.Length == 1  && String.Equals("<ALL>", filter.TargetInstances[0]))
         {
            builderCodes.Append("any SQL Server ") ;
            link.Append("any SQL Server ") ;
         }
         else
         {
            //  In this case, the user hasn't specified target instances
            if(filter.TargetInstances.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified SQL Servers\cf0\ulnone ") ;
               link.AppendLink("specified SQL Servers", "TargetInstances") ;
            }
            else
            {
               if(filter.TargetInstances.Length > 1)
               {
                  builderCodes.Append("SQL Servers ") ;
                  link.Append("SQL Servers ") ;
               }
               else
               {
                  builderCodes.Append("SQL Server ") ;
                  link.Append("SQL Server ") ;
               }
               string sInstances = filter.TargetInstanceList ;
               sInstances = link.AppendLink(sInstances, "TargetInstances") ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(sInstances)) ;
            }
         }

         if(dbLevel != null)
         {
            builderCodes.Append(@"\line   and ") ;
            link.Append("   and ") ;
            if(dbLevel.StringArray == null || dbLevel.StringArray.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified databases\cf0\ulnone ") ;
               link.AppendLink("specified databases", dbLevel) ;
            }
            else
            {
               string sDatabases = String.Join(",", dbLevel.StringArray) ;

               if(dbLevel.Inclusive)
               {
                  if(dbLevel.StringArray.Length == 1)
                  {
                     builderCodes.Append("database ") ;
                     link.Append("database ") ;
                  }
                  else
                  {
                     builderCodes.Append("databases ") ;
                     link.Append("databases ") ;
                  }
               }
               else
               {
                  builderCodes.Append("all databases except ") ;
                  link.Append("all databases except ") ;
               }
               sDatabases = link.AppendLink(sDatabases, dbLevel) ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(sDatabases)) ;
            }
         }

         if(objectLevel != null)
         {
            builderCodes.Append(@"\line   and ") ;
            link.Append("   and ") ;
            if(objectLevel.StringArray == null || objectLevel.StringArray.Length == 0)
            {
               builderCodes.Append(@"\ul\cf2specified objects\cf0\ulnone ") ;
               link.AppendLink("specified objects", objectLevel) ;
            }
            else
            {
               string sObjects = String.Join(",", objectLevel.StringArray) ;

               if(objectLevel.Inclusive)
               {
                  if(objectLevel.StringArray.Length == 1)
                  {
                     builderCodes.Append("object ") ;
                     link.Append("object ") ;
                  }
                  else
                  {
                     builderCodes.Append("objects ") ;
                     link.Append("objects ") ;
                  }
               }
               else
               {
                  builderCodes.Append("all objects except ") ;
                  link.Append("all objects except ") ;
               }
               sObjects = link.AppendLink(sObjects, objectLevel) ;
               builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone ", RtfEscape(sObjects)) ;
            }
         }
      }

      private static void GenerateFiltersString(EventFilter filter, StringBuilder builderCodes, LinkString link, FiltersConfiguration config)
      {
         bool hasWhere = false ;
         string tempString ;
         StringBuilder tempBuilder = new StringBuilder(128);

         foreach(EventCondition condition in filter.Conditions)
         {
            // These fields are used for conditions other than filter
            if(condition.FieldId == (int)AlertableEventFields.objectName)
               continue ;
            if(condition.FieldId == (int)AlertableEventFields.databaseName)
               continue ;
            if(condition.FieldId == (int)AlertableEventFields.eventCategory)
               continue ;
            if(condition.FieldId == (int)AlertableEventFields.eventType)
               continue ;

            if(!hasWhere)
            {
               builderCodes.Append(@"\line   where ") ;
               link.Append(@"   where ") ;
               hasWhere = true ;
            }
            else
            {
               builderCodes.Append(@"\line   and ") ;
               link.Append("   and ") ;
            }

            switch(condition.ConditionType)
            {
               case MatchType.Bool:
                  builderCodes.AppendFormat(@"{0} is \ul\cf1 {1}\cf0\ulnone ", RtfEscape(condition.TargetEventField.DisplayName), RtfEscape(condition.BooleanValue.ToString())) ;
                  link.Append(String.Format("{0} is ", condition.TargetEventField.DisplayName)) ;
                  link.AppendLink(condition.BooleanValue.ToString(), condition) ;
                  break ;
               case MatchType.Integer:
                  if(condition.FieldId == (int)AlertableEventFields.eventType && !condition.Inclusive)
                  {
                     builderCodes.Append("event type not ") ;
                     link.Append("event type not ") ;
                     tempString = "" ;
                     for(int i = 0 ; i < condition.IntegerArray.Length ; i++)
                     {
                        CMEventType evType = config.LookupEventType(condition.IntegerArray[i], EventType.SqlServer) ;
                        tempString += evType.Name ;
                        if(i < condition.IntegerArray.Length - 1)
                           tempString += "," ;
                     }
                     if(tempString.Length == 0)
                     {
                        builderCodes.Append(@"\ul\cf2select event types\cf0\ulnone") ;
                        link.AppendLink("select event types", condition) ;
                     }
                     else
                     {
                        tempString = link.AppendLink(tempString, condition) ;
                        builderCodes.AppendFormat(@"\ul\cf1 {0}\cf0\ulnone", RtfEscape(tempString)) ;
                     }
                  }
                  break ;
               case MatchType.String:
                  builderCodes.AppendFormat("{0} ", RtfEscape(condition.TargetEventField.DisplayName)) ;
                  link.Append(String.Format("{0} ", condition.TargetEventField.DisplayName)) ;

                  if(condition.IsValid)
                  {
                     tempBuilder.Remove(0, tempBuilder.Length);

                     if (condition.Nulls)
                     {
                        if (condition.Inclusive)
                           tempBuilder.Append(" is Null");
                        else
                           tempBuilder.Append(" is not Null");

                        if (condition.Blanks)
                        {
                           if (condition.Inclusive)
                              tempBuilder.Append(" or");
                           else
                              tempBuilder.Append(" and");
                        }
                     }

                     if (condition.Blanks)
                     {
                        if (condition.Inclusive)
                           tempBuilder.Append(" Is Empty or Blank");
                        else
                           tempBuilder.Append(" not Empty or Blank");
                     }

                     if (condition.Blanks || condition.Nulls)
                     {
                        if (condition.StringArray.Length > 0)
                        {
                           if (condition.Inclusive)
                              tempBuilder.Append(" or like ");
                           else
                              tempBuilder.Append(" and not like ");
                        }
                     }
                     else
                     {
                        if (condition.StringArray.Length > 0)
                        {
                           if (condition.Inclusive)
                              tempBuilder.Append(" like ");
                           else
                              tempBuilder.Append(" not like ");
                        }
                     }

                     for(int i = 0 ; i < condition.StringArray.Length; i++)
                     {
                        tempBuilder.AppendFormat("'{0}'", condition.StringArray[i]);
                        if(i < condition.StringArray.Length - 1)
                        {
                           if(condition.Inclusive)
                              tempBuilder.Append(" or ") ;
                           else 
                              tempBuilder.Append(" and ") ;
                        }
							
                     }
                     tempString = link.AppendLink(tempBuilder.ToString(), condition) ;
                     builderCodes.AppendFormat(@"\ul\cf1{0}\cf0\ulnone ", RtfEscape(tempString)) ;
                  }
                  else
                  {
                     builderCodes.Append(@"\ul\cf2 specified words\cf0\ulnone ") ;
                     link.AppendLink(" specified words", condition) ;
                  }
                  break ;
            }                        
         }
      }

      private static string RtfEscape(string s)
      {
         ASCIIEncoding encoder = new ASCIIEncoding() ;
         StringBuilder retVal = new StringBuilder() ;

         byte[] myBytes = encoder.GetBytes(s) ;
         for(int i = 0 ; i < myBytes.Length ; i++)
         {
            byte b = myBytes[i] ;
            if((b >= 0 && b < 0x20) ||
               (b >= 0x80 && b <= 0xFF) ||
               b == 0x5C || b == 0x7B || b == 0x7D)
            {
               retVal.AppendFormat("\\'{0:X2}",  b) ;
            }
            else
               retVal.Append(s[i]) ;
         }
         return retVal.ToString(); 
      }
	}
}
