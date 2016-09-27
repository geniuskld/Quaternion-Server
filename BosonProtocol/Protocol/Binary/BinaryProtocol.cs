using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using QuaternionProtocol.Serializers;

namespace QuaternionProtocol.Protocol.Binary
{
    public class BinaryProtocol : ProtocolBase<byte[]>, IEnumerable<byte>
    {
        private byte[] _header;
        protected readonly string CommandName;
        public static int HeaderLength => PacketSizeLength + CommandLength + CheckSumLength;
        private const int CommandLength = 6;
        private const int CheckSumLength = 6;
        private const int PacketSizeLength = 4;
        

        public BinaryProtocol(byte[] data, string commandName)
        {
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentNullException(nameof(commandName));
            if (data == null || data.Length == 0) throw new ArgumentNullException(nameof(data));

            _header = new byte[HeaderLength];
            Data = data;
            GenerateHeader(commandName);
            CommandName = commandName;
        }

        //Use when got data from socket
        public BinaryProtocol(byte[] data)
        {
          FillFromData(data);
        }

        private void FillFromData(byte[] data)
        {
            if (data.Length < HeaderLength + 1) throw new InvalidDataException("Data length is shorter that header length");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            _header = new byte[HeaderLength];
            var bodyLength = data.Length - HeaderLength;
            Data = new byte[bodyLength];

            Buffer.BlockCopy(data, 0, _header, 0, HeaderLength);
            Buffer.BlockCopy(data, HeaderLength, Data, 0, bodyLength);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Have to be serializeable with custom genericSerializer</typeparam>
        /// <param name="obj"></param>
        /// <param name="commandName"></param>
        /// <param name="genericSerializer"></param>
        public static BinaryProtocol FromData<T>(T obj, string commandName, IGenericSerializer<T, byte[]> genericSerializer) where T : new()
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(commandName)) throw new ArgumentNullException(nameof(commandName));
            if (genericSerializer == null) throw new ArgumentNullException(nameof(genericSerializer));

            var body = genericSerializer.Serialize(obj);
            return new BinaryProtocol(body, commandName);
        }

        public static BinaryProtocol FromData<T>(T obj, IGenericSerializer<T, byte[]> genericSerializer) where T:new()
        {
            return FromData(obj, obj.GetType().Name, genericSerializer);
        }

        public byte[] GetCommandBytes()
        {
            var commandHash = new byte[CommandLength];
            Buffer.BlockCopy(_header,PacketSizeLength,commandHash,0,CommandLength);
            return commandHash;
        }
        public static byte[] GetCheckSumFromPacket(byte[] bytes)
        {
            var checksumBytes = new byte[CommandLength];
            Buffer.BlockCopy(bytes, CommandLength+PacketSizeLength, checksumBytes, 0, CheckSumLength);
            return checksumBytes;
        }

        public bool IsCheckSumValid()
        {
            var sum = GetCheckSumFromPacket(_header);
            var currentDataTailChecksum = CalcCheckSum(Data);

            if (sum.Length != currentDataTailChecksum.Length) return false;

            for (var i = 0; i < sum.Length; i++)
            {
                if (sum[i] != currentDataTailChecksum[i]) return false;
            }
            return true;
        }

        public byte[] CalcCheckSum(byte[] bytes)
        {
            var sumBytes = HashHelper.GetCheckSum(bytes);
            var wraped = StreamHelper.WrapBytes(sumBytes, CheckSumLength);
            return wraped;
        }

        protected void GenerateHeader(string commandName)
        {
            //Header
            var commandBytes = StreamHelper.WrapBytes(HashHelper.GeHash(commandName), CommandLength);
            var checksumBytes = CalcCheckSum(Data);
            var lenghtBytes = BitConverter.GetBytes(HeaderLength + Data.Length);

            Buffer.BlockCopy(lenghtBytes, 0, _header, 0, 4);

            var step = PacketSizeLength;
            commandBytes.CopyTo(_header, step);
            step += CommandLength;

            checksumBytes.CopyTo(_header, step);
            step += CheckSumLength;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return (IEnumerator<byte>)Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public override byte[] GetDataToSend()
        {
            var len = HeaderLength + Data.Length;
            var headerAndBody = new byte[len];
            Buffer.BlockCopy(_header, 0, headerAndBody, 0, HeaderLength);
            Buffer.BlockCopy(Data, 0, headerAndBody, HeaderLength, Data.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(headerAndBody);

            return headerAndBody;
        }

        public override byte[] GetBody()
        {
            return Data;
        }

        public override byte[] CleanData(byte[] receivedData)
        {
            FillFromData(receivedData);
            return Data;
        }

        public static int GetMessageLength(byte[] message)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(message);

            var msgLengthBytes = new byte[4];
            Buffer.BlockCopy(message,0,msgLengthBytes,0,4);
            return BitConverter.ToInt32(msgLengthBytes, 0);
        }
    }
}