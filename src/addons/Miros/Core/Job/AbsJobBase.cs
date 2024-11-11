using System;
using System.Collections.Generic;


namespace Miros.Core;

public abstract class AbsJobBase : AbsJob, IJob
{
    private protected readonly AbsState state;

    protected AbsJobBase(AbsState state) : base(state)
    {
        this.state = state;
    }

    public AbsState State => state;

    public virtual void Enter()
    {
        state.Status = RunningStatus.Running;

        _Enter();
    }

    public void Exit()
    {
        if(CanExit())
            OnSucceed();
        else
            OnFailed();

        _Exit();
    }


    public virtual void Pause()
    {
        state.Status = RunningStatus.NoRun;
        _Pause();
    }


    public virtual void Resume()
    {
        state.Status = RunningStatus.Running;
        _Resume();
    }


    public virtual bool CanEnter()
    {
        return state.EnterCondition?.Invoke(state) ?? true;
    }


    public virtual bool CanExit()
    {
        return state.ExitCondition?.Invoke(state) ?? true;
    }
    

    public virtual void Update(double delta)
    {
        if (state.Status != RunningStatus.Running) return;
        _Update(delta);
    }

    public virtual void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
    }

    protected virtual void OnSucceed()
    {
        state.Status = RunningStatus.Succeed;
        _OnSucceed();
    }

    protected virtual void OnFailed()
    {
        state.Status = RunningStatus.Failed;
        _OnFailed();
    }



}