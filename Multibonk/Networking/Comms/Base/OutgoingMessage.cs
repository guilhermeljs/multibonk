using System.Text;

namespace Multibonk.Networking.Comms.Base
{
    public class OutgoingMessage
    {
        private readonly List<byte> buffer;

        public OutgoingMessage()
        {
            buffer = new List<byte>();
        }

        public void WritePlayerId(byte value) => WriteByte(value);

        public void WriteByte(byte value) => buffer.Add(value);

        public void WriteShort(short value) => buffer.AddRange(BitConverter.GetBytes(value));

        public void WriteInt(int value) => buffer.AddRange(BitConverter.GetBytes(value));

        public void WriteUInt(uint value) => buffer.AddRange(BitConverter.GetBytes(value));

        public void WriteLong(long value) => buffer.AddRange(BitConverter.GetBytes(value));

        public void WriteFloat(float value) => buffer.AddRange(BitConverter.GetBytes(value));

        public void WriteUShort(ushort value) => buffer.AddRange(BitConverter.GetBytes(value));
        public void WriteDouble(double value) => buffer.AddRange(BitConverter.GetBytes(value));

        public void WriteBool(bool value) => buffer.Add(value ? (byte)1 : (byte)0);

        public void WriteString(string value)
        {
            if (value == null) value = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(value);
            if (data.Length > short.MaxValue)
                throw new ArgumentException("String too long to write");
            WriteShort((short)data.Length); // prefix length
            buffer.AddRange(data);
        }

        public void WriteBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            buffer.AddRange(data);
        }

        public byte[] ToArray() => buffer.ToArray();

        public int Length => buffer.Count;

        public void Clear() => buffer.Clear();
    }
}
