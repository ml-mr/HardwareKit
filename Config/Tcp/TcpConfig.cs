namespace HardwareKit.Config.Tcp
{
    /// <summary>
    ///  Tcp配置
    /// </summary>
    internal sealed class TcpConfig:ConfigBase
    {
        /// <summary>
        /// 远程主机IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 远程主机端口号
        /// </summary>
        public int Port { get; set; }
    }
}
