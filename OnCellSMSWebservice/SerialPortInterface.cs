using System;
using System.Collections.Generic;
using System.IO.Ports;
using log4net;

namespace OnCellSMSWebservice
{
    /// <summary>
    ///     Interfaces with a serial port. There should only be one instance
    ///     of this class for each serial port to be used.
    /// </summary>
    public class SerialPortInterface
    {
        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof (SerialPortInterface));

        /// <summary>
        /// Serial port class
        /// </summary>
        private readonly SerialPort _serialPort;

        /// <summary>
        /// Transcript over data sent over serial port
        /// </summary>
        private readonly List<string> _transcript = new List<string>();

        /// <summary>
        /// Create a new instnace of the SerialPortInterface
        /// </summary>
        /// <param name="serialPort">The serial port to connect to</param>
        /// <param name="baudRate">Baud rate to use</param>
        public SerialPortInterface(string serialPort, int baudRate)
        {
            try
            {
                this._serialPort = new SerialPort(serialPort, baudRate);
            }
            catch (Exception ex)
            {
                _log.Error("Error setting baudrate and serial port", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Sets the current settings for the Comport and tries to open it.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool Open()
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    _log.Error("Error opening com port", ex);
                    return false;
                }
            }

            try
            {
                _serialPort.DtrEnable = true;
            }
            catch
            {
                _log.Error("Error setting DtrEnabled");
            }

            try
            {
                _serialPort.RtsEnable = true;
            }
            catch
            {
                _log.Error("Error setting RtsEnabled");
            }

            return true;
        }

        /// <summary>
        /// Send data to serial port
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Send(byte[] data)
        {
            Open();

            try
            {
                LogTranscript(data.ToString());
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                _log.Error("Error sending data to serial device", ex);
                _log.Debug("Data to send was: " + data);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Send data to serial port
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Send(string data)
        {
            Open();

            try
            {
                LogTranscript(data);
                _serialPort.Write(data);
            }
            catch (Exception ex)
            {
                _log.Error("Error sending data to serial device", ex);
                _log.Debug("Data to send was: " + data);

                return false;
            }
            return true;
        }

        /// <summary>
        /// Close connection to serial port
        /// </summary>
        public void Close()
        {
            _serialPort.Close();
            if (_log.IsDebugEnabled)
            {
                _log.Debug("Full _transcript playback:\n");
                foreach (var message in _transcript)
                {
                    _log.Debug(message);
                }
            }
        }

        /// <summary>
        /// Log data sent over serial connection
        /// </summary>
        /// <param name="message">Message to log</param>
        private void LogTranscript(string message)
        {
            if (_log.IsDebugEnabled)
            {
                _transcript.Add(message);
            }
        }
    }
}