using System;
using System.Security.Cryptography;
using QuaternionProtocol.Protocol.Binary;

namespace QuaternionProtocol.Protocol
{
    public class BinaryEncryptableProtocol : BinaryProtocol
    {
        public BinaryEncryptableProtocol(byte[] data, string commandName) : base(data, commandName)
        {
        }

        public BinaryEncryptableProtocol(byte[] data) : base(data)
        {
        }

        public static void GenerateKeys(out byte[] privateKey, out byte[] publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                privateKey = rsa.ExportCspBlob(true);
                publicKey = rsa.ExportCspBlob(false);
            }
        }

        public void Encrypt(byte[] otherSidePublicKey)
        {
            if (otherSidePublicKey == null || otherSidePublicKey.Length == 0) throw new ArgumentNullException(nameof(otherSidePublicKey));
            if (!string.IsNullOrEmpty(CommandName))
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportCspBlob(otherSidePublicKey);
                    Data = rsa.Encrypt(Data, false);
                    GenerateHeader(CommandName);
                }
        }

        public void Decrypt(byte[] myPrivateKey)
        {
            if (myPrivateKey == null || myPrivateKey.Length == 0) throw new ArgumentNullException(nameof(myPrivateKey));

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(myPrivateKey);
                var cleanData = rsa.Decrypt(Data, false);
                Data = cleanData;
            }
        }

      
    }
}