using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HardwareKit.Protocol
{
    /// <summary>
    /// 协议基础接口
    /// </summary>
    internal interface IProtocol
    {
        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; set; }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <returns></returns>
        Task<Result> ConnectAsync();

        /// <summary>
        /// 异步断开连接
        /// </summary>
        /// <returns></returns>
        Task<Result> DisconnectAsync();

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        Task<Result> SendAsync(byte[] buffer);

    }
}
