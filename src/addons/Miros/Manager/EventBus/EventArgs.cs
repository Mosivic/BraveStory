using System;

namespace Miros.EventBus;

public class GameEventArgs : EventArgs
{
    public string EventName { get; }
    
    public GameEventArgs(string eventName)
    {
        EventName = eventName;
    }
}