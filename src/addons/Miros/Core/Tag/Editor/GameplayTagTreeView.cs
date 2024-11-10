using Godot;
using System.Collections.Generic;

using Miros.Core;
public partial class GameplayTagTreeView : Tree
{
    private TagManager _tagManager;
    private TreeItem _root;
    private TreeItem _itemToDelete;
    private TreeItem _itemToRename;
    
    public override void _Ready()
    {
        _tagManager = TagManager.Instance;
        
        // 设置树的基本属性
        SelectMode = SelectModeEnum.Single;
        HideRoot = true;
        
        // 创建列
        Columns = 2;
        SetColumnTitle(0, "Tag Name");
        SetColumnTitle(1, "Full Path");
        
        // 初始化树
        RefreshTree();
    }
    
    public void RefreshTree()
    {
        Clear();
        _root = CreateItem();
        _root.SetText(0, "Root");
        
        // 获取所有根标签（没有父标签的标签）
        var rootTags = GetRootTags();
        foreach (var tag in rootTags)
        {
            CreateTagTreeItem(tag, _root);
        }
    }
    
    private void CreateTagTreeItem(Tag tag, TreeItem parent)
    {
        var item = CreateItem(parent);
        
        // 设置标签名称（最后一个部分）
        string[] parts = tag.ToString().Split('.');
        item.SetText(0, parts[^1]);
        
        // 设置完整路径
        item.SetText(1, tag.ToString());
        
        // 设置图标（可选）
        item.SetIcon(0, GetTagIcon(tag));
        
        // 递归添加子标签
        foreach (var childTag in _tagManager.GetDirectChildTags(tag))
        {
            CreateTagTreeItem(childTag, item);
        }
    }
    
    private IEnumerable<Tag> GetRootTags()
    {
        var result = new HashSet<Tag>();
        foreach (var tag in _tagManager.GetAllRegisteredTags())
        {
            if (!tag.ToString().Contains('.'))
            {
                result.Add(tag);
            }
        }
        return result;
    }
    
    private Texture2D GetTagIcon(Tag tag)
    {
        // 可以根据标签类型返回不同的图标
        return ResourceLoader.Load<Texture2D>("res://assets/icons/tag_icon.png");
    }
    
    // 右键菜单支持
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
            {
                ShowContextMenu();
            }
        }
    }
    
    private void ShowContextMenu()
    {
        var popup = new PopupMenu();
        popup.AddItem("Add New Tag", 0);
        popup.AddItem("Delete Tag", 1);
        popup.AddItem("Rename Tag", 2);
        
        popup.Connect("id_pressed", new Callable(this, nameof(OnContextMenuSelected)));
        popup.Position = (Vector2I)GetGlobalMousePosition();
        popup.Show();
    }
    
    private void OnContextMenuSelected(int id)
    {
        var selectedItem = GetSelected();
        if (selectedItem == null && id != 0) return;
        
        switch (id)
        {
            case 0: // Add New Tag
                ShowAddTagDialog(selectedItem);
                break;
            case 1: // Delete Tag
                DeleteSelectedTag(selectedItem);
                break;
            case 2: // Rename Tag
                ShowRenameDialog(selectedItem);
                break;
        }
    }
    
    private void ShowAddTagDialog(TreeItem parentItem)
    {
        var dialog = new AddTagDialog();
        dialog.Initialize(parentItem);
        AddChild(dialog);
        dialog.TagAdded += OnTagAdded;
        dialog.Show();
    }
    
    private void DeleteSelectedTag(TreeItem item)
    {
        _itemToDelete = item;
        
        var confirmDialog = new ConfirmationDialog
        {
            Title = "Confirm Delete",
            DialogText = "Are you sure you want to delete this tag and all its children?",
            MinSize = new Vector2I(300, 100)
        };
        
        AddChild(confirmDialog);
        confirmDialog.Confirmed += OnDeleteConfirmed;
        confirmDialog.Show();
    }
    
    private void ShowRenameDialog(TreeItem item)
    {
        _itemToRename = item;
        
        var dialog = new LineEdit
        {
            Text = item.GetText(0),
            CustomMinimumSize = new Vector2I(200, 30)
        };
        
        var popup = new Window
        {
            Title = "Rename Tag",
            Size = new Vector2I(250, 80),
            Exclusive = true,
            AlwaysOnTop = true
        };
        
        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 10);
        margin.AddThemeConstantOverride("margin_right", 10);
        margin.AddThemeConstantOverride("margin_top", 10);
        margin.AddThemeConstantOverride("margin_bottom", 10);
        popup.AddChild(margin);
        
        margin.AddChild(dialog);
        AddChild(popup);
        
        // 连接信号
        dialog.TextSubmitted += OnRenameSubmitted;
        dialog.TextChanged += (string newText) => ValidateTagName(dialog, newText);
        
        // 处理取消操作
        popup.CloseRequested += () => 
        {
            _itemToRename = null;
            popup.QueueFree();
        };
        
        popup.Show();
        dialog.GrabFocus();
    }
    
    private void OnTagAdded(string tagName, TreeItem parentItem)
    {
        var tag = _tagManager.RequestGameplayTag(tagName);
        if (parentItem != null)
        {
            CreateTagTreeItem(tag, parentItem);
        }
        else
        {
            CreateTagTreeItem(tag, _root);
        }
    }
    
    private void OnDeleteConfirmed()
    {
        if (_itemToDelete == null) return;
        
        // 获取要删除的标签路径
        string tagPath = _itemToDelete.GetText(1);
        var tagToDelete = _tagManager.RequestGameplayTag(tagPath);
        
        // 递归删除所有子标签
        DeleteTagAndChildren(_itemToDelete);
        
        // 从树中移除项
        _itemToDelete.Free();
        _itemToDelete = null;
    }
    
    private void DeleteTagAndChildren(TreeItem item)
    {
        // 首先递归处理所有子项
        var child = item.GetFirstChild();
        while (child != null)
        {
            var next = child.GetNext(); // 在删除前保存下一个兄弟项
            DeleteTagAndChildren(child);
            child = next;
        }
    }
    
    private void OnRenameSubmitted(string newName)
    {
        if (_itemToRename == null || string.IsNullOrWhiteSpace(newName)) return;
        
        // 获取当前完整路径
        string oldPath = _itemToRename.GetText(1);
        
        // 构建新的完整路径
        string newPath;
        var parent = _itemToRename.GetParent();
        if (parent != null && parent != _root)
        {
            string parentPath = parent.GetText(1);
            newPath = $"{parentPath}.{newName}";
        }
        else
        {
            newPath = newName;
        }
        
        // 检查新名称是否已存在
        if (_tagManager.IsTagNameRegistered(newPath))
        {
            ShowError("Tag name already exists!");
            return;
        }
        
        // 更新标签
        if (UpdateTagName(oldPath, newPath))
        {
            // 更新树项
            _itemToRename.SetText(0, newName);
            _itemToRename.SetText(1, newPath);
            
            // 更新所有子项的路径
            UpdateChildrenPaths(_itemToRename, oldPath, newPath);
        }
        
        // 清理
        _itemToRename = null;
        GetParent().QueueFree(); // 关闭重命名窗口
    }
    
    private void ValidateTagName(LineEdit lineEdit, string newText)
    {
        // 验证标签名称的有效性
        bool isValid = !string.IsNullOrWhiteSpace(newText) && 
                        System.Text.RegularExpressions.Regex.IsMatch(newText, @"^[a-zA-Z][a-zA-Z0-9_]*$");
        
        lineEdit.Modulate = isValid ? Colors.White : Colors.Red;
    }
    
    private bool UpdateTagName(string oldPath, string newPath)
    {
        // 这里需要在 GameplayTagManager 中实现重命名功能
        return _tagManager.RenameTag(oldPath, newPath);
    }
    
    private void UpdateChildrenPaths(TreeItem item, string oldBasePath, string newBasePath)
    {
        var child = item.GetFirstChild();
        while (child != null)
        {
            string oldChildPath = child.GetText(1);
            string newChildPath = oldChildPath.Replace(oldBasePath, newBasePath);
            
            child.SetText(1, newChildPath);
            
            // 递归更新子项
            UpdateChildrenPaths(child, oldBasePath, newBasePath);
            
            child = child.GetNext();
        }
    }
    
    private void ShowError(string message)
    {
        var dialog = new AcceptDialog
        {
            Title = "Error",
            DialogText = message,
            MinSize = new Vector2I(200, 100)
        };
        AddChild(dialog);
        dialog.Show();
    }
} 