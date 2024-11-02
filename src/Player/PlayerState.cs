using BraveStory.State;

namespace BraveStory.Player;

public class PlayerState(Player host, PlayerData properties) : CharacterState<Player, PlayerData>
{
    public override Player Host { get; set; } = host;
    public override PlayerData Properties { get; set; } = properties;
}