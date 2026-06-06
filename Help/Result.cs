using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Help
{
    /// <summary>
    ///  结果类
    /// </summary>
    public  class Result
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; } = true;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///  详细异常信息
        /// </summary>
        public Exception Exception { get; set; }

        public Result()
        {
            
        }
        public Result(bool isSuccess,string message)
        {
            IsSuccess=isSuccess;
            Message=message;
        }

        /// <summary>
        /// 隐式转换为bool类型，表示结果是否成功
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator bool(Result result)
        {
            return result.IsSuccess;
        }
    }

    /// <summary>
    /// 结果类，包含一个泛型属性Data，用于存储执行结果的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>:Result
    {
        public Result()
        {
            
        }
        public Result(T obj,bool isSuccess,string message)
        {
            Data = obj;
            IsSuccess=isSuccess;
            Message=message;
        }
        public Result(T obj)
        {
            Data = obj;
        }
        public T Data { get; set; }
    }
}
