
namespace Miros.Core;

public class HostState<THost> : AbsState<HostState<THost>>
{
    public THost Host { get; set; }
    
    public HostState(THost host){
        Host = host;
    }
}