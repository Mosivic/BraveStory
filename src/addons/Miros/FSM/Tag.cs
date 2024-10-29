using System;
using FSM.States;

namespace FSM;

public readonly struct Tag
{
    private Layer Layer { get; }
    private string Name { get; }


    public Tag(Layer layer, string name)
    {
        Layer = layer;
        Name = name;
    }

    public Tag(AbsState state)
    {
        Layer = state.Layer;
        Name = state.Name;
    }

    public override bool Equals(object obj)
    {
        return obj is Tag other && Equals(other);
    }

    private bool Equals(Tag other)
    {
        return Equals(Layer, other.Layer) && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Layer, Name);
    }

    public static bool operator ==(Tag a, Tag b)
    {
        return a.Layer == b.Layer && a.Name == b.Name;
    }

    public static bool operator !=(Tag a, Tag b)
    {
        return !(a.Layer == b.Layer && a.Name == b.Name);
    }

    public override string ToString()
    {
        return $"{Layer.Name}.{Name}";
    }
}