using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Config
{
    /// <summary>
    /// 配置抽象基类
    /// </summary>
    internal abstract class ConfigBase : IConfig
    {
        public int ReadTimeout { get; set; } = 2000;
        public int WriteTimeout { get; set; } = 2000;
    }
}
