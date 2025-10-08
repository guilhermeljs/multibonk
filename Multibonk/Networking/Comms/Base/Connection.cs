using System.Collections.Concurrent;
using System.Net.Sockets;
using MelonLoader;

namespace Multibonk.Networking.Comms.Base
{
    public class Connection
    {
        private const int HEADER_LENGTH = 2;
        private const int MAX_PACKET_SIZE = 1024;

        private readonly TcpClient client;
        private NetworkStream stream;
        private readonly byte[] buffer = new byte[MAX_PACKET_SIZE];

        public event Action<Connection> OnClose;
        public event Action<Connection, byte[]> OnMessageReceived;

        private ConcurrentQueue<OutgoingPacket> outgoingPackets = new ConcurrentQueue<OutgoingPacket>();
        private readonly SemaphoreSlim _queueSignal = new SemaphoreSlim(0);


        private bool isReading = false;

        public Connection(TcpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }


        public Connection(TcpClient client, List<OutgoingPacket> packets)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            foreach(var packet in packets)
            {
                outgoingPackets.Enqueue(packet);
            }
        }

        public void Start()
        {
            if (isReading) return;

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            stream = client.GetStream();

            isReading = true;
            _ = Task.Run(async () => await ReadLoop());
            _ = Task.Run(async () => await SendLoop());
        }

        public void EnqueuePacket(OutgoingPacket packet)
        {
            outgoingPackets.Enqueue(packet);
            _queueSignal.Release();
        }

        private async Task ReadLoop()
        {
            try
            {
                await ReadMessage();
            }
            catch (Exception e)
            {
                MelonLogger.Msg("Failed to read message " + e);
            }
            finally
            {
                Close();
            }
        }

        private async Task SendLoop()
        {
            while (client.Connected)
            {
                try
                {
                    await _queueSignal.WaitAsync();

                    while (outgoingPackets.TryDequeue(out var packet))
                    {

                        if (client != null && client.Connected)
                            await InternalSend(packet.ToBuffer());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in dequeue loop: {ex}");
                }
            }
        }

        private async Task ReadMessage()
        {
            while (client.Connected)
            {
                await ReadExact(buffer, 0, HEADER_LENGTH);
                short packetLength = BitConverter.ToInt16(buffer, 0);


                if (packetLength <= 0 || packetLength > MAX_PACKET_SIZE - HEADER_LENGTH)
                    throw new IOException("Invalid packet length");

                await ReadExact(buffer, HEADER_LENGTH, packetLength);

                byte[] packet = new byte[packetLength];
                Array.Copy(buffer, HEADER_LENGTH, packet, 0, packetLength);

                OnMessageReceived?.Invoke(this, packet);
            }
        }

        private async Task ReadExact(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int bytesRead = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead);
                if (bytesRead == 0) throw new EndOfStreamException("Connection closed by remote host (received 0)");
                totalRead += bytesRead;
            }
        }

        private async Task InternalSend(byte[] data)
        {
            if (!client.Connected) throw new InvalidOperationException("Client not connected.");
            if (data.Length + HEADER_LENGTH > MAX_PACKET_SIZE)
                throw new ArgumentException($"Packet too large, max {MAX_PACKET_SIZE - HEADER_LENGTH} bytes");

            byte[] packet = new byte[data.Length + HEADER_LENGTH];
            Array.Copy(BitConverter.GetBytes((short)data.Length), packet, HEADER_LENGTH);
            Array.Copy(data, 0, packet, HEADER_LENGTH, data.Length);


            await stream.WriteAsync(packet, 0, packet.Length);
        }

        public void Close()
        {
            try { stream.Close(); } catch { }
            try { client.Close(); } catch { }
            OnClose?.Invoke(this);
        }
    }

}
