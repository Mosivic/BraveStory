using BraveStory.State;
using Godot;

namespace BraveStory.Player;

public class PlayerState(Player host, PlayerProperties properties) : CharacterState<Player, PlayerProperties>
{
    public override Player Host { get; set; } = host;
    public override PlayerProperties Properties { get; set; } = properties;
}