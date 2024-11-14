// This file is auto-generated. Do not modify.

using Godot;
using Miros.Core;

namespace BraveStory;

public class MagicData
{
    public bool elementType = true;
    public int magicDamage = 5;
}

public class SwordData
{
    public float attackSpeed = 1.2f;
    public string weaponType = "melee";
}

public class WeaponData
{
    public int baseDamage = 10;
    public int durabilityMax = 100;
}

public class ItemData
{
    public bool stackable = false;
    public float weight = 1.0f;
}

public class BuffData
{
    public int duration = 10;
}

public class DebuffData
{
    public int duration = 5;
}

public class BaseData
{
    public float AirAcceleration = 800.0f;
    public float FloorAcceleration = 1000.0f;
    public float Gravity = 980.0f;
    public float JumpVelocity = -300.0f;
    public float RunSpeed = 200.0f;
    public Vector2 WallJumpVelocity = new(1000f, -320f);
}

public class WarriorData
{
    public int Defense = 8;
    public int Strength = 10;
}

public class PlayerData
{
    public float AirAcceleration = 800.0f;
    public float attackSpeed = 1.2f;
    public int Defense = 8;
    public float FloorAcceleration = 1000.0f;
    public float Gravity = 980.0f;
    public float JumpVelocity = -300.0f;
    public float RunSpeed = 200.0f;
    public int Strength = 10;
    public Vector2 WallJumpVelocity = new(1000f, -320f);
    public string weaponType = "melee";
}

public class EnemyData
{
    public float AirAcceleration = 800.0f;
    public float FloorAcceleration = 1000.0f;
    public float Gravity = 980.0f;
    public float JumpVelocity = -300.0f;
    public float RunSpeed = 200.0f;
    public float WalkSpeed = 80.0f;
    public Vector2 WallJumpVelocity = new(1000f, -320f);
}

public static class Tags
{
    private static TagManager TagManager => TagManager.Instance;

    public static Tag LayerMovement { get; } = TagManager.RequestTag("StateLayer.LayerMovement");

    public static Tag LayerBuff { get; } = TagManager.RequestTag("StateLayer.LayerBuff");

    public static Tag Magic { get; } = TagManager.RequestTag("Item.Weapon.Sword.Magic");

    public static Tag Sword { get; } = TagManager.RequestTag("Item.Weapon.Sword");

    public static Tag Weapon { get; } = TagManager.RequestTag("Item.Weapon");

    public static Tag Item { get; } = TagManager.RequestTag("Item");

    public static Tag Buff { get; } = TagManager.RequestTag("Effect.Buff");

    public static Tag Debuff { get; } = TagManager.RequestTag("Effect.Debuff");

    public static Tag Effect { get; } = TagManager.RequestTag("Effect");

    public static Tag Character { get; } = TagManager.RequestTag("Character");

    public static Tag Idle { get; } = TagManager.RequestTag("State.Action.Idle");

    public static Tag Run { get; } = TagManager.RequestTag("State.Action.Run");

    public static Tag Walk { get; } = TagManager.RequestTag("State.Action.Walk");

    public static Tag Jump { get; } = TagManager.RequestTag("State.Action.Jump");

    public static Tag Fall { get; } = TagManager.RequestTag("State.Action.Fall");

    public static Tag Attack { get; } = TagManager.RequestTag("State.Action.Attack");

    public static Tag DoubleJump { get; } = TagManager.RequestTag("State.Action.DoubleJump");

    public static Tag WallJump { get; } = TagManager.RequestTag("State.Action.WallJump");

    public static Tag Landing { get; } = TagManager.RequestTag("State.Action.Landing");

    public static Tag WallSlide { get; } = TagManager.RequestTag("State.Action.WallSlide");

    public static Tag Hit { get; } = TagManager.RequestTag("State.Action.Hit");

    public static Tag Sliding { get; } = TagManager.RequestTag("State.Action.Sliding");

    public static Tag Attack1 { get; } = TagManager.RequestTag("State.Action.Attack1");

    public static Tag Attack11 { get; } = TagManager.RequestTag("State.Action.Attack11");

    public static Tag Attack111 { get; } = TagManager.RequestTag("State.Action.Attack111");

    public static Tag Action { get; } = TagManager.RequestTag("State.Action");

    public static Tag Die { get; } = TagManager.RequestTag("State.Status.Die");

    public static Tag Status { get; } = TagManager.RequestTag("State.Status");

    public static Tag Base { get; } = TagManager.RequestTag("Character.Base");

    public static Tag Warrior { get; } = TagManager.RequestTag("Character.Warrior");

    public static Tag Player { get; } = TagManager.RequestTag("Character.Player");

    public static Tag Enemy { get; } = TagManager.RequestTag("Character.Enemy");
}