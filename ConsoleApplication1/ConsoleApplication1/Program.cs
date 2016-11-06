using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Crypto();
            Console.ReadKey();
            byte[] kiv = new byte[16];
            RandomNumberGenerator.Create().GetBytes(kiv);
            string encrypted = Encrypt("Yeah!", kiv, kiv);
            Console.WriteLine(encrypted); // R1/5gYvcxyR2vzPjnT7yaQ==
            string decrypted = Decrypt(encrypted, kiv, kiv);
            Console.WriteLine(decrypted); // Yeah!
            Console.ReadKey();
          

        }

        static async void Crypto()
        {             // Use default key/iv for demo.
            using (Aes algorithm = Aes.Create())
            {
                using (ICryptoTransform encryptor = algorithm.CreateEncryptor())
                using (Stream f = File.Create("serious.bin"))
                using (Stream c = new CryptoStream(f, encryptor, CryptoStreamMode.Write))
                using (Stream d = new DeflateStream(c, CompressionMode.Compress))
                using (StreamWriter w = new StreamWriter(d))
                    await w.WriteLineAsync("Small and secure!");
                using (ICryptoTransform decryptor = algorithm.CreateDecryptor())
                using (Stream f = File.OpenRead("serious.bin"))
                using (Stream c = new CryptoStream(f, decryptor, CryptoStreamMode.Read))
                using (Stream d = new DeflateStream(c, CompressionMode.Decompress))
                using (StreamReader r = new StreamReader(d))
                    Console.WriteLine(await r.ReadLineAsync()); // Small and secure!
            }
        }
        public static string Encrypt(string data, byte[] key, byte[] iv)
        {
            return Convert.ToBase64String(
            Encrypt(Encoding.UTF8.GetBytes(data), key, iv));
        }
        public static string Decrypt(string data, byte[] key, byte[] iv)
        {
            return Encoding.UTF8.GetString(
            Decrypt(Convert.FromBase64String(data), key, iv));
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
                return Crypt(data, encryptor);
        }
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes algorithm = Aes.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
                return Crypt(data, decryptor);
        }
        static byte[] Crypt(byte[] data, ICryptoTransform cryptor)
        {
            MemoryStream m = new MemoryStream();
            using (Stream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
                c.Write(data, 0, data.Length);
            return m.ToArray();
        }

    }
}
