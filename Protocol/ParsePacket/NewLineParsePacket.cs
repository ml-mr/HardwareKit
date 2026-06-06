using HardwareKit.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HardwareKit.Protocol.ParsePacket
{
    internal sealed class NewLineParsePacket : IParsePacket
    {
        public NewLineParsePacket(string newLine)
        {
            NewLine = newLine;
            CacheBuffer=new List<byte>();
        }

        /// <summary>
        /// 换行符
        /// </summary>
        private string NewLine { get; set; }

        /// <summary>
        /// 缓存Buffer，用于存储未处理完的数据，直到接收到完整的数据包（即包含换行符）为止
        /// </summary>
        private List<byte> CacheBuffer { get; set; }

        public Result<byte[]> UnPacket(byte[] packet)
        {
            //1、先将换行符转换为字节数组
            byte[] newLineBytes = Encoding.UTF8.GetBytes(NewLine);

            //2、开始进行判断
            if (packet!=null && packet.Count()>0)
            {
                CacheBuffer.AddRange(packet);
            }

            //3、查找换行符
            int startIndex = 0;
            while (startIndex < CacheBuffer.Count)
            {
                int newLineIndex = Array.IndexOf(CacheBuffer.ToArray(), newLineBytes[0], startIndex);
                if (newLineIndex == -1)
                {
                    //如果没有找到换行符，则退出
                    return new Result<byte[]>(null, false, "未找到完整数据包");
                }

                //检查换行符后面是否还有数据
                if (newLineIndex + newLineBytes.Length < CacheBuffer.Count)
                {
                    //如果有数据，说明找到了一帧完整的数据包
                    byte[] completePacket = new byte[newLineIndex + newLineBytes.Length];
                    Buffer.BlockCopy(CacheBuffer.ToArray(), 0, completePacket, 0, completePacket.Length);
                    CacheBuffer.Clear();
                    return new Result<byte[]>(completePacket);
                }

                //继续查找下一个换行符
                startIndex = newLineIndex + 1;
            }

            return new Result<byte[]>(null, false, "未找到完整数据包");
        }
    }
}
