using System.Collections.Generic;
using Godot;
using GPC;
using GPC.Scheduler;
using GPC.States;

[Icon("res://addons/Miros/Material/Icon/state_debugger.svg")]
public partial class StateDebugger : MarginContainer
{
    private double _elapsedTime;

    private ItemList _evaluatorStatus;

    private Texture2D _greenPointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/green_point.svg");

    private string _historyInfo;
    private RichTextLabel _historyLabel;
    private int _historyMaxCountLimit = 300;

    private readonly Dictionary<Layer, TreeItem> _layerTreeItemDict = new();

    private Texture2D _orangePointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/orange_point.svg");

    private Texture2D _redPointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/red_point.svg");

    private Layer _rootLayer;

    private AbsScheduler _scheduler;
    private List<AbsState> _states;
    private Tree _stateTree;
    private readonly Dictionary<AbsState, TreeItem> _stateTreeItemDict = new();
    [Export] public bool Enabled = true;
    [Export] public Node WatchNode;


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
            root.SetText(1, _rootLayer.Name);
            _layerTreeItemDict[_rootLayer] = root;

            foreach (var childLayer in _rootLayer.ChildrenLayer) CreateTreeChild(root, childLayer);

            foreach (var state in _states)
            {
                var layer = state.Layer;
                var treeItem = _layerTreeItemDict[layer];
                var layerTreeItem = _stateTree.CreateItem(treeItem);
                _stateTreeItemDict[state] = layerTreeItem;

                layerTreeItem.SetText(0, state.Name);
                layerTreeItem.SetIcon(1, _redPointTexture);
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
        layerTreeItem.SetText(0, layer.Name);
        _layerTreeItemDict[layer] = layerTreeItem;

        if (layer.ChildrenLayer == null) return;
        foreach (var childLayer in layer.ChildrenLayer) CreateTreeChild(layerTreeItem, childLayer);
    }

    private void UpdateStateDisplay(AbsState state)
    {
        var treeItem = _stateTreeItemDict[state];
        if (state.Status == JobRunningStatus.Running)
            treeItem.SetIcon(1, _greenPointTexture);
        else
            treeItem.SetIcon(1, _redPointTexture);
    }

    private void UpdateHistoryDisplay(AbsState state)
    {
        var info =
            $"[{Engine.GetProcessFrames().ToString()}] [color=green] {state.Name}[/color]({state.Layer.Name}) : {state.Status}";
        _historyInfo += info + "\n";
        _historyLabel.SetText(_historyInfo);
    }

    private void OnStateChanged(AbsState state)
    {
        UpdateStateDisplay(state);
        //UpdateHistoryDisplay(state, status);
    }

    private void OnStatePrepared(AbsState state)
    {
        var treeItem = _stateTreeItemDict[state];
        if (state.Status == JobRunningStatus.NoRun)
            treeItem.SetIcon(1, _orangePointTexture);
    }
}