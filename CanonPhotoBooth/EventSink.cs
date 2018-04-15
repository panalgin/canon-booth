using CanonPhotoBooth.Contracts;
using System;

namespace CanonPhotoBooth
{
    public static class EventSink
    {
        public delegate void OnDevToolsRequested(Type requestedFrom);
        public delegate void OnVisitorRegistered(Visitor visitor);
        public delegate void OnGameInitialized();
        public delegate void OnGameTriggered();
        public delegate void OnGameStarted();
        public delegate void OnGameUpdated(int timeLeft);
        public delegate void OnGameFinished(Player winner);
        public delegate void OnGameReset();

        public delegate void OnRecordRequested();
        public delegate void OnGifGenerated(int playerIndex, string filePath);

        public delegate void OnPlayerJoined(Player player);
        public delegate void OnPlayerUpdated(Player player);

        public static event OnDevToolsRequested DevToolsRequested;
        public static event OnVisitorRegistered VisitorRegistered;
        public static event OnGameInitialized GameInitialized;
        public static event OnGameTriggered GameTriggered;
        public static event OnGameStarted GameStarted;
        public static event OnGameUpdated GameUpdated;
        public static event OnGameFinished GameFinished;
        public static event OnGameReset GameReset;
        public static event OnRecordRequested RecordRequested;
        public static event OnGifGenerated GifGenerated;

        public static event OnPlayerJoined PlayerJoined;
        public static event OnPlayerUpdated PlayerUpdated;

        public static void InvokeDevToolsRequested(Type requestedFrom)
        {
            DevToolsRequested?.Invoke(requestedFrom);
        }

        public static void InvokeVisitorRegistered(Visitor visitor)
        {
            VisitorRegistered?.Invoke(visitor);
        }

        public static void InvokeGameInitialized()
        {
            GameInitialized?.Invoke();
        }

        public static void InvokeGameTriggered()
        {
            GameTriggered?.Invoke();
        }

        public static void InvokeGameStarted()
        {
            GameStarted?.Invoke();
        }

        public static void InvokeGameUpdated(int timeLeft)
        {
            GameUpdated?.Invoke(timeLeft);
        }

        public static void InvokeGameFinished(Player winner)
        {
            GameFinished?.Invoke(winner);
        }

        public static void InvokeGameReset()
        {
            GameReset?.Invoke();
        }

        public static void InvokePlayerJoined(Player player)
        {
            PlayerJoined?.Invoke(player);
        }

        public static void InvokePlayerUpdated(Player player)
        {
            PlayerUpdated?.Invoke(player);
        }

        public static void InvokeRecordRequested()
        {
            RecordRequested?.Invoke();
        }

        public static void InvokeGifGenerated(int playerIndex, string filePath)
        {
            GifGenerated?.Invoke(playerIndex, filePath);
        }
    }
}