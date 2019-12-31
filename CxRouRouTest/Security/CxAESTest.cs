using System.Text;

using CxSolution.CxRouRou.Expand;
using CxSolution.CxRouRou.Security;

namespace CxSolution.CxRouRouTest.Security
{
    /// <summary>
    /// AES加解密测试
    /// </summary>
    public class CxAESTest : CxTestBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="testName"></param>
        public CxAESTest(string testName) : base(testName)
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
            var infoString = "测试AES加解密";
            PrintL("初始字符串:{0}", infoString);
            var infoData = Encoding.UTF8.GetBytes(infoString);
            PrintL("初始字节数组:{0}", infoData.ToStringEx());
            var aesKeyInfo = CxAES.CreateAESKey();
            PrintL("AES秘钥:{0}", aesKeyInfo.Key);
            PrintL("AES向量:{0}", aesKeyInfo.IV);

            var encryptData = CxAES.Encrypt(aesKeyInfo.Key, aesKeyInfo.IV, infoData);
            PrintL("加密后数据:{0}", encryptData.ToStringEx());

            var decryptData = CxAES.Decrypt(aesKeyInfo.Key, aesKeyInfo.IV, encryptData);
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
    }
}
