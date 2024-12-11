using Miros.Core;

namespace BraveStory;

public partial class Attack111Action : StateNode<State,Player>
{
    public override State State => new State(Tags.State_Action_Attack111, Agent);
    public override Tag StateTag  => Tags.State_Action_Attack111;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerStateMachine;
    public override Transition[] Transitions  => [
            new (Tags.State_Action_Idle),
            new (Tags.State_Action_Attack111,() => Host.KeyDownAttack(), TransitionMode.DelayFront),
        ];
    

    protected override void Enter()
    {
        Host.PlayAnimation("attack111");
    }

    protected override bool ExitCondition()
    {
        return Host.IsAnimationFinished();
    }
}
