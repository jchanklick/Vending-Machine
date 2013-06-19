using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ProjectKService
{
    class SerialPortHandler
    {
        private static Object _lock = new Object();
        private static SerialPort _serialPort = null;
        private static string _inputBuffer = "";
        private static char _stopChar = (char)13;

        public static void Start()
        {
            lock (_lock)
            {
                if (_serialPort == null)
                {
                    SerialPort mySerialPort = new SerialPort("COM4");

                    mySerialPort.BaudRate = 9600;
                    mySerialPort.Parity = Parity.None;
                    mySerialPort.StopBits = StopBits.One;
                    mySerialPort.DataBits = 8;
                    mySerialPort.Handshake = Handshake.None;

                    mySerialPort.DataReceived += new SerialDataReceivedEventHandler(_DataReceivedHandler);

                    _serialPort = mySerialPort;
                }

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();

                    Logger.WriteLine("Serial Port " + _serialPort.PortName + " Open");
                }
            }
        }

        public static void Stop()
        {
            lock(_lock)
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                }
            }
        }

        public static void Write(string message)
        {
            // this will only start the service if we need to, so it's good to call in case something went wrong
            Start();

            Logger.WriteLine("Sending message: " + message);
            if (!_serialPort.IsOpen)
            {
                Logger.WriteLine("Serial Port is closed");
            }
            _serialPort.Write(message + _stopChar);
        }

        private static void _DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            Logger.WriteLine("Data Received: " + data);

            string msg = null;

            lock (_lock)
            {
                _inputBuffer += data;

                // replace line feed with carriage return -- we seem to be getting both
                _inputBuffer = _inputBuffer.Replace((char)10, _stopChar);

                int index = _inputBuffer.IndexOf(_stopChar);
                while (index >= 0)
                {
                    Logger.WriteLine("Input: " + _inputBuffer + " Index: " + index);

                    if (index > 0)
                    {
                        string command = _inputBuffer.Substring(0, index);
                        Logger.WriteLine("Command: " + command);
                        msg = CommandHandler.ProcessCommand(command);
                    }

                    if (_inputBuffer.Length > (index + 1))
                    {
                        _inputBuffer = _inputBuffer.Substring(index + 1, _inputBuffer.Length - index - 1);
                    }
                    else {
                        _inputBuffer = "";
                    }

                    index = _inputBuffer.IndexOf(_stopChar);
                }
            }

            if (msg != null)
            {
                Write(msg);
            }
        }
    }
}
