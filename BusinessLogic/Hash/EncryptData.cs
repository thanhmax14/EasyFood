using BusinessLogic.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Hash
{
    public class EncryptData
    {

        public static byte[] ObjectToByteArray<T>(T obj)
        {
            if (obj == null) return null;
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        /// <summary>
        /// Chuyển đổi mảng byte thành object sử dụng System.Text.Json
        /// </summary>
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            if (arrBytes == null || arrBytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(arrBytes);
        }
        public static bool EncryptQuestions_SaveToFile(string fname, byte[] data, string key)
        {
            bool result;
            try
            {
                FileStream fileStream = new FileStream(fname, FileMode.Create, FileAccess.Write);
                CryptoStream cryptoStream = new CryptoStream(fileStream, new DESCryptoServiceProvider
                {
                    Key = Encoding.ASCII.GetBytes(key),
                    IV = Encoding.ASCII.GetBytes(key)
                }.CreateEncryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.Close();
                fileStream.Close();
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static byte[] DecryptQuestions_FromFile(string fname, string key)
        {
            byte[] result;
            try
            {
                FileStream fileStream = new FileStream(fname, FileMode.Open, FileAccess.Read);
                CryptoStream cryptoStream = new CryptoStream(fileStream, new DESCryptoServiceProvider
                {
                    Key = Encoding.ASCII.GetBytes(key),
                    IV = Encoding.ASCII.GetBytes(key)
                }.CreateDecryptor(), CryptoStreamMode.Read);
                byte[] array = new byte[fileStream.Length];
                int num = cryptoStream.Read(array, 0, (int)fileStream.Length);
                cryptoStream.Close();
                fileStream.Close();
                result = array;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string Encryption(byte[] data, string key)
        {
            DES des = new DESCryptoServiceProvider
            {
                Key = NormalizeKey(key),
                IV = NormalizeKey(key),
                Padding = PaddingMode.PKCS7
            };

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray()); // Xuất ra Base64
        }

        public static byte[] Decryption(string encryptedData, string key)
        {
            DES des = new DESCryptoServiceProvider
            {
                Key = NormalizeKey(key),
                IV = NormalizeKey(key),
                Padding = PaddingMode.PKCS7
            };

            byte[] encryptedBytes = Convert.FromBase64String(encryptedData); // Chuyển lại từ Base64

            using MemoryStream memoryStream = new MemoryStream(encryptedBytes);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(), CryptoStreamMode.Read);

            MemoryStream output = new MemoryStream();
            cryptoStream.CopyTo(output);
            return output.ToArray();
        }
        public static string GetMD5(string msg)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] bytes = md.ComputeHash(Encoding.Unicode.GetBytes(msg));
            return Encoding.Unicode.GetString(bytes);
        }
        private static byte[] NormalizeKey(string key)
        {
            return Encoding.ASCII.GetBytes(key.PadRight(8, '0').Substring(0, 8));
        }


/*
        var originalData = "thanh";

        // Chuyển object thành byte array

        var str = EncryptData.ObjectToByteArray(originalData);
        Console.WriteLine($"Original: {originalData}");


        var encryptedData = EncryptData.Encryption(str, "12345678");
        Console.WriteLine($"Encrypted: {encryptedData}");

        // Giải mã
        var decryptedBytes = EncryptData.Decryption(encryptedData, "12345678");
        var decryptedData = EncryptData.ByteArrayToObject<string>(decryptedBytes);
        Console.WriteLine($"Decrypted: {decryptedData}");*/

    }
}