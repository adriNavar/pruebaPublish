using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Geosit.Core.Utils.Crypto
{
    public static class RijndaelCypher
    {
        public static string EncryptText(string text, string key = null)
        {
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, initialize(key).CreateEncryptor(), CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(text);
                writer.Close();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        public static string DecryptText(string text, string key = null)
        {
            using (var memoryStream = new MemoryStream(Convert.FromBase64String(text)))
            using (var cryptoStream = new CryptoStream(memoryStream, initialize(key).CreateDecryptor(), CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }

        private static RijndaelManaged initialize(string key = null)
        {
            int keySize = 256;
            byte[] keyToBytes()
            {
                byte[] bytes = Encoding.UTF8.GetBytes(key);
                if ((bytes.Length * 8) != keySize)
                {
                    throw new ArgumentOutOfRangeException("key", "La clave no tiene un tamaño válido, 256 bits.");
                }
                return bytes;
            }
            return new RijndaelManaged()
            {
                Mode = CipherMode.ECB,
                KeySize = keySize,
                Key = string.IsNullOrEmpty(key) ? Convert.FromBase64String("xcZ7W7DYkrviDHUXsRIBIqDAqPp19NUzkJncLDV3NTo=") : keyToBytes(),
                IV = Convert.FromBase64String("RlwBNwjRubhB+k/YrrkwJg==")
            };
        }
    }
}