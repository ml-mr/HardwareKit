using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Config
{
    /// <summary>
    ///  配置基础接口
    /// </summary>
    internal interface IConfig
    {
        /// <summary>
        /// 读取超时时间，单位毫秒
        /// </summary>
        uint ReadTimeout { get; set; }

        /// <summary>
        /// 写入超时时间，单位毫秒
        /// </summary>
        uint WriteTimeout { get; set; }

    }
}
