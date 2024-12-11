using Miros.Core;

namespace BraveStory;

public partial class HitAction : StateNode<State, Player>
{
    public override Tag StateTag  => Tags.State_Action_Hit;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition AnyTransition  => new (Tags.State_Action_Idle,() => Res["Hurt"], TransitionMode.Force); 

    public override Transition[] Transitions  => [
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
