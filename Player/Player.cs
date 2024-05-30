using System.Collections.Generic;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.State;

public class CommonLib : EvaluatorLib
{
    public readonly Evaluator IsJumpKeyDown =
        new(state => Input.IsActionJustPressed("jump"));

    public readonly Evaluator IsMoveKeyDown =
        new(state => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")));
}


public class PlayerLib : EvaluatorLib
{
    public readonly Evaluator IsOnFloor =
        new(state => ((PlayerState)state).Host.IsOnFloor());

    public readonly Evaluator IsVelocityYPositive =
        new(state => ((PlayerState)state).Host.Velocity.Y >= 0f);
}


public partial class Player : CharacterBody2D
{
    private ConditionMachine<PlayerState> _cm;

    public override void _Ready()
    {
        EvaluatorSpace<CommonLib, PlayerLib> es = new(new CommonLib(), new PlayerLib());

        var ss = new StateSpace();
        ss.Add(new PlayerState(this)
        {
            Id = "1",
            Type = typeof(Move<PlayerState>),
            Name = "Run",
            Priority = 2,
            PreCondition = new Condition(new Dictionary<Evaluator, bool>
            {
                { es.Common.IsMoveKeyDown, true }
                // { es.Private.IsOnFloor, true }
            }),
            FailedCondition = new Condition(new Dictionary<Evaluator, bool>
            {
                { es.Common.IsMoveKeyDown, false }
            })
        }).Add(new PlayerState(this)
        {
            Id = "2",
            Name = "Idle",
            Priority = 1,
            Type = typeof(Idle<PlayerState>),
            PreCondition = new Condition(new Dictionary<Evaluator, bool>
            {
                { es.Common.IsMoveKeyDown, false },
                { es.Private.IsOnFloor, true }
            })
        }).Add(new PlayerState(this)
            {
                Id = "3",
                Name = "Jump",
                Priority = 3,
                Type = typeof(Jump<PlayerState>),
                PreCondition = new Condition(new Dictionary<Evaluator, bool>
                {
                    { es.Common.IsJumpKeyDown, true },
                    { es.Private.IsOnFloor, true }
                })
            }
        ).Add(new PlayerState(this)
        {
            Id = "4",
            Name = "Fall",
            Priority = 9,
            Type = typeof(Fall<PlayerState>),
            PreCondition = new Condition(new Dictionary<Evaluator, bool>
            {
                { es.Private.IsOnFloor, false },
                { es.Private.IsVelocityYPositive, true }
            }),
            FailedCondition = new Condition(new Dictionary<Evaluator, bool>
            {
                { es.Private.IsOnFloor, true }
            })
        });


        _cm = new ConditionMachine<PlayerState>(ss);
    }

    public override void _Process(double delta)
    {
        _cm.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _cm.PhysicsUpdate(delta);
    }
}