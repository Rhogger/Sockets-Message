using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sockets.Server;
public class Server
{
  public void Start()
  {
    var hostName = Dns.GetHostName();
    IPHostEntry localHost = Dns.GetHostEntry(hostName);

    IPAddress localHostAddress = localHost.AddressList[0];
    IPEndPoint ipEndPoint = new IPEndPoint(localHostAddress, 11011);

    using Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    listener.Bind(ipEndPoint);
    listener.Listen(100);

    var handler = listener.Accept();

    Console.WriteLine("[SERVER] Server started.");

    Thread receiveThread = new Thread(() =>
    {
      while (true)
      {
        var buffer = new byte[1024];
        var received = handler.Receive(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);

        if (response.Any())
        {
          Console.WriteLine($"[CLIENT]: {response}");
        }
      }
    });

    receiveThread.Start();

    while (true)
    {
      Console.Write("[SERVER]: ");
      var message = Console.ReadLine();
      var messageBytes = Encoding.UTF8.GetBytes(message);
      handler.Send(messageBytes);
    }
  }
}