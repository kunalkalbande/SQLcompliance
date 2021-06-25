using System;

namespace Idera.SQLcompliance.Core.EmailSummaryNotification
{
    public class AlertMessage
    {
        private string _body;
        private string _recipients;
        private string _title;

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public string Recipients
        {
            get { return _recipients; }
            set { _recipients = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
