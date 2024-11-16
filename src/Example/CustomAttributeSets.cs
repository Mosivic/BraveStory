using Miros.Core;

namespace Example;

public class CharacterAttributeSet : AttributeSet
{
    public CharacterAttributeSet()
    {
        Gravity = new AttributeBase("Character", "Gravity", 980.0f);
        RunSpeed = new AttributeBase("Character", "RunSpeed", 200.0f);
        JumpVelocity = new AttributeBase("Character", "JumpVelocity", -300.0f);
        FloorAcceleration = new AttributeBase("Character", "FloorAcceleration", 1000.0f);
        AirAcceleration = new AttributeBase("Character", "AirAcceleration", 800.0f);
    }

    public override string[] AttributeNames =>
    [
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration"
    ];

    public override AttributeBase this[string key] =>
        key switch
        {
            "Gravity" => Gravity,
            "RunSpeed" => RunSpeed,
            "JumpVelocity" => JumpVelocity,
            "FloorAcceleration" => FloorAcceleration,
            "AirAcceleration" => AirAcceleration,
            _ => null
        };

    public AttributeBase Gravity { get; }

    public AttributeBase RunSpeed { get; }

    public AttributeBase JumpVelocity { get; }

    public AttributeBase FloorAcceleration { get; }

    public AttributeBase AirAcceleration { get; }
}

public class WarriorAttributeSet : CharacterAttributeSet
{
    public WarriorAttributeSet()
    {
        Strength = new AttributeBase("Warrior", "Strength", 10.0f);
        Defense = new AttributeBase("Warrior", "Defense", 8.0f);
    }

    public override string[] AttributeNames => new[]
    {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration", "Strength", "Defense"
    };

    public AttributeBase Strength { get; }

    public AttributeBase Defense { get; }
}

public class PlayerAttributeSet : CharacterAttributeSet
{
    public override string[] AttributeNames => new[]
    {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration"
    };
}

public class EnemyAttributeSet : CharacterAttributeSet
{
    public EnemyAttributeSet()
    {
        WalkSpeed = new AttributeBase("Enemy", "WalkSpeed", 80.0f);
    }

    public override string[] AttributeNames =>
    [
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration", "WalkSpeed"
    ];

    public AttributeBase WalkSpeed { get; }
}