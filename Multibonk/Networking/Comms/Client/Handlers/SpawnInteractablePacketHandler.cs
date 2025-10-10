using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers
{
  /// <summary>
  /// Client handler to process BaseInteractable spawn packets sent by the server
  /// </summary>
  public class SpawnInteractablePacketHandler : IClientPacketHandler
  {
    public byte PacketId => (byte)ServerSentPacketId.SPAWN_INTERACTABLE_PACKET;

    public SpawnInteractablePacketHandler() { }

    public void Handle(IncomingMessage msg, Connection conn)
    {
      var packet = new SpawnInteractablePacket(msg);

      // Enqueue execution on Unity's main thread
      GameDispatcher.Enqueue(() =>
      {
        GameFunctions.SpawnNetworkInteractable(
                  packet.InstanceId,
                  packet.PrefabName,
                  packet.Position,
                  packet.Rotation,
                  packet.Scale,
                  packet.IsItemSource,
                  packet.ShowOutline
              );
      });
    }
  }
}

