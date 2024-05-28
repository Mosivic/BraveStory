using System.Collections.Generic;
using Godot;
using GPC;
using GPC.AI;
using GPC.Job.Config;

public class CommonCL<T> : ConditionLib<T> where T : IState
{
    public PredicateCondition<T> IsMoveKeyDown =
            new (state => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")));

    public PredicateCondition<T> IsJumpKeyDown = 
            new (state => Input.IsActionJustPressed("jump"));
}


public class PlayerCL<T> : CommonCL<T> where T : PlayerState
{
    public PredicateCondition<T> IsOnFloor = 
        new (state => state.Host.IsOnFloor());
}

public partial class Player : CharacterBody2D
{
    private ConditionMachine<PlayerState> _cm;

    public override void _Ready()
    {
        var cl = new PlayerCL<PlayerState>();
        cl.IsJumpKeyDown.IsSatisfy();
        
        var states = new List<PlayerState>([
            new PlayerState(this)
            {
                Id = "1",
                Type = typeof(Move<PlayerState>),
                Name = "Run",
            
                Priority = 2,
                Preconditions = new Dictionary<ICondition<PlayerState>, bool>
                {
                    { cl.IsMoveKeyDown, true }, { cl.IsOnFloor, true }
                },
                FailedConditions = new Dictionary<ICondition<PlayerState>, bool>
                {
                    { cl.IsOnFloor, false }
                }
            },
            new PlayerState(this)
            {
                Id = "2",
                Name = "Idle",
                Priority = 1,
                Type = typeof(Idle<PlayerState>),
                Preconditions = new Dictionary<ICondition, bool>
                {
                    { isMoveKeyDown, false }, { isOnFloor, true }
                },
                FailedConditions = new Dictionary<ICondition, bool>
                {
                    { isOnFloor, false }
                }
            },
            new PlayerState(this)
            {
                Id = "3",
                Name = "Jump",
                Priority = 3,
                Type = typeof(Jump<PlayerState>),
                Preconditions = new Dictionary<ICondition, bool>
                {
                    { isJumpKeyDown, true }, { isOnFloor, true }
                },
                FailedConditions = new Dictionary<ICondition, bool>()
            }
        ]);


        _cm = new ConditionMachine<PlayerState>(states, new PlayerConditionLib());
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