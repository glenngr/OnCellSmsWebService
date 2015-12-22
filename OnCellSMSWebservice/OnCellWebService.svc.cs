using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace OnCellSMSWebservice
{
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnCellWebService.svc or OnCellWebService.svc.cs at the Solution Explorer and start debugging.
    public class OnCellWebService : IOnCellWebService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(OnCellWebService));
        private static QueueProcessor _queueProcessor = QueueProcessor.GetInstance();

        public void SmsTo(string phoneNumbers, string message)
        {
            Task.Run(() =>
            {
                foreach (var phone in phoneNumbers.Split(','))
                {
                    _log.Debug("Queueing message to " + phone);
                    _queueProcessor.AddSmsMessageTask(new SmsMessage()
                    {
                        PhoneNumber = phone,
                        Message = message
                    });
                }
            });
        }

        /// <summary>
        /// Get the SMS Queue Size
        /// </summary>
        /// <returns>SMS Queue Size</returns>
        public int GetSmsQueueSize()
        {
            return _queueProcessor.GetQueueSize();
        }
    }
}
