using CanonPhotoBooth.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonPhotoBooth
{
    public static class Game
    {
        public enum GameState
        {
            Awaiting, //Awaiting players
            Initialized, //Two players are present
            Running,
            Finished
        }

        const int MaxAllowedPlayers = 2;

        public static List<Player> Players = new List<Player>();
        public static GameState State { get; set; }

        public static bool IsJoinable()
        {
            if (Players.Count < 2)
                return true;

            return false;
        }

        public static void Reset()
        {
            Players.Clear();
            State = GameState.Awaiting;
        }

        public static void Join(Visitor visitor)
        {
            Player player = visitor as Player;
            Players.Add(player);

            if (Players.Count == MaxAllowedPlayers)
            {
                State = GameState.Initialized;
                EventSink.InvokeGameInitialized();
            }
        }

        public static void Start()
        {
            if (Players.Count == MaxAllowedPlayers && State == GameState.Initialized)
            {
                State = GameState.Running;
                EventSink.InvokeGameStarted();
            }
        }
    }
}
