using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareKit.Help.Bytes
{
    internal static class BytesExtension
    {
        /// <summary>
        /// 将字节数组转换为十六进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将十六进制字符串转换为字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToBytes(this string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return new byte[0];
            }
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("十六进制字符串长度必须为偶数");
            }
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// 将字节数组转换为字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="encoding">编码 </param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes,Encoding encoding)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }
            if (encoding == null)
            {
                encoding=Encoding.UTF8;
            }

            return encoding.GetString(bytes);
        }
    }
}
