using System.Collections.Generic;
using Godot;
using GPC;
using GPC.AI;
using GPC.Job.Config;

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
}


public partial class Player : CharacterBody2D
{
    private ConditionMachine<PlayerState> _cm;

    public override void _Ready()
    {
        EvaluatorSpace<CommonLib, PlayerLib> es = new(new CommonLib(), new PlayerLib());

        StateSpace ss = new StateSpace();
        ss.Add<PlayerState>(new(this)
        {

            Id = "1",
            Type = typeof(Move<PlayerState>),
            Name = "Run",
            Priority = 2,
            PreCondition = new Condition(new Dictionary<Evaluator, bool>
                {
                    { es.Common.IsMoveKeyDown, true },
					// { es.Private.IsOnFloor, true }
				}),
            // FailedCondition = new Condition(new Dictionary<Evaluator, bool>
            // {
            //     { es.Private.IsOnFloor, false }
            // })

        }).Add<PlayerState>(new(this)
        {
            Id = "2",
            Name = "Idle",
            Priority = 1,
            Type = typeof(Idle<PlayerState>),
            PreCondition = new Condition(new Dictionary<Evaluator, bool>
                {
                    { es.Common.IsMoveKeyDown, false },
                    { es.Private.IsOnFloor, true }
                }),
            FailedCondition = new Condition(new Dictionary<Evaluator, bool>
                {
                    { es.Private.IsOnFloor, false }
                })
        }).Add<PlayerState>(new(this)
        {
            Id = "3",
            Name = "Jump",
            Priority = 3,
            Type = typeof(Jump<PlayerState>),
            PreCondition = new Condition(new Dictionary<Evaluator, bool>
                {
                    { es.Common.IsJumpKeyDown, true },
                    { es.Private.IsOnFloor, true }
                }),
            FailedCondition = new Condition(new Dictionary<Evaluator, bool>()
                {
                    {es.Private.IsOnFloor,false}
                })
        }
        );


        _cm = new ConditionMachine<PlayerState>(states);
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
