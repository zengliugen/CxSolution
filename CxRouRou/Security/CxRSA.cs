using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CxSolution.CxRouRou.Security
{
    /// <summary>
    /// RSA加密
    /// </summary>
    public class CxRSA
    {
        /// <summary>
        /// 公钥加解密对象
        /// </summary>
        private readonly RSACryptoServiceProvider publicRAS;
        /// <summary>
        /// 私钥加解密对象
        /// </summary>
        private readonly RSACryptoServiceProvider privateRAS;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="privateKey">私钥</param>
        public CxRSA(string publicKey, string privateKey)
        {
            if (!string.IsNullOrEmpty(publicKey))
            {
                publicRAS = new RSACryptoServiceProvider();
            }
            if (!string.IsNullOrEmpty(privateKey))
            {
                privateRAS = new RSACryptoServiceProvider();
            }
        }
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fOAEP"></param>
        /// <returns></returns>
        public byte[] PublicEncrypt(byte[] data, bool fOAEP = false)
        {
            try
            {
                var _data = publicRAS.Encrypt(data, fOAEP);
                return _data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 公钥解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fOAEP"></param>
        /// <returns></returns>
        public byte[] PublicDecrypt(byte[] data, bool fOAEP = false)
        {
            try
            {
                var _data = publicRAS.Decrypt(data, fOAEP);
                return _data;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// 私钥加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fOAEP"></param>
        /// <returns></returns>
        public byte[] PrivateEncrypt(byte[] data, bool fOAEP = false)
        {
            try
            {
                var _data = privateRAS.Encrypt(data, fOAEP);
                return _data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fOAEP"></param>
        /// <returns></returns>
        public byte[] PrivateDecrypt(byte[] data, bool fOAEP = false)
        {
            try
            {
                var _data = privateRAS.Decrypt(data, fOAEP);
                return _data;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
