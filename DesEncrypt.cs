using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpTools
{
   public class DesEncrypt
    {
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public string EncryptDes(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = System.Text.Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(encryptString);
                System.Security.Cryptography.DESCryptoServiceProvider dCSP = new System.Security.Cryptography.DESCryptoServiceProvider();
                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public string DecryptDes(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = System.Text.Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                System.Security.Cryptography.DESCryptoServiceProvider DCSP = new System.Security.Cryptography.DESCryptoServiceProvider();
                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                System.Security.Cryptography.CryptoStream cStream = new System.Security.Cryptography.CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return System.Text.Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
    }
}
