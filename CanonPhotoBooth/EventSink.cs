using System;

namespace CanonPhotoBooth
{
    public static class EventSink
    {
        public delegate void OnDevToolsRequested(Type requestedFrom);

        public static event OnDevToolsRequested DevToolsRequested;

        public static void InvokeDevToolsRequested(Type requestedFrom)
        {
            DevToolsRequested?.Invoke(requestedFrom);
        }
    }
}