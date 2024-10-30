using Godot;

public partial class GameplayTagEditorWindow : Window
{
	private GameplayTagTreeView _treeView;
	
	public override void _Ready()
	{
		Title = "Gameplay Tag Editor";
		
		// var tagManager = GameplayTagManager.Instance;
		// // 创建一些测试标签
        // var weaponTag = tagManager.RequestGameplayTag("Item.Weapon");
        // var meleeTag = tagManager.RequestGameplayTag("Item.Weapon.Melee");
        // var swordTag = tagManager.RequestGameplayTag("Item.Weapon.Melee.Sword");
        // var brokenTag = tagManager.RequestGameplayTag("Status.Broken");
        // var rareTag = tagManager.RequestGameplayTag("Quality.Rare");
		Test();

		// 创建主布局
		var vbox = new VBoxContainer();
		AddChild(vbox);
		
		// 添加工具栏
		var toolbar = new HBoxContainer();
		vbox.AddChild(toolbar);
		
		var refreshButton = new Button();
		refreshButton.Text = "Refresh";
		refreshButton.Connect("pressed", new Callable(this, nameof(OnRefreshPressed)));
		toolbar.AddChild(refreshButton);
		
		// 添加树视图
		_treeView = new GameplayTagTreeView();
		_treeView.CustomMinimumSize = new Vector2(400, 600);
		vbox.AddChild(_treeView);
	}
	
	private void OnRefreshPressed()
	{
		_treeView.RefreshTree();
	}


	private void Test(){
		var tagManager = GameplayTagManager.Instance;
		var tagInheritanc = new GameplayTagInheritance();

		var tagYamlLoader = new GameplayTagYamlLoader(tagManager,tagInheritanc);
		tagYamlLoader.LoadFromFile("res://Example/gameplay_tags.yaml");
	}
} 
