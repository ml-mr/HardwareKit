using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Protocol.Enums
{
    /// <summary>
    /// 解析包类型
    /// </summary>
    public enum ParsePacketType
    {
        /// <summary>
        ///  接收多少字节返回多少字节
        /// </summary>
        None = 0,

        /// <summary>
        /// 按照换行符解析为一包数据
        /// </summary>
        NewLine = 1,

        /// <summary>
        /// 按照长度解析为一包数据
        /// </summary>
        Length=2,
    }
}
