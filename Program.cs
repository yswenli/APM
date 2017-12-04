using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APM.ConsoleDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "APM.Core test";

            Console.WriteLine("输入1测试APM tcp通讯");

            Console.WriteLine("输入2测试APM转发");

            var str = Console.ReadLine();

            if (!string.IsNullOrEmpty(str) && str != "1")
            {
                APMServer();
                APMClient();
                APMClient();
                APMClient();
                APMClient();
            }
            else
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(5000);
                    ClientProcess();
                });

                ServerProcess(true);

            }
            Console.ReadLine();
        }

        private static APM.Core.Server server;
        static void ServerProcess(bool falg = false)
        {
            Console.WriteLine("server test");
            server = new APM.Core.Server(8889, 65536);
            server.OnAccepted += Server_OnAccepted;
            server.OnMessage += Server_OnReceived;
            server.OnDisConnected += Server_OnDisConnected;
            server.OnOnError += Server_OnOnError;
            server.Start();
            Console.WriteLine("server is running...");
            if (falg)
            {
                while (true)
                {
                    Console.ReadLine();
                    Console.WriteLine(string.Format("serverinfo[ClientCount:{0},ReceiveCount:{1},SendCount:{2}]", server.ClientCount, server.ReceiveCount, server.SendCount));
                }
            }

        }


        private static APM.Core.Client client;
        static void ClientProcess()
        {

            Console.WriteLine("Client test");

            var localIP = GetLocalIp() + ":8889";

            Console.WriteLine("Client send test");
            client = new APM.Core.Client(Guid.NewGuid().ToString("N"), 10, localIP);
            client.OnConnected += Client_OnConnected;
            client.OnMessage += Client_OnMessage;
            client.OnDisConnected += Client_OnDisConnected;
            client.OnError += Client_OnError;
            client.Connect();

            Console.Title = "APM Server & Client";
            Console.WriteLine("MutiClients test");
            for (int i = 0; i < 10000; i++)
            {
                new APM.Core.Client(Guid.NewGuid().ToString("N"), 10, localIP).Connect();
            }

        }

        #region server events
        private static void Server_OnAccepted(APM.Core.UserToken remote)
        {
            Console.WriteLine("收到客户端连接：" + remote.Client.RemoteEndPoint);

        }
        private static void Server_OnReceived(APM.Core.UserToken remote, byte[] data)
        {
            if (server.ClientCount <= 10)
            {
                Console.WriteLine("收到客户端消息：" + remote.Client.RemoteEndPoint + "  " + Encoding.UTF8.GetString(data));
            }
            server.SendMsg(remote, "server:hello   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        private static void Server_OnDisConnected(APM.Core.UserToken remote, Exception ex)
        {
            Console.WriteLine(string.Format("客户端{0}已断开连接，断开消息:{1}", remote.ID, ex.Message));
        }

        private static void Server_OnOnError(APM.Core.UserToken remote, Exception ex)
        {
            Console.WriteLine(string.Format("操作客户端{0}异常，断开消息:{1}", remote.ID, ex.Message));
        }


        #endregion

        #region client events

        static string msg = "hellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohellohelloh";
        private static void Client_OnConnected(APM.Core.Client c)
        {
            if (c != null)
                c.SendMsg(string.Format("client:{0}    {1}", msg, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));

        }

        private static void Client_OnMessage(byte[] data)
        {
            Console.WriteLine("收到服务器信息：" + Encoding.UTF8.GetString(data));
            if (client != null)
                client.SendMsg(string.Format("client:{0}    {1}", msg, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
        }

        private static void Client_OnDisConnected(Exception ex)
        {
            Console.WriteLine(string.Format("客户端断开连接，断开消息:{0}", ex.Message));
        }

        private static void Client_OnError(Exception ex)
        {
            Console.WriteLine(string.Format("客户端异常，异常消息:{0}", ex.Message));
        }


        #endregion

        #region MyRegion
        static string GetLocalIp()
        {
            string hostname = Dns.GetHostName();//得到本机名    
            //IPHostEntry localhost = Dns.GetHostByName(hostname);//方法已过期，只得到IPv4的地址     
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            IPAddress localaddr = localhost.AddressList.Where(b => b.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Last();
            return localaddr.ToString();
        }
        #endregion


        #region APM转发
        public static void APMServer()
        {
            Console.WriteLine("APMServer test");
            APM.Server.APMServer server = new Server.APMServer(8890, 1024);
            server.OnAccepted += Server_OnAccepted;
            server.OnDisConnected += Server_OnDisConnected;
            server.OnError += Server_OnOnError;
            server.OnMessage += Server_OnMessage;
            server.Start();
            Console.WriteLine("APMServer 已启动...");
        }

        private static void Server_OnMessage(Core.Extention.Message msg)
        {
            Console.WriteLine("APMServer 收到并转发消息：ID {0},Sender {1},SessionID {2},SendTick {3}", msg.ID, msg.Sender, msg.SessionID, msg.SendTick);
        }

        public static void APMClient()
        {
            Console.WriteLine("APMClient test");
            var userID = "张三" + new Random((int)DateTime.Now.Ticks).Next(10000, 99999);
            APM.Client.APMClient apmClient = new Client.APMClient(userID, GetLocalIp(), 8890);
            apmClient.OnConnected += ApmClient_OnConnected;
            apmClient.OnDisConnected += Client_OnDisConnected;
            apmClient.OnError += Client_OnError;
            apmClient.OnMessage += APMClient_OnMessage;
            apmClient.Connect();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (apmClient.Connected)
                    {
                        apmClient.SendChannelMsg("all", string.Format("大家好，我是client:{0}    {1}", msg, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    }
                    Thread.Sleep(10);
                }
            });
            Console.WriteLine("APMClient:{0} 已连接到服务器", userID);
        }

        private static void ApmClient_OnConnected(Client.APMClient c)
        {
            c.Subscribe("all");            
        }

        private static void APMClient_OnMessage(Core.Extention.Message msg)
        {
            Console.WriteLine("APMClient 收到消息：ID {0},Sender {1},SessionID {2},SendTick {3}", msg.ID, msg.Sender, msg.SessionID, msg.SendTick);
        }
        #endregion
    }
}
