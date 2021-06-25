using System;

namespace Idera.SQLcompliance.Core.EmailSummaryNotification
{
    public class AlertRule
    {
        private int _ruleId;
        private bool _hasEmailSummaryMessage;
        private int _summaryEmailFrequency;
        private DateTime _lastEmailSummarySendTime;

        public int RuleId
        {
            get { return _ruleId; }
            set { _ruleId = value; }
        }

        public bool HasEmailSummaryMessage
        {
            get { return _hasEmailSummaryMessage; }
            set { _hasEmailSummaryMessage = value; }
        }

        public int SummaryEmailFrequency
        {
            get { return _summaryEmailFrequency; }
            set { _summaryEmailFrequency = value; }
        }

        public DateTime LastEmailSummarySendTime
        {
            get { return _lastEmailSummarySendTime; }
            set { _lastEmailSummarySendTime = value; }
        }
    }
}