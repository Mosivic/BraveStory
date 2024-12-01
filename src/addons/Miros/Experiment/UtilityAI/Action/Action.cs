namespace Miros.Experiment.UtilityAI;

public abstract class ActionBase
{
    public Consideration consideration;
    public string targetTag;

    public virtual void Init(Context context)
    {
    }

    public float CalculateUtility(Context context)
    {
        return consideration.Evaluate(context);
    }

    public abstract void Execute(Context context);
}