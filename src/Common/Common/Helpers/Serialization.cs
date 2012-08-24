using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Common.Helpers
{
    public enum SerializationType
    {
        Xml,
        Binary
    }

    public static class Serialization
    {
        /// <summary>
        /// Serializes the target into a string of the type specified
        /// </summary>
        public static string Serialize<T>(T target, SerializationType type = SerializationType.Xml)
        {
            using (var ms = new MemoryStream())
            {
                if (type == SerializationType.Xml)
                {
                    var xml = new XmlSerializer(typeof(T));
                    xml.Serialize(ms, target);
                    return Encoding.UTF8.GetString(ms.GetBuffer());
                }
                else if (type == SerializationType.Binary)
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(ms, target);
                    return Convert.ToBase64String(ms.GetBuffer());
                }
                else
                {
                    throw new NotImplementedException("No implementation for this type of serialization");
                }
            }
        }

        /// <summary>
        /// Deserializes an object stored as a string
        /// </summary>
        public static T Deserialize<T>(string graph, SerializationType type = SerializationType.Xml)
        {
            //using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(graph)))
            //{
            //    return Deserialize<T>(ms, type);
            //}
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(graph)))
            {
                return Deserialize<T>(ms, type);
            }
        }

        public static T Deserialize<T>(byte[] graph, SerializationType type = SerializationType.Xml)
        {
            using (var ms = new MemoryStream(graph))
            {
                return Deserialize<T>(ms, type);
            }
        }

        /// <summary>
        /// Deserializes a serialized object that is stored in a stream
        /// </summary>
        public static T Deserialize<T>(Stream stream, SerializationType type = SerializationType.Xml)
        {
            if (type == SerializationType.Xml)
            {
                var xml = new XmlSerializer(typeof(T));
                return (T)xml.Deserialize(stream);
            }
            else if (type == SerializationType.Binary)
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
            else
            {
                throw new NotImplementedException("No implementation for this type of deserialization");
            }
        }
    }
}
