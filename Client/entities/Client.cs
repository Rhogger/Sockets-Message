using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sockets.Client;
public class Client
{
  public void Start()
  {
    var hostName = Dns.GetHostName();
    IPHostEntry localHost = Dns.GetHostEntry(hostName);

    IPAddress localHostAddress = localHost.AddressList[0];
    IPEndPoint ipEndPoint = new IPEndPoint(localHostAddress, 11011);

    using Socket client = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    client.Connect(ipEndPoint);

    Console.WriteLine("[CLIENT] Connected on server.");

    Thread receiveThread = new Thread(() =>
    {
      while (true)
      {
        var buffer = new byte[1024];
        var received = client.Receive(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);

        if (response.Any())
        {
          Console.WriteLine($"[SERVER]: {response}");
        }
      }
    });

    receiveThread.Start();

    while (true)
    {
      Console.Write("[CLIENT]: ");
      var message = Console.ReadLine();
      var messageBytes = Encoding.UTF8.GetBytes(message);
      client.Send(messageBytes);
    }
  }
}