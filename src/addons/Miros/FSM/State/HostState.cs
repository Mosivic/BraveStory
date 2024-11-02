
namespace FSM.States;

public class HostState<THost> : AbsState
{
    public THost Host { get; set; }
    
    public HostState(THost host){
        Host = host;
    }
}