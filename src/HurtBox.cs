using Godot;
using System;

[Signal]
public delegate void HitEventHandler(HurtBox hurtBox);

public partial class HurtBox : Area2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void OnAreaEntered(Area2D hurtBox){
		GD.Print($"[Hit] {Owner.Name} -> {hurtBox.Owner.Name}");
		EmitSignal(SignalName.HitEventHandler, hurtBox);
	}
}
