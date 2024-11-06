using FSM.States.Buff;
using Godot;
using System;

public class HitEventArgs : EventArgs
{
    public Area2D HurtBox { get; }

    public HitEventArgs(Area2D hurtBox)
    {
        HurtBox = hurtBox;
    }
}

public partial class HitBox : Area2D
{
    public event EventHandler<HitEventArgs> OnHit;
    
    public BuffState BuffState { get; set; }

    public HitBox()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D hurtBox)
    {
        if(hurtBox is HurtBox box)
        {
            GD.Print($"[Hit] {Owner.Name} -> {hurtBox.Name}");
            OnHit?.Invoke(this, new HitEventArgs(hurtBox));
            box.Emit(this, BuffState);
        }
    }
}

