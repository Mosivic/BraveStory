using System.Collections.Generic;
using Godot;
using GPC;
using GPC.State;

public class Condition(Dictionary<Evaluator, bool> evaluators)
{
    public bool IsAllSatisfy(IState state)
    {
        var frames = Engine.GetProcessFrames();
        foreach (var key in evaluators.Keys)
        {
            if (key.Checksum.Equals(frames) && key.Result != evaluators[key]) return false;

            if (key.Evaluate(state) != evaluators[key])
            {
                key.Checksum = frames;
                return false;
            }
        }

        return true;
    }

    public bool IsAnySatisfy(IState state)
    {
        var frames = Engine.GetProcessFrames();
        foreach (var key in evaluators.Keys)
        {
            if (key.Checksum.Equals(frames) && key.Result == evaluators[key]) return true;

            if (key.Evaluate(state) == evaluators[key])
            {
                key.Checksum = frames;
                return true;
            }
        }

        return false;
    }
}