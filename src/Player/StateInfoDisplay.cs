using Godot;
using Miros.Core;

namespace BraveStory;

public partial class StateInfoDisplay : Node2D
{
	private Label _label;

	// private MultiLayerStateMachineConnect _connect;
	private Tag _layerTag;
	private Node2D _target;

	[Export] public Vector2 Offset { get; set; } = new(-15, -25);
	[Export] public Vector2 TextScale { get; set; } = new(0.5f, 0.5f);
	[Export] public Color TextColor { get; set; } = Colors.Yellow;

	public override void _Ready()
	{
		_label = new Label
		{
			Position = Offset,
			HorizontalAlignment = HorizontalAlignment.Center,
			Scale = TextScale,
			Modulate = TextColor
		};
		AddChild(_label);

		_target = GetParent<Node2D>();
		if (_target == null) GD.PrintErr("StateInfoDisplay must be child of a Node2D");
	}

	// public void Setup(MultiLayerStateMachineConnect connect, Tag layerTag)
	// {
	//     _connect = connect;
	//     _layerTag = layerTag;
	// }

	// public override void _Process(double delta)
	// {
	//     if (_target != null)
	//     {
	//         GlobalPosition = _target.GlobalPosition + Offset;
	//     }

	//     if (_connect == null) return;

	//     var currentState = _connect.GetNowState(_layerTag);
	//     if (currentState != null)
	//     {
	//         _label.Text = $"{currentState.Name} - {currentState.Status}";
	//     }
	// }
}
