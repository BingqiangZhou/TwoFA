using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TwoFA.Utils.ToolsClass
{
    public class GenerateCode
    {
        public static int GenerateEmailCode(int minValue,int maxValue)
        {
            return new Random().Next(minValue, maxValue);
        }

        public static string GenerateTwoFAKey(string mid, string token)
        {
            string str;
            using (var sha = new SHA1Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(token);
                byte[] hash = sha.ComputeHash(textData);
                StringBuilder displayString = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    displayString.Append(hash[i].ToString("X2"));
                }
                str = displayString.ToString();
            };
            
            return str;
        }

        public static int GenerateTwoFACode(string key,int counter)
        {
            HMACSHA1 hMACSHA = new HMACSHA1(Encoding.Default.GetBytes(counter.ToString()));

            //HMACMD5 provider = new HMACMD5(Encoding.UTF8.GetBytes(password));            
            byte[] hashedPassword = hMACSHA.ComputeHash(Encoding.Default.GetBytes(key));
            //转为16进制
            StringBuilder displayString = new StringBuilder();
            for (int i = 0; i < hashedPassword.Length; i++)
            {
                displayString.Append(hashedPassword[i].ToString("X2"));
            }
            int offset = hashedPassword[19] & 0xf;
            var v = (hashedPassword[offset] & 0x7f) << 24 |
            (hashedPassword[offset + 1] & 0xff) << 16 |
            (hashedPassword[offset + 2] & 0xff) << 8 |
            (hashedPassword[offset + 3] & 0xff);
            double vv = v % (10e5);
            vv = vv * Math.Pow(10,6 - vv.ToString().Length);
            //Console.WriteLine("hash-value：{0}\nverify-code:{1}\noffset:{2}", displayString, v % (10e5), offset);
            return (int)vv;
        }

        public static byte[] EncryptAes(string plainText,byte[] key,byte[] iv)
        {
            //检查参数
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            byte[] encrypted;

            //创建AesManage对象，设置key和iv(initialization vector)
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                //创建一个加密器执行流转换
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                //创建用于加密的流
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //将数据写入流
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;

        }

        public static string DecryptAes(byte[] cipherText,byte[] key,byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
