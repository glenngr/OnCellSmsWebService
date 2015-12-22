using System;
using System.Configuration;

namespace SendSmsSoapRequest
{
    class SendSmsSoapRequest
    {
        /// <summary>
        /// Default help message with parameters
        /// </summary>
        private const string HelpMessage = "Please supply the following command line arguments:" +
                                            "\nService URL." +
                                            "\nPhone numbers, comma delimited." +
                                            "\nThe message." +
                                            "\nExample:" +
                                            "SendSmsSoapRequest.exe \"40404040,40404048 \"Test message\"";
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(HelpMessage);
                return;
            }

            var url = Properties.Settings.Default.ConsoleApplication1_OnCellWebService_OnCellWebService;
            var client = new OnCellWebService.OnCellWebService {Url = url};
            var phoneNumbers = args[0].Split(',');
            var message = args[1];

            foreach (var phoneNumber in phoneNumbers)
            {
                if (phoneNumber.Length < 8)
                {
                    Console.WriteLine("Invalid phone number, " + phoneNumber + "ignored.");
                }
            }

            // Send message to SOAP service
            try
            {
                client.SmsTo(args[0], message);
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // Always close the client.
            client.Dispose();
        }
    }
}
