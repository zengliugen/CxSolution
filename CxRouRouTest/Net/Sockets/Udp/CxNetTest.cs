using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CxSolution.CxRouRou.Expands;
using CxSolution.CxRouRou.Net.Sockets;
using CxSolution.CxRouRou.Net.Sockets.Udp;

namespace CxSolution.CxRouRouTest.Net.Sockets.Udp
{
    /// <summary>
    /// 网络测试(Udp)
    /// </summary>
    public class CxNetTest : CxTestBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="testName"></param>
        public CxNetTest(string testName) : base(testName)
        {
        }
        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 测试内容
        /// </summary>
        /// <returns></returns>
        public override bool Test()
        {
            var result = true;
            string localHost = "127.0.0.1";
            ushort serverPort = 60001;
            ushort clientPort = 60002;

            var serverNet = new TestNet();
            serverNet.StartListen(serverPort);

            var serverTestBytes = CxArray.CreateRandomByte(1000);
            var serverTestMsg = new CxMsgPacket(1024);
            serverTestMsg.Push(serverTestBytes);

            var clientNet = new TestNet();
            clientNet.StartListen(clientPort);

            var clientTestBytes = CxArray.CreateRandomByte(1000);
            var clientTestMsg = new CxMsgPacket(1024);
            clientTestMsg.Push(clientTestBytes);

            var testCount = 0;
            var testMaxCount = 5;
            var isCheckOver = false;

            serverNet.OnReceiveDataAction = (endPoint, data) =>
            {

                PrintL("收到客户端数据 Length:{0}", data.Length);
                var isOk = clientTestBytes.EqualsEx(data);
                result = isOk ? result : isOk;
                PrintL("数据校验结果:{0}", isOk);

                Thread.Sleep(1000);

                PrintL("向客户端发送数据 Length:{0}", serverTestMsg.Length);
                clientNet.SendMsgPacket(endPoint, serverTestMsg);
            };

            clientNet.OnReceiveDataAction = (endPoint, data) =>
            {
                PrintL("收到服务器数据 Length:{0}", data.Length);
                var isOk = clientTestBytes.EqualsEx(data);
                result = isOk ? result : isOk;
                PrintL("数据校验结果:{0}", isOk);

                if (++testCount >= testMaxCount)
                {
                    isCheckOver = true;
                    return;
                }
                Thread.Sleep(1000);

                PrintL("向服务器发送数据 Length:{0}", clientTestMsg.Length);
                clientNet.SendMsgPacket(endPoint, clientTestMsg);
            };

            PrintL("向服务器发送数据 Length:{0}", clientTestMsg.Length);
            clientNet.SendMsgPacket(localHost, serverPort, clientTestMsg);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                if (isCheckOver)
                {
                    break;
                }
                if (sw.ElapsedMilliseconds > 20 * 1000)
                {
                    PrintL("测试超时,视为失败");
                    break;
                }
                Thread.Sleep(1);
            }
            return result;
        }
        class TestNet : CxNet
        {
            public Action<EndPoint, byte[]> OnReceiveDataAction;
            public TestNet()
            {
            }
            protected override void OnReceiveMsgPacket(EndPoint endPoint, CxMsgPacket msgPacket)
            {
                base.OnReceiveMsgPacket(endPoint, msgPacket);
                OnReceiveDataAction?.Invoke(endPoint, msgPacket.Pop_bytes());
            }
        }
    }
}
