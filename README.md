# APM

High performance TCP communication framework,It is very simple to use，and the general server easy  connection on millions ;message forwarding support for publish/subscribe、private  model.

高性能的TCP通信框架，它使用非常简单，和一般的服务器容易连接上百万；消息转发方式支持发布/订阅、私人模式。

<b>Server Example:</b>

Console.Title = "Server";

Console.WriteLine("server test");

server = new Server(8889, 500);

server.OnAccepted += Server_OnAccepted;

server.OnMessage += Server_OnReceived;

server.OnDisConnected += Server_OnDisConnected;

server.OnOnError += Server_OnOnError;

server.Start();

Console.WriteLine("server is running...");


<b>Client Example:</b>

Console.WriteLine("Client send test");

var client = new Client(Guid.NewGuid().ToString("N"), 10, localIP);

client.OnConnected += Client_OnConnected;

client.OnMessage += Client_OnMessage;

client.OnDisConnected += Client_OnDisConnected;

client.OnError += Client_OnError;

client.Connect();


<b>Server send message</b>

server.SendMsg(remote, "server:hello   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

<b>Server send file</b>

server.SendFile(UserToken remote, string fileName);

<b>Client send message</b>

client.SendMsg("client:hello   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

<b>Client send file</b>

client.sendFile(string fileName);

