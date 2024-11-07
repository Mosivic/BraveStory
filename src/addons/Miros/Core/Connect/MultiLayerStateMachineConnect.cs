using System.Collections.Generic;

namespace Miros.Core;

public class MultiLayerStateMachineConnect : Connect<StaticJobProvider, MultiLayerStateMachine>
{
    public MultiLayerStateMachineConnect(HashSet<AbsState> states,GameplayTagContainer ownedTags) : base(states)
    {
        _scheduler.SetOwnedTags(ownedTags);
    }

    public void AddLayer(GameplayTag layer,AbsState defaultState,StateTransitionContainer transitionContainer){
        _scheduler.AddLayer(layer,defaultState,transitionContainer);
    }
}