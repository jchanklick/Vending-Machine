using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ProjectKService
{
    class SerialPortHandler
    {
        private static Object _currentLock = new Object();
        private static SerialPortHandler _current = null;
        private static SerialPort _serialPort = null;

        public SerialPortHandler() : base()
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

        public static SerialPortHandler Current
        {
            get
            {
                lock (_currentLock)
                {
                    if (_current == null)
                    {
                        _current = new SerialPortHandler();
                    }
                }
                return _current;
            }
        }

        public void Start()
        {
            lock (_serialPort)
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    Logger.WriteLine("Serial Port " + _serialPort.PortName + " Open");
                }
            }
        }

        public void Stop()
        {
            lock (_serialPort)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                }
            }
        }

        public void Write(string message)
        {
            Logger.WriteLine("Sending message: " + message);
            if (!_serialPort.IsOpen)
            {
                Logger.WriteLine("Serial Port is closed");
            }
            _serialPort.Write(message + (char)13);
        }


        private static string _buffer = "";
        // use CR (13) as stop
        private static void _DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            _buffer += indata;
            Logger.WriteLine("Data Received: " + indata);
            Logger.WriteLine("Buffer: " + _buffer);

            if (_buffer.IndexOf("System Ready") >= 0)
            {
                Current.Write("9999");
            }
        }
    }
}
