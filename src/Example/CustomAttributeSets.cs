// This file is auto-generated. Do not modify.

namespace Miros.Core;

public static partial class Tags
{
    public static Tag AttributeSet_Character { get; } = TagManager.RequestTag("AttributeSet.Character");
    public static Tag AttributeSet_Character_Warrior { get; } = TagManager.RequestTag("AttributeSet.Character.Warrior");
    public static Tag AttributeSet_Character_Player { get; } = TagManager.RequestTag("AttributeSet.Character.Player");
    public static Tag AttributeSet_Character_Enemy { get; } = TagManager.RequestTag("AttributeSet.Character.Enemy");
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
    protected AttributeBase _airacceleration;
    protected AttributeBase _flooracceleration;

    protected AttributeBase _gravity;
    protected AttributeBase _jumpvelocity;
    protected AttributeBase _runspeed;

    public CharacterAttributeSet()
    {
        _gravity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_Gravity, 980.0f);
        _runspeed = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_RunSpeed, 200.0f);
        _jumpvelocity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_JumpVelocity, -300.0f);
        _flooracceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_FloorAcceleration, 1000.0f);
        _airacceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_AirAcceleration, 800.0f);
    }

    public override Tag AttributeSetTag => Tags.AttributeSet_Character;

    public override AttributeBase[] Attributes =>
    [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration
    ];

    public override Tag[] AttributeTags =>
    [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration
    ];

    public AttributeBase Gravity => _gravity;
    public AttributeBase RunSpeed => _runspeed;
    public AttributeBase JumpVelocity => _jumpvelocity;
    public AttributeBase FloorAcceleration => _flooracceleration;
    public AttributeBase AirAcceleration => _airacceleration;
}

public class WarriorAttributeSet : CharacterAttributeSet
{
    protected AttributeBase _defense;

    protected AttributeBase _strength;

    public WarriorAttributeSet()
    {
        _strength = new AttributeBase(Tags.AttributeSet_Character_Warrior, Tags.Attribute_Strength, 10.0f);
        _defense = new AttributeBase(Tags.AttributeSet_Character_Warrior, Tags.Attribute_Defense, 8.0f);
    }

    public override Tag AttributeSetTag => Tags.AttributeSet_Character_Warrior;

    public override AttributeBase[] Attributes =>
    [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration, _strength, _defense
    ];

    public override Tag[] AttributeTags =>
    [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration, Tags.Attribute_Strength, Tags.Attribute_Defense
    ];

    public AttributeBase Strength => _strength;
    public AttributeBase Defense => _defense;
}

public class PlayerAttributeSet : CharacterAttributeSet
{
    public PlayerAttributeSet()
    {
        _jumpvelocity.SetValueWithoutEvent(-500.0f);
    }

    public override Tag AttributeSetTag => Tags.AttributeSet_Character_Player;


    public override AttributeBase[] Attributes =>
    [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration
    ];

    public override Tag[] AttributeTags =>
    [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration
    ];
}

public class EnemyAttributeSet : CharacterAttributeSet
{
    protected AttributeBase _walkspeed;

    public EnemyAttributeSet()
    {
        _walkspeed = new AttributeBase(Tags.AttributeSet_Character_Enemy, Tags.Attribute_WalkSpeed, 80.0f);
    }

    public override Tag AttributeSetTag => Tags.AttributeSet_Character_Enemy;

    public override AttributeBase[] Attributes =>
    [
        _gravity, _runspeed, _jumpvelocity, _flooracceleration, _airacceleration, _walkspeed
    ];

    public override Tag[] AttributeTags =>
    [
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration, Tags.Attribute_WalkSpeed
    ];

    public AttributeBase WalkSpeed => _walkspeed;
}