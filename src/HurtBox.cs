using System;
using Godot;

public class HurtEventArgs : EventArgs
{
    public HurtEventArgs(Area2D hitbox)
    {
        Hitbox = hitbox;
    }

    public Area2D Hitbox { get; }
}

public partial class HurtBox : Area2D
{
    public event EventHandler<HurtEventArgs> OnHurt;

    public void Emit(Area2D hitbox)
    {
        OnHurt?.Invoke(this, new HurtEventArgs(hitbox));
    }
}