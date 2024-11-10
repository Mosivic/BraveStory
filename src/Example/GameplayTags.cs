// This file is auto-generated. Do not modify.
using Godot;
using Miros.Core;

namespace BraveStory
{
    public class BaseData
    {
        public float Gravity = 980.0f;
        public float RunSpeed = 200.0f;
        public float JumpVelocity = -300.0f;
        public float FloorAcceleration = 1000.0f;
        public float AirAcceleration = 800.0f;
        public Vector2 WallJumpVelocity = new Vector2(1000f, -320f);
    }

    public class WarriorData
    {
        public int Strength = 10;
        public int Defense = 8;
    }

    public class PlayerData
    {
        public float Gravity = 980.0f;
        public float RunSpeed = 200.0f;
        public float JumpVelocity = -300.0f;
        public float FloorAcceleration = 1000.0f;
        public float AirAcceleration = 800.0f;
        public Vector2 WallJumpVelocity = new Vector2(1000f, -320f);
        public int Strength = 10;
        public int Defense = 8;
        public float attackSpeed = 1.2f;
        public string weaponType = "melee";
    }

    public class EnemyData
    {
        public float Gravity = 980.0f;
        public float RunSpeed = 200.0f;
        public float JumpVelocity = -300.0f;
        public float FloorAcceleration = 1000.0f;
        public float AirAcceleration = 800.0f;
        public Vector2 WallJumpVelocity = new Vector2(1000f, -320f);
        public float WalkSpeed = 80.0f;
    }

    public class MagicData
    {
        public int magicDamage = 5;
        public bool elementType = true;
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
        public float weight = 1.0f;
        public bool stackable = false;
    }

    public class BuffData
    {
        public int duration = 10;
    }

    public class DebuffData
    {
        public int duration = 5;
    }

    public static class Tags
    {
        private static TagManager TagManager => TagManager.Instance;

        public static Tag Base { get; } = TagManager.RequestGameplayTag("Character.Base");

        public static Tag Warrior { get; } = TagManager.RequestGameplayTag("Character.Warrior");

        public static Tag Player { get; } = TagManager.RequestGameplayTag("Character.Player");

        public static Tag Enemy { get; } = TagManager.RequestGameplayTag("Character.Enemy");

        public static Tag Magic { get; } = TagManager.RequestGameplayTag("Item.Weapon.Sword.Magic");

        public static Tag Sword { get; } = TagManager.RequestGameplayTag("Item.Weapon.Sword");

        public static Tag Weapon { get; } = TagManager.RequestGameplayTag("Item.Weapon");

        public static Tag Item { get; } = TagManager.RequestGameplayTag("Item");

        public static Tag Buff { get; } = TagManager.RequestGameplayTag("Effect.Buff");

        public static Tag Debuff { get; } = TagManager.RequestGameplayTag("Effect.Debuff");

        public static Tag Effect { get; } = TagManager.RequestGameplayTag("Effect");

        public static Tag Character { get; } = TagManager.RequestGameplayTag("Character");

        public static Tag LayerMovement { get; } = TagManager.RequestGameplayTag("StateLayer.LayerMovement");

        public static Tag LayerBuff { get; } = TagManager.RequestGameplayTag("StateLayer.LayerBuff");

        public static Tag Idle { get; } = TagManager.RequestGameplayTag("State.Action.Idle");

        public static Tag Run { get; } = TagManager.RequestGameplayTag("State.Action.Run");

        public static Tag Walk { get; } = TagManager.RequestGameplayTag("State.Action.Walk");

        public static Tag Jump { get; } = TagManager.RequestGameplayTag("State.Action.Jump");

        public static Tag Fall { get; } = TagManager.RequestGameplayTag("State.Action.Fall");

        public static Tag Attack { get; } = TagManager.RequestGameplayTag("State.Action.Attack");

        public static Tag DoubleJump { get; } = TagManager.RequestGameplayTag("State.Action.DoubleJump");

        public static Tag WallJump { get; } = TagManager.RequestGameplayTag("State.Action.WallJump");

        public static Tag Landing { get; } = TagManager.RequestGameplayTag("State.Action.Landing");

        public static Tag WallSlide { get; } = TagManager.RequestGameplayTag("State.Action.WallSlide");

        public static Tag Hit { get; } = TagManager.RequestGameplayTag("State.Action.Hit");

        public static Tag Action { get; } = TagManager.RequestGameplayTag("State.Action");

        public static Tag Die { get; } = TagManager.RequestGameplayTag("State.Status.Die");

        public static Tag Status { get; } = TagManager.RequestGameplayTag("State.Status");

    }
}