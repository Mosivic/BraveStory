using FSM.States.Buff;
using Godot;
using System;

public class HurtEventArgs : EventArgs
{
    public Area2D Hitbox { get; }
    public BuffState Buff { get; }

    public HurtEventArgs(Area2D hitbox, BuffState buff)
    {
        Hitbox = hitbox;
        Buff = buff;
    }
}

public partial class HurtBox : Area2D
{
    public event EventHandler<HurtEventArgs> OnHurt;

    public void Emit(Area2D hitbox, BuffState buff)
    {
        OnHurt?.Invoke(this, new HurtEventArgs(hitbox, buff));
    }
}   