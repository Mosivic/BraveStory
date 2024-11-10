using Godot;
using System;
using Miros.Core;

public class HurtEventArgs : EventArgs
{
    public Area2D Hitbox { get; }
    public Buff Buff { get; }

    public HurtEventArgs(Area2D hitbox, Buff buff)
    {
        Hitbox = hitbox;
        Buff = buff;
    }
}

public partial class HurtBox : Area2D
{
    public event EventHandler<HurtEventArgs> OnHurt;

    public void Emit(Area2D hitbox, Buff buff)
    {
        OnHurt?.Invoke(this, new HurtEventArgs(hitbox, buff));
    }
}   