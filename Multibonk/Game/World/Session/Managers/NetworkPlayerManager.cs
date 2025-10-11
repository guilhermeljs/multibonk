using Il2Cpp;
using Multibonk.Game.World.Session.Models;
using UnityEngine;

namespace Multibonk.Game.World.Session.Managers
{
    public class NetworkPlayerManager
    {
        private readonly Dictionary<byte, NetworkPlayer> _players = new();

        private static List<CharacterData> _indexedByName;
        public static List<CharacterData> CharacterData
        {
            get
            {
                if (_indexedByName != null)
                    return _indexedByName;

                var allCharacters = Resources.FindObjectsOfTypeAll<CharacterData>();

                _indexedByName = allCharacters
                    .OrderBy(c => c.description)
                    .ToList();

                return _indexedByName;
            }
        }

        public NetworkPlayer GetPlayer(byte playerId)
        {
            if (_players.TryGetValue(playerId, out var player) && !player.IsNullOrDestroyed())
                return player;

            _players.Remove(playerId);
            return null;
        }

        /// <summary>
        /// Spawns a new player and adds it to the manager.
        /// </summary>
        public NetworkPlayer SpawnPlayer(byte playerId, ECharacter character, Vector3 position, Quaternion rotation)
        {
            var data = CharacterData.Find(d => d.eCharacter == character);

            var playerObj = new GameObject($"player-from-id-{playerId}");
            playerObj.transform.position = position;
            playerObj.transform.rotation = rotation;

            var rb = playerObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            var rendererContainer = new GameObject("NetworkPlayer");
            rendererContainer.transform.SetParent(playerObj.transform);
            rendererContainer.layer = LayerMask.NameToLayer("Default");

            var renderer = rendererContainer.AddComponent<PlayerRenderer>();
            var inv = new PlayerInventory(data);
            renderer.SetCharacter(data, inv, position);
            renderer.CreateMaterials(1);

            rendererContainer.transform.localPosition = new Vector3(0, -(data.colliderHeight / 2), 0);
            rendererContainer.transform.localRotation = Quaternion.identity;

            var networkPlayer = new NetworkPlayer(playerObj);
            _players[playerId] = networkPlayer;

            return networkPlayer;
        }

        public void RemovePlayer(byte playerId)
        {
            if (_players.TryGetValue(playerId, out var player) && player.PlayerObject != null)
                GameObject.Destroy(player.PlayerObject);

            _players.Remove(playerId);
        }

        public void Clear()
        {
            foreach (var player in _players.Values)
                if (player.PlayerObject != null)
                    GameObject.Destroy(player.PlayerObject);

            _players.Clear();
        }
    }
}
