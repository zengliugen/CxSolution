using System;
using System.Security.Cryptography;

namespace CxSolution.CxRouRou.Security
{
    /// <summary>
    /// RSA秘钥信息
    /// </summary>
    public struct RSAKeyInfo
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public string Public { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string Private { get; set; }
    }
    /// <summary>
    /// RSA加密
    /// </summary>
    public class CxRSA
    {
        /// <summary>
        /// 生成RSA秘钥信息
        /// </summary>
        /// <returns></returns>
        public static RSAKeyInfo CreateRSAKey()
        {
            var rsa = new RSACryptoServiceProvider();
            return new RSAKeyInfo { Private = rsa.ToXmlString(true), Public = rsa.ToXmlString(false) };
        }
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="data">待加密数据</param>
        /// <param name="fOAEP">是否使用OAEP填充方式</param>
        /// <returns>加密后的数据</returns>
        public static byte[] Encrypt(string publicKey, byte[] data, bool fOAEP = false)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                var _data = rsa.Encrypt(data, fOAEP);
                return _data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="data">待解密数据</param>
        /// <param name="fOAEP">是否使用OAEP填充方式</param>
        /// <returns>解密后的数据</returns>
        public static byte[] Decrypt(string privateKey, byte[] data, bool fOAEP = false)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                var _data = rsa.Decrypt(data, fOAEP);
                return _data;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// 私钥数字签名
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="data">待数字签名的数据</param>
        /// <param name="halg">要用于创建哈希值的哈希算法</param>
        /// <returns>数字签名数据</returns>
        public static byte[] SignData(string privateKey, byte[] data, object halg = null)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                if (halg == null)
                {
                    return rsa.SignData(data, new SHA1CryptoServiceProvider());
                }
                else
                {
                    return rsa.SignData(data, halg);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 公钥验证数字签名
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="data">带验证数字签名的数据</param>
        /// <param name="signData">数字签名数据</param>
        /// <param name="halg">要用于创建哈希值的哈希算法</param>
        /// <returns>数字签名是否有效</returns>
        public static bool VerifyData(string publicKey, byte[] data, byte[] signData, object halg = null)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                if (halg == null)
                {
                    return rsa.VerifyData(data, new SHA1CryptoServiceProvider(), signData);
                }
                else
                {
                    return rsa.VerifyData(data, halg, signData);
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
