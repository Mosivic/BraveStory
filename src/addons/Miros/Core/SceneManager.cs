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

    // 存储场景状态的字典
    private Dictionary<string, SceneState> _sceneStates = new Dictionary<string, SceneState>();

    /// <summary>
    /// 场景状态类，用于存储场景中节点的状态
    /// </summary>
    private class SceneState
    {
        public HashSet<string> DestroyedNodes { get; set; } = new HashSet<string>();
        public Dictionary<string, Vector2> NodePositions { get; set; } = new Dictionary<string, Vector2>();
        public Dictionary<string, bool> NodeVisibility { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, Dictionary<string, object>> CustomProperties { get; set; } = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, bool> CollisionStates { get; set; } = new Dictionary<string, bool>();
    }

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
            var transitionScene = GD.Load<PackedScene>("res://addons/Miros/Core/SceneTransitionStyle/RainTransition.tscn");
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

            // 实例化新场景后，应用保存的状态
            if (_sceneStates.ContainsKey(scenePath))
            {
                var sceneState = _sceneStates[scenePath];
                
                // 应用节点销毁状态
                foreach (var nodePath in sceneState.DestroyedNodes)
                {
                    var node = instancedScene.GetNodeOrNull(nodePath);
                    if (node != null)
                    {
                        node.QueueFree();
                    }
                }

                // 应用节点位置
                foreach (var kvp in sceneState.NodePositions)
                {
                    if (instancedScene.GetNodeOrNull(kvp.Key) is Node2D node)
                    {
                        node.GlobalPosition = kvp.Value;
                    }
                }

                // 应用节点可见性
                foreach (var kvp in sceneState.NodeVisibility)
                {
                    var node = instancedScene.GetNodeOrNull(kvp.Key);
                    if (node != null)
                    {
                        (node as Node2D).Visible = kvp.Value;
                    }
                }

                // 应用碰撞状态
                foreach (var kvp in sceneState.CollisionStates)
                {
                    if (instancedScene.GetNodeOrNull(kvp.Key) is CollisionObject2D collisionNode)
                    {
                        collisionNode.ProcessMode = kvp.Value ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
                    }
                }

                // 应用自定义属性
                foreach (var nodeProp in sceneState.CustomProperties)
                {
                    var node = instancedScene.GetNodeOrNull(nodeProp.Key);
                    if (node != null)
                    {
                        foreach (var prop in nodeProp.Value)
                        {
                            node.Set(prop.Key, (Variant)prop.Value);
                        }
                    }
                }
            }

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

    /// <summary>
    /// 记录节点被销毁
    /// </summary>
    public void RecordNodeDestroyed(Node node)
    {
        if (_currentScene == null) return;
        
        string scenePath = _currentScene.SceneFilePath;
        if (!_sceneStates.ContainsKey(scenePath))
        {
            _sceneStates[scenePath] = new SceneState();
        }
        
        // 保存节点的场景唯一路径
        string nodePath = _currentScene.GetPathTo(node);
        _sceneStates[scenePath].DestroyedNodes.Add(nodePath);
    }

    /// <summary>
    /// 清除指定场景的状态
    /// </summary>
    public void ClearSceneState(string scenePath)
    {
        if (_sceneStates.ContainsKey(scenePath))
        {
            _sceneStates.Remove(scenePath);
        }
    }

    /// <summary>
    /// 清除所有场景状态
    /// </summary>
    public void ClearAllSceneStates()
    {
        _sceneStates.Clear();
    }

    /// <summary>
    /// 记录节点位置
    /// </summary>
    public void RecordNodePosition(Node2D node)
    {
        if (_currentScene == null || node == null) return;
        
        string scenePath = _currentScene.SceneFilePath;
        string nodePath = _currentScene.GetPathTo(node);
        
        EnsureSceneState(scenePath);
        _sceneStates[scenePath].NodePositions[nodePath] = node.GlobalPosition;
    }

    /// <summary>
    /// 记录节点可见性
    /// </summary>
    public void RecordNodeVisibility(Node node, bool isVisible)
    {
        if (_currentScene == null || node == null) return;
        
        string scenePath = _currentScene.SceneFilePath;
        string nodePath = _currentScene.GetPathTo(node);
        
        EnsureSceneState(scenePath);
        _sceneStates[scenePath].NodeVisibility[nodePath] = isVisible;
    }

    /// <summary>
    /// 记录节点碰撞状态
    /// </summary>
    public void RecordCollisionState(CollisionObject2D node, bool enabled)
    {
        if (_currentScene == null || node == null) return;
        
        string scenePath = _currentScene.SceneFilePath;
        string nodePath = _currentScene.GetPathTo(node);
        
        EnsureSceneState(scenePath);
        _sceneStates[scenePath].CollisionStates[nodePath] = enabled;
    }

    /// <summary>
    /// 记录自定义属性
    /// </summary>
    public void RecordCustomProperty(Node node, string propertyName, object value)
    {
        if (_currentScene == null || node == null) return;
        
        string scenePath = _currentScene.SceneFilePath;
        string nodePath = _currentScene.GetPathTo(node);
        
        EnsureSceneState(scenePath);
        if (!_sceneStates[scenePath].CustomProperties.ContainsKey(nodePath))
        {
            _sceneStates[scenePath].CustomProperties[nodePath] = new Dictionary<string, object>();
        }
        _sceneStates[scenePath].CustomProperties[nodePath][propertyName] = value;
    }

    private void EnsureSceneState(string scenePath)
    {
        if (!_sceneStates.ContainsKey(scenePath))
        {
            _sceneStates[scenePath] = new SceneState();
        }
    }
}