using HardwareKit.Config;
using HardwareKit.Config.SerialPort;
using HardwareKit.Help;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareKit.Protocol.SerialPort
{
    /// <summary>
    /// 串口协议实现类
    /// </summary>
    internal sealed class SerialPortProtocol : ProtocolBase
    {
        public SerialPortProtocol(IConfig config, ProtocolOptions options) : base(config, options)
        {
            _serialPort = new System.IO.Ports.SerialPort();
            _serialPort.DataReceived += OnDataReceived;
            ResponseQueue = new BlockingCollection<byte>();
            StopProcessDataToken = new CancellationTokenSource();
        }

        /// <summary>
        /// 处理数据的任务，用于从ResponseQueue中读取数据并进行处理
        /// </summary>
        private Task ProcessDataTask { get; set; }

        /// <summary>
        /// 停止处理数据令牌
        /// </summary>
        private CancellationTokenSource StopProcessDataToken { get; set; }

        /// <summary>
        /// 串口对象
        /// </summary>
        private System.IO.Ports.SerialPort _serialPort { get; set; }

        /// <summary>
        /// 报文队列
        /// </summary>
        private BlockingCollection<byte> ResponseQueue { get; set; }


        public override bool IsConnected => _serialPort.IsOpen;

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort.BytesToRead > 0)
            {
                byte[] buffer = new byte[_serialPort.BytesToRead];
                _serialPort.Read(buffer, 0, buffer.Length);

                //接收数据并入队
                foreach (byte data in buffer)
                {
                    ResponseQueue.Add(data);
                }
            }
        }

        public override Task<Result> ConnectAsync()
        {
            if (IsConnected)
            {
                return Task.FromResult(new Result());
            }
            else
            {
                var serialConfig = Config as SerialPortConfig;
                if (serialConfig != null)
                {
                    _serialPort.BaudRate = serialConfig.BaudRate;
                    _serialPort.DataBits = serialConfig.DataBits;
                    _serialPort.Parity = serialConfig.Parity;
                    _serialPort.StopBits = serialConfig.StopBits;
                    _serialPort.PortName = serialConfig.PortName;
                    _serialPort.ReadTimeout = serialConfig.ReadTimeout;
                    _serialPort.WriteTimeout = serialConfig.WriteTimeout;
                    try
                    {
                        //初始化参数
                        Init();

                        _serialPort.Open();

                        //启动处理数据任务
                        if (ProcessDataTask == null)
                        {
                            ProcessDataTask = ProcessData();
                            ProcessDataTask.Start();
                        }

                        return Task.FromResult(new Result());
                    }
                    catch (Exception ex)
                    {
                        return Task.FromResult(new Result(false, $"连接串口失败，异常信息：{ex.Message}"));
                    }

                }
                else
                {
                    return Task.FromResult(new Result(false, "配置错误，无法转换为SerialPortConfig"));
                }
            }
        }

        public override async Task<Result> DisconnectAsync()
        {
            if (IsConnected)
            {
                _serialPort.Close();

                //停止任务
                StopProcessDataToken.Cancel();

                //等待任务完成
                await ProcessDataTask;

                //资源清理
                StopProcessDataToken.Dispose();
                StopProcessDataToken = null;
                ResponseQueue = null;
                ProcessDataTask = null;

            }
            return new Result();
        }

        public override async Task<Result> SendAsync(byte[] buffer)
        {
            if (IsConnected)
            {
                if (buffer == null || buffer.Count() == 0)
                {
                    return new Result(false, "发送数据不能为空");
                }
                else
                {
                    try
                    {
                        await SyncSem.WaitAsync();
                        _serialPort.Write(buffer, 0, buffer.Length);
                        return new Result();
                    }
                    catch (Exception ex)
                    {
                        return new Result(false, $"发送数据失败，异常信息：{ex.Message}");
                    }
                    finally
                    {
                        SyncSem.Release();
                    }
                }
            }
            else
            {
                return new Result(false, "串口未连接");
            }
        }

        public override async Task<Result<byte[]>> SendWaitReplyAsync(byte[] buffer)
        {
            if (IsConnected)
            {
                if (buffer == null || buffer.Count() == 0)
                {
                    return new Result<byte[]>(null, false, "发送数据不能为空");
                }
                else
                {
                    try
                    {
                        await SyncSem.WaitAsync();
                        ByteResponseTcs = new TaskCompletionSource<byte[]>();
                        _serialPort.Write(buffer, 0, buffer.Length);

                        //等待回复数据，直到超时
                        var task = await Task.WhenAny(ByteResponseTcs.Task, Task.Delay(_serialPort.ReadTimeout));
                        if (task.Equals(ByteResponseTcs.Task))
                        {

                            //收到回复
                            return new Result<byte[]>(ByteResponseTcs.Task.Result);
                        }
                        else
                        {
                            //超时
                             return new Result<byte[]>(null, false, $"等待回复超时,等待时间[{Config.ReadTimeout}]ms");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<byte[]>(null, false, $"发送数据失败，异常信息：{ex.Message}");
                    }
                    finally
                    {
                        SyncSem.Release();
                    }
                }
            }
            else
            {
                return new Result<byte[]>(null, false, "串口未连接");
            }
        }

        public override async Task<Result<TResponse>> SendWaitReplyAsync<TResponse>(byte[] buffer)
        {
            if (IsConnected)
            {
                if (buffer == null || buffer.Count() == 0)
                {
                    return new Result<TResponse>(null, false, "发送数据不能为空");
                }
                else
                {
                    try
                    {
                        await SyncSem.WaitAsync();
                        ByteResponseTcs = new TaskCompletionSource<byte[]>();
                        _serialPort.Write(buffer, 0, buffer.Length);

                        //等待回复数据，直到超时
                        var task = await Task.WhenAny(ByteResponseTcs.Task, Task.Delay(_serialPort.ReadTimeout));

                        if (task.Equals(ByteResponseTcs.Task))
                        {
                            //收到回复
                            byte[] response = ByteResponseTcs.Task.Result;

                            //创建报文
                            TResponse responseObj = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[] { response });

                            //返回结果
                            return new Result<TResponse>(responseObj);
                        }
                        else
                        {
                            //超时
                            return new Result<TResponse>(null, false, $"等待回复超时,等待时间[{_serialPort.ReadTimeout}]ms");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<TResponse>(null, false, $"发送数据失败，异常信息：{ex.Message}");
                    }
                    finally
                    {
                        SyncSem.Release();
                    }
                }
            }
            else
            {
                return new Result<TResponse>(null, false, "串口未连接");
            }
        }

        /// <summary>
        /// 处理数据线程,因为该线程是一个长时间运行线程，即不使用线程池中的线程，所以使用TaskCreationOptions.LongRunning选项创建任务
        /// </summary>
        /// <returns></returns>
        private Task ProcessData()
        {
            if (ProcessDataTask != null)
            {
                return ProcessDataTask;
            }
            else
            {
                ProcessDataTask = new Task(() =>
                {
                    while (!StopProcessDataToken.IsCancellationRequested)
                    {
                        //1、通过分隔符进行解包

                        //2、通过长度进行解包

                        //3、直接返回
                    }
                }, StopProcessDataToken.Token, TaskCreationOptions.LongRunning);
            }
            return ProcessDataTask;
        }

        private void Init()
        {
            if (ResponseQueue == null)
            {
                ResponseQueue = new BlockingCollection<byte>();
            }
            if (StopProcessDataToken == null)
            {
                StopProcessDataToken = new CancellationTokenSource();
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
