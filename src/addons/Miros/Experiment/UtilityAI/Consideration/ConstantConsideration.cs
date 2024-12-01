namespace Miros.Experiment.UtilityAI;

public class ConstantConsideration(float value) : Consideration
{
    public float value = value;

    public override float Evaluate(Context context)
    {
        return value;
    }
}