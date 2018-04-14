using CanonPhotoBooth.Contracts;
using System;

namespace CanonPhotoBooth
{
    public static class EventSink
    {
        public delegate void OnDevToolsRequested(Type requestedFrom);
        public delegate void OnVisitorRegistered(Visitor visitor);
        public delegate void OnGameInitialized();

        public static event OnDevToolsRequested DevToolsRequested;
        public static event OnVisitorRegistered VisitorRegistered;
        public static event OnGameInitialized GameInitialized;

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
    }
}