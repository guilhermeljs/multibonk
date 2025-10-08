using Il2Cpp;
using Multibonk.Networking.Comms.Base;

namespace Multibonk.Networking.Lobby
{
    public enum LobbyState
    {
        None, Hosting, Connected, AwaitingForHost
    }

    public class LobbyPlayer
    {
        private static int _uuidCounter = 0; // contador interno para IDs únicos

        public Connection Connection { get; private set; }
        public ushort UUID { get; private set; }
        public string Name { get; private set; }
        public string SelectedCharacter { get; set; }
        public int Ping { get; set; }

        public LobbyPlayer(
            string name = "Unknown",
            ushort? uuid = null,
            string selectedCharacter = "None",
            Connection connection = null
        )
        {
            Name = name;
            SelectedCharacter = selectedCharacter;
            Ping = 0;
            Connection = connection;

            if (uuid.HasValue)
            {
                UUID = uuid.Value;
            }
            else
            {
                UUID = (ushort)System.Threading.Interlocked.Increment(ref _uuidCounter);
            }
        }
    }

    public static class LobbyPatchFlags
    {
        public static bool IsHosting;
    }

    public class LobbyContext
    {
        private List<LobbyPlayer> players = new List<LobbyPlayer>();
        private readonly object _lock = new object();
        private LobbyPlayer myself;

        public LobbyState State { get; private set; }

        public event Action<LobbyContext> OnLobbyCreated;
        public event Action<LobbyContext> OnLobbyJoin;
        public event Action<string> OnLobbyJoinFailed;
        public event Action<LobbyPlayer, LobbyContext> OnPlayerJoined;
        public event Action<LobbyPlayer, LobbyContext> OnPlayerLeft;
        public event Action<LobbyState, LobbyState, LobbyContext> OnLobbyStateChanged;
        public event Action<LobbyContext> OnLobbyClosed;

        public void SetState(LobbyState state)
        {
            var previous = State;
            State = state;
            OnLobbyStateChanged?.Invoke(previous, State, this);
        }

        public LobbyPlayer GetMyself()
        {
            return myself;
        }

        public void SetMyself(LobbyPlayer player)
        {
            myself = player;
            AddPlayer(player);
        }

        public LobbyPlayer AddPlayer(string name)
        {
            var player = new LobbyPlayer(name);
            lock (_lock)
            {
                players.Add(player);
            }
            OnPlayerJoined?.Invoke(player, this);
            return player;
        }

        public void ClearPlayers()
        {
            lock (_lock)
            {
                players.Clear();
            }
        }

        public void AddPlayer(LobbyPlayer player)
        {
            lock (_lock)
            {
                players.Add(player);
            }
        }

        public LobbyPlayer RemovePlayer(Guid uuid)
        {
            var player = players.Find(p => uuid.Equals(p.UUID));
            if (player != null)
            {
                lock (_lock)
                {
                    players.Remove(player);
                }
                OnPlayerLeft?.Invoke(player, this);
            }
            return player;
        }

        public LobbyPlayer GetPlayer(ushort uuid)
        {
            var player = players.Find(p => uuid.Equals(p.UUID));

            return player;
        }


        public LobbyPlayer GetPlayer(Connection conn)
        {
            var player = players.Find(p => conn.Equals(p.Connection));

            return player;
        }

        /// <summary>
        /// This method is currently not zero-copy. It always returns a copy of the players list to be thread-safe.
        /// </summary>
        /// <returns></returns>
        public List<LobbyPlayer> GetPlayers()
        {
            lock (_lock)
            {
                return players.ToList();
            }
        }

        public void TriggerLobbyCreated() => OnLobbyCreated?.Invoke(this);
        public void TriggerLobbyJoin() => OnLobbyJoin?.Invoke(this);
        public void TriggerLobbyJoinFailed(string reason) => OnLobbyJoinFailed?.Invoke(reason);
        public void TriggerLobbyClosed() => OnLobbyClosed?.Invoke(this);
    }
}
