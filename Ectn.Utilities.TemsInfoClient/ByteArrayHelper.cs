using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Ectn.Utilities.TemsInfo {

    public class ByteArrayHelper {

        /// <summary>
        /// Writes given message (struct) into a byte array.
        /// </summary>
        /// <param name="message"><c>Struct</c> to write into a byte array.</param>
        /// <returns>The byte array containing the <c>Struct</c>.</returns>
        public static byte[] GetByteArray(object message) {
            byte[] result = new byte[Marshal.SizeOf(message)];
            GCHandle handle = GCHandle.Alloc(result, GCHandleType.Pinned);
            try {
                IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(result, 0);
                Marshal.StructureToPtr(message, ptr, true);
            } finally {
                handle.Free();
            }
            return result;
        }

        /// <summary>
        /// Reads a message (struct) out of given byte array.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message (struct) stored in given bytes.</typeparam>
        /// <param name="messageBytes">Byte array containing a message of given type.</param>
        /// <returns>A <c>Struct</c> of given type filled with the information stored in the byte array.</returns>
        public static MessageType GetMessage<MessageType>(byte[] messageBytes) {
            int offset = 0;
            try {
                return GetMessagePart<MessageType>(messageBytes, ref offset);
            } finally {
                if (offset != messageBytes.Length) {
                    throw new ArgumentException(string.Format("Not all bytes ({0}) were needed to restore a message of type '{1}'!", messageBytes.Length, typeof(MessageType).Name));
                }
            }
        }

        /// <summary>
        /// Reads a message part (struct) out of given byte array starting at given offset.
        /// </summary>
        /// <typeparam name="MessagePartType">Type of the message part (struct) stored in given bytes.</typeparam>
        /// <param name="messageBytes">Byte array containing a message part of given type.</param>
        /// <param name="offset">Start index of the message part.</param>
        /// <returns>A <c>Struct</c> of given type filled with the information stored in the byte array.</returns>
        public static MessagePartType GetMessagePart<MessagePartType>(byte[] messageBytes, ref int offset) {
            Type type = typeof(MessagePartType);
            int size = Marshal.SizeOf(type);

            if (size > messageBytes.Length - offset) {
                throw new ArgumentException(string.Format("Not enough bytes ({0}) left to restore message of type '{1}' ({2} bytes)", messageBytes.Length - offset, type.Name, size));
            }

            GCHandle handle = GCHandle.Alloc(messageBytes, GCHandleType.Pinned);
            try {
                IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(messageBytes, offset);
                MessagePartType result = (MessagePartType)Marshal.PtrToStructure(ptr, type);
                offset += size;
                return result;
            } finally {
                handle.Free();
            }
        }

        #region XDocument Handling

        /// <summary>
        /// Loads a XML document from given bytes.
        /// </summary>
        public static XDocument GetXDocument(byte[] buffer) {
            return GetXDocument(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Loads a XML document from given bytes increasing the offset.
        /// </summary>
        /// <param name="offset">Index in the buffer where the document begins.</param>
        /// <param name="count">Size (amount of bytes) of the document.</param>
        public static XDocument GetXDocument(byte[] buffer, ref int offset, int count) {
            XDocument result = GetXDocument(buffer, offset, count);
            offset += count;
            return result;
        }

        /// <summary>
        /// Loads a XML document from given bytes.
        /// </summary>
        /// <param name="offset">Index in the buffer where the document begins.</param>
        /// <param name="count">Size (amount of bytes) of the document.</param>
        public static XDocument GetXDocument(byte[] buffer, int offset, int count) {
            if (count > 0) {
                using (MemoryStream stream = new MemoryStream(buffer, offset, count)) {
                    return XDocument.Load(stream);
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Saves given document into a byte array.
        /// </summary>
        public static byte[] GetBytes(XDocument document) {
            if (document != null) {
                using (MemoryStream stream = new MemoryStream()) {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings() { Encoding = new UTF8Encoding(false), Indent = true })) {
                        document.Save(xmlWriter);
                    }
                    return stream.ToArray();
                }
            } else {
                return new byte[0];
            }
        }

        #endregion XDocument Handling
    }
}