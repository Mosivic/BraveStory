using System;

public readonly struct GameplayTag : IEquatable<GameplayTag>
{
    private readonly string _tagName;
    
    public bool IsValid => !string.IsNullOrEmpty(_tagName);
    
    public GameplayTag(string tagName)
    {
        _tagName = tagName?.ToLower() ?? string.Empty;
    }
    
    public bool Equals(GameplayTag other)
    {
        return _tagName == other._tagName;
    }
    
    public override bool Equals(object obj)
    {
        return obj is GameplayTag other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        return _tagName?.GetHashCode() ?? 0;
    }
    
    public static bool operator ==(GameplayTag a, GameplayTag b) => a.Equals(b);
    public static bool operator !=(GameplayTag a, GameplayTag b) => !a.Equals(b);
    
    public override string ToString() => _tagName;
} 