using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsJob(NativeState state)
{
    public event Action<NativeState> OnEntered;
    public event Action<NativeState> OnExited;
    public event Action<NativeState, double> OnUpdated;
    public event Action<NativeState, double> OnPhysicsUpdated;
    public event Action<NativeState> OnSucceeded;
    public event Action<NativeState> OnFailed;
    public event Action<NativeState> OnPaused;
    public event Action<NativeState> OnResumed;

    public event Func<NativeState, bool> EnterCondition;
    public event Func<NativeState, bool> ExitCondition;





    protected virtual void OnEnter()
    {
        OnEntered?.Invoke(state); 
    }

    protected virtual void OnExit()
    {
        OnExited?.Invoke(state);
    }

    protected virtual void OnPause()
    {
        OnPaused?.Invoke(state);
    }

    protected virtual void OnResume()
    {
        OnResumed?.Invoke(state);
    }       

    protected virtual void OnUpdate(double delta)
    {
        OnUpdated?.Invoke(state, delta);
    }   

    protected virtual void OnPhysicsUpdate(double delta)
    {
        OnPhysicsUpdated?.Invoke(state, delta);
    }

    protected virtual void OnSucceed()
    {
        OnSucceeded?.Invoke(state );
    }

    protected virtual void OnFail()
    {
        OnFailed?.Invoke(state);
    }

    protected virtual bool OnCanEnter()
    {
        return EnterCondition?.Invoke(state) ?? true;
    }

    protected virtual bool OnCanExit()
    {
        return ExitCondition?.Invoke(state) ?? true;
    }
}   