using CanonPhotoBooth.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CanonPhotoBooth
{
    public static class Game
    {
        public enum GameState
        {
            Awaiting, //Awaiting players
            Initialized, //Two players are present
            Countdown, //Countdown to beginning
            Running,
            Aftermath,
        }

        const int MaxAllowedPlayers = 2;
        const int GameDuration = 90; //seconds
        const int AftermathDuration = 180; //seconds

        private static Timer GameTimer = new Timer(1000);
        private static Timer AftermathTimer = new Timer(1000);

        private static int CurrentGameTime = GameDuration;
        private static int CurrentAftermathTime = AftermathDuration;

        public static List<Player> Players = new List<Player>();
        public static GameState State { get; set; }

        public static void Initialize()
        {
            EventSink.VisitorRegistered += EventSink_VisitorRegistered;
        }

        private static void EventSink_VisitorRegistered(Visitor visitor)
        {
            if (State == GameState.Awaiting)
                Join(visitor);
        }

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
            AftermathTimer.Stop();

            EventSink.InvokeGameReset();
        }

        public static void Join(Visitor visitor)
        {
            Player player = Player.FromVisitor(visitor);
            Players.Add(player);

            if (Players.Count < 2)
                player.Board = World.Boards[0];
            else
                player.Board = World.Boards[1];

            EventSink.InvokePlayerJoined(player);

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
                State = GameState.Countdown;
                EventSink.InvokeGameTriggered();

                Task.Run(async () =>
                {
                    await StartWithDelay();
                });
            }
        }

        async static Task<bool> StartWithDelay()
        {
            await Task.Delay(3000);

            EventSink.InvokeGameStarted();

            State = GameState.Running;

            GameTimer.Elapsed -= GameTimer_Elapsed;
            GameTimer.Elapsed += GameTimer_Elapsed;
            GameTimer.Start();

            CurrentGameTime = GameDuration;

            return true;
        }

        private static void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentGameTime--;

            if (CurrentGameTime <= 0)
            {
                
                Finish();
            }
        }

        private static void Finish()
        {
            GameTimer.Stop();

            CurrentAftermathTime = AftermathDuration;

            AftermathTimer.Elapsed -= AftermathTimer_Elapsed;
            AftermathTimer.Elapsed += AftermathTimer_Elapsed;

            AftermathTimer.Start();

            EventSink.InvokeGameFinished();

            
            //decide winner
            //post results
        }

        private static void AftermathTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentAftermathTime--;

            if (CurrentAftermathTime <= 0)
            {
                Reset();
            }
        }
    }
}
