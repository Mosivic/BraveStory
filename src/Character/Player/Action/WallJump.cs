using Godot;
using Miros.Core;

namespace BraveStory;

public class WallJumpActionState : ActionState
{
    private PlayerContext _ctx;

    private Player _host;
    public override Tag Tag => Tags.State_Action_WallJump;
    public override Tag Layer => Tags.StateLayer_Movement;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Fall)
    ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc = OnEnter;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("jump");
        _host.Velocity = new Vector2(_host.Graphics.Scale.X * 400, -320);
    }
}