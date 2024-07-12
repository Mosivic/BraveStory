using FSM;
using FSM.Job;
using FSM.States;
using Godot;

namespace BraveStory.State;

public abstract class CharacterState<THost,TProperty> : AbsState
{
    public abstract THost Host { get; set; }
    public abstract TProperty Properties { get; set; }

}