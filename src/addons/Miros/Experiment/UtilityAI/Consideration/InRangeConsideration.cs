namespace Miros.Experiment.UtilityAI;


//TODO：需完善
public class InRangeConsideration(float min, float max) : Consideration
{
    public float min = min;
    public float max = max;

    public override float Evaluate(Context context)
    {
        var value = context.Get<float>("value");
        if (value > max || value < min)
        {
            return 0f;
        }
        return 1f;
    }
}
