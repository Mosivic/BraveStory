using Godot;

namespace Miros.Experiment.UtilityAI;

//TODO：曲线需完善
public class CurveConsideration(Curve curve) : Consideration
{
    public Curve curve = curve;

    public override float Evaluate(Context context)
    {
        return curve.Sample(context.Get<float>("value"));
    }
}