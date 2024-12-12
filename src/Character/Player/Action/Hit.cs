using Miros.Core;

namespace BraveStory;

public class HitAction : Task<State, Player,PlayerContext>
{
    public override Tag StateTag  => Tags.State_Action_Hit;
    public override Tag LayerTag => Tags.StateLayer_Movement;
    public override ExecutorType ExecutorType => ExecutorType.MultiLayerExecutor;

    public override Transition[] Transitions  => [
            new (Tags.State_Action_Idle, () => Host.IsAnimationFinished()),
            new (Tags.State_Action_Hit, () => Context.IsHurt, TransitionMode.Force, 0, true)
        ];
    
    protected override void OnEnter()
    {
        Host.PlayAnimation("hit");
        Context.IsHurt = false;
    }


}
