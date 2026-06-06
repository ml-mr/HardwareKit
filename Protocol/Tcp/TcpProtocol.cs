using HardwareKit.Config;
using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HardwareKit.Protocol.Tcp
{
    /// <summary>
    /// TCP协议实现类
    /// </summary>
    internal sealed class TcpProtocol : ProtocolBase
    {
        public TcpProtocol(IConfig config, ProtocolOptions options) : base(config, options)
        {
            
        }
        public override bool IsConnected { get => throw new NotImplementedException();  }

        public override Task<Result> ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<Result> DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<Result> SendAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public override Task<Result<byte[]>> SendWaitReplyAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public override Task<Result<TResponse>> SendWaitReplyAsync<TResponse>(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    
    }
}
