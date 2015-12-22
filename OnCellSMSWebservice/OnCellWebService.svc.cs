using System;
using log4net;

namespace OnCellSMSWebservice
{
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnCellWebService.svc or OnCellWebService.svc.cs at the Solution Explorer and start debugging.
    public class OnCellWebService : IOnCellWebService
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(OnCellWebService));

        public void SmsTo(string phoneNumbers, string message)
        {
            Console.WriteLine(phoneNumbers.Split(','));
            Console.WriteLine(message);
            var comport = "COM5";
            foreach (var phone in phoneNumbers.Split(','))
            {
                _log.Debug("Sending message to " + phone + " via " + comport);
                OnCellConnector.SendMessageToSerialPort(comport, phone, message);
            }
        }
    }
}
