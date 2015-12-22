using System.Collections;
using System.Collections.Generic;

namespace OnCellSMSWebservice
{
    using System;
    using System.IO.Ports;
    using log4net;

    /// <summary>
    /// Interfaces with a serial port. There should only be one instance
    /// of this class for each serial port to be used.
    /// </summary>
    public class SerialPortInterface
    {
        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof(SerialPortInterface));

        /// <summary>
        /// Serial port class
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// BaudRate set to default for Serial Port Class
        /// </summary>
        private int baudRate = 9600;

        /// <summary>
        /// DataBits set to default for Serial Port Class
        /// </summary>
        private int dataBits = 8;

        /// <summary>
        /// Handshake set to default for Serial Port Class
        /// </summary>
        private Handshake handshake = Handshake.None;

        /// <summary>
        /// Parity set to default for Serial Port Class
        /// </summary>
        private Parity parity = Parity.None;

        /// <summary>
        /// Communication Port name, not default in SerialPort.
        /// </summary>
        private string portName = String.Empty;

        /// <summary>
        /// StopBits set to default for Serial Port Class
        /// </summary>
        private StopBits stopBits = StopBits.One;

        /// <summary>
        /// Holds data received until we get a terminator.
        /// </summary>
        private string tString = string.Empty;

        /// <summary>
        /// End of transmition byte in this case EOT (ASCII 4).
        /// </summary>
        private byte terminator = 0x4;

        private List<string> transcript = new List<string>();

        public SerialPortInterface(string serialPort, int baudRate)
        {
            try {
                this.serialPort = new SerialPort(serialPort, baudRate);
             }
            catch (Exception ex)
            {
                _log.Error("Error setting baudrate and serial port", ex);
                throw ex;
            }
}

        /// <summary>
        /// Gets or sets BaudRate (Default: 9600)
        /// </summary>
        public int BaudRate { get { return this.baudRate; } set { this.baudRate = value; } }

        /// <summary>
        /// Gets or sets DataBits (Default: 8)
        /// </summary>
        public int DataBits { get { return this.dataBits; } set { this.dataBits = value; } }

        /// <summary>
        /// Gets or sets Handshake (Default: None)
        /// </summary>
        public Handshake Handshake { get { return this.handshake; } set { this.handshake = value; } }

        /// <summary>
        /// Gets or sets Parity (Default: None)
        /// </summary>
        public Parity Parity { get { return this.parity; } set { this.parity = value; } }

        /// <summary>
        /// Gets or sets PortName (Default: COM1)
        /// </summary>
        public string PortName { get { return this.portName; } set { this.portName = value; } }

        /// <summary>
        /// Gets or sets StopBits (Default: One}
        /// </summary>
        public StopBits StopBits { get { return this.stopBits; } set { this.stopBits = value; } }
        /// <summary>
        /// Sets the current settings for the Comport and tries to open it.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool Open()
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                }
                catch (Exception ex)
                {
                    _log.Error("Error opening com port", ex);
                    return false;
                }
            }

            try
            {
                serialPort.DtrEnable = true;
            }
            catch
            {
                _log.Error("Error setting DtrEnabled");
            }

            try
            {
                serialPort.RtsEnable = true;
            }
            catch
            {
                _log.Error("Error setting RtsEnabled");
            }

            return true;
        }
        public bool Send(byte[] data)
        {
            Open();

            try
            {
                LogTranscript(data.ToString());
                serialPort.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                _log.Error("Error sending data to serial device", ex);
                _log.Debug("Data to send was: " + data);

                return false;
            }

            return true;
        }
        public bool Send(string data)
        {
            Open();

            try
            {
                LogTranscript(data);
                serialPort.Write(data);
            }
            catch (Exception ex)
            {
                _log.Error("Error sending data to serial device", ex);
                _log.Debug("Data to send was: " + data);

                return false;
            }
            return true;
        }
      
        public void Close()
        {
            serialPort.Close();
            if (_log.IsDebugEnabled)
            {
                _log.Debug("Full transcript playback:\n");
                foreach (var message in transcript)
                {
                     _log.Debug(message);
                }
            }
        }

        private void LogTranscript(string message)
        {
            if (_log.IsDebugEnabled)
            {
                transcript.Add(message);
            }
        }
    }
}
