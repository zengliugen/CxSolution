using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CxRouRou.Collections;
using CxRouRou.Net.Sockets.Tcp;
using CxRouRou.Util;

namespace CxNetTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerNet net = new ServerNet();
            while (true)
            {
                CxConsole.WriteLine("请输入指令");
                string[] cmds = Console.ReadLine().Split(' ');
                string cmd = cmds[0];
                switch (cmd)
                {
                    case "accept":
                        net.StartAccept(12345);
                        break;
                    case "stop":
                        net.StopAccept();
                        break;
                    case "close":
                        net.Close(ushort.Parse(cmds[1]));
                        break;
                    case "online":
                        CxConsole.WriteLine(net.OnlineNum);
                        break;
                    case "exit":
                        return;
                    default:
                        break;
                }
            }
        }
        class ServerNet : CxNet
        {
            protected override void OnStartSuccess(ushort port, string message)
            {
                base.OnStartSuccess(port, message);
                CxConsole.WriteLine("启动服务完成 port:{0} message:{1}", port, message);
            }
            protected override void OnAcceptSuccess(uint id, string address)
            {
                base.OnAcceptSuccess(id, address);
                CxConsole.WriteLine("收到客户端连接 id:{0} address:{1}", id, address);
                //Send(id, new byte[] { 11, 12, 13, 14, 15 }, 0, 5, true);
            }
            protected override void OnReceiveData(IReceiveData receiveData, int length)
            {
                CxConsole.WriteLine("收到客户端数据 id:{0} data:{1}", receiveData.ID, receiveData.Buffer.GetArray(0, length).ToStringEx());
                base.OnReceiveData(receiveData, length);
                Send(receiveData.ID, new byte[] { 6, 7, 8, 9, 10 }, 0, 5, true);
            }
            protected override void OnLossConnection(uint id, CloseType closeType, string message)
            {
                base.OnLossConnection(id, closeType, message);
                CxConsole.WriteLine("客户端连接被关闭 id:{0} closeType:{1} message:{2}", id, closeType, message);
            }
            protected override void OnClose(string error)
            {
                base.OnClose(error);
                CxConsole.WriteLine(error);
            }
        }
    }
}
