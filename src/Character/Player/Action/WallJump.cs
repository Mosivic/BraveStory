using Godot;
using Miros.Core;

namespace BraveStory;

public class WallJumpActionState : ActionState<Player, PlayerContext>
{
    public override Tag Tag => Tags.State_Action_WallJump;
    public override Tag Layer => Tags.StateLayer_Movement;
    public override Transition[] Transitions => [
        new(Tags.State_Action_Fall)
    ];

    public override void Init(Player host, PlayerContext context)
    {
        base.Init(host, context);

        EnterFunc += OnEnter;
    }

    private void OnEnter()
    {
        Host.PlayAnimation("jump");
        Host.Velocity = new Vector2(Host.Graphics.Scale.X * 400, -320);
    }
}