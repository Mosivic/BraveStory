// This file is auto-generated. Do not modify.
using Godot;
using System.Runtime.CompilerServices;

namespace BraveStory
{
    public class BaseData
    {
        public float Gravity { get; set;} = 980.0f;
        public float RunSpeed { get; set;} = 200.0f;
        public float JumpVelocity { get; set;} = -300.0f;
        public float FloorAcceleration { get; set;} = 1000.0f;
        public bool AirAcceleration { get; set;} = true;
    }

    public class WarriorData
    {
        public int Strength { get; set;} = 10;
        public int Defense { get; set;} = 8;
    }

    public class MagicData
    {
        public int magicDamage { get; set;} = 5;
        public bool elementType { get; set;} = true;
    }

    public class SwordData
    {
        public float attackSpeed { get; set;} = 1.2f;
        public string weaponType { get; set;} = "melee";
    }

    public class WeaponData
    {
        public int baseDamage { get; set;} = 10;
        public int durabilityMax { get; set;} = 100;
    }

    public class ItemData
    {
        public float weight { get; set;} = 1.0f;
        public bool stackable { get; set;} = false;
    }

    public class BuffData
    {
        public int duration { get; set;} = 10;
    }

    public class DebuffData
    {
        public int duration { get; set;} = 5;
    }

    public class PlayerData : BaseData
    {
        public new float JumpVelocity { get; } = -300.0f;
    }

    public static class Tags
    {
        private static GameplayTagManager TagManager => GameplayTagManager.Instance;

        public static GameplayTag Base { get; } = TagManager.RequestGameplayTag("Character.Base");

        public static GameplayTag Warrior { get; } = TagManager.RequestGameplayTag("Character.Warrior");

        public static GameplayTag Player { get; } = TagManager.RequestGameplayTag("Character.Player");

        public static GameplayTag KeyDownJump { get; } = TagManager.RequestGameplayTag("Condition.KeyDownJump");

        public static GameplayTag KeyDownMove { get; } = TagManager.RequestGameplayTag("Condition.KeyDownMove");

        public static GameplayTag OnFloor { get; } = TagManager.RequestGameplayTag("Condition.OnFloor");

        public static GameplayTag OnAir { get; } = TagManager.RequestGameplayTag("Condition.OnAir");

        public static GameplayTag VelocityYPositive { get; } = TagManager.RequestGameplayTag("Condition.VelocityYPositive");

        public static GameplayTag HandColliding { get; } = TagManager.RequestGameplayTag("Condition.HandColliding");

        public static GameplayTag FootColliding { get; } = TagManager.RequestGameplayTag("Condition.FootColliding");

        public static GameplayTag Magic { get; } = TagManager.RequestGameplayTag("Item.Weapon.Sword.Magic");

        public static GameplayTag Sword { get; } = TagManager.RequestGameplayTag("Item.Weapon.Sword");

        public static GameplayTag Weapon { get; } = TagManager.RequestGameplayTag("Item.Weapon");

        public static GameplayTag Item { get; } = TagManager.RequestGameplayTag("Item");

        public static GameplayTag Buff { get; } = TagManager.RequestGameplayTag("Status.Buff");

        public static GameplayTag Debuff { get; } = TagManager.RequestGameplayTag("Status.Debuff");

        public static GameplayTag Status { get; } = TagManager.RequestGameplayTag("Status");

        public static GameplayTag Character { get; } = TagManager.RequestGameplayTag("Character");

        public static GameplayTag LayerMovement { get; } = TagManager.RequestGameplayTag("StateLayer.LayerMovement");

        public static GameplayTag LayerBuff { get; } = TagManager.RequestGameplayTag("StateLayer.LayerBuff");

    }
}