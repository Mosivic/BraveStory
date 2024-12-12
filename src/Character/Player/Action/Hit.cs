using Miros.Core;

namespace BraveStory;

public class HitAction : Stator<State, Player,PlayerShared>
{
    public override Tag StateTag  => Tags.State_Action_Hit;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    public override Transition[] Transitions  => [
            new (Tags.State_Action_Idle, () => Host.IsAnimationFinished()),
            new (Tags.State_Action_Hit, () => Shared.IsHurt, TransitionMode.Force, 0, true)
        ];
    
    protected override void Enter()
    {
        Host.PlayAnimation("hit");
        Shared.IsHurt = false;
    }


}
