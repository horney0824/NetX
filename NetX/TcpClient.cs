using System.Net.Sockets;
using System.Net;

namespace NetX
{
    public class TcpClient
    {
        Socket socket;

        public TcpConnection Connect(string address, int port)
        {
            var addr = IPAddress.Parse(address);

            IPEndPoint connectPoint = new IPEndPoint(addr, port);

            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(connectPoint);

            return new TcpConnection(socket);
        }
    }
}