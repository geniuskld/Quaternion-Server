using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using QuaternionProtocol;
using Tests.Emulators.Transport;
using Transport;
using Xunit;

namespace Tests.Transport
{
    public class BaseEmulatorTests
    {
       
    }

    [Serializable]
    public class CustomSerializationObject
    {
        public CustomSerializationObject()
        {
            Text = "Test";
            Integer = 42;
            Dictionary.Add(Text, Integer);
            List.Add(Text);
        }
        public string Text { get; set; }
        public int Integer { get; set; }
        public Dictionary<string, int> Dictionary = new Dictionary<string, int>();
        public List<string> List = new List<string>();
    }

    [Serializable]
    public class CustomSerializationObject2
    {
        public CustomSerializationObject2()
        {
            Text = "Test";
            Integer = 42;
            Dictionary.Add(Text, Integer);
            List.Add(Text);
        }
        public string Text { get; set; }
        public int Integer { get; set; }
        public Dictionary<string, int> Dictionary = new Dictionary<string, int>();
        public readonly List<string> List = new List<string>();
        public byte[] Serialize()
        {
          var serializer = new BinaryFormatter();
            using (var outputStream = new MemoryStream())
            {
                serializer.Serialize(outputStream,this);
                return StreamHelper.GetBytes(outputStream);
            }
        }

        public object Deserialize(byte[] stream)
        {
            var serializer = new BinaryFormatter();
            using (var outputStream = new MemoryStream(stream))
            {
                return serializer.Deserialize(outputStream);
            }
        }
    }
}