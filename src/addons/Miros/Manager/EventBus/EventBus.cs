using System;
using System.Collections.Generic;
using Godot;

namespace Miros.EventBus;

public class EventBus
{
    private static EventBus _instance;
    public static EventBus Instance => _instance ??= new EventBus();

    // 使用字典存储事件及其对应的处理器
    private readonly Dictionary<string, List<Delegate>> _eventHandlers = new();

    private EventBus() { }

    // 订阅无参数事件
    public void Subscribe(string eventName, Action handler)
    {
        Subscribe<GameEventArgs>(eventName, (sender, args) => handler());
    }

    // 订阅带参数事件
    public void Subscribe<T>(string eventName, EventHandler<T> handler) where T : GameEventArgs
    {
        if (!_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName] = new List<Delegate>();
        }
        _eventHandlers[eventName].Add(handler);
    }

    // 取消订阅无参数事件
    public void Unsubscribe(string eventName, Action handler)
    {
        Unsubscribe<GameEventArgs>(eventName, (sender, args) => handler());
    }

    // 取消订阅带参数事件
    public void Unsubscribe<T>(string eventName, EventHandler<T> handler) where T : GameEventArgs
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName].Remove(handler);
            if (_eventHandlers[eventName].Count == 0)
            {
                _eventHandlers.Remove(eventName);
            }
        }
    }

    // 发布无参数事件
    public void Publish(string eventName)
    {
        Publish(eventName, new GameEventArgs(eventName));
    }

    // 发布带参数事件
    public void Publish<T>(string eventName, T args) where T : GameEventArgs
    {
        if (!_eventHandlers.ContainsKey(eventName))
        {
            return;
        }

        foreach (var handler in _eventHandlers[eventName].ToArray())
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
                GD.PrintErr($"Error handling event {eventName}: {e}");
            }
        }
    }

    // 清除所有事件订阅
    public void Clear()
    {
        _eventHandlers.Clear();
    }
}