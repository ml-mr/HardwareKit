using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HardwareKit.Help
{
    /// <summary>
    /// Json 帮助类
    /// </summary>
    internal sealed class JsonHelp
    {
        private readonly static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
        };

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static Result Serialize(object obj, string fileName)
        {
            try
            {
                if (obj == null)
                {
                    return new Result()
                    {
                        IsSuccess = false,
                        Message = $"对象不能为空:[{nameof(obj)}]"
                    };
                }
                using (var stream = new StreamWriter(fileName))
                {
                    var json = JsonConvert.SerializeObject(obj, settings);
                    stream.Write(json);
                }
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Exception = ex
                };
            }
        }

        /// <summary>
        ///  反序列化
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static Result<T> DeSerialize<T>(string fileName) where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return new Result<T>()
                    {
                        IsSuccess = false,
                        Message = $"文件名不能为空:[{nameof(fileName)}]"
                    };
                }
                T obj;
                using (var stream = new StreamReader(fileName))
                {
                    var json = stream.ReadToEnd();
                    obj= JsonConvert.DeserializeObject<T>(json, settings);
                }
                return new Result<T>(obj);
            }
            catch (Exception ex)
            {
                return new Result<T>()
                {
                    Data = null,
                    Exception = ex,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
