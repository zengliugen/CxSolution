using CxRouRou.Collections;
using CxRouRou.Net.Sockets;
using CxRouRou.Net.Sockets.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
namespace CXRouRouTest.Net.Sockets.Tcp
{
    /// <summary>
    /// 网络测试
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
        }
        /// <summary>
        /// 测试内容
        /// </summary>
        /// <returns></returns>
        public override bool Test()
        {
            ushort port = 12345;
            string ip = "192.168.2.236";

            bool result = false;
            bool isCheckOver = false;
            TestNet net = new TestNet();
            net.StartListenAcceptOkAction = () =>
            {
                Thread.Sleep(1000);
                net.StartConnect(ip, port);
            };
            net.CheckMsgLength = 5;
            net.ConnectOkAction = () =>
            {
                Thread.Sleep(1000);
                CxMsgPacket msgPacket = new CxMsgPacket(1400);
                msgPacket.Push(CxArray.CreateRandomByte(1400 - CxMsgPacket.FreeNum - 2));
                net.SendMsg(msgPacket);
                Thread.Sleep(1000);
                msgPacket = new CxMsgPacket(1400);
                msgPacket.Push(CxArray.CreateRandomByte(1400 - CxMsgPacket.FreeNum - 2));
                net.SendMsg(msgPacket);
                Thread.Sleep(1000);
                msgPacket = new CxMsgPacket(1400);
                msgPacket.Push(CxArray.CreateRandomByte(1400 - CxMsgPacket.FreeNum - 2));
                net.SendMsg(msgPacket);
                Thread.Sleep(1000);
                msgPacket = new CxMsgPacket(1400);
                msgPacket.Push(CxArray.CreateRandomByte(1400 - CxMsgPacket.FreeNum - 2));
                net.SendMsg(msgPacket);
                Thread.Sleep(1000);
                msgPacket = new CxMsgPacket(1400);
                msgPacket.Push(CxArray.CreateRandomByte(1400 - CxMsgPacket.FreeNum - 2));
                net.SendMsg(msgPacket);
            };
            net.ErrorAction = (error) =>
            {
                PrintL("测试错误:{0}", error);
            };
            net.RunResulttAction = (isOk) =>
            {
                result = isOk;
                isCheckOver = true;
            };
            net.MsgAction = (message) =>
            {
                PrintL(message);
            };
            net.StartListenAccept(port);

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
                    PrintL("测试超时，视为失败");
                    break;
                }
                Thread.Sleep(1);
            }
            net.Dispose();
            net = null;
            return result;
        }
        class TestNet : CxNet
        {
            public uint ConnectID = 0;
            public List<CxMsgPacket> SendMsgs = new List<CxMsgPacket>();
            public List<CxMsgPacket> ReceiveMsgs = new List<CxMsgPacket>();
            public Action StartListenAcceptOkAction;
            public Action ConnectOkAction;
            public Action<string> ErrorAction;
            public int CheckMsgLength = 5;
            public Action<bool> RunResulttAction;
            public Action<string> MsgAction;
            public TestNet()
            {
                AddMsgHandler(1, (id, msgPacket) =>
                {
                    MsgAction.Invoke("收到消息包");
                    ReceiveMsgs.Add(msgPacket);
                    if (ReceiveMsgs.Count >= CheckMsgLength)
                    {
                        if (SendMsgs.Count != ReceiveMsgs.Count)
                        {
                            RunResulttAction(false);
                        }
                        else
                        {
                            bool isOk = true;
                            for (int i = 0; i < SendMsgs.Count; i++)
                            {
                                if (!SendMsgs[i].Equals(ReceiveMsgs[i]))
                                {
                                    isOk = false;
                                    break;
                                }
                            }
                            RunResulttAction(isOk);
                        }
                    }
                });
            }
            public void SendMsg(CxMsgPacket msgPacket)
            {
                MsgAction.Invoke("发送消息包");
                SendMsgPacket(ConnectID, 1, msgPacket);
                SendMsgs.Add(msgPacket);
            }
            protected override void OnStartListenAcceptFail(ushort port, string error)
            {
                ErrorAction.Invoke(error);
            }
            protected override void OnStartListenAcceptSuccess(ushort port, string message)
            {
                MsgAction.Invoke(message);
                StartListenAcceptOkAction.Invoke();
            }
            protected override void OnConnetFail(string address, string error)
            {
                ErrorAction.Invoke(error);
            }
            protected override void OnConnetSuccess(uint id, string address)
            {
                MsgAction.Invoke("连接服务器成功");
                ConnectID = id;
                ConnectOkAction.Invoke();
            }
            protected override void OnLossConnection(uint id, CloseType closeType, string message)
            {
                ErrorAction.Invoke(message);
            }
            protected override void OnError(uint id, ErrorType errorType, string error)
            {
                ErrorAction.Invoke(error);
            }
            protected override void OnAcceptFail(string message)
            {
                MsgAction.Invoke(message);
            }
            protected override void OnAcceptSuccess(uint id, string address)
            {
                MsgAction.Invoke("接受连接成功");
            }
        }
    }
}
