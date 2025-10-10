using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    /// <summary>
    /// Packet sent from server to clients when a BaseInteractable is destroyed
    /// </summary>
    public class SendDestroyInteractablePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.DESTROY_INTERACTABLE_PACKET;

        public SendDestroyInteractablePacket(int instanceId)
        {
            Message.WriteByte(Id);
            Message.WriteInt(instanceId);
        }
    }

    /// <summary>
    /// Packet sent from client to server when a BaseInteractable is destroyed on the client
    /// </summary>
    public class SendClientDestroyInteractablePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.DESTROY_INTERACTABLE_PACKET;

        public SendClientDestroyInteractablePacket(int instanceId)
        {
            Message.WriteByte(Id);
            Message.WriteInt(instanceId);
        }
    }

  internal class DestroyInteractablePacket
  {
    public int InstanceId { get; private set; }

    public DestroyInteractablePacket(IncomingMessage msg)
    {
      InstanceId = msg.ReadInt();
    }
  }
}

