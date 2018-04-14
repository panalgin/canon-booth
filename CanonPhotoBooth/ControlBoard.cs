using System;
using System.IO.Ports;

namespace CanonPhotoBooth
{
    public class ControlBoard
    {
        private string inputBuffer;
        private SerialPort serialPort;

        public delegate void OnSampleAcquired(ulong totalRevs, ulong timePassed);
        public event OnSampleAcquired SampleAcquired;

        public void Connect(string comPort, int baudRate)
        {
            try
            {
                if (serialPort != null)
                    Disconnect();

                serialPort = new SerialPort(comPort, baudRate);
                serialPort.Open();

                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.DataReceived += SerialPort_DataReceived;
            }
            catch(Exception ex)
            {
                
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort.BytesToRead > 0)
            {
                char c = (char)serialPort.ReadChar();

                inputBuffer += c;

                if (c == '\n')
                {
                    Parse();

                    inputBuffer = "";
                }
            }
        }

        void Parse()
        {
            inputBuffer = inputBuffer.Replace("\r", "").Replace("\n", "");

            if (inputBuffer.StartsWith("Commence"))
            {
                Game.Start();
            }
            else if (inputBuffer.StartsWith("D:"))
            {
                inputBuffer = inputBuffer.Replace("D:", "");

                string[] parsed = inputBuffer.Split(',');

                if (parsed.Length == 2)
                {
                    try
                    {
                        ulong revCount = Convert.ToUInt64(parsed[0]);
                        ulong period = Convert.ToUInt64(parsed[1]);

                        InvokeSampleAcquired(revCount, period);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
        }

        void Disconnect()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.Close();
                serialPort.Dispose();
            }
        }

        void InvokeSampleAcquired(ulong totalRevs, ulong timePassed)
        {
            SampleAcquired?.Invoke(totalRevs, timePassed);
        }
    }
}