using CanonPhotoBooth.Contracts;
using System;

namespace CanonPhotoBooth
{
    public static class EventSink
    {
        public delegate void OnDevToolsRequested(Type requestedFrom);
        public delegate void OnVisitorRegistered(Visitor visitor);
        public delegate void OnGameInitialized();
        public delegate void OnGameStarted();
        public delegate void OnGameFinished();
        public delegate void OnGameReset();

        public static event OnDevToolsRequested DevToolsRequested;
        public static event OnVisitorRegistered VisitorRegistered;
        public static event OnGameInitialized GameInitialized;
        public static event OnGameStarted GameStarted;
        public static event OnGameFinished GameFinished;
        public static event OnGameReset GameReset;

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

        public static void InvokeGameStarted()
        {
            GameStarted?.Invoke();
        }

        public static void InvokeGameFinished()
        {
            GameFinished?.Invoke();
        }

        public static void InvokeGameReset()
        {
            GameReset?.Invoke();
        }
    }
}