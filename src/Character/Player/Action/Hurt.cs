using Miros.Core;

namespace BraveStory;

public class HurtActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Action_Hurt;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
            new(Tags.State_Action_Idle, () => Host.IsAnimationFinished()),
            new(Tags.State_Action_Hurt, () => Context.IsHurt, TransitionMode.Force, 0, true)
        ];

    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("hurt");
        Context.IsHurt = false;
    }
}