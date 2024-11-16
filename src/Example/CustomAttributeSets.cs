using Miros.Core;

namespace Example;

public class CharacterAttributeSet : AttributeSet
{
    private readonly AttributeBase _gravity;
    private readonly AttributeBase _runspeed;
    private readonly AttributeBase _jumpvelocity;
    private readonly AttributeBase _flooracceleration;
    private readonly AttributeBase _airacceleration;

    public override string[] AttributeNames => [
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration"
    ];

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
    private readonly AttributeBase _strength;
    private readonly AttributeBase _defense;

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

    public override string[] AttributeNames => new[] {
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration"
    };

    public PlayerAttributeSet() : base()
    {
    }

}

public class EnemyAttributeSet : CharacterAttributeSet
{
    private readonly AttributeBase _walkspeed;

    public override string[] AttributeNames => [
        "Gravity", "RunSpeed", "JumpVelocity", "FloorAcceleration", "AirAcceleration", "WalkSpeed"
    ];

    public EnemyAttributeSet() : base()
    {
        _walkspeed = new AttributeBase("Enemy", "WalkSpeed", 80.0f);
    }

    public AttributeBase WalkSpeed => _walkspeed;
}
