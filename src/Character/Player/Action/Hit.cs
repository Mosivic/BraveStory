using Miros.Core;

namespace BraveStory;

public partial class HitAction : StateNode<Player>
{
    protected override Tag StateTag  => Tags.State_Action_Hit;
    protected override Tag LayerTag => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    protected override Transition AnyTransition  => new (Tags.State_Action_Idle,() => Res["Hurt"], TransitionMode.Force); 

    protected override Transition[] Transitions  => [
            new (Tags.State_Action_Idle, () => Host.IsAnimationFinished()),
        ];
    
    protected override void ShareRes()
    {
        Res["Hurt"] = true;
    }
    protected override void Enter()
    {
        Host.PlayAnimation("hit");
        Res["Hurt"] = false;
    }


}
