
namespace Miros.Experiment.UtilityAI;

public abstract class ActionBase
{
    public string targetTag;
    public Consideration consideration;

    public virtual void Init(Context context){}

    public float CalculateUtility(Context context)
    {
        return consideration.Evaluate(context);
    }

    public abstract void Execute(Context context);
}
