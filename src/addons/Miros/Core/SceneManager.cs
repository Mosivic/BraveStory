using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }

    // 当前加载的场景
    private Node _currentScene;

    // 场景过渡
    private ISceneTransition _transition;
    private Node _transitionNode;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        // 获取初始场景
        _currentScene = GetTree().CurrentScene;
        if (_currentScene == null)
        {
            GD.PrintErr("Current scene is null!");
        }

        // 加载过渡动画场景
        try
        {
            var transitionScene = GD.Load<PackedScene>("res://addons/Miros/Core/SceneTransitionStyle/SpiralTransition.tscn");
            if (transitionScene != null)
            {
                _transitionNode = transitionScene.Instantiate();
                _transition = _transitionNode as ISceneTransition;
                if (_transition == null)
                {
                    GD.PrintErr("Transition scene does not implement ISceneTransition!");
                    return;
                }
                AddChild(_transitionNode);
            }
            else
            {
                GD.PrintErr("Failed to load transition scene!");
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading transition: {e.Message}");
        }
    }

    /// <summary>
    /// 切换到新场景
    /// </summary>
    /// <param name="scenePath">场景路径</param>
    /// <param name="transferNodes">需要转移到新场景的节点列表</param>
    /// <param name="playTransition">是否播放过渡动画</param>
    public async Task ChangeScene(string scenePath, Node2D transferNode = null, Vector2? entryPoint = null, bool playTransition = true)
    {
        try
        {
            GD.Print($"Starting scene change to: {scenePath}");
            
            // 检查场景路径
            if (string.IsNullOrEmpty(scenePath))
            {
                GD.PrintErr("Scene path is empty!");
                return;
            }

            // 检查并保存要转移的节点
            Node2D nodeToTransfer = null;
            if (transferNode != null && IsInstanceValid(transferNode))
            {
                nodeToTransfer = transferNode;
                var parent = transferNode.GetParent();
                if (parent != null)
                {
                    parent.RemoveChild(transferNode);
                    AddChild(transferNode);
                    GD.Print($"Transferred node {transferNode.Name} to SceneManager");
                }
            }

            // 播放过渡动画
            if (playTransition && _transition != null)
            {
                await _transition.TransitionOut();
            }

            // 加载新场景
            var newScene = GD.Load<PackedScene>(scenePath);
            if (newScene == null)
            {
                GD.PrintErr($"Failed to load scene: {scenePath}");
                return;
            }

            // 实例化新场景
            var instancedScene = newScene.Instantiate();
            if (instancedScene == null)
            {
                GD.PrintErr("Failed to instantiate new scene");
                return;
            }

            // 切换场景
            var tree = GetTree();
            if (_currentScene != null)
            {
                tree.Root.RemoveChild(_currentScene);
            }
            tree.Root.AddChild(instancedScene);
            tree.CurrentScene = instancedScene;
            _currentScene = instancedScene;

            // 转移节点到新场景
            if (nodeToTransfer != null)
            {
                RemoveChild(nodeToTransfer);
                _currentScene.AddChild(nodeToTransfer);
                if (entryPoint.HasValue)
                {
                    nodeToTransfer.GlobalPosition = entryPoint.Value;
                }
                GD.Print($"Transferred node {nodeToTransfer.Name} to new scene");
            }

            // 播放淡入动画
            if (playTransition && _transition != null)
            {
                await _transition.TransitionIn();
            }

            GD.Print("Scene change completed successfully");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Scene change failed: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public async Task ReloadCurrentScene(Node2D transferNode = null)
    {
        var currentScenePath = _currentScene.SceneFilePath;
        await ChangeScene(currentScenePath, transferNode);
    }

    /// <summary>
    /// 获取当前场景
    /// </summary>
    public Node GetCurrentScene()
    {
        return _currentScene;
    }
}