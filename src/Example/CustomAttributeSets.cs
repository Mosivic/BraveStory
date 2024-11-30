// This file is auto-generated. Do not modify.

using Miros.Core;

namespace Example;

public static class AttributeTags
{
    private static TagManager TagManager => TagManager.Instance;
    public static Tag Character_Gravity { get; } = TagManager.RequestTag("Character.Gravity");
    public static Tag Character_RunSpeed { get; } = TagManager.RequestTag("Character.RunSpeed");
    public static Tag Character_JumpVelocity { get; } = TagManager.RequestTag("Character.JumpVelocity");
    public static Tag Character_FloorAcceleration { get; } = TagManager.RequestTag("Character.FloorAcceleration");
    public static Tag Character_AirAcceleration { get; } = TagManager.RequestTag("Character.AirAcceleration");
    public static Tag Warrior_Gravity { get; } = TagManager.RequestTag("Warrior.Gravity");
    public static Tag Warrior_RunSpeed { get; } = TagManager.RequestTag("Warrior.RunSpeed");
    public static Tag Warrior_JumpVelocity { get; } = TagManager.RequestTag("Warrior.JumpVelocity");
    public static Tag Warrior_FloorAcceleration { get; } = TagManager.RequestTag("Warrior.FloorAcceleration");
    public static Tag Warrior_AirAcceleration { get; } = TagManager.RequestTag("Warrior.AirAcceleration");
    public static Tag Warrior_Strength { get; } = TagManager.RequestTag("Warrior.Strength");
    public static Tag Warrior_Defense { get; } = TagManager.RequestTag("Warrior.Defense");
    public static Tag Player_Gravity { get; } = TagManager.RequestTag("Player.Gravity");
    public static Tag Player_RunSpeed { get; } = TagManager.RequestTag("Player.RunSpeed");
    public static Tag Player_JumpVelocity { get; } = TagManager.RequestTag("Player.JumpVelocity");
    public static Tag Player_FloorAcceleration { get; } = TagManager.RequestTag("Player.FloorAcceleration");
    public static Tag Player_AirAcceleration { get; } = TagManager.RequestTag("Player.AirAcceleration");
    public static Tag Enemy_Gravity { get; } = TagManager.RequestTag("Enemy.Gravity");
    public static Tag Enemy_RunSpeed { get; } = TagManager.RequestTag("Enemy.RunSpeed");
    public static Tag Enemy_JumpVelocity { get; } = TagManager.RequestTag("Enemy.JumpVelocity");
    public static Tag Enemy_FloorAcceleration { get; } = TagManager.RequestTag("Enemy.FloorAcceleration");
    public static Tag Enemy_AirAcceleration { get; } = TagManager.RequestTag("Enemy.AirAcceleration");
    public static Tag Enemy_WalkSpeed { get; } = TagManager.RequestTag("Enemy.WalkSpeed");
}

public class CharacterAttributeSet : AttributeSet
{
    private static TagManager TagManager => TagManager.Instance;
    private readonly AttributeBase _gravity;
    private readonly AttributeBase _runspeed;
    private readonly AttributeBase _jumpvelocity;
    private readonly AttributeBase _flooracceleration;
    private readonly AttributeBase _airacceleration;
    public static Tag GravityTag => TagManager.RequestTag("Character.Gravity");
    public static Tag RunSpeedTag => TagManager.RequestTag("Character.RunSpeed");
    public static Tag JumpVelocityTag => TagManager.RequestTag("Character.JumpVelocity");
    public static Tag FloorAccelerationTag => TagManager.RequestTag("Character.FloorAcceleration");
    public static Tag AirAccelerationTag => TagManager.RequestTag("Character.AirAcceleration");

    public override string[] AttributeNames => new[] {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration"
    };

    public CharacterAttributeSet() : base()
    {
        _gravity = new AttributeBase("Character", "Gravity", 980.0f);
        _runspeed = new AttributeBase("Character", "RunSpeed", 200.0f);
        _jumpvelocity = new AttributeBase("Character", "JumpVelocity", -300.0f);
        _flooracceleration = new AttributeBase("Character", "FloorAcceleration", 1000.0f);
        _airacceleration = new AttributeBase("Character", "AirAcceleration", 800.0f);
    }

    public override AttributeBase this[string key] =>
        key switch
        {
            "Gravity" => _gravity,
            "RunSpeed" => _runspeed,
            "JumpVelocity" => _jumpvelocity,
            "FloorAcceleration" => _flooracceleration,
            "AirAcceleration" => _airacceleration,
            _ => null
        };

    public AttributeBase Gravity => _gravity;
    public AttributeBase RunSpeed => _runspeed;
    public AttributeBase JumpVelocity => _jumpvelocity;
    public AttributeBase FloorAcceleration => _flooracceleration;
    public AttributeBase AirAcceleration => _airacceleration;
}

public class WarriorAttributeSet : CharacterAttributeSet
{
    private static TagManager TagManager => TagManager.Instance;
    private readonly AttributeBase _strength;
    private readonly AttributeBase _defense;
    public static Tag StrengthTag => TagManager.RequestTag("Warrior.Strength");
    public static Tag DefenseTag => TagManager.RequestTag("Warrior.Defense");

    public override string[] AttributeNames => new[] {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration", "Strength", "Defense"
    };

    public WarriorAttributeSet() : base()
    {
        _strength = new AttributeBase("Warrior", "Strength", 10.0f);
        _defense = new AttributeBase("Warrior", "Defense", 8.0f);
    }

    public AttributeBase Strength => _strength;
    public AttributeBase Defense => _defense;
}

public class PlayerAttributeSet : CharacterAttributeSet
{
    private static TagManager TagManager => TagManager.Instance;

    public override string[] AttributeNames => new[] {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration"
    };

    public PlayerAttributeSet() : base()
    {
    }

}

public class EnemyAttributeSet : CharacterAttributeSet
{
    private static TagManager TagManager => TagManager.Instance;
    private readonly AttributeBase _walkspeed;
    public static Tag WalkSpeedTag => TagManager.RequestTag("Enemy.WalkSpeed");

    public override string[] AttributeNames => new[] {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration", "WalkSpeed"
    };

    public EnemyAttributeSet() : base()
    {
        _walkspeed = new AttributeBase("Enemy", "WalkSpeed", 80.0f);
    }

    public AttributeBase WalkSpeed => _walkspeed;
}
