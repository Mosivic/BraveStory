using Miros.Core;

namespace BraveStory;

public class HurtActionState : ActionState<PlayerContext>
{
    private Player _host;

    public override Tag Tag => Tags.State_Action_Hurt;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
            new(Tags.State_Action_Idle, () => _host.IsAnimationFinished()),
            new(Tags.State_Action_Hurt, () => Context.IsHurt, TransitionMode.Force, 0, true)
        ];

    public override void Init(PlayerContext context)
    {
        base.Init(context);
        _host = context.Host;

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("hurt");
        Context.IsHurt = false;
    }
}