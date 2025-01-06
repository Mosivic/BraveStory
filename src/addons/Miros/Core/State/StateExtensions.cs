namespace Miros.Core;

public static class StateExtensions
{

    public static void Enter(this State state)
    {
        state.Task.Enter(state);
    }

    public static void Exit(this State state)
    {
        state.Task.Exit(state);
    }

    public static void Pause(this State state)
    {
        state.Task.Pause(state);
    }

    public static void Resume(this State state)
    {
        state.Task.Resume(state);
    }

    public static bool CanEnter(this State state)
    {
        return state.Task.CanEnter(state);
    }

    public static bool CanExit(this State state)
    {
        return state.Task.CanExit(state);
    }

    public static bool CanRemove(this State state)
    {
        return state.Task.CanRemove(state);
    }

    public static void Update(this State state, double delta)
    {
        state.Task.Update(state, delta);
    }

    public static void PhysicsUpdate(this State state, double delta)
    {
        state.Task.PhysicsUpdate(state, delta);
    }

    public static void Stack(this State state, bool IsFromSameSource)
    {
        state.Task.Stack(state, IsFromSameSource);
    }

}