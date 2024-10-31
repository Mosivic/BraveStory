using Godot;

public partial class DebugManager : Node
{
    private static DebugManager _instance;
    public static DebugManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DebugManager();
            }
            return _instance;
        }
    }

    private EvaluatorDebugWindow _evaluatorWindow;
    
    // 快捷键设置
    private const Key TOGGLE_DEBUG_KEY = Key.F3;

    public override void _Ready()
    {
        CreateEvaluatorWindow();
    }

    private void CreateEvaluatorWindow()
    {
        _evaluatorWindow = new EvaluatorDebugWindow();
        AddChild(_evaluatorWindow);
        _evaluatorWindow.Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Keycode == TOGGLE_DEBUG_KEY && eventKey.Pressed && !eventKey.Echo)
            {
                ToggleEvaluatorWindow();
            }
        }
    }

    private void ToggleEvaluatorWindow()
    {
        if (_evaluatorWindow.Visible)
        {
            _evaluatorWindow.Hide();
        }
        else
        {
            _evaluatorWindow.Show();
        }
    }
} 