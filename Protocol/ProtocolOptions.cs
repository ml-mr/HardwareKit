using HardwareKit.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Protocol
{
    /// <summary>
    /// 协议选项类，包含协议相关的配置选项
    /// </summary>
    public sealed class ProtocolOptions
    {
        /// <summary>
        /// 解包类型，默认不进行解包，即接收什么返回什么
        /// </summary>
        public ParsePacketType PPType { get; set; } = ParsePacketType.None;

        /// <summary>
        /// 通信数据记录类型，默认为BufferLogType.Byte
        /// </summary>
        public BufferLogType LogType { get; set; } = BufferLogType.Byte;

        /// <summary>
        /// 是否启用重试机制，默认为true，启用后在发送数据失败时会自动重试发送，直到成功或达到最大重试次数
        /// </summary>
        public bool EnableRetry { get; set; } = true;

        /// <summary>
        /// 最大重试次数，默认为3次，当启用重试机制时，如果发送数据失败，会自动重试发送，直到成功或达到最大重试次数为止
        /// </summary>
        public byte MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// 最大重试延迟，单位为毫秒，默认为200ms，当启用重试机制时，如果发送数据失败，会自动重试发送，每次重试之间会有一个随机的延迟，延迟时间在0到最大重试延迟之间随机生成，以避免多个设备同时重试导致的网络拥堵
        /// </summary>
        public ushort MaxRetryDelay { get; set; } = 200;

        /// <summary>
        /// 是否启用日志记录，默认为false，启用后会记录协议的发送和接收日志，方便调试和排查问题
        /// </summary>
        public bool EnableLog { get; set; } = false;

    }
}
