// This file is auto-generated. Do not modify.

namespace Miros.Core;

public static partial class Tags
{
    public static Tag AttributeSet_Character { get; } = TagManager.RequestTag("AttributeSet.Character");
    public static Tag AttributeSet_Warrior { get; } = TagManager.RequestTag("AttributeSet.Warrior");
    public static Tag AttributeSet_Player { get; } = TagManager.RequestTag("AttributeSet.Player");
    public static Tag AttributeSet_Enemy { get; } = TagManager.RequestTag("AttributeSet.Enemy");
    public static Tag Attribute_Gravity { get; } = TagManager.RequestTag("Attribute.Gravity");
    public static Tag Attribute_RunSpeed { get; } = TagManager.RequestTag("Attribute.RunSpeed");
    public static Tag Attribute_JumpVelocity { get; } = TagManager.RequestTag("Attribute.JumpVelocity");
    public static Tag Attribute_FloorAcceleration { get; } = TagManager.RequestTag("Attribute.FloorAcceleration");
    public static Tag Attribute_AirAcceleration { get; } = TagManager.RequestTag("Attribute.AirAcceleration");
    public static Tag Attribute_Strength { get; } = TagManager.RequestTag("Attribute.Strength");
    public static Tag Attribute_Defense { get; } = TagManager.RequestTag("Attribute.Defense");
    public static Tag Attribute_WalkSpeed { get; } = TagManager.RequestTag("Attribute.WalkSpeed");
}

public class CharacterAttributeSet : AttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Character;

    protected readonly AttributeBase _gravity;
    protected readonly AttributeBase _runspeed;
    protected readonly AttributeBase _jumpvelocity;
    protected readonly AttributeBase _flooracceleration;
    protected readonly AttributeBase _airacceleration;

    public override AttributeBase[] Attributes => [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration
    ];

    public CharacterAttributeSet() : base()
    {
        _gravity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_Gravity, 980.0f);
        _runspeed = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_RunSpeed, 200.0f);
        _jumpvelocity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_JumpVelocity, -300.0f);
        _flooracceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_FloorAcceleration, 1000.0f);
        _airacceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_AirAcceleration, 800.0f);
    }

    public AttributeBase Gravity => _gravity;
    public AttributeBase RunSpeed => _runspeed;
    public AttributeBase JumpVelocity => _jumpvelocity;
    public AttributeBase FloorAcceleration => _flooracceleration;
    public AttributeBase AirAcceleration => _airacceleration;
}

public class WarriorAttributeSet : CharacterAttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Warrior;

    protected readonly AttributeBase _strength;
    protected readonly AttributeBase _defense;

    public override AttributeBase[] Attributes => [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration, _strength, _defense
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration, Tags.Attribute_Strength, Tags.Attribute_Defense
    ];

    public WarriorAttributeSet() : base()
    {
        _strength = new AttributeBase(Tags.AttributeSet_Warrior, Tags.Attribute_Strength, 10.0f);
        _defense = new AttributeBase(Tags.AttributeSet_Warrior, Tags.Attribute_Defense, 8.0f);
    }

    public AttributeBase Strength => _strength;
    public AttributeBase Defense => _defense;
}

public class PlayerAttributeSet : CharacterAttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Player;


    public override AttributeBase[] Attributes => [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration
    ];

    public PlayerAttributeSet() : base()
    {
    }

}

public class EnemyAttributeSet : CharacterAttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Enemy;

    protected readonly AttributeBase _walkspeed;

    public override AttributeBase[] Attributes => [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration, _walkspeed
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration, Tags.Attribute_WalkSpeed
    ];

    public EnemyAttributeSet() : base()
    {
        _walkspeed = new AttributeBase(Tags.AttributeSet_Enemy, Tags.Attribute_WalkSpeed, 80.0f);
    }

    public AttributeBase WalkSpeed => _walkspeed;
}
