using Godot;
using System;

namespace Miros.Core;

public class Evaluator
{
    protected string _name;
    protected readonly Func<bool> _func;
    protected ulong _checksum;
    protected bool _result = false;
    private readonly GameplayTagContainer _targetTags;
    private readonly GameplayTag _tagToApply;
    private bool _isTagApplied;

    public Evaluator(
        string name,
        Func<bool> func, 
        GameplayTagContainer targetTags, 
        GameplayTag tagToApply) 
    {
        _name = name;
        _func = func;
        _targetTags = targetTags;
        _tagToApply = tagToApply;
    }

    protected void CalcFunc()
    {
        var frames = Engine.GetProcessFrames();
        if(_checksum == frames) return;

        _result = _func.Invoke();
    }


    public void Evaluate(){
        CalcFunc();
        
        // 根据条件添加或移除标签
        if (_result && !_isTagApplied)
        {
            
#if DEBUG && false
            GD.Print($"[Evaluator] Add Tag: {_tagToApply}");
#endif
            _targetTags.AddTag(_tagToApply);
            _isTagApplied = true;

        }
        else if (!_result && _isTagApplied)
        {
#if DEBUG && false
            GD.Print($"[Evaluator] Remove Tag: {_tagToApply}");
#endif
            _targetTags.RemoveTag(_tagToApply);
            _isTagApplied = false;
        }
    }

} 