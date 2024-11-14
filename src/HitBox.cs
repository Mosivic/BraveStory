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
    private Buff _buffState;

    public HitBox()
    {
        AreaEntered += OnAreaEntered;
    }

    public event EventHandler<HitEventArgs> OnHit;


    public void SetBuffState(Buff buffState)
    {
        _buffState = buffState;
    }

    private void OnAreaEntered(Area2D hurtBox)
    {
        if (hurtBox is HurtBox box)
        {
            GD.Print($"[Hit] {Owner.Name} -> {hurtBox.Owner.Name}");
            OnHit?.Invoke(this, new HitEventArgs(hurtBox));
            box.Emit(this, _buffState);
        }
    }
}