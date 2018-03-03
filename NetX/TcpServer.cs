using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace NetX
{
    public class TcpServer
    {
        Socket socket;

        ConcurrentBag<TcpConnection> connections = new ConcurrentBag<TcpConnection>();

        public delegate void AcceptCB(TcpConnection connection);

        public AcceptCB onAccept;

        void OnIOComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    OnAccept(e);
                    break;
            }
        }

        void OnAccept(SocketAsyncEventArgs e)
        {
            var connection = new TcpConnection(e.AcceptSocket);

            connections.Add(connection);

            onAccept?.Invoke(connection);

            ContinueAccept();
        }

        private void ContinueAccept()
        {
            var asyncEventArgs = new SocketAsyncEventArgs();
            asyncEventArgs.Completed += OnIOComplete;
            if (!socket.AcceptAsync(asyncEventArgs))
            {
                OnAccept(asyncEventArgs);
            }
        }

        public void Listen(string address, int port)
        {
            var addr = string.IsNullOrEmpty(address) ? IPAddress.Any : IPAddress.Parse(address);

            IPEndPoint listenPoint = new IPEndPoint(addr, port);

            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(listenPoint);

            socket.Listen(1000);

            ContinueAccept();
        }
    }
}
