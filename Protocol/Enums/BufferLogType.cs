using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Protocol.Enums
{
    /// <summary>
    /// 数据记录类型
    /// </summary>
    [Flags]
    public enum BufferLogType
    {
        /// <summary>
        /// 记录原始字节数据，默认值
        /// </summary>
        Byte = 0x01,

        /// <summary>
        /// 记录十六进制字符串数据，启用后会将原始字节数据转换为十六进制字符串进行记录
        /// </summary>
        HexString = 0x02,
    }
}
