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
    
    private BuffState _buffState;

    public HitBox()
    {
        AreaEntered += OnAreaEntered;
    }


    public void SetBuffState(BuffState buffState)
    {
        _buffState = buffState;
    }

    private void OnAreaEntered(Area2D hurtBox)
    {
        if(hurtBox is HurtBox box)
        {
            GD.Print($"[Hit] {Owner.Name} -> {hurtBox.Owner.Name}");
            OnHit?.Invoke(this, new HitEventArgs(hurtBox));
            box.Emit(this, _buffState);
        }
    }
}

