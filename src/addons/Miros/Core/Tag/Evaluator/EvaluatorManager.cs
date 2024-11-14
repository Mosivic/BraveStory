using System;
using System.Collections.Generic;
using Godot;

namespace Miros.Core;

public class EvaluatorManager
{
    // 单例实现
    private static EvaluatorManager _instance;

    // 存储所有评估器的字典
    private readonly Dictionary<string, Evaluator> _evaluators = new();

    public static EvaluatorManager Instance
    {
        get
        {
            if (_instance == null) _instance = new EvaluatorManager();
            return _instance;
        }
    }


    // 获取评估器
    public Evaluator GetEvaluator(string key)
    {
        if (!_evaluators.ContainsKey(key))
        {
            GD.PrintErr($"Evaluator with key {key} not found!");
            return null;
        }

        return _evaluators[key];
    }

    // 移除评估器
    public void RemoveEvaluator(string key)
    {
        if (_evaluators.ContainsKey(key)) _evaluators.Remove(key);
    }

    // 清除所有评估器
    public void ClearEvaluators()
    {
        _evaluators.Clear();
    }

    // 检查评估器是否存在
    public bool HasEvaluator(string key)
    {
        return _evaluators.ContainsKey(key);
    }


    // 创建带标签的评估器
    public Evaluator CreateEvaluator(
        string key,
        Func<bool> func,
        TagContainer targetTags,
        Tag tagToApply)
    {
        if (_evaluators.ContainsKey(key))
        {
#if DEBUG && true
            GD.PrintErr($"Evaluator with key {key} already exists!");
#endif
            return GetEvaluator(key);
        }

        var evaluator = new Evaluator(key, func, targetTags, tagToApply);
        _evaluators[key] = evaluator;
        return evaluator;
    }

    // 每帧执行所有评估器
    public void ProcessEvaluators()
    {
        foreach (var evaluator in _evaluators.Values) evaluator.Evaluate();
    }
}