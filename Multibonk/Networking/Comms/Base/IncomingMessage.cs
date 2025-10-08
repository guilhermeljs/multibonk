using System.Text;

namespace Multibonk.Networking.Comms.Packet.Base
{

    namespace Multibonk.Networking.Comms
    {
        public class IncomingMessage
        {
            private readonly byte[] buffer;
            private int offset;

            public IncomingMessage(byte[] data)
            {
                buffer = data;
                offset = 0;
            }

            public byte ReadByte()
            {
                EnsureAvailable(1);
                return buffer[offset++];
            }

            public short ReadShort()
            {
                EnsureAvailable(2);
                short value = BitConverter.ToInt16(buffer, offset);
                offset += 2;
                return value;
            }

            public ushort ReadUShort()
            {
                EnsureAvailable(2);
                ushort value = BitConverter.ToUInt16(buffer, offset);
                offset += 2;
                return value;
            }


            public int ReadInt()
            {
                EnsureAvailable(4);
                int value = BitConverter.ToInt32(buffer, offset);
                offset += 4;
                return value;
            }

            public long ReadLong()
            {
                EnsureAvailable(8);
                long value = BitConverter.ToInt64(buffer, offset);
                offset += 8;
                return value;
            }

            public float ReadFloat()
            {
                EnsureAvailable(4);
                float value = BitConverter.ToSingle(buffer, offset);
                offset += 4;
                return value;
            }

            public double ReadDouble()
            {
                EnsureAvailable(8);
                double value = BitConverter.ToDouble(buffer, offset);
                offset += 8;
                return value;
            }

            public bool ReadBool()
            {
                return ReadByte() != 0;
            }

            public string ReadString()
            {
                short length = ReadShort();
                EnsureAvailable(length);
                string value = Encoding.UTF8.GetString(buffer, offset, length);
                offset += length;
                return value;
            }

            public byte[] ReadBytes(int count)
            {
                EnsureAvailable(count);
                byte[] data = new byte[count];
                Array.Copy(buffer, offset, data, 0, count);
                offset += count;
                return data;
            }

            private void EnsureAvailable(int length)
            {
                if (offset + length > buffer.Length)
                    throw new IndexOutOfRangeException($"Not enough bytes to read {length} bytes from packet.");
            }

            public int Remaining => buffer.Length - offset;
        }
    }
}
