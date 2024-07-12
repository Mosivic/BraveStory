using FSM.Scheduler;

namespace FSM;

public interface IGpcToken
{
    AbsScheduler GetScheduler();
    Layer GetRootLayer();
}