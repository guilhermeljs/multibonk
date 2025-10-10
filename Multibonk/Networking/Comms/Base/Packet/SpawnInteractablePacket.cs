using Il2Cpp;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{
    /// <summary>
    /// Packet sent from server to clients when a BaseInteractable is created
    /// </summary>
    public class SendSpawnInteractablePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.SPAWN_INTERACTABLE_PACKET;

        public SendSpawnInteractablePacket(
            int instanceId,
            string prefabName,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            bool isItemSource,
            bool showOutline)
        {
            Message.WriteByte(Id);
            Message.WriteInt(instanceId);
            Message.WriteString(prefabName);

            // Position
            Message.WriteFloat(position.x);
            Message.WriteFloat(position.y);
            Message.WriteFloat(position.z);

            // Rotation
            Message.WriteFloat(rotation.x);
            Message.WriteFloat(rotation.y);
            Message.WriteFloat(rotation.z);
            Message.WriteFloat(rotation.w);

            // Scale
            Message.WriteFloat(scale.x);
            Message.WriteFloat(scale.y);
            Message.WriteFloat(scale.z);

            // Properties
            Message.WriteBool(isItemSource);
            Message.WriteBool(showOutline);
        }
    }

    internal class SpawnInteractablePacket
    {
        public int InstanceId { get; private set; }
        public string PrefabName { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Scale { get; private set; }
        public bool IsItemSource { get; private set; }
        public bool ShowOutline { get; private set; }

        public SpawnInteractablePacket(IncomingMessage msg)
        {
            InstanceId = msg.ReadInt();
            PrefabName = msg.ReadString();

            // Position
            Position = new Vector3(
                msg.ReadFloat(),
                msg.ReadFloat(),
                msg.ReadFloat()
            );

            // Rotation
            Rotation = new Quaternion(
                msg.ReadFloat(),
                msg.ReadFloat(),
                msg.ReadFloat(),
                msg.ReadFloat()
            );

            // Scale
            Scale = new Vector3(
                msg.ReadFloat(),
                msg.ReadFloat(),
                msg.ReadFloat()
            );

            // Properties
            IsItemSource = msg.ReadBool();
            ShowOutline = msg.ReadBool();
        }
    }
}

