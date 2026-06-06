using HardwareKit.Config;
using HardwareKit.Protocol;
using HardwareKit.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Communication
{
    /// <summary>
    /// 创建通信客户端的扩展类
    /// </summary>
    internal static class CreateClientExtension
    {
        public static IClient CreateClient(this IClient client,IConfig config, ProtocolOptions options)
        {
            if (options == null)
            { 
                throw new ArgumentNullException(nameof(options), "协议选项不能为空");
            }


            switch (options.CommunicationType)
            {
                case CommunicationType.SerialPort:
                    break;
                case CommunicationType.Tcp:
                    break;
                case CommunicationType.ModbusRtu:
                    break;
                case CommunicationType.ModbusTcp:
                    break;
                default:
                    break;
            }
        }
    }
}
