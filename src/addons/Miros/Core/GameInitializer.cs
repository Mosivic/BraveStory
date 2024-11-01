using Godot;
using System;

public partial class GameInitializer : Node
{
    // 使用Godot的单例模式
    public static GameInitializer Instance { get; private set; }

    // 初始化状态标志
    private bool _isInitialized = false;
    
    // 游戏全局数据
    private int _gameScore = 0;
    private float _gameTime = 0f;

    // 构造函数
    public GameInitializer()
    {
        Instance = this;
    }
    
    // 在游戏启动时调用
    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public override void _Ready()
    {
        base._Ready();
        if (_isInitialized) return;
        
        InitializeGameplayTagSystem();
        InitializeEvaluators();
        InitializeManagers();
        InitializeServices();
        
        _isInitialized = true;
        GD.Print("Game initialization completed!");
    }

    // 初始化所有评估器
    private void InitializeEvaluators()
    {
        var evaluatorManager = EvaluatorManager.Instance;

        // 游戏状态评估器
        // evaluatorManager.CreateEvaluator(
        //     EvaluatorKeys.GAME_SCORE, 
        //     () => _gameScore
        // );
        
        // evaluatorManager.CreateEvaluator(
        //     EvaluatorKeys.GAME_TIME, 
        //     () => _gameTime
        // );

        evaluatorManager.CreateEvaluator(
            EvaluatorKeys.KEYDOWN_JUMP,
            () => Input.IsActionJustPressed("jump")
        );

        evaluatorManager.CreateEvaluator(
            EvaluatorKeys.KEYDOWN_MOVE,
            () => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right"))
        );

        GD.Print("Evaluators initialized!");
    }

    // 初始化所有管理器
    private void InitializeManagers()
    {
        try
        {
            // 这里初始化其他管理器
            // 例如：音频管理器、资源管理器、UI管理器等
            // InitializeAudioManager();
            // InitializeResourceManager();
            // InitializeUIManager();
            
            GD.Print("Managers initialized!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to initialize managers: {e.Message}");
        }
    }

    // 初始化游戏服务
    private void InitializeServices()
    {
        try
        {
            // 这里初始化各种服务
            // 例如：存档服务、网络服务、成就系统等
            // InitializeSaveSystem();
            // InitializeNetworkService();
            // InitializeAchievementSystem();
            
            // 在开发模式下初始化调试管理器
            #if DEBUG
            AddChild(DebugManager.Instance);
            GD.Print("Debug manager initialized!");
            #endif
            
            GD.Print("Services initialized!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to initialize services: {e.Message}");
        }
    }

    // 示例：音频管理器初始化
    private void InitializeAudioManager()
    {
        // 实现音频管理器初始化逻辑
    }

    // 示例：资源管理器初始化
    private void InitializeResourceManager()
    {
        // 实现资源管理器初始化逻辑
    }

    // 示例：UI管理器初始化
    private void InitializeUIManager()
    {
        // 实现UI管理器初始化逻辑
    }

    // 示例：存档系统初始化
    private void InitializeSaveSystem()
    {
        // 实现存档系统初始化逻辑
    }

    // 示例：网络服务初始化
    private void InitializeNetworkService()
    {
        // 实现网络服务初始化逻辑
    }

    // 示例：成就系统初始化
    private void InitializeAchievementSystem()
    {
        // 实现成就系统初始化逻辑
    }

    private void InitializeGameplayTagSystem()
    {
        var tagYamlLoader = new GameplayTagYamlLoader();
        tagYamlLoader.LoadFromFile("res://Example/gameplay_tags.yaml");
        tagYamlLoader.LoadFromFile("res://Example/character_tags.yaml");
    }

    // 游戏退出时的清理工作
    public override void _ExitTree()
    {
        if (!_isInitialized) return;

        // 清理所有评估器
        EvaluatorManager.Instance.ClearEvaluators();

        // 清理其他资源
        CleanupManagers();
        CleanupServices();

        _isInitialized = false;
        GD.Print("Game cleanup completed!");
    }

    private void CleanupManagers()
    {
        // 实现管理器清理逻辑
    }

    private void CleanupServices()
    {
        // 实现服务清理逻辑
    }
} 