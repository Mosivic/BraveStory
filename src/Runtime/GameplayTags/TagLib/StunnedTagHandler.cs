using Godot;

public class StunnedTagHandler : GameplayTagEventHandler
{
    public StunnedTagHandler() 
        : base(GameplayTagManager.Instance.RequestGameplayTag("Status.Stunned"))
    {
    }
    
    public override void OnTagAdded(GameplayTagContainer container, Node owner)
    {
        // 应用眩晕效果
        if (owner is CharacterBody2D character)
        {
            character.SetPhysicsProcess(false);
            //character.PlayAnimation("stunned");
        }
    }
    
    public override void OnTagRemoved(GameplayTagContainer container, Node owner)
    {
        // 移除眩晕效果
        if (owner is CharacterBody2D character)
        {
            character.SetPhysicsProcess(true);
            //character.PlayAnimation("idle");
        }
    }
}