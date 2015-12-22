using System;
using System.Configuration;
using System.Threading;
using log4net;

namespace OnCellSMSWebservice
{
    public class OnCellConnector
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(OnCellConnector));
        private static readonly string ATInitializeCommand = "at+cmgf=1";
        private static readonly string CtrlZ = char.ConvertFromUtf32(26);
        private static readonly string newLine = "\r\n";

        /// <summary>
        /// Connect to serial port and send sms message to a phone number.
        /// </summary>
        /// <param name="smsMessage">The SmsMessage object with phone number and message</param>
        public static void SendMessageToSerialPort(SmsMessage smsMessage)
        {
            // Get config from web.config
            var serialPort = ConfigurationManager.AppSettings["SerialPort"];
            if (serialPort == null || !serialPort.ToUpper().Contains("COM"))
            {
                throw new ConfigurationErrorsException("Missing or invalid serial port configuration in Web.config");
            }

            var serial = new SerialPortInterface(serialPort.ToUpper(), 9600);

            _log.Debug("Opening serial port " + serialPort);
            if (!serial.Open())
            {
                LogErrorAndWriteToConsole("Error while opening serial port " + serialPort);
            }
            else
            {
                _log.Debug("Serial port opened");
            }
            Thread.Sleep(250);

            // Initialize modem
            _log.Debug("Initializing modem");
            if (!serial.Send(ATInitializeCommand + newLine))
            {
                LogErrorAndWriteToConsole("Error while initializing modem with AT command: " + ATInitializeCommand);
                return;
            }

            _log.Debug("Modem initialized");
            Thread.Sleep(250);


            // Phone number to send to
            _log.Debug("Sending phonenumber");
            if (!serial.Send("at+cmgs=" + smsMessage.PhoneNumber + newLine))
            {
                LogErrorAndWriteToConsole("Error while setting phonenumber to " + smsMessage.PhoneNumber);
                return;
            }
            _log.Debug("Phone number set OK");
            Thread.Sleep(250);

            _log.Debug("Sending message");
            if (!serial.Send(smsMessage.Message + newLine))
            {
                LogErrorAndWriteToConsole("Error while sending message to serial port");
                return;
            }
            _log.Debug("Message set OK");
            Thread.Sleep(250);


            _log.Debug("Trying to end message");
            if (!serial.Send(CtrlZ))
            {
                LogErrorAndWriteToConsole("Error sending CTRL-Z to serial port");
            }
            _log.Debug("Message ended OK with CTRL-Z");
            Thread.Sleep(250);

            _log.Debug("Closing serial connection");
            serial.Close();
        }

        private static void LogErrorAndWriteToConsole(string message)
        {
            _log.Error(message);
            Console.WriteLine(message);
        }
    }
}