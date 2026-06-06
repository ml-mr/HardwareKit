using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HardwareKit.Protocol.ParsePacket
{
    /// <summary>
    /// 按照长度解析
    /// </summary>
    internal sealed class LengthParsePacket : IParsePacket
    {
        public LengthParsePacket(int packetLength)
        {
            PacketLength = packetLength;
            CacheBuffer = new List<byte>();
        }

        /// <summary>
        /// 包的长度
        /// </summary>
        private int PacketLength { get; set; }

        /// <summary>
        /// 缓存区，用于存储接收到的数据，直到达到包的长度
        /// </summary>
        private List<byte> CacheBuffer { get; set; }
        public Result<byte[]> UnPacket(byte[] packet)
        {
            //1、添加数据到缓存区
            CacheBuffer.AddRange(packet);

            //2、判断缓存区中的数据是否已经达到包的长度，如果达到，则将缓存区中的数据作为一包返回，并清空缓存区
            if (CacheBuffer.Count >= PacketLength)
            {
                //从缓存区中取出包的长度的数据作为一包返回
                var result = CacheBuffer.Take(PacketLength).ToArray();

                //清空缓存区
                CacheBuffer.Clear();

                //返回包数据
                return new Result<byte[]>(result);
            }
            return new Result<byte[]>(null, false, $"数据包长度不足,实际需要长度[{PacketLength}],当前缓存区长度[{CacheBuffer.Count}]");
        }
    }
}
