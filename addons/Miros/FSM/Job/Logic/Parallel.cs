using FSM.Job.Executor;
using FSM.States;

namespace FSM.Job;


/* Execute many jobs at some time. 
 * Ignore substate's prepare condition.
 * 
 */
internal class Parallel(IJobProvider jobProvider,CompoundState state) : JobBase(state)
{
    
    public override void Enter()
    {
        foreach (var absState in state.SubStates)
        {
            jobProvider.GetJob(absState).Enter();
        }
        base.Enter();
    }

    public override void Exit()
    {
        foreach (var absState in state.SubStates)
        {
            jobProvider.GetJob(absState).Exit();
        }
        base.Exit();
    }


    public override void Pause()
    {
        foreach (var absState in state.SubStates)
        {
            jobProvider.GetJob(absState).Pause();
        }
        base.Pause();
    }

    public override void Resume()
    {
        foreach (var absState in state.SubStates)
        {
            jobProvider.GetJob(absState).Resume();
        }
        base.Resume();
    }
    
}