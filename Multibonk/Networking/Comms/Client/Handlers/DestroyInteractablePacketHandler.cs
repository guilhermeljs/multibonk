using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers
{
  /// <summary>
  /// Client handler to process BaseInteractable destruction packets sent by the server
  /// </summary>
  public class DestroyInteractablePacketHandler : IClientPacketHandler
  {
    public byte PacketId => (byte)ServerSentPacketId.DESTROY_INTERACTABLE_PACKET;

    public DestroyInteractablePacketHandler() { }

    public void Handle(IncomingMessage msg, Connection conn)
    {
      var packet = new DestroyInteractablePacket(msg);

      // Enqueue execution on Unity's main thread
      GameDispatcher.Enqueue(() =>
      {
        GameFunctions.DestroyNetworkInteractable(packet.InstanceId);
      });
    }
  }
}

