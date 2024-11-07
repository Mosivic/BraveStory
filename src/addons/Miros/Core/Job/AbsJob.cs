namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsJob(AbsState state)
{
    protected virtual void _Enter()
    {
        state.EnterFunc?.Invoke(state);
    }

    protected virtual void _Exit()
    {
        state.ExitFunc?.Invoke(state);
    }

    protected virtual void _Pause()
    {
        state.PauseFunc?.Invoke(state);
    }

    protected virtual void _Resume()
    {
        state.ResumeFunc?.Invoke(state);
    }

    protected virtual void _OnSucceed()
    {
        state.OnSucceedFunc?.Invoke(state);
    }

    protected virtual void _OnFailed()
    {
        state.OnFailedFunc?.Invoke(state);
    }


    protected virtual void _OnPeriodOver()
    {
        state.OnPeriodOverFunc?.Invoke(state);
    }

    protected virtual void _OnStack()
    {
        state.OnStackFunc?.Invoke(state);
    }

    protected virtual void _OnStackOverflow()
    {
        state.OnStackOverflowFunc?.Invoke(state);
    }

    protected virtual void _OnDurationOVer()
    {
        state.OnDurationOverFunc?.Invoke(state);
    }

    protected virtual void _PhysicsUpdate(double delta)
    {
        state.PhysicsUpdateFunc?.Invoke(state, delta);
    }

    protected virtual void _Update(double delta)
    {
        state.UpdateFunc?.Invoke(state, delta);
    }
}