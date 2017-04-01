using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Tumbler.Addin.Formatters
{
    /// <summary>
    /// 消息格式化器。
    /// </summary>
    public class MessageFormatter
    {
        #region Fields

        /// <summary>
        /// 地址字节长度。
        /// </summary>
        public const Int32 AddressLength = 32;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core 实例。
        /// </summary>
        public MessageFormatter()
        {

        }

        #endregion



        #region Methods

        #region Public

        /// <summary>
        /// 序列化消息。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <returns>字节数组。</returns>
        public virtual Byte[] Serialize(Message message)
        {
            Int32 contentLength = 0;
            Byte[] contentBytes = null;
            if (message.Content != null)
            {
                String json = JsonConvert.SerializeObject(message.Content);
                contentBytes = Encoding.UTF8.GetBytes(json);
                contentLength = contentBytes.Length;
            }
            using (MemoryStream stream = new MemoryStream(71 + contentLength))
            {
                stream.WriteByte(0x5A);
                stream.Write(Encoding.ASCII.GetBytes(Version), 0, 3);
                stream.Write(Encoding.ASCII.GetBytes(message.Destination), 0, AddressLength);
                stream.Write(Encoding.ASCII.GetBytes(message.Source), 0, AddressLength);
                stream.Write(BitConverter.GetBytes(contentLength), 0, 4);
                if (contentLength != 0)
                {
                    stream.Write(contentBytes, 0, contentLength);
                }
                stream.WriteByte(0xA5);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化消息。
        /// </summary>
        /// <param name="data">字节数组。</param>
        /// <returns>消息。</returns>
        public virtual Message Deserialize(Byte[] data)
        {
            Message message = new Message();
            Byte[] temp = null;
            using (MemoryStream stream = new MemoryStream(data))
            {
                if (stream.ReadByte() != 0x5A) throw new FormatException("Bad starting value.");
                temp = new Byte[3];
                stream.Read(temp, 0, 3);
                if (Encoding.ASCII.GetString(temp) != Version)
                {
                    throw new FormatException("Bad version.");
                }

                temp = new Byte[AddressLength];
                stream.Read(temp, 0, AddressLength);
                message.Destination = Encoding.ASCII.GetString(temp);
                stream.Read(temp, 0, AddressLength);
                message.Source = Encoding.ASCII.GetString(temp);

                temp = new Byte[4];
                stream.Read(temp, 0, 4);
                Int32 contentLength = BitConverter.ToInt32(temp, 0);

                if (contentLength != 0)
                {
                    temp = new Byte[contentLength];
                    stream.Read(temp, 0, contentLength);
                    String str = Encoding.UTF8.GetString(temp);
                    message.Content = JsonConvert.DeserializeObject(str);
                }

                if (stream.ReadByte() != 0xA5) throw new FormatException("Bad end value.");

                return message;
            }
        }

        #endregion

        #endregion
    }
}
