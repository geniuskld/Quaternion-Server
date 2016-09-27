using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QuaternionProtocol
{
    public static class HashHelper
    {
        private static readonly SHA1 HashMehod = SHA1.Create();

        public static byte[] GeHash(string str)
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(str)))
            {
                return HashMehod.ComputeHash(stream);
            }
        }

        public static byte[] GetCheckSum(byte[] bytes)
        {
            return HashMehod.ComputeHash(bytes);
        }
    }
}