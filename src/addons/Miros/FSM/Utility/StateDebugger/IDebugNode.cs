using FSM.Job.Executor;

namespace FSM.Utility.StateDebugger;

public interface IDebugNode
{
    Layer GetRootLayer();
    IConnect GetConnect();
}