using System.Collections.Generic;
using System.Linq;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.States;

[Icon("res://addons/Miros/Material/Icon/state_debugger.svg")]
public partial class StateDebugger : MarginContainer
{
    [Export] public bool Enabled = true;
    [Export] public Node WatchNode;
    
    private double _elapsedTime;
    private string _historyInfo;
    private int _historyMaxCountLimit = 300;

    private ItemList _evaluatorStatus;
    private Tree _stateTree;
    private RichTextLabel _historyLabel;
    
    private Dictionary<Layer, TreeItem> _layerTreeItemDict = new();
    private Dictionary<AbsState, TreeItem> _stateTreeItemDict = new();
    
    private AbsScheduler _scheduler;
    private Layer _rootLayer;
    private List<AbsState> _states;

    private Texture2D _redPointTexture = 
        GD.Load<Texture2D>("addons/Miros/Material/Icon/red_point.svg");
    private Texture2D _greenPointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/green_point.svg");   
    private Texture2D _orangePointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/orange_point.svg");
    

    public override void _Ready()
    {
        _evaluatorStatus = GetNode<ItemList>("TabContainer/Evaluators/Status");
        _stateTree = GetNode<Tree>("TabContainer/States/Tree");
        _historyLabel = GetNode<RichTextLabel>("TabContainer/History/Label");

        if (WatchNode != null)
        {
            _scheduler = (WatchNode as IGpcToken).GetScheduler();
            _rootLayer = (WatchNode as IGpcToken).GetRootLayer();
        }
            
        
        if (_scheduler != null)
        {
            _scheduler.StatePrepared += OnStatePrepared;
            _scheduler.StateChanged += OnStateChanged;
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
                _stateTreeItemDict[state] = layerTreeItem;
                    
                layerTreeItem.SetText(0,state.Name);
                layerTreeItem.SetIcon(1,_redPointTexture);
                
            }
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

    private void UpdateStateDisplay(AbsState state)
    {
        var treeItem = _stateTreeItemDict[state];
        if (state.IsRunning)
        {
            treeItem.SetIcon(1,_greenPointTexture);
        }
        else
        {
            treeItem.SetIcon(1,_redPointTexture);
        }
    }

    private void UpdateHistoryDisplay(AbsState state,JobRunningStatus status)
    {
        var info = $"[{Engine.GetProcessFrames().ToString()}] [color=green] {state.Name}[/color]({state.Layer.Name}) : {status}";
        _historyInfo += info + "\n";
        _historyLabel.SetText(_historyInfo);
    }

    private void OnStateChanged(AbsState state,JobRunningStatus status)
    {
        UpdateStateDisplay(state);
        UpdateHistoryDisplay(state, status);
    }
    private void OnStatePrepared(AbsState state)
    {
        var treeItem = _stateTreeItemDict[state];
        if(state.IsRunning == false)
            treeItem.SetIcon(1,_orangePointTexture);
    }
    
}