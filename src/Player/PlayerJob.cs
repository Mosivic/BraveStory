
using FSM.Job;
using FSM.States;

namespace BraveStory.Player;

internal class PlayerJob(HostState<Player> state) : JobBase(state)
{
}