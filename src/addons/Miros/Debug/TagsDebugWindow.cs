using BraveStory;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class TagDebugWindow : Control
{
    private Label _tagLabel;
    private HashSet<GameplayTag> _tags;
    
    private const float UPDATE_INTERVAL = 0.1f; // 更新间隔（秒）
    private float _timeSinceLastUpdate;

    public TagDebugWindow(HashSet<GameplayTag> tags)
    {
        _tags = tags;
    }
    
    public override void _Ready()
    {
        // 设置窗口属性
        CustomMinimumSize = new Vector2(200, 100);
        Position = new Vector2(10, 10);
        
        // 创建半透明背景
        var panel = new Panel();
        panel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(panel);
        
        // 创建标签显示
        _tagLabel = new Label
        {
            Position = new Vector2(10, 10),
            Theme = new Theme(),
            Modulate = Colors.White
        };
        AddChild(_tagLabel);
        
        // 设置透明度
        Modulate = new Color(1, 1, 1, 0.8f);
        
        // 初始显示标签
        UpdateTagDisplay();
    }

    public override void _Process(double delta)
    {
        _timeSinceLastUpdate += (float)delta;
        if (_timeSinceLastUpdate >= UPDATE_INTERVAL)
        {
            UpdateTagDisplay();
            _timeSinceLastUpdate = 0;
        }
    }
    
    private void UpdateTagDisplay()
    {
        string tagText = "Tags:\n";
        foreach (var tag in _tags)
        {
            tagText += $"• {tag}\n";
        }
        _tagLabel.Text = tagText;
    }
}