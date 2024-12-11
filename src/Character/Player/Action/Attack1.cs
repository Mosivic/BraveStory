using Miros.Core;

namespace BraveStory;

public partial class Attack1Action : StateNode<State, Player>
{
    
    public override Tag StateTag  => Tags.State_Action_Attack1;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;

    public override Transition[] Transitions => [
            new (Tags.State_Action_Idle),
            new (Tags.State_Action_Attack11,() => Host.KeyDownAttack(), TransitionMode.DelayFront),
        ];
    

    


    protected override void Enter()
    {
        Host.PlayAnimation("attack1");
    }

    protected override bool ExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}
