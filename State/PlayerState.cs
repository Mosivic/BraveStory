using BraveStory.Player;
using Godot;
using GPC.States;

namespace BraveStory.State;

public class PlayerState(CharacterBody2D host, PlayerNodes node, PlayerProperties properties) : CompoundState
{
    public PlayerNodes Nodes { get; set; } = node;
    public PlayerProperties Properties { get; set; } = properties;
    public CharacterBody2D Host { get; set; } = host;
}