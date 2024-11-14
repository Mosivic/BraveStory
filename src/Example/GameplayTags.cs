// This file is auto-generated. Do not modify.
using Godot;
using Miros.Core;

namespace BraveStory
{
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

        public static Tag None { get; } = TagManager.RequestTag("State.Action.None");

        public static Tag Idle { get; } = TagManager.RequestTag("State.Action.Idle");

        public static Tag Run { get; } = TagManager.RequestTag("State.Action.Run");

        public static Tag Walk { get; } = TagManager.RequestTag("State.Action.Walk");

        public static Tag Jump { get; } = TagManager.RequestTag("State.Action.Jump");

        public static Tag Fall { get; } = TagManager.RequestTag("State.Action.Fall");

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
}