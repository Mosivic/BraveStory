using Godot;
using GPC.States;

namespace GPC.Job;

internal class JobSingle(State state) : AbsJob(state), IJob
{
    private readonly State _state = state;

    public void Enter()
    {
#if DEBUG
        GD.Print($"{_state.Name} Enter.");
#endif
        _state.Status = Status.Running;
        _Enter();
        _state.EnterAttachFunc?.Invoke(_state);
    }

    public void Exit()
    {
#if DEBUG
        GD.Print($"{_state.Name} Exit.");
#endif
        _Exit();
        _state.ExitAttachFunc?.Invoke(_state);
    }

    public void Pause()
    {
        _state.Status = Status.Pause;
        _Pause();
        _state.PauseAttachFunc?.Invoke(_state);
    }

    public void Resume()
    {
        _state.Status = Status.Running;
        _Resume();
        _state.ResumeAttachFunc?.Invoke(_state);
    }

    public bool IsSucceed()
    {
        return _IsSucceed();
    }

    public bool IsPrepared()
    {
        return _IsPrepared();
    }

    public bool IsFailed()
    {
        return _IsFailed();
    }
    
    public void Update(double delta)
    {
        if (_state.Status == Status.Pause) return;

        _Update(delta);
        _state.RunningAttachFunc?.Invoke(_state);
        _UpdateJob();
    }

    public void PhysicsUpdate(double delta)
    {
        _PhysicsUpdate(delta);
        _state.RunningPhysicsAttachFunc?.Invoke(_state);
    }


    private void _UpdateJob()
    {
        if (IsFailed())
            _state.Status = Status.Failed;
        //applyEffect()
        else if (IsSucceed())
            _state.Status = Status.Succeed;
        //applyEffect()
        else
            _state.Status = Status.Running;
    }
}