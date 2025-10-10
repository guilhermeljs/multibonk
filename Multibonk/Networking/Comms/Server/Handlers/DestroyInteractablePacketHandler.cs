using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Server.Handlers
{
  /// <summary>
  /// Server handler to process BaseInteractable destruction requests sent by clients
  /// </summary>
  public class DestroyInteractablePacketHandler : IServerPacketHandler
  {
    public byte PacketId => (byte)ClientSentPacketId.DESTROY_INTERACTABLE_PACKET;

    private readonly LobbyContext _lobbyContext;

    public DestroyInteractablePacketHandler(LobbyContext lobbyContext)
    {
      _lobbyContext = lobbyContext;
    }

    public void Handle(IncomingMessage msg, Connection conn)
    {
      var packet = new DestroyInteractablePacket(msg);

      // Enqueue execution on Unity's main thread
      GameDispatcher.Enqueue(() =>
      {
        // Destroy the interactable on the server
        GameFunctions.DestroyNetworkInteractable(packet.InstanceId);
      });

      // Propagate destruction to all other clients
      foreach (var player in _lobbyContext.GetPlayers())
      {
        // Don't send back to the requester
        if (player.Connection == null || player.Connection == conn)
          continue;

        var destroyPacket = new SendDestroyInteractablePacket(packet.InstanceId);
        player.Connection.EnqueuePacket(destroyPacket);
      }
    }
  }
}

