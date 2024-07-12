using FSM;

namespace BraveStory.Player;

public class PlayerProperties:CharacterProperties
{
    public override BindableProperty<float> Gravity { get; set; } = new(980f);
    public override BindableProperty<float> RunSpeed { get; set; } = new(200f);
    public override BindableProperty<float> JumpVelocity { get; set; } = new(-300f);
    public override BindableProperty<float> FloorAcceleration { get; set; } = new(1000f);
    public override BindableProperty<float> AirAcceleration { get; set; } = new(200f * 5);
}