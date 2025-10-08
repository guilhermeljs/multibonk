namespace Multibonk.Networking.Comms.Base
{
    public class OutgoingPacket
    {
        protected OutgoingMessage Message { get; set; } = new OutgoingMessage();

        public byte[] ToBuffer()
        {
            if (Message == null) return null;

            var payload = Message.ToArray();
            return payload;
        }
    }
}
