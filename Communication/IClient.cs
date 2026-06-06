using HardwareKit.Help;
using HardwareKit.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HardwareKit.Communication
{
    /// <summary>
    /// 通信客户端
    /// </summary>
    public interface IClient
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

        /// <summary>
        /// 异步发送数据并等待回复
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        Task<Result<byte[]>> SendWaitReplyAsync(byte[] buffer);

        /// <summary>
        /// 异步发送数据并等待回复，回复数据返回为指定类型的对象
        /// </summary>
        /// <typeparam name="TResponse">指定类型，该类型必须具备字节构造函数</typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        Task<Result<TResponse>> SendWaitReplyAsync<TResponse>(byte[] buffer) where TResponse:class;

        /// <summary>
        /// 连接成功事件
        /// </summary>
        event Action ConnectSuccessEvt;

        /// <summary>
        /// 连接失败事件
        /// </summary>
        event Action ConnectFailedEvt;

        /// <summary>
        /// 断开连接成功事件
        /// </summary>
        event Action DisconnectSuccessEvt;

        /// <summary>
        /// 断开连接失败事件
        /// </summary>
        event Action DisconnectFailedEvt;

    }
}
