using Godot;
using System;
using System.Collections.Generic;
using BraveStory.Player;
using GPC;
using GPC.Scheduler;
using GPC.States;

[Icon("res://addons/Miros/GPC/Utility/StateDebugger/state_debugger.svg")]
public partial class StateDebugger : MarginContainer
{
    [Export] public bool Enabled = true;
    [Export] public Node WatchNode;

    
    private PackedScene _stateStatus =
        GD.Load<PackedScene>("res://addons/Miros/GPC/Utility/StateDebugger/state_status.tscn");
    
    
    private bool _hasScheduler = false;
    private AbsScheduler _scheduler = null;
    private List<AbsState> _states = null;

    private double _elapsedTime = 0.0f;

    private ItemList _evaluatorStatus;
    
    public override void _Ready()
    {
        _evaluatorStatus = GetNode<ItemList>("TabContainer/Evaluators/Status");
        
        if (WatchNode != null)
        {
            _scheduler = (WatchNode as IGpcToken)?.GetScheduler();
            _hasScheduler = _scheduler != null;
        }

        if (_hasScheduler)
        {
            _states = _scheduler.StateSet.States;

            foreach (var state in _states)
            {
                var stateStatus = _stateStatus.Instantiate() as StateStatus;
                
                GetNode<VBoxContainer>("TabContainer/States/VBoxContainer")
                    .AddChild(stateStatus);
                
                _scheduler.StateChanged += stateStatus.OnStateChanged;
                stateStatus.Init(state);
            }
        }
        
    }

    public override void _Process(double delta)
    {
        _evaluatorStatus.Clear();
        _evaluatorStatus.AddItem(Evaluators.IsMoveKeyDown.Name +" : "+ Evaluators.IsMoveKeyDown.GetFuncValueString());
        _evaluatorStatus.AddItem(Evaluators.IsJumpKeyDown.Name +" : "+  Evaluators.IsJumpKeyDown.GetFuncValueString());
    }
}
