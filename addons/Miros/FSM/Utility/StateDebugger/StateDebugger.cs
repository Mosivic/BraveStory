using System.Collections.Generic;
using FSM;
using FSM.Job;
using FSM.Job.Executor;
using FSM.States;
using FSM.Utility.StateDebugger;
using Godot;

[Icon("res://addons/Miros/Material/Icon/state_debugger.svg")]
public partial class StateDebugger : MarginContainer
{
    private readonly Dictionary<IJob, TreeItem> _jobTreeItemDict = new();
    private readonly Dictionary<Layer, TreeItem> _layerTreeItemDict = new();
    private IConnect _connect;

    private double _elapsedTime;
    private ItemList _evaluatorStatus;

    private Texture2D _greenPointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/green_point.svg");

    private string _historyInfo;


    private RichTextLabel _historyLabel;
    private int _historyMaxCountLimit = 300;
    private IJob[] _jobs;

    private Texture2D _orangePointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/orange_point.svg");

    private Texture2D _redPointTexture =
        GD.Load<Texture2D>("addons/Miros/Material/Icon/red_point.svg");

    private Layer _rootLayer;

    private Tree _stateTree;

    [Export] public bool Enabled = true;
    [Export] public Node WatchNode;


    public override void _Ready()
    {
        _stateTree = GetNode<Tree>("TabContainer/States/Tree");
        _historyLabel = GetNode<RichTextLabel>("TabContainer/History/Label");

        if (WatchNode != null)
        {
            _rootLayer = (WatchNode as IDebugNode).GetRootLayer();
            _connect = (WatchNode as IDebugNode).GetConnect();
        }
        
        if (_connect == null) return;
        
        var root = _stateTree.CreateItem();
        root.SetText(0, _rootLayer.Name);
        _layerTreeItemDict[_rootLayer] = root;

        foreach (var childLayer in _rootLayer.ChildrenLayer)
            CreateTreeChild(root, childLayer);

        _jobs = _connect.GetAllJobs();
        foreach (var job in _jobs)
        {
            GD.Print(job.Name);
            var layer = job.Layer;
            var treeItem = _layerTreeItemDict[layer];
            var layerTreeItem = _stateTree.CreateItem(treeItem);
            _jobTreeItemDict[job] = layerTreeItem;

            layerTreeItem.SetText(0, job.Name);
            layerTreeItem.SetIcon(1, _redPointTexture);
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
        foreach (var childLayer in layer.ChildrenLayer)
            CreateTreeChild(layerTreeItem, childLayer);
    }


    public override void _Process(double delta)
    {
        foreach (var job in _jobs)
        {
            var treeItem = _jobTreeItemDict[job];
            if (job.Status == JobRunningStatus.Running)
                treeItem.SetIcon(1, _greenPointTexture);
            else
                treeItem.SetIcon(1, _redPointTexture);
        }
    }

    private void UpdateStateDisplay(IJob job)
    {
        var treeItem = _jobTreeItemDict[job];
        if (job.Status == JobRunningStatus.Running)
            treeItem.SetIcon(1, _greenPointTexture);
        else
            treeItem.SetIcon(1, _redPointTexture);
    }

    private void UpdateHistoryDisplay(AbsState state)
    {
        var info =
            $"[{Engine.GetProcessFrames().ToString()}] [color=green] {state.Name}[/color]({state.Layer.Name}) : {state.Status}";
        _historyInfo += info + "\n";
        //_historyLabel.SetText(_historyInfo);
    }

    private void OnStateChanged(IJob state)
    {
        UpdateStateDisplay(state);
        //UpdateHistoryDisplay(state, status);
    }

    private void OnStatePrepared(IJob state)
    {
        var treeItem = _jobTreeItemDict[state];
        if (state.Status == JobRunningStatus.NoRun)
            treeItem.SetIcon(1, _orangePointTexture);
    }
}