using Godot;
using Miros.Core;

public partial class EnemyAgent : Agent
{
	public override void _Ready()
	{
		base._Ready();

		Initialize(GetParent<Node2D>(), [typeof(BoarAttributeSet)]);
	}
}
