using System.Collections.Generic;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.States;

[Icon("res://addons/Miros/GPC/Utility/StateDebugger/state_debugger.svg")]
public partial class StateDebugger : MarginContainer
{
    private double _elapsedTime;

    private ItemList _evaluatorStatus;
    private Tree _stateTree;
    private Dictionary<Layer, TreeItem> _layerTreeItemDict = new();
    
    private AbsScheduler _scheduler;
    private Layer _rootLayer;
    private List<AbsState> _states;


    private PackedScene _stateStatus =
        GD.Load<PackedScene>("res://addons/Miros/GPC/Utility/StateDebugger/state_status.tscn");

    [Export] public bool Enabled = true;
    [Export] public Node WatchNode;

    public override void _Ready()
    {
        _evaluatorStatus = GetNode<ItemList>("TabContainer/Evaluators/Status");
        _stateTree = GetNode<Tree>("TabContainer/States/Tree");

        if (WatchNode != null)
        {
            _scheduler = (WatchNode as IGpcToken)?.GetScheduler();
            _rootLayer = (WatchNode as IGpcToken)?.GetRootLayer();
        }
            
        
        if (_scheduler != null)
        {
            _states = _scheduler.StateSet.States;

            var root = _stateTree.CreateItem();
            root.SetText(1,_rootLayer.Name);
            _layerTreeItemDict[_rootLayer] = root;
            
            foreach (var childLayer in _rootLayer.ChildrenLayer)
            {
                CreateTreeChild(root, childLayer);
            }

            foreach (var state in _states)
            {
                var layer = state.Layer;
                var treeItem = _layerTreeItemDict[layer];
                var layerTreeItem = _stateTree.CreateItem(treeItem);
                layerTreeItem.SetText(0,state.Name);
               
                layerTreeItem.SetIcon(1,GD.Load<Texture2D>("res://addons/Miros/GPC/Utility/StateDebugger/state_debugger.svg"));
                
            }
            // foreach (var state in _states)
            // {
            //     var stateStatus = _stateStatus.Instantiate() as StateStatus;
            //
            //     GetNode<VBoxContainer>("TabContainer/States/VBoxContainer")
            //         .AddChild(stateStatus);
            //
            //     _scheduler.StateChanged += stateStatus.OnStateChanged;
            //     stateStatus.Init(state);
            // }
        }
    }

    // public override void _Process(double delta)
    // {
    //     _evaluatorStatus.Clear();
    //     _evaluatorStatus.AddItem(Evaluators.IsMoveKeyDown.Name +" : "+ Evaluators.IsMoveKeyDown.GetFuncValueString());
    //     _evaluatorStatus.AddItem(Evaluators.IsJumpKeyDown.Name +" : "+  Evaluators.IsJumpKeyDown.GetFuncValueString());
    // }

    private void CreateTreeChild(TreeItem parent, Layer layer)
    {
        var layerTreeItem = _stateTree.CreateItem(parent);
        layerTreeItem.SetText(0,layer.Name);
        _layerTreeItemDict[layer] = layerTreeItem;

        if (layer.ChildrenLayer == null) return;
        foreach (var childLayer in layer.ChildrenLayer)
        {
            CreateTreeChild(layerTreeItem, childLayer);
        }
    }
}