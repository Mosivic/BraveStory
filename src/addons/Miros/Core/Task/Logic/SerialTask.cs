/*
    串行任务，所有子任务都执行完毕后，任务成功
    注意⚠️：
    1. 因为结束条件固定为所有子任务顺序执行完毕，所以 ExitConditionFunc 无效
*/

namespace Miros.Core;

public class SerialTask : TaskBase<State>
{
    private int _currentIndex;
    private State _currentState;
    private bool _isFinished;
    private int _subStatesCount;

    public override void TriggerOnAdd(State state)
    {
        base.TriggerOnAdd(state);

        foreach (var subState in state.SubStates)
        {
            var subTask = TaskProvider.GetTask(subState.TaskType);
            subState.Task = subTask;
            subState.Init();
        }

        _subStatesCount = state.SubStates.Length;
    }

    public override void Enter(State state)
    {
        base.Enter(state);

        _currentIndex = 0;
        _isFinished = false;

        if (_subStatesCount > 0)
        {
            _currentState = state.SubStates[_currentIndex];
            _currentState.Task.Enter(_currentState);
        }
    }


    public override void Update(State state, double delta)
    {
        base.Update(state, delta);

        if (_isFinished == false && _currentIndex < _subStatesCount)
        {
            _currentState.Task.Update(_currentState, delta);

            if (_currentState.Task.CanExit(_currentState))
            {
                _currentState.Task.Exit(_currentState);
                _currentIndex++;

                if (_currentIndex == _subStatesCount)
                {
                    _isFinished = true;
                    state.Status = RunningStatus.Succeed;
                    return;
                }

                _currentState = state.SubStates[_currentIndex];
                _currentState.Task.Enter(_currentState);
            }
        }
    }

    public override void PhysicsUpdate(State state, double delta)
    {
        base.PhysicsUpdate(state, delta);

        if (_isFinished == false && _currentIndex < _subStatesCount)
            _currentState.Task.PhysicsUpdate(_currentState, delta);
    }

    public override bool CanExit(State state)
    {
        return _currentIndex == _subStatesCount || _subStatesCount == 0;
    }
}