using HardwareKit.Config;
using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HardwareKit.Protocol.Tcp
{
    internal sealed class TcpProtocol : IProtocol
    {
        public TcpProtocol(IConfig config, ProtocolOptions options)
        {
            Options = options;
        }
        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ProtocolOptions Options { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
