using FSM;

namespace BraveStory.Player;

public abstract class CharacterProperties
{
    public abstract BindableProperty<float> Gravity { get; set; }
    public abstract BindableProperty<float> RunSpeed { get; set; }
    public abstract BindableProperty<float> JumpVelocity { get; set; }
    public abstract BindableProperty<float> FloorAcceleration { get; set; }
    public abstract BindableProperty<float> AirAcceleration { get; set; }
}