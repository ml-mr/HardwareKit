using HardwareKit.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Communication
{
    /// <summary>
    /// 通信客户端
    /// </summary>
    public interface IClient
    {

        event Action ConnectSuccessEvt;

        event Action ConnectFailedEvt;

        event Action DisconnectSuccessEvt;

        event Action DisconnectFailedEvt;
    
    }
}
