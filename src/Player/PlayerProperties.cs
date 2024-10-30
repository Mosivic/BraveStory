using FSM;

namespace BraveStory.Player;

public class PlayerProperties : CharacterProperties
{
    public PlayerProperties()
    {
        InitializeFromTags();
    }

    private void InitializeFromTags()
    {
        var playerTag = GameplayTagManager.Instance.RequestGameplayTag("Character.Player");
        
        Gravity.Value = GameplayTagInheritance.Instance.GetPropertyValue<float>(playerTag, "gravity", 980f);
        RunSpeed.Value = GameplayTagInheritance.Instance.GetPropertyValue<float>(playerTag, "runSpeed", 200f);
        JumpVelocity.Value = GameplayTagInheritance.Instance.GetPropertyValue<float>(playerTag, "jumpVelocity", -300f);
        FloorAcceleration.Value = GameplayTagInheritance.Instance.GetPropertyValue<float>(playerTag, "floorAcceleration", 1000f);
        AirAcceleration.Value = GameplayTagInheritance.Instance.GetPropertyValue<float>(playerTag, "airAcceleration", 1000f);
    }

    public override BindableProperty<float> Gravity { get; set; }
    public override BindableProperty<float> RunSpeed { get; set; }
    public override BindableProperty<float> JumpVelocity { get; set; }
    public override BindableProperty<float> FloorAcceleration { get; set; }
    public override BindableProperty<float> AirAcceleration { get; set; }
}