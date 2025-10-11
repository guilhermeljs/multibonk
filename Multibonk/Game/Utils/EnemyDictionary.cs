namespace Multibonk.Game.Utils
{

    /// <summary>
    /// Stores enemies with unique IDs generated deterministically using a 24-bit counter
    /// and an 8-bit client ID. 
    /// Thread-safe, allows each client to spawn and generate their own id for their enemies.
    /// Different client ids must ALWAYS return different numbers.
    /// TODO: Unit testing for this class
    /// </summary>
    /// <typeparam name="TEnemy"></typeparam>
    public class EnemyDictionary<TEnemy>
    {
        private readonly Dictionary<uint, TEnemy> enemies = new();
        private readonly Dictionary<TEnemy, uint> reverseEnemies = new();

        private uint counter = 0;
        private readonly byte clientId;
        private readonly object lockObj = new();

        public EnemyDictionary(byte clientId)
        {
            this.clientId = clientId;
        }

        public void Add(uint id, TEnemy enemy)
        {
            lock (lockObj)
            {
                enemies[id] = enemy;
                reverseEnemies[enemy] = id;
            }
        }

        public uint Add(TEnemy enemy)
        {
            uint id;
            lock (lockObj)
            {
                id = ComposeId(clientId, counter);
                counter = (counter + 1) & 0x00FFFFFFu;
                enemies[id] = enemy;
                reverseEnemies[enemy] = id;
            }
            return id;
        }

        public TEnemy Get(uint id)
        {
            lock (lockObj)
            {
                return enemies.TryGetValue(id, out var e) ? e : default;
            }
        }

        public bool Remove(uint id)
        {
            lock (lockObj)
            {
                if (enemies.TryGetValue(id, out var enemy))
                    reverseEnemies.Remove(enemy);

                return enemies.Remove(id);
            }
        }

        public uint GetId(TEnemy e)
        {
            lock (lockObj)
            {
                return reverseEnemies.TryGetValue(e, out var id) ? id : 0;
            }
        }

        public bool Contains(uint id)
        {
            lock (lockObj)
            {
                return enemies.ContainsKey(id);
            }
        }

        private static uint ComposeId(byte clientId, uint counter)
        {
            return ((uint)clientId << 24) | (counter & 0x00FFFFFFu);
        }
    }
}