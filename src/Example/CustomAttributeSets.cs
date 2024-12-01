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
    public CharacterAttributeSet()
    {
        Gravity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_Gravity, 980.0f);
        RunSpeed = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_RunSpeed, 200.0f);
        JumpVelocity = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_JumpVelocity, -300.0f);
        FloorAcceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_FloorAcceleration, 1000.0f);
        AirAcceleration = new AttributeBase(Tags.AttributeSet_Character, Tags.Attribute_AirAcceleration, 800.0f);
    }

    public override Tag[] AttributeSigns => new[]
    {
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration
    };

    public override AttributeBase this[Tag sign] =>
        sign.Equals(Tags.Attribute_Gravity) ? Gravity :
        sign.Equals(Tags.Attribute_RunSpeed) ? RunSpeed :
        sign.Equals(Tags.Attribute_JumpVelocity) ? JumpVelocity :
        sign.Equals(Tags.Attribute_FloorAcceleration) ? FloorAcceleration :
        sign.Equals(Tags.Attribute_AirAcceleration) ? AirAcceleration :
        null;

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
        Strength = new AttributeBase(Tags.AttributeSet_Warrior, Tags.Attribute_Strength, 10.0f);
        Defense = new AttributeBase(Tags.AttributeSet_Warrior, Tags.Attribute_Defense, 8.0f);
    }

    public override Tag[] AttributeSigns => new[]
    {
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration, Tags.Attribute_Strength, Tags.Attribute_Defense
    };

    public override AttributeBase this[Tag sign] =>
        sign.Equals(Tags.Attribute_Strength) ? Strength :
        sign.Equals(Tags.Attribute_Defense) ? Defense :
        base[sign];

    public AttributeBase Strength { get; }

    public AttributeBase Defense { get; }
}

public class PlayerAttributeSet : CharacterAttributeSet
{
    public override Tag[] AttributeSigns => new[]
    {
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration
    };

    public override AttributeBase this[Tag sign] =>
        base[sign];
}

public class EnemyAttributeSet : CharacterAttributeSet
{
    public EnemyAttributeSet()
    {
        WalkSpeed = new AttributeBase(Tags.AttributeSet_Enemy, Tags.Attribute_WalkSpeed, 80.0f);
    }

    public override Tag[] AttributeSigns => new[]
    {
        Tags.Attribute_Gravity, Tags.Attribute_RunSpeed, Tags.Attribute_JumpVelocity, Tags.Attribute_FloorAcceleration,
        Tags.Attribute_AirAcceleration, Tags.Attribute_WalkSpeed
    };

    public override AttributeBase this[Tag sign] =>
        sign.Equals(Tags.Attribute_WalkSpeed) ? WalkSpeed : base[sign];

    public AttributeBase WalkSpeed { get; }
}