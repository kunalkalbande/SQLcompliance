using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
    internal class XEventData
    {
        List<XEventSingleEvent> eventData = new List<XEventSingleEvent>();

        internal List<XEventSingleEvent> EventData
        {
            get { return eventData; }
            set { eventData = value; }
        }
        public XEventData(string filePath)
        {
            ActiveQueryCollector assemblyData = new ActiveQueryCollector();
            object events=null;
            if (assemblyData.IsAssemblyLoaded)
            {
                try
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.AssemblyResolve += new ResolveEventHandler(XEventHelper.XEventDependencyHandler);
                    Type QuerayableXEventData = assemblyData.Assembly.GetType("Microsoft.SqlServer.XEvent.Linq.QueryableXEventData");
                    events = Activator.CreateInstance(QuerayableXEventData, new object[] { filePath });
                }
                catch(Exception ex)
                {
                    ErrorLog.Instance.Write("TraceJob::Start",
                                        String.Format(
                                           CoreConstants.Exception_ErrorProcessingXELFile,
                                           filePath),
                                        ex,
                                        true);
                    throw;
                }
                IEnumerable eventList = events as IEnumerable;
                if (eventList != null)
                {
                    object[] eventsArray = eventList.Cast<object>().ToArray();
                    int count = eventsArray.Count();
                    for (int j = 0; j < count; j++)
                    {
                        object evnt = eventsArray[j];
                        Type PublishedEvent = evnt.GetType();
                        MethodInfo getEventName = PublishedEvent.GetMethod("get_Name");
                        MethodInfo getEventFields = PublishedEvent.GetMethod("get_Fields");
                        MethodInfo getEventActions = PublishedEvent.GetMethod("get_Actions");
                        MethodInfo getEventTime = PublishedEvent.GetMethod("get_Timestamp");
                        String eventName = getEventName.Invoke(evnt, null).ToString();
                        DateTime eventTime = ((DateTimeOffset)getEventTime.Invoke(evnt, null)).DateTime;
                        int lastDigitOfMSeccond = eventTime.Millisecond % 10;
                        if (lastDigitOfMSeccond == 2 || lastDigitOfMSeccond == 9 || lastDigitOfMSeccond == 6)
                            eventTime = eventTime.AddMilliseconds(1);
                        else if (lastDigitOfMSeccond == 1 || lastDigitOfMSeccond == 8 || lastDigitOfMSeccond == 4)
                            eventTime = eventTime.AddMilliseconds(-1);
                        else if (lastDigitOfMSeccond == 5)
                            eventTime = eventTime.AddMilliseconds(2);
                        object fieldList = getEventFields.Invoke(evnt, null);
                        object actionList = getEventActions.Invoke(evnt, null);
                        Dictionary<string, object> eventFieldNameToFieldMap = getEventFieldNameToValueMap(fieldList,actionList);
                        XEventSingleEvent xeventSingleEvent = new XEventSingleEvent();
                        xeventSingleEvent.EventData = eventFieldNameToFieldMap;
                        xeventSingleEvent.EventName = eventName;
                        xeventSingleEvent.EventTime = eventTime;
                        eventData.Add(xeventSingleEvent);
                    }
                }
            }
            if(events!=null)
                ((IDisposable)events).Dispose();
        }

        public Dictionary<string, object> getEventFieldNameToValueMap(object eventFieldList, object actionList)
        {
            Dictionary<string, object> eventFieldNameToFieldMap = new Dictionary<string, object>();
            IEnumerable fields = eventFieldList as IEnumerable;
            if (fields != null)
            {
                foreach (object field in fields)
                {
                    string fieldName = getFieldName(field);
                    object fieldValue = getFieldValue(field);
                    eventFieldNameToFieldMap.Add(fieldName, fieldValue);
                }
            }

            fields = actionList as IEnumerable;
            if (fields != null)
            {
                foreach (object field in fields)
                {
                    string fieldName = getActionName(field);
                    object fieldValue = getActionValue(field);
                    eventFieldNameToFieldMap.Add(fieldName, fieldValue);
                }
            }

            return eventFieldNameToFieldMap;
        }

        public object getFieldValue(object field)
        {
            object fieldValue;
            Type PublishedEventField = field.GetType();
            MethodInfo methodFieldValue = PublishedEventField.GetMethod("get_Value");
            fieldValue = methodFieldValue.Invoke(field, null);
            return fieldValue;
        }

        public string getFieldName(object field)
        {
            Type PublishedEventField = field.GetType();
            MethodInfo getFieldName = PublishedEventField.GetMethod("get_Name");
            string fieldValue = (string)getFieldName.Invoke(field, null);
            return fieldValue;
        }

        public object getActionValue(object action)
        {
            object actionValue;
            Type PublishedEventAction = action.GetType();
            MethodInfo methodFieldValue = PublishedEventAction.GetMethod("get_Value");
            actionValue = methodFieldValue.Invoke(action, null);
            return actionValue;
        }

        public string getActionName(object action)
        {
            Type PublishedEventAction = action.GetType();
            MethodInfo getActionName = PublishedEventAction.GetMethod("get_Name");
            string fieldValue = (string)getActionName.Invoke(action, null);
            return fieldValue;
        }
    }

    internal class XEventSingleEvent{

        private Dictionary<String, object> eventData = new Dictionary<string, object>();
        private string eventName;
        private DateTime eventTime;

        public DateTime EventTime
        {
            get { return eventTime; }
            set { eventTime = value; }
        }

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        } 

        public Dictionary<String, object> EventData
        {
            get { return eventData; }
            set { eventData = value; }
        }

    }
}
