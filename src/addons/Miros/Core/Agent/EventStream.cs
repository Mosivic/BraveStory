using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class EventStream
{
    // 使用字典存储事件及其对应的处理器
    private readonly Dictionary<string, List<Delegate>> _eventHandlers = [];


    // 截取无参数事件
    public void Throttle(string eventStreamSliceName, Action handler)
    {
        Throttle<EventStreamArgs>(eventStreamSliceName, (sender, args) => handler());
    }

    // 截取带参数事件
    public void Throttle<T>(string eventStreamSliceName, EventHandler<T> handler) where T : EventStreamArgs
    {
        if (!_eventHandlers.ContainsKey(eventStreamSliceName))
        {
            _eventHandlers[eventStreamSliceName] = new List<Delegate>();
        }
        _eventHandlers[eventStreamSliceName].Add(handler);
    }

    // 取消截取无参数事件
    public void Unthrottle(string eventStreamSliceName, Action handler)
    {
        Unthrottle<EventStreamArgs>(eventStreamSliceName, (sender, args) => handler());
    }

    // 取消截取带参数事件
    public void Unthrottle<T>(string eventStreamSliceName, EventHandler<T> handler) where T : EventStreamArgs
    {
        if (_eventHandlers.ContainsKey(eventStreamSliceName))
        {
            _eventHandlers[eventStreamSliceName].Remove(handler);
            if (_eventHandlers[eventStreamSliceName].Count == 0)
            {
                _eventHandlers.Remove(eventStreamSliceName);
            }
        }
    }

    // 发布无参数事件
    public void Push(string eventStreamSliceName)
    {
        Push<EventStreamArgs>(eventStreamSliceName, new EventStreamArgs(eventStreamSliceName));
    }

    // 发布带参数事件
    public void Push<T>(string eventStreamSliceName, T args) where T : EventStreamArgs
    {
        if (!_eventHandlers.ContainsKey(eventStreamSliceName))
        {
            return;
        }

        foreach (var handler in _eventHandlers[eventStreamSliceName].ToArray())
        {
            try
            {
                if (handler is EventHandler<T> typedHandler)
                {
                    typedHandler(this, args);
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error handling event {eventStreamSliceName}: {e}");
            }
        }
    }

    // 清除所有事件订阅
    public void Clear()
    {
        _eventHandlers.Clear();
    }
}