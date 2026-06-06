using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Protocol.ParsePacket
{
    /// <summary>
    ///  解包
    /// </summary>
    internal interface IParsePacket
    {
        /// <summary>
        /// 解包
        /// </summary>
        /// <returns></returns>
        Result<byte[]> UnPacket(byte[] packet);

    }
}
