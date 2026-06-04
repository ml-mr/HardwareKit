using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace HardwareKit.Protocol.SerialPort
{
    internal sealed class SerialPortProtocol : IProtocol
    {
        private readonly System.IO.Ports.SerialPort serialPort;

        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<Result> ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result> DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
