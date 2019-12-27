using System.IO;
using System.Security.Cryptography;
using CxSolution.CxRouRou.Collections;

namespace CxSolution.CxRouRou.Security
{
    /// <summary>
    /// AES秘钥信息
    /// </summary>
    public struct AESKeyInfo
    {
        /// <summary>
        /// 秘钥
        /// </summary>
        public byte[] Key;
        /// <summary>
        /// 向量
        /// </summary>
        public byte[] IV;
    }
    /// <summary>
    /// AES加密
    /// </summary>
    public class CxAES
    {
        /// <summary>
        /// 创建AES秘钥信息
        /// </summary>
        /// <param name="keySize">密匙大小(以位为单位)</param>
        /// <returns></returns>
        public static AESKeyInfo CreateAESKey(int keySize = 256)
        {
            var aes = new RijndaelManaged()
            {
                KeySize = keySize,
            };
            aes.GenerateKey();
            aes.GenerateIV();
            return new AESKeyInfo()
            {
                Key = aes.Key,
                IV = aes.IV,
            };
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="iv">向量</param>
        /// <param name="data">待加密数据</param>
        /// <param name="cipherMode">运算模式</param>
        /// <param name="paddingMode">填充模式</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] key, byte[] iv, byte[] data, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            using (var rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Key = key;
                rijndaelManaged.IV = iv;
                rijndaelManaged.Mode = cipherMode;
                rijndaelManaged.Padding = paddingMode;

                var cryptoTransform = rijndaelManaged.CreateEncryptor();
                var _data = cryptoTransform.TransformFinalBlock(data, 0, data.Length);
                return _data;
            }
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="iv">向量</param>
        /// <param name="data">待加密数据</param>
        /// <param name="cipherMode">运算模式</param>
        /// <param name="paddingMode">填充模式</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] data, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            using (var rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Key = key;
                rijndaelManaged.IV = iv;
                rijndaelManaged.Mode = cipherMode;
                rijndaelManaged.Padding = paddingMode;

                var cryptoTransform = rijndaelManaged.CreateDecryptor();
                var _data = cryptoTransform.TransformFinalBlock(data, 0, data.Length);
                return _data;
            }
        }
    }
}
