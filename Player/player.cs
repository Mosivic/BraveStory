using System.Collections.Generic;
using Godot;
using GPC;
using GPC.AI;
using GPC.Job.Config;

public class CommonLib<T> : EvaluatorLib<T> where T : IState
{
    public readonly Evaluator<T> IsMoveKeyDown =
        new (state => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")));

    public readonly Evaluator<T> IsJumpKeyDown = 
        new (state => Input.IsActionJustPressed("jump"));
}


public class PlayerLib<T> : EvaluatorLib<T> where T : PlayerState
{
    public readonly Evaluator<T> IsOnFloor = 
        new (state => state.Host.IsOnFloor() );
}




public partial class Player : CharacterBody2D
{
    private ConditionMachine<PlayerState> _cm;

    public override void _Ready()
    {
        EvaluatorSpace<PlayerState, CommonLib<PlayerState>, PlayerLib<PlayerState>> space =
            new(new CommonLib<PlayerState>(),new PlayerLib<PlayerState>());
        


    
        var states = new List<PlayerState>(){
            new PlayerState(this)
            {
                Id = "1",
                Type = typeof(Move<PlayerState>),
                Name = "Run",
                Priority = 2,
                PreConditions = new(){
                    {space.Common.IsMoveKeyDown,true}
                },
                Preconditions = [
                    new Condition<PlayerState>(),
                    new Condition<PlayerState>(space.Private.IsOnFloor),
                    ],
                FailedConditions = [
                    new Condition<PlayerState>(space.Private.IsOnFloor,false)
                ]

           
            },
            new PlayerState(this)
            {
                Id = "2",
                Name = "Idle",
                Priority = 1,
                Type = typeof(Idle<PlayerState>),
                Preconditions = [

                ]
                
                new Dictionary<ICondition, bool>
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
        ])}


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

