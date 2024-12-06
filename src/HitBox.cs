using System;
using Godot;
using Miros.Core;

public class HitEventArgs : EventArgs
{
    public HitEventArgs(Area2D hurtBox)
    {
        HurtBox = hurtBox;
    }

    public Area2D HurtBox { get; }
}

public partial class HitBox : Area2D
{


    public HitBox()
    {
        AreaEntered += OnAreaEntered;
    }

    public event EventHandler<HitEventArgs> OnHit;



    private void OnAreaEntered(Area2D area2D)
    {
        if (area2D is HurtBox hurtBox)
        {
            GD.Print($"[Hit] {Owner.Name} -> {hurtBox.Owner.Name}");
            OnHit?.Invoke(this, new HitEventArgs(hurtBox));
            hurtBox.Emit(this); //Emit HurtBox OnHurt
        }
    }
}