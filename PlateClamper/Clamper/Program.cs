using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clamper
{
    class Program
    {
        static SerialPort _serialPort;
        static int steps = int.Parse(ConfigurationManager.AppSettings["steps"]);
        public static void Main()
        {
            try
            {
                DoJob();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Please enter any key to exit!");
                Console.ReadKey();
            }
            finally
            {
                _serialPort.Close();
            }
            
        }

        private static void DoJob()
        {
            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();
            // Allow the user to set the appropriate properties.
            _serialPort.PortName = GetPortName();
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();
            string[] argsArray = System.Environment.GetCommandLineArgs();
            if (argsArray.Length != 2)
            {
                throw new Exception("No arguments inputed, please check!\r\n You can input 'h' for reset, 'f' for forward, 'b' for backward.");
            }
            string sCommand = argsArray[1];
            if (sCommand == "h")
            {
                BackHome();
            }
            else if (sCommand == "f")
            {
                GoForward();
            }
            else if (sCommand == "b")
            {
                GoBackward();
            }
            else
            {
                throw new Exception("Wrong arguments inputed, please check!\r\n You can input 'h' for reset, 'f' for forward, 'b' for backward.");
            }
            
        }
        private static void Go(bool isForward)
        {
            byte[] forwardBytes = new byte[2];
            forwardBytes[0] = isForward? (byte)0x01 : (byte)0x02;
            forwardBytes[1] = (byte)steps;
            _serialPort.Write(forwardBytes, 0, 2);
        }

        private static void GoBackward()
        {
            Go(false);
        }

        private static void GoForward()
        {
            Go(true);
        }

        private static void BackHome()
        {
            byte[] homeCommand = { 0x00 };
            _serialPort.Write(homeCommand, 0, 1);
        }

        public static string GetPortName()
        {
            var portNames = SerialPort.GetPortNames();
            string expectedName = ConfigurationManager.AppSettings["portNum"];
            if(!portNames.Contains(expectedName))
                throw new Exception(string.Format("Cannot find the port: {0}",expectedName));
            return expectedName;
        }

     
    }
}
