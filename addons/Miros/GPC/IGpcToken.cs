using GPC.Scheduler;

namespace GPC;

public interface IGpcToken
{
    AbsScheduler GetScheduler();
}