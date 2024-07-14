using BraveStory.Player;
using FSM.Job;

namespace BraveStory.Job;

internal class PlayerJob(PlayerState state) : JobBase(state)
{
}