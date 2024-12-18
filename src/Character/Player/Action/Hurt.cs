using Miros.Core;

namespace BraveStory;

public class HurtActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;

    public override Tag Tag => Tags.State_Action_Hurt;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
            new(Tags.State_Action_Idle, () => _host.IsAnimationFinished()),
            new(Tags.State_Action_Hurt, () => _ctx.IsHurt, TransitionMode.Force, 0, true)
        ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc = OnEnter;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("hurt");
        _ctx.IsHurt = false;
    }
}