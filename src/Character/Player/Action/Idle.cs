using Miros.Core;

namespace BraveStory;

public class IdleActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;

    public override Tag Tag => Tags.State_Action_Idle;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override bool AsDefaultTask => true;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Run, () => _host.KeyDownMove()),
        new(Tags.State_Action_Fall, () => !_host.IsOnFloor()),
        new(Tags.State_Action_Jump, () => _host.KeyDownJump()),
        new(Tags.State_Action_Attack1, () => _host.KeyDownAttack()),
        new(Tags.State_Action_Sliding, () => _host.KeyDownSliding())
    ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc = OnEnter;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("idle");
        _ctx.JumpCount = 0;
    }
}