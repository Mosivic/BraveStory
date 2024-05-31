namespace BraveStory.State;

public class PlayerState(PlayerParams param) : GPC.State.State
{
    public PlayerParams Params{get;set;} = param;
    public float Gravity { get; } = 980;
    public float RunSpeed { get; } = 200;
    public float JumpVeocity { get; } = -300;
    public float FloorAcceleration { get; } = 200 * 5;
    public float AirAcceleration { get; } = 200 * 50;
}