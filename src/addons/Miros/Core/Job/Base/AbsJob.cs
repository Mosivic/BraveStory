using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsJob
{
    public event Action<NativeState> OnEnter;
    public event Action<NativeState> OnExit;
    public event Action<NativeState, double> OnUpdate;
    public event Action<NativeState, double> OnPhysicsUpdate;
    public event Action<NativeState> OnSucceed;
    public event Action<NativeState> OnFailed;
    public event Action<NativeState> OnPause;
    public event Action<NativeState> OnResume;

    public event Func<NativeState, bool> EnterCondition;
    public event Func<NativeState, bool> ExitCondition;


    protected NativeState State { get; }

    protected AbsJob(NativeState state)
    {
        State = state;
        RegisterHandlers();
    }

    /// <summary>
    /// 注册组件的事件处理器
    /// </summary>
    protected virtual void RegisterHandlers()
    {
        foreach (var component in State.Components.Values)
        {
            component.RegisterHandler();
        }
    }

    /// <summary>
    /// 注销组件的事件处理器
    /// </summary>
    protected virtual void UnregisterHandlers()
    {
        foreach (var component in State.Components.Values)
        {
            component.UnregisterHandler();
        }
    }

    protected virtual void _Enter()
    {
        OnEnter?.Invoke(State); 
    }

    protected virtual void _Exit()
    {
        OnExit?.Invoke(State);
    }

    protected virtual void _Pause()
    {
        OnPause?.Invoke(State);
    }

    protected virtual void _Resume()
    {
        OnResume?.Invoke(State);
    }       

    protected virtual void _Update(double delta)
    {
        OnUpdate?.Invoke(State, delta);
    }   

    protected virtual void _PhysicsUpdate(double delta)
    {
        OnPhysicsUpdate?.Invoke(State, delta);
    }

    protected virtual void _Succeed()
    {
        OnSucceed?.Invoke(State );
    }

    protected virtual void _Failed()
    {
        OnFailed?.Invoke(State);
    }

    protected virtual bool _CanEnter()
    {
        return EnterCondition?.Invoke(State) ?? true;
    }

    protected virtual bool _CanExit()
    {
        return ExitCondition?.Invoke(State) ?? true;
    }
}   