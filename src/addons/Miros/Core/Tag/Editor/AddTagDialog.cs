using Godot;

public partial class AddTagDialog : Window
{
    [Signal]
    public delegate void TagAddedEventHandler(string tagName, TreeItem parentItem);

    private Button _cancelButton;
    private LineEdit _nameEdit;
    private Button _okButton;
    private TreeItem _parentItem; // 存储父标签项

    public AddTagDialog()
    {
        Title = "Add New Tag";
        Size = new Vector2I(300, 150);
        Exclusive = true;
        AlwaysOnTop = true;
    }

    public void Initialize(TreeItem parentItem)
    {
        _parentItem = parentItem;
    }

    public override void _Ready()
    {
        var margin = new MarginContainer();
        margin.CustomMinimumSize = new Vector2I(280, 130);
        margin.AddThemeConstantOverride("margin_left", 10);
        margin.AddThemeConstantOverride("margin_right", 10);
        margin.AddThemeConstantOverride("margin_top", 10);
        margin.AddThemeConstantOverride("margin_bottom", 10);
        AddChild(margin);

        var vbox = new VBoxContainer();
        margin.AddChild(vbox);

        // 添加标签名称输入框
        var nameLabel = new Label { Text = "Tag Name:" };
        vbox.AddChild(nameLabel);

        _nameEdit = new LineEdit();
        _nameEdit.PlaceholderText = "Enter tag name";
        vbox.AddChild(_nameEdit);

        // 添加按钮容器
        var buttonContainer = new HBoxContainer();
        buttonContainer.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
        vbox.AddChild(buttonContainer);

        // 添加确定按钮
        _okButton = new Button { Text = "OK" };
        _okButton.CustomMinimumSize = new Vector2I(70, 0);
        buttonContainer.AddChild(_okButton);
        _okButton.Pressed += OnOkPressed;

        // 添加取消按钮
        _cancelButton = new Button { Text = "Cancel" };
        _cancelButton.CustomMinimumSize = new Vector2I(70, 0);
        buttonContainer.AddChild(_cancelButton);
        _cancelButton.Pressed += OnCancelPressed;

        // 设置初始焦点
        _nameEdit.GrabFocus();
    }

    private void OnOkPressed()
    {
        var tagName = _nameEdit.Text.Trim();
        if (!string.IsNullOrEmpty(tagName))
        {
            // 如果有父标签，添加完整路径
            if (_parentItem != null)
            {
                var parentPath = _parentItem.GetText(1); // 获取完整路径
                tagName = $"{parentPath}.{tagName}";
            }

            EmitSignal(SignalName.TagAdded, tagName, _parentItem);
        }

        QueueFree();
    }

    private void OnCancelPressed()
    {
        QueueFree();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
            {
                OnCancelPressed();
                GetViewport().SetInputAsHandled();
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.Enter)
            {
                OnOkPressed();
                GetViewport().SetInputAsHandled();
            }
        }
    }
}