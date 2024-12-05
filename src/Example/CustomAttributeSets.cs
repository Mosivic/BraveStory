// This file is auto-generated. Do not modify.

namespace Miros.Core;

public static partial class Tags
{
    public static Tag AttributeSet_Character { get; } = TagManager.RequestTag("AttributeSet.Character");
    public static Tag AttributeSet_Character_Player { get; } = TagManager.RequestTag("AttributeSet.Character.Player");
    public static Tag AttributeSet_Character_Boar { get; } = TagManager.RequestTag("AttributeSet.Character.Boar");
    public static Tag Attribute_HP { get; } = TagManager.RequestTag("Attribute.HP");
    public static Tag Attribute_Defense { get; } = TagManager.RequestTag("Attribute.Defense");
    public static Tag Attribute_Attack { get; } = TagManager.RequestTag("Attribute.Attack");
    public static Tag Attribute_Gravity { get; } = TagManager.RequestTag("Attribute.Gravity");
    public static Tag Attribute_RunSpeed { get; } = TagManager.RequestTag("Attribute.RunSpeed");
    public static Tag Attribute_WalkSpeed { get; } = TagManager.RequestTag("Attribute.WalkSpeed");
    public static Tag Attribute_JumpVelocity { get; } = TagManager.RequestTag("Attribute.JumpVelocity");
    public static Tag Attribute_FloorAcceleration { get; } = TagManager.RequestTag("Attribute.FloorAcceleration");
    public static Tag Attribute_AirAcceleration { get; } = TagManager.RequestTag("Attribute.AirAcceleration");
    public static Tag Attribute_SlidingDeceleration { get; } = TagManager.RequestTag("Attribute.SlidingDeceleration");
}

public class CharacterAttributeSet : AttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Character;

    protected AttributeBase _hp;
    protected AttributeBase _defense;
    protected AttributeBase _attack;
    protected AttributeBase _gravity;
    protected AttributeBase _runspeed;
    protected AttributeBase _walkspeed;
    protected AttributeBase _jumpvelocity;
    protected AttributeBase _flooracceleration;
    protected AttributeBase _airacceleration;
    protected AttributeBase _slidingdeceleration;

    public override AttributeBase[] Attributes => [
        _hp, _defense, _attack, _gravity, _runspeed, _walkspeed, _jumpvelocity, _flooracceleration, _airacceleration, _slidingdeceleration
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_HP, Tags.Attribute_Defense, Tags.Attribute_Attack, Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_WalkSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration, Tags.Attribute_SlidingDeceleration
    ];

    public CharacterAttributeSet() : base()
    {
        _hp = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_HP, 100.0f);
        _defense = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_Defense, 5.0f);
        _attack = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_Attack, 10.0f);
        _gravity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_Gravity, 980.0f);
        _runspeed = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_RunSpeed, 200.0f);
        _walkspeed = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_WalkSpeed, 80.0f);
        _jumpvelocity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_JumpVelocity, -300.0f);
        _flooracceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_FloorAcceleration, 1000.0f);
        _airacceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_AirAcceleration, 800.0f);
        _slidingdeceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_SlidingDeceleration, 600.0f);
    }

    public AttributeBase HP => _hp;
    public AttributeBase Defense => _defense;
    public AttributeBase Attack => _attack;
    public AttributeBase Gravity => _gravity;
    public AttributeBase RunSpeed => _runspeed;
    public AttributeBase WalkSpeed => _walkspeed;
    public AttributeBase JumpVelocity => _jumpvelocity;
    public AttributeBase FloorAcceleration => _flooracceleration;
    public AttributeBase AirAcceleration => _airacceleration;
    public AttributeBase SlidingDeceleration => _slidingdeceleration;
}

public class PlayerAttributeSet : CharacterAttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Character_Player;


    public override AttributeBase[] Attributes => [
        _hp, _defense, _attack, _gravity, _runspeed, _walkspeed, _jumpvelocity, _flooracceleration, _airacceleration, _slidingdeceleration
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_HP, Tags.Attribute_Defense, Tags.Attribute_Attack, Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_WalkSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration, Tags.Attribute_SlidingDeceleration
    ];

    public PlayerAttributeSet() : base()
    {
        _attack.SetValueWithoutEvent(15.0f);
    }

}

public class BoarAttributeSet : CharacterAttributeSet
{
    public override Tag AttributeSetTag => Tags.AttributeSet_Character_Boar;


    public override AttributeBase[] Attributes => [
        _hp, _defense, _attack, _gravity, _runspeed, _walkspeed, _jumpvelocity, _flooracceleration, _airacceleration, _slidingdeceleration
    ];

    public override Tag[] AttributeTags => [
        Tags.Attribute_HP, Tags.Attribute_Defense, Tags.Attribute_Attack, Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_WalkSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration, Tags.Attribute_AirAcceleration, Tags.Attribute_SlidingDeceleration
    ];

    public BoarAttributeSet() : base()
    {
        _hp.SetValueWithoutEvent(30.0f);
    }

}
