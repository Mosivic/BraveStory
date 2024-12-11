using Miros.Core;

namespace BraveStory;

public partial class Attack11Action : StateNode<Player>
{
    protected override Tag StateTag  => Tags.State_Action_Attack11;
    protected override Tag LayerTag => Tags.StateLayer_Movement;
    protected override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    protected override Transition[] Transitions  => [
            new (Tags.State_Action_Idle),
            new (Tags.State_Action_Attack11,() => Host.KeyDownAttack(), TransitionMode.DelayFront),
        ];
    

    protected override void Enter()
    {
        Host.PlayAnimation("attack11");
    }

    protected override bool ExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}
