using Miros.Core;

namespace BraveStory;

public class Attack11ActionState : ActionState
{
    private PlayerContext _ctx;
    private Player _host;

    public override Tag Tag => Tags.State_Action_Attack11;
    public override Tag Layer => Tags.StateLayer_Movement;

    public override Transition[] Transitions =>
    [
        new(Tags.State_Action_Idle),
        new(Tags.State_Action_Attack111, () => _host.KeyDownAttack(), TransitionMode.DelayFront)
    ];

    public override void Init()
    {
        _ctx = Context as PlayerContext;
        _host = _ctx.Host;

        EnterFunc = OnEnter;
        PhysicsUpdateFunc = OnPhysicsUpdate;
        ExitCondition = OnExitCondition;
    }

    private void OnEnter()
    {
        _host.PlayAnimation("attack11");
    }

    private void OnPhysicsUpdate(double delta)
    {
        if (_ctx.IsHit && _ctx.HitAgent != null)
        {
            var damageEffect = new Effect
            {
                Tag = Tags.Effect_Buff,
                SourceAgent = OwnerAgent,
                RemovePolicy = RemovePolicy.WhenExited,
                DurationPolicy = DurationPolicy.Instant,
                Executions = [new CustomAttackDamageExecution(OwnerAgent.Atr("Attack") + 1)]
            };

            _ctx.HitAgent.AddEffect(damageEffect);
            _ctx.IsHit = false;
        }
    }

    private bool OnExitCondition()
    {
        return _host.IsAnimationFinished();
    }
}