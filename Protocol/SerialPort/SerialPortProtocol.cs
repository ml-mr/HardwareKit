using HardwareKit.Config;
using HardwareKit.Config.SerialPort;
using HardwareKit.Help;
using HardwareKit.Protocol.ParsePacket;
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
            PortClient = new System.IO.Ports.SerialPort();
            PortClient.DataReceived += OnDataReceived;
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
        private System.IO.Ports.SerialPort PortClient { get; set; }

        /// <summary>
        /// 报文队列
        /// </summary>
        private BlockingCollection<byte[]> ResponseQueue { get; set; }


        public override bool IsConnected => PortClient.IsOpen;

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (PortClient.BytesToRead > 0)
            {
                byte[] buffer = new byte[PortClient.BytesToRead];
                PortClient.Read(buffer, 0, buffer.Length);

                //接收数据并入队
                ResponseQueue.Add(buffer);
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
                    PortClient.BaudRate = serialConfig.BaudRate;
                    PortClient.DataBits = serialConfig.DataBits;
                    PortClient.Parity = serialConfig.Parity;
                    PortClient.StopBits = serialConfig.StopBits;
                    PortClient.PortName = serialConfig.PortName;
                    PortClient.ReadTimeout = serialConfig.ReadTimeout;
                    PortClient.WriteTimeout = serialConfig.WriteTimeout;
                    try
                    {
                        //初始化参数
                        Init();

                        PortClient.Open();

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
                PortClient.Close();

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
                        PortClient.Write(buffer, 0, buffer.Length);
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
                        PortClient.Write(buffer, 0, buffer.Length);

                        //等待回复数据，直到超时
                        var task = await Task.WhenAny(ByteResponseTcs.Task, Task.Delay(PortClient.ReadTimeout));
                        if (task.Equals(ByteResponseTcs.Task))
                        {
                            if (task.IsCompleted)
                            {
                                //收到回复
                                return new Result<byte[]>(ByteResponseTcs.Task.Result);
                            }
                            else
                            {
                                return new Result<byte[]>(null, false, "解析报文出现异常") { Exception=task.Exception};
                            }
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
                        PortClient.Write(buffer, 0, buffer.Length);

                        //等待回复数据，直到超时
                        var task = await Task.WhenAny(ByteResponseTcs.Task, Task.Delay(PortClient.ReadTimeout));

                        if (task.Equals(ByteResponseTcs.Task))
                        {
                            if (task.IsCompleted)
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
                                //返回结果
                                return new Result<TResponse>(null, false, "解析报文出现异常") { Exception = task.Exception };
                            }
                        }
                        else
                        {
                            //超时
                            return new Result<TResponse>(null, false, $"等待回复超时,等待时间[{PortClient.ReadTimeout}]ms");
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
                ProcessDataTask = new Task(async () =>
                {
                    try
                    {
                        while (!StopProcessDataToken.IsCancellationRequested)
                        {
                            if (ResponseQueue.TryTake(out byte[] response, Timeout.Infinite, StopProcessDataToken.Token))
                            {
                                var result = ParsePacket.UnPacket(response);
                                if (result)
                                {
                                    ByteResponseTcs.SetResult(result.Data);
                                }
                                else
                                {
                                    await Task.Delay(1);
                                }
                            }
                            else
                            {
                                await Task.Delay(5);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //设置异常
                        ByteResponseTcs.SetException(ex);
                    }
                }, StopProcessDataToken.Token, TaskCreationOptions.LongRunning);
            }
            return ProcessDataTask;
        }

        private void Init()
        {
            ResponseQueue = new BlockingCollection<byte[]>();
            StopProcessDataToken = new CancellationTokenSource();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
