namespace Miros.Core;

public class StandardDelegateHandler<T> : IStateComponentHandler<T> where T : AbsState<T>
{
    private readonly T _state;

    private AbsJob _job;

    public StandardDelegateHandler(T state)
    {
        _state = state;
    }

    public void Initialize(AbsJob job)
    {
        _job = job;
        if (_state.StandardDelegate != null)
        {
            // 订阅事件或设置回调
            _job.OnEnter += HandleEnter;
            _job.OnExit += HandleExit;
            // ... 其他标准委托的订阅
        }
    }

    public void Cleanup()
    {
        if (_state.StandardDelegate != null)
        {
            // 取消订阅
            _job.OnEnter -= HandleEnter;
            _job.OnExit -= HandleExit;
            // ... 其他清理工作
        }
    }

    private void HandleEnter()
    {
        _state.StandardDelegate.EnterFunc?.Invoke(_state);
    }

    private void HandleExit()
    {
        _state.StandardDelegate.ExitFunc?.Invoke(_state);
    }

    private void HandleOnSucceed()
    {
        _state.StandardDelegate.OnSucceedFunc?.Invoke(_state);
    }

    private void HandleOnFailed()
    {
        _state.StandardDelegate.OnFailedFunc?.Invoke(_state);
    }

    private void HandlePause()
    {
        _state.StandardDelegate.PauseFunc?.Invoke(_state);
    }

    private void HandleResume()
    {
        _state.StandardDelegate.ResumeFunc?.Invoke(_state);
    }

    private void HandleUpdate(double delta)
    {
        _state.StandardDelegate.UpdateFunc?.Invoke(_state, delta);
    }


    private void HandlePhysicsUpdate(double delta)
    {
        _state.StandardDelegate.PhysicsUpdateFunc?.Invoke(_state, delta);
    }


}