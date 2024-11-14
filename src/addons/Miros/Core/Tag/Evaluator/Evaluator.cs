using System;
using Godot;

namespace Miros.Core;

public class Evaluator
{
    protected readonly Func<bool> _func;
    private readonly Tag _tagToApply;
    private readonly TagContainer _targetTags;
    protected ulong _checksum;
    private bool _isTagApplied;
    protected string _name;
    protected bool _result;

    public Evaluator(
        string name,
        Func<bool> func,
        TagContainer targetTags,
        Tag tagToApply)
    {
        _name = name;
        _func = func;
        _targetTags = targetTags;
        _tagToApply = tagToApply;
    }

    protected void CalcFunc()
    {
        var frames = Engine.GetProcessFrames();
        if (_checksum == frames) return;

        _result = _func.Invoke();
    }


    public void Evaluate()
    {
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