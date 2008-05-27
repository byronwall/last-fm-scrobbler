using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace LastFM
{
    class Log
    {
        private static Log _instance = null;

        internal static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Log();
                }
                return _instance;
            }
        }

        private delegate void AddEventToLog(LogEvent.LogEventSender sender, string content, LogEvent.LogEventStatus status);
        private ObservableCollection<LogEvent> _LogEvents = new ObservableCollection<LogEvent>();

        public ObservableCollection<LogEvent> LogEvents
        {
            get { return _LogEvents; }
            set { _LogEvents = value; }
        }

        public void AddEvent(LogEvent.LogEventSender sender, string content, LogEvent.LogEventStatus status)
        {
            LogEvent item = new LogEvent(sender, content, status);
            if (Window1.Instance == null) return;
            if (Window1.Instance.Dispatcher.CheckAccess())
            {
                this.LogEvents.Add(item);
                AppendToFile(item);
            }
            else
            {
                Window1.Instance.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new AddEventToLog(AddEvent), sender, content, status);
            }
        }
        /// <summary>
        /// Logging method for the logging of a general debug message.  These belong to the sender Debug and will have a status of neutral.
        /// </summary>
        /// <param name="content">String that is to be written to the log.</param>
        public void AddEvent(string content)
        {
            AddEvent(LogEvent.LogEventSender.Debug, content, LogEvent.LogEventStatus.Neutral);
        }
        public void AddEvent(Exception e)
        {
            string innerEx = (e.InnerException == null) ? "No inner exception" : e.InnerException.ToString();
            AddEvent(LogEvent.LogEventSender.Debug, String.Format("Exception: stack: {0}\r\n tostring:{1}\r\n source:{2}\r\nInner exception:{3}", e.StackTrace, e.ToString(), e.Source, innerEx), LogEvent.LogEventStatus.Neutral);
        }
        static void AppendToFile(LogEvent item)
        {
            using (FileStream fs = new FileStream("log.txt", FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(string.Format("{0}\t[{1},{2}]\t{3}", DateTime.Now.ToString(), item.Sender, item.Status, item.Content));
                }
            }
        }
    }
    public class LogEvent
    {
        string _content;

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        LogEventStatus _status;

        public LogEventStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        LogEventSender _sender;

        public LogEventSender Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public enum LogEventStatus
        {
            Success, Failure, Neutral
        }
        public enum LogEventSender
        {
            iTunes, LastFM, Database, Other, Debug
        }

        public LogEvent(LogEventSender sender, string content, LogEventStatus status)
        {
            Sender = sender;
            Content = content;
            Status = status;
        }
    }
}
