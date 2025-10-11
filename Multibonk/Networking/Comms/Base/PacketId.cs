namespace Multibonk.Networking.Comms.Base
{


    public enum ServerSentPacketId : byte
    {
       LOBBY_PLAYER_LIST_PACKET = 0,
       PLAYER_SELECTED_CHARACTER = 1,
       START_GAME = 2,

       MAP_FINISHED_LOADING = 5,
       SPAWN_PLAYER_PACKET = 6,

       PLAYER_MOVED_PACKET = 7,
       PLAYER_ROTATED_PACKET = 8,
       MAP_OBJECT_CHUNK_PACKET = 9,
       XP_PACKET = 10,
       PLAYER_ANIMATOR_CHANGED_PACKET = 11,
       ENEMY_DEATH_PACKET = 12,
       ENEMY_SPAWNED_PACKET = 13,


        GAME_PAUSED = 3,
        GAME_UNPAUSED = 4,
    }

    public enum ClientSentPacketId : byte
    {
        JOIN_LOBBY_PACKET = 0,
        CHARACTER_SELECTION = 1,
        GAME_LOADED_PACKET = 2,
        PLAYER_MOVE_PACKET = 3,
        PLAYER_ROTATE_PACKET = 4,
        PICKUP_XP_PACKET = 5,
        PLAYER_ANIMATOR_PACKET = 6,
        SPAWN_ENEMY_PACKET = 7,
        KILL_ENEMY_PACKET = 8,

        PAUSE_GAME = 9,
        UNPAUSE_GAME = 10,
    }
}
