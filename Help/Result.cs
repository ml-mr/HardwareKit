using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Help
{
    /// <summary>
    ///  结果类
    /// </summary>
    internal  class Result
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }

        public Exception Exception { get; set; }

        /// <summary>
        /// 隐式转换为bool类型，表示结果是否成功
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator bool(Result result)
        {
            return result.IsSuccess;
        }
    }

    internal  class Result<T>:Result
    {
        public Result()
        {
            
        }
        public Result(T obj)
        {
            Data = obj;
        }
        public T Data { get; set; }
    }
}
