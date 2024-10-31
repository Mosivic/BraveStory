using FSM.Evaluator;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class EvaluatorDebugWindow : Window
{
    private VBoxContainer _container;
    private Dictionary<string, Label> _valueLabels = new();
    private Button _refreshButton;
    private Timer _updateTimer;
    private CheckButton _autoUpdateToggle;

    public override void _Ready()
    {
        // 设置窗口属性
        Title = "Evaluator Debug";
        Size = new Vector2I(300, 400);
        Position = new Vector2I(50, 50);
        
        // 创建主容器
        _container = new VBoxContainer();
        AddChild(_container);
        
        // 创建控制面板
        CreateControlPanel();
        
        // 创建滚动容器
        var scroll = new ScrollContainer
        {
            CustomMinimumSize = new Vector2(280, 320)
        };
        _container.AddChild(scroll);
        
        var valueContainer = new VBoxContainer();
        scroll.AddChild(valueContainer);
        
        // 初始化定时器
        _updateTimer = new Timer
        {
            WaitTime = 0.5f, // 更新频率（秒）
            OneShot = false
        };
        AddChild(_updateTimer);
        _updateTimer.Timeout += UpdateValues;
        
        // 初始化评估器显示
        InitializeEvaluatorLabels(valueContainer);
        
        // 首次更新值
        UpdateValues();
    }

    private void CreateControlPanel()
    {
        var controlPanel = new HBoxContainer();
        _container.AddChild(controlPanel);
        
        // 刷新按钮
        _refreshButton = new Button
        {
            Text = "Refresh"
        };
        _refreshButton.Pressed += UpdateValues;
        controlPanel.AddChild(_refreshButton);
        
        // 自动更新开关
        _autoUpdateToggle = new CheckButton
        {
            Text = "Auto Update"
        };
        _autoUpdateToggle.Toggled += OnAutoUpdateToggled;
        controlPanel.AddChild(_autoUpdateToggle);
    }

    private void InitializeEvaluatorLabels(VBoxContainer container)
    {
        var evaluators = EvaluatorManager.Instance.GetAllEvaluators();
        
        foreach (var kvp in evaluators)
        {
            var evaluatorName = kvp.Key;
            
            // 创建评估器容器
            var evaluatorContainer = new VBoxContainer();
            container.AddChild(evaluatorContainer);
            
            // 添加分隔线
            var separator = new HSeparator();
            evaluatorContainer.AddChild(separator);
            
            // 创建名称标签
            var nameLabel = new Label
            {
                Text = $"[{evaluatorName}]"
            };
            evaluatorContainer.AddChild(nameLabel);
            
            // 创建值标签
            var valueLabel = new Label
            {
                Text = "Value: Loading..."
            };
            evaluatorContainer.AddChild(valueLabel);
            _valueLabels[evaluatorName] = valueLabel;
        }
    }

    private void UpdateValues()
    {
        var evaluators = EvaluatorManager.Instance.GetAllEvaluators();
        
        foreach (var kvp in evaluators)
        {
            var evaluatorName = kvp.Key;
            var evaluator = kvp.Value;
            
            if (_valueLabels.TryGetValue(evaluatorName, out var label))
            {
                string currentValue = evaluator.GetFuncValueString();
                string lastValue = "N/A";
                
                // 尝试获取LastValue（如果可用）
                if (evaluator is IEvaluator evaluatorWithLast)
                {
                    lastValue = evaluatorWithLast.GetLastValueString();
                }
                
                label.Text = $"Current: {currentValue}\nLast: {lastValue}";
            }
        }
    }

    private void OnAutoUpdateToggled(bool buttonPressed)
    {
        if (buttonPressed)
        {
            _updateTimer.Start();
        }
        else
        {
            _updateTimer.Stop();
        }
    }

    public override void _ExitTree()
    {
        _updateTimer.Stop();
        base._ExitTree();
    }
} 