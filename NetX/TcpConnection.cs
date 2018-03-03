using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NetX
{
    using NetPacketAndBuffer = System.Tuple<NetPacketHeader, NetBuffer>;

    public class TcpConnection
    {
        public object attachedObject;

        public delegate void ReceiveCB(TcpConnection conn, NetPacketAndBuffer packetAndBuffer);

        public bool manuallyFetchReceivedBuffer = false;

        public int maxReceiveBodySize = 1024 * 1024;

        bool sending = false;

        Socket socket;

        SocketAsyncEventArgs sendEventArgs = null;
        SocketAsyncEventArgs receiveEventArgs = null;

        public ConcurrentQueue<NetBuffer> sendBufferQueue = new ConcurrentQueue<NetBuffer>();
        NetBuffer sendingBuffer = NetBuffer.Empty;
        object sendLock = new object();
        object combineSendLock = new object();

        public ReceiveCB onReceive;
        public Dictionary<ushort, ReceiveCB> onReceiveCbDict = new Dictionary<ushort, ReceiveCB>();
        ConcurrentQueue<NetPacketAndBuffer> receiveBufferQueue = new ConcurrentQueue<NetPacketAndBuffer>();

        NetPacketHeader receivingHeader = NetPacketHeader.Empty;
        int receivingHeaderOffset = 0;
        NetBuffer receivingBuffer = NetBuffer.Empty;

        public TcpConnection(Socket socket)
        {
            this.socket = socket;

            BeginReceive(0);
        }

        public void Close()
        {
            socket.Close(1);
        }

        bool OnSent(SocketAsyncEventArgs e, bool callSendWhenFinished, out NetBuffer remainBuffer)
        {
#if ENABLE_NET_DIAGNOSTICS
        if (callSendWhenFinished)
        {
            Output.Log("xxx");
        }

        Output.LogFormat("SENT <{0} {1}/{2}> {3} Q = {4} T = {5}",
            sendingBuffer.offset, sendingBuffer.size, e.BytesTransferred,
            callSendWhenFinished, sendBufferQueue.Count, System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif

            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (e.BytesTransferred == sendingBuffer.size)
                {
                    sendingBuffer = NetBuffer.Empty;

                    var buffer = new NetBuffer();
                    if (sendBufferQueue.TryDequeue(out buffer))
                    {
                        if (!callSendWhenFinished)
                        {
                            remainBuffer = buffer;
                            return true;
                        }

                        BeginSend(buffer, true);
                    }
                    else
                    {
                        lock (sendLock)
                        {
                            sending = false;
                        }
                    }
                }
                else if (e.BytesTransferred < sendingBuffer.size)
                {
                    sendingBuffer.Consume(e.BytesTransferred);

                    if (!callSendWhenFinished)
                    {
                        remainBuffer = sendingBuffer;
                        return true;
                    }

                    BeginSend(sendingBuffer, true);
                }
            }
            else
            {
                remainBuffer = NetBuffer.Empty;

                Output.LogFormat("SEND ERROR: {0}", e.SocketError);
                Close();

                return false;
            }

            remainBuffer = NetBuffer.Empty;

            return true;
        }

        void DoReceiveEvent(NetPacketAndBuffer packetAndBuffer)
        {
            onReceive?.Invoke(this, packetAndBuffer);

            ReceiveCB cb;
            if (onReceiveCbDict.TryGetValue(packetAndBuffer.Item1.cmd, out cb))
            {
                cb?.Invoke(this, packetAndBuffer);
            }
        }

        public NetPacketAndBuffer FetchReceivedBuffer()
        {
            if (!manuallyFetchReceivedBuffer)
                return null;

            NetPacketAndBuffer packetAndBuffer = null;
            receiveBufferQueue.TryDequeue(out packetAndBuffer);

            return packetAndBuffer;
        }

        bool OnReceived(SocketAsyncEventArgs e, bool callReceiveWhenFinished)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                var headerSize = Marshal.SizeOf<NetPacketHeader>();

                var remainBuffer = new NetBuffer
                {
                    byteArray = e.Buffer,
                    offset = 0,
                    size = e.BytesTransferred
                };

                while (remainBuffer.size > 0)
                {
                    if (receivingHeaderOffset < headerSize)
                    {
                        var oldRemainBuffer = remainBuffer;

                        var originHeaderOffset = receivingHeaderOffset;
                        var remainBufferOffset = receivingHeader.ReadPartial(
                            remainBuffer.byteArray, receivingHeaderOffset,
                            remainBuffer.offset, remainBuffer.offset + remainBuffer.size - 1);

                        var readBytes = remainBufferOffset - remainBuffer.offset;
                        receivingHeaderOffset += readBytes;

                        remainBuffer.Consume(readBytes);

#if ENABLE_NET_DIAGNOSTICS
                    Output.LogErrorFormat(
                        "READ HEADER (CMD {0} FLAGS {1} SLCODE {2} SIZE {3}) (OFFSET {4} => {5}) (REMAIN {6} => {7})  T = {8}",
                        receivingHeader.cmd, receivingHeader.flags, receivingHeader.slCode,
                        receivingHeader.size,
                        originHeaderOffset, receivingHeaderOffset,
                        oldRemainBuffer.offset, oldRemainBuffer.offset + oldRemainBuffer.size - 1,
                        System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
                        if (receivingHeaderOffset >= headerSize)
                        {
                            if (receivingHeaderOffset > headerSize ||
                               receivingHeader.size > maxReceiveBodySize)
                            {
                                Output.LogErrorFormat(
                                    "CORRUPT HEADER (CMD {0} FLAGS {1} SLCODE {2} SIZE {3}) (OFFSET {4} => {5}) (REMAIN {6} => {7})  T = {8}",
                                    receivingHeader.cmd, receivingHeader.flags, receivingHeader.slCode,
                                    receivingHeader.size,
                                    originHeaderOffset, receivingHeaderOffset,
                                    oldRemainBuffer.offset, oldRemainBuffer.offset + oldRemainBuffer.size - 1,
                                    System.Threading.Thread.CurrentThread.ManagedThreadId);
                                Output.LogErrorFormat(
                                    "{0:X} {1:X} {2:X} {3:X} {4:X} {5:X} {6:X} {7:X} {8:X} {9:X}",
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 1],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 2],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 3],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 4],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 5],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 6],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 7],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 8],
                                    oldRemainBuffer.byteArray[oldRemainBuffer.offset + 9]
                                );
                                Close();
                                return false;
                            }

                            receivingBuffer = new NetBuffer
                            {
                                byteArray = new byte[receivingHeader.size],
                                offset = 0,
                                size = (int)receivingHeader.size
                            };
                        }
                    }
                    else
                    {
                        var byteNumToCopy = Math.Min(remainBuffer.size, receivingBuffer.size);

                        Buffer.BlockCopy(
                            remainBuffer.byteArray, (int)remainBuffer.offset,
                            receivingBuffer.byteArray, (int)receivingBuffer.offset,
                            byteNumToCopy);

                        remainBuffer.Consume(byteNumToCopy);
                        receivingBuffer.Consume(byteNumToCopy);
                    }

                    if (receivingBuffer.byteArray != null && receivingBuffer.size == 0)
                    {
                        var packetAndBuffer = new NetPacketAndBuffer(
                                receivingHeader, receivingBuffer.Begin);

                        receivingHeader = NetPacketHeader.Empty;
                        receivingBuffer = NetBuffer.Empty;
                        receivingHeaderOffset = 0;

                        if (packetAndBuffer.Item2.byteArray != null)
                        {
                            try
                            {
                                if (manuallyFetchReceivedBuffer)
                                {
                                    receiveBufferQueue.Enqueue(packetAndBuffer);
                                }
                                else
                                {
                                    DoReceiveEvent(packetAndBuffer);
                                }
                            }
                            catch (Exception exception)
                            {
                                Output.LogError(exception);
                            }
                        }
                    }
                }

                if (callReceiveWhenFinished)
                {
                    BeginReceive(0);
                }
            }
            else if (e.SocketError != SocketError.Success)
            {
                Output.LogFormat("RECV ERROR: {0}", e.SocketError);
                Close();
                return false;
            }

            return true;
        }

        void BeginReceive(int bufferSize)
        {
            int maxBufferSize = 65535;

            if (bufferSize <= 0)
                bufferSize = maxBufferSize;

            if (receiveEventArgs == null)
            {
                receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.Completed += OnIOComplete;
                receiveEventArgs.SetBuffer(new byte[bufferSize], 0, maxBufferSize);
            }

            receiveEventArgs.SetBuffer(0, bufferSize);

            while (socket.Connected && !socket.ReceiveAsync(receiveEventArgs))
            {
                if (!OnReceived(receiveEventArgs, false))
                {
                    break;
                }
            }
        }

        void OnIOComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    {
                        OnReceived(e, true);
                        break;
                    }
                case SocketAsyncOperation.Send:
                    {
                        NetBuffer dummyBuffer = NetBuffer.Empty;
                        OnSent(e, true, out dummyBuffer);
                    }
                    break;
                default:
                    break;
            }
        }

#if ENABLE_NET_DIAGNOSTICS
    int _bufferIndex = 1;
#endif
        void BeginSend(NetBuffer buffer, bool forceSend)
        {
            lock (sendLock)
            {
                if (sending && !forceSend)
                {
#if ENABLE_NET_DIAGNOSTICS
                buffer.index = _bufferIndex;
                _bufferIndex++;
#endif
                    sendBufferQueue.Enqueue(buffer);
                    return;
                }
                else
                {
                    sending = true;
                }
            }

            if (sendEventArgs == null)
            {
                sendEventArgs = new SocketAsyncEventArgs();
                sendEventArgs.Completed += OnIOComplete;
            }

            sendEventArgs.SetBuffer(buffer.byteArray, buffer.offset, buffer.size);

            sendingBuffer = buffer;

            while (true)
            {
#if ENABLE_NET_DIAGNOSTICS
            Output.LogFormat("SEND <{0} {1}> {2} T = {3}",
                sendingBuffer.offset, sendingBuffer.size,
                sendingBuffer.index,
                System.Threading.Thread.CurrentThread.ManagedThreadId);

            if (sendingBuffer.size == 10)
            {
                NetPacketHeader dummyHeader = new NetPacketHeader();

                dummyHeader.Read(sendingBuffer.byteArray, sendingBuffer.offset, sendingBuffer.size);

                Output.LogFormat("HEADER CMD = {0}, SIZE = {1}", dummyHeader.cmd, dummyHeader.size);
            }
#endif
                if (!socket.Connected || socket.SendAsync(sendEventArgs))
                {
                    break;
                }

                if (!OnSent(sendEventArgs, false, out sendingBuffer))
                {
                    break;
                }

                if (sendingBuffer.size == 0)
                {
                    break;
                }

                sendEventArgs.SetBuffer(sendingBuffer.byteArray, sendingBuffer.offset, sendingBuffer.size);
            }
        }

        public void Send(ushort cmd, SaveFlags flags, ushort slCode, byte[] byteArray)
        {
            Send(
                new NetPacketHeader()
                {
                    cmd = cmd,
                    slCode = slCode,
                    flags = flags
                },
                NetBuffer.Create(byteArray));
        }

        public void Send(NetPacketHeader header, NetBuffer buffer)
        {
            header.size = (uint)buffer.size;

            var headerSize = Marshal.SizeOf(header);
            var headerByteArray = new byte[Marshal.SizeOf(header)];

            header.Write(headerByteArray, 0, headerSize - 1);

            lock (combineSendLock)
            {
                Send(headerByteArray, 0, headerByteArray.Length);
                if(buffer.size != 0)
                {
                    Send(buffer);
                }
            }
        }

        void Send(byte[] byteArray, int offset, int size)
        {
            var headerBuffer = new NetBuffer
            {
                byteArray = byteArray,
                offset = offset,
                size = size
            };

            Send(headerBuffer);
        }

        public void Send(NetBuffer buffer)
        {
            BeginSend(buffer, false);
        }
    }
}