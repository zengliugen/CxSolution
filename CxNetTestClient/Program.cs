using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using CxRouRou.Collections;
using CxRouRou.Net.Sockets.Tcp;
using CxRouRou.Util;

namespace CxNetTestClient
{
    class Program
    {
        public class IPAddressComparer : IComparer<IPAddress>
        {
            public int Compare(IPAddress x, IPAddress y)
            {
                if (x.AddressFamily == y.AddressFamily)
                {
                    return -1;
                }
                else if (x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return 1;
                }
                return -1;
            }
        }
        static void Main(string[] args)
        {
            ClientNet net = new ClientNet();
            while (true)
            {
                CxConsole.WriteLine("请输入指令");
                string[] cmds = Console.ReadLine().Split(' ');
                string cmd = cmds[0];
                switch (cmd)
                {
                    case "connect":
                        for (int i = 0; i < int.Parse(cmds.Length < 2 ? "1" : cmds[1]); i++)
                        {
                            net.StartConnect("zeng.com", 12345);
                            Thread.Sleep(1);
                        }
                        break;
                    case "send":
                        net.Send(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
                        break;
                    case "stop":
                        net.StopConnect();
                        break;
                    case "online":
                        CxConsole.WriteLine(net.OnlineNum);
                        break;
                    case "exit":
                        net.Dispose();
                        return;
                    default:
                        break;
                }
            }
        }

        class ClientNet : CxNet
        {
            public uint SessionID;
            protected override void OnConnetSuccess(uint id, string address)
            {
                base.OnConnetSuccess(id, address);
                SessionID = id;
                CxConsole.WriteLine("连接服务器成功 id:{0} address:{1}", id, address);
            }
            protected override void OnConnetFail(string address, string error)
            {
                CxConsole.WriteLine("连接服务器失败 address:{0} error:{1}", address, error);
            }
            public void Send(byte[] buffer, int index, int length)
            {
                Send(SessionID, buffer, index, length, true);
                CxConsole.WriteLine("向服务器发送消息 data:{0}", buffer.ToStringEx());
            }
            protected override void OnReceiveData(IReceiveData receiveData, int length)
            {
                base.OnReceiveData(receiveData, length);
                CxConsole.WriteLine("收到服务器消息 data:{0}", receiveData.Buffer.GetArray(0, length).ToStringEx());
            }
            protected override void OnLossConnection(uint id, CloseType closeType, string message)
            {
                base.OnLossConnection(id, closeType, message);
                CxConsole.WriteLine("连接被关闭 id:{0} closeType:{1} message:{2}", id, closeType, message);
            }
            protected override void OnClose(string error)
            {
                base.OnClose(error);
                CxConsole.WriteLine(error);
            }
        }
    }
}
