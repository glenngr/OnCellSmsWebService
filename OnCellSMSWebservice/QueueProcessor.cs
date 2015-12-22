using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using log4net;

namespace OnCellSMSWebservice
{
    public class QueueProcessor
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(QueueProcessor));

        // Create a scheduler that uses ONE threads. 
        private static LimitedConcurrencyLevelTaskScheduler _lcts = new LimitedConcurrencyLevelTaskScheduler(1);
        private static List<Task> _tasks = new List<Task>();

        // Create a TaskFactory and pass it our custom scheduler. 
        private static TaskFactory _factory = new TaskFactory(_lcts);
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static QueueProcessor _instance;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static QueueProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QueueProcessor();
                    _log.Debug("Created new instance of QueueProcessor");
                }

                return _instance;
            }
        }

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private QueueProcessor() { }

        public void AddSmsMessageTask(SmsMessage smsMessage)
        {
            var comPort = "COM5";
           Task t = _factory.StartNew(() =>
            {
                OnCellConnector.SendMessageToSerialPort(comPort, smsMessage);
            }, _cts.Token);

            _tasks.Add(t);
            _log.Info("Added task to queue. Sms message to " + smsMessage.PhoneNumber);
        }

        /// <summary>
        /// Return the QueueProcessor Instance
        /// </summary>
        /// <returns></returns>
        public static QueueProcessor GetInstance()
        {
            return Instance;
        }
    }
}