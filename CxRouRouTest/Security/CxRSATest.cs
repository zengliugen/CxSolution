using System.Text;

using CxSolution.CxRouRou.Collections;
using CxSolution.CxRouRou.Security;

namespace CxSolution.CxRouRouTest.Security
{
    /// <summary>
    /// RSA加解密测试
    /// </summary>
    public class CxRSATest : CxTestBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="testName"></param>
        public CxRSATest(string testName) : base(testName)
        {
        }
        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public override void Dispose()
        {
        }
        /// <summary>
        /// 测试内容
        /// </summary>
        /// <returns></returns>
        public override bool Test()
        {
            var infoString = "测试RSA加解密";
            PrintL("初始字符串:{0}", infoString);
            var infoData = Encoding.UTF8.GetBytes(infoString);
            PrintL("初始字节数组:{0}", infoData.ToStringEx());
            var rsaKeyInfo = CxRSA.CreateRSAKey();
            PrintL("RSA公钥:{0}", rsaKeyInfo.Public);
            PrintL("RSA私钥:{0}", rsaKeyInfo.Private);

            var encryptData = CxRSA.Encrypt(rsaKeyInfo.Public, infoData);
            PrintL("加密后数据:{0}", encryptData.ToStringEx());
            var signData = CxRSA.SignData(rsaKeyInfo.Private, encryptData);
            PrintL("数字签名数据:{0}", signData.ToStringEx());

            if (CxRSA.VerifyData(rsaKeyInfo.Public, encryptData, signData))
            {
                PrintL("数字签名验证成功");
                var decryptData = CxRSA.Decrypt(rsaKeyInfo.Private, encryptData);
                PrintL("解密后数据:{0}", decryptData.ToStringEx());
                var decryptString = Encoding.UTF8.GetString(decryptData);
                PrintL("解密后字符串:{0}", decryptString);
                if (decryptString.Equals(infoString))
                {
                    PrintL("解密后数据与原始数据一致");
                    return true;
                }
                else
                {
                    PrintL("解密后数据与原始数据不一致");
                    return false;
                }
            }
            else
            {
                PrintL("数字签名验证失败");
                return false;
            }
        }
    }
}
