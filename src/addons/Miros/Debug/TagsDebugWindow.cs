using System.Collections.Generic;
using System.Linq;
using Godot;
using Miros.Core;

public partial class TagDebugWindow(HashSet<Tag> tags) : Control
{
    private const float UpdateInterval = 0.1f; // 更新间隔（秒）
    private Tree _tagTree;
    private float _timeSinceLastUpdate;

    public override void _Ready()
    {
        // 设置窗口属性
        CustomMinimumSize = new Vector2(300, 600);
        Position = new Vector2(10, 10);

        // 创建半透明背景
        var panel = new Panel();
        panel.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(panel);

        // 创建树形控件
        _tagTree = new Tree
        {
            Position = new Vector2(10, 10),
            Size = new Vector2(280, 580),
            Theme = new Theme()
        };
        AddChild(_tagTree);

        // 设置透明度
        Modulate = new Color(1, 1, 1, 0.8f);

        // 初始显示标签
        UpdateTagDisplay();
    }

    public override void _Process(double delta)
    {
        _timeSinceLastUpdate += (float)delta;
        if (_timeSinceLastUpdate >= UpdateInterval)
        {
            UpdateTagDisplay();
            _timeSinceLastUpdate = 0;
        }
    }

    private void UpdateTagDisplay()
    {
        _tagTree.Clear();
        var root = _tagTree.CreateItem();
        root.SetText(0, "Tags");

        // 创建标签路径字典
        var tagPaths = new Dictionary<string, TreeItem>
        {
            [""] = root
        };

        foreach (var tag in tags.OrderBy(t => t.ToString()))
        {
            var segments = tag.ToString()?.Split('.');
            if (segments == null) return;

            var currentPath = "";

            foreach (var segment in segments)
            {
                var newPath = currentPath == "" ? segment : $"{currentPath}.{segment}";

                if (!tagPaths.ContainsKey(newPath))
                {
                    var parent = tagPaths[currentPath];
                    var item = _tagTree.CreateItem(parent);
                    item.SetText(0, segment);
                    tagPaths[newPath] = item;
                }

                currentPath = newPath;
            }
        }
    }
}