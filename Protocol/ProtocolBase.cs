using HardwareKit.Config;
using HardwareKit.Help;
using HardwareKit.Protocol.ParsePacket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareKit.Protocol
{
    /// <summary>
    /// 协议抽象类
    /// </summary>
    internal abstract class ProtocolBase : IProtocol
    {
        protected ProtocolBase(IConfig config, ProtocolOptions options) 
        {
            Config=config;
            Options=options;
            SyncSem=new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// 协议配置
        /// </summary>
        protected IConfig Config { get; set; }

        /// <summary>
        /// 数据包解析器
        /// </summary>
        protected IParsePacket ParsePacket { get; set; }


        /// <summary>
        ///  等待响应的TaskCompletionSource对象，用于异步等待串口数据接收完成
        /// </summary>
        protected TaskCompletionSource<byte[]> ByteResponseTcs { get; set; }

        /// <summary>
        /// 资源释放
        /// </summary>
        protected bool IsDisposed { get; set; }

        /// <summary>
        ///  同步信号量
        /// </summary>
        protected SemaphoreSlim SyncSem { get; }

        /// <summary>
        /// 协议选项
        /// </summary>
        protected ProtocolOptions Options { get; set; }

        protected void SwitchParsePacket()
        {
            switch (Options.ParsePacketType)
            {
                case ParsePacketType.NewLine:
                    ParsePacket = new NewLineParsePacket(Options.NewLine);
                    break;
                case ParsePacketType.Length:
                    ParsePacket = new LengthParsePacket(Options.PacketLength);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract bool IsConnected { get; }

        public abstract Task<Result> ConnectAsync();
        public abstract Task<Result> DisconnectAsync();
        public abstract Task<Result> SendAsync(byte[] buffer);
        public abstract Task<Result<byte[]>> SendWaitReplyAsync(byte[] buffer);
        public abstract Task<Result<TResponse>> SendWaitReplyAsync<TResponse>(byte[] buffer) where TResponse : class;
        public abstract void Dispose();
    }
}
