using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Protocol.Enums
{
    /// <summary>
    /// 通信类型枚举
    /// </summary>
    public enum CommunicationType
    {
        /// <summary>
        /// 串口
        /// </summary>
        SerialPort = 0,

        /// <summary>
        /// Tcp
        /// </summary>
        Tcp=1,

        /// <summary>
        /// ModbusRtu
        /// </summary>
        ModbusRtu=2,

        /// <summary>
        /// ModbusTcp
        /// </summary>
        ModbusTcp=3,
    }
}
