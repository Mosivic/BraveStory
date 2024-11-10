using Godot;

namespace Miros.Core;
public class StunnedTagHandler : TagEventHandler
{
    public StunnedTagHandler() 
        : base(TagManager.Instance.RequestGameplayTag("Status.Stunned"))
    {
    }
    
    public override void OnTagAdded(TagContainer container, Node owner)
    {
        // 应用眩晕效果
        if (owner is CharacterBody2D character)
        {
            character.SetPhysicsProcess(false);
            //character.PlayAnimation("stunned");
        }
    }
    
    public override void OnTagRemoved(TagContainer container, Node owner)
    {
        // 移除眩晕效果
        if (owner is CharacterBody2D character)
        {
            character.SetPhysicsProcess(true);
            //character.PlayAnimation("idle");
        }
    }
}