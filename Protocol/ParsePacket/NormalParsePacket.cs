using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Protocol.ParsePacket
{
    /// <summary>
    /// 正常解析包，接收多少字节返回多少字节
    /// </summary>
    internal sealed class NormalParsePacket : IParsePacket
    {
        public Result<byte[]> UnPacket(byte[] packet)
        {
           return new Result<byte[]>(packet);
        }
    }
}
