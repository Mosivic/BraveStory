using FSM;
using FSM.Evaluator;
using System;

public class TaggedEvaluator<T> : Evaluator<T> where T : IComparable
{
    private readonly GameplayTagContainer _targetTags;
    private readonly GameplayTag _tagToApply;
    private bool _isTagApplied;

    public TaggedEvaluator(
        Func<T> func, 
        GameplayTagContainer targetTags, 
        GameplayTag tagToApply) : base(func)
    {
        _targetTags = targetTags;
        _tagToApply = tagToApply;
    }

    public new bool Is(T expectValue, CompareType type = CompareType.Equals)
    {
        bool result = base.Is(expectValue, type);
        
        // 根据条件添加或移除标签
        if (result && !_isTagApplied)
        {
            _targetTags.AddTag(_tagToApply);
            _isTagApplied = true;
        }
        else if (!result && _isTagApplied)
        {
            _targetTags.RemoveTag(_tagToApply);
            _isTagApplied = false;
        }

        return result;
    }
} 