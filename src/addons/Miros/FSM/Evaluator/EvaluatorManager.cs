using FSM.Evaluator;
using Godot;
using System;
using System.Collections.Generic;

public class EvaluatorManager
{
    // 单例实现
    private static EvaluatorManager _instance;
    public static EvaluatorManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EvaluatorManager();
            }
            return _instance;
        }
    }

    // 存储所有评估器的字典
    private Dictionary<string, IEvaluator> _evaluators = new();
    
    // 用于类型安全的获取评估器
    private Dictionary<string, Type> _evaluatorTypes = new();

    // 创建并注册评估器
    public Evaluator<T> CreateEvaluator<T>(string key, Func<T> func) where T : IComparable
    {
        if (_evaluators.ContainsKey(key))
        {
            GD.PrintErr($"Evaluator with key {key} already exists!");
            return GetEvaluator<T>(key);
        }

        var evaluator = new Evaluator<T>(func);
        _evaluators[key] = evaluator;
        _evaluatorTypes[key] = typeof(T);
        return evaluator;
    }

    // 获取评估器
    public Evaluator<T> GetEvaluator<T>(string key) where T : IComparable
    {
        if (!_evaluators.ContainsKey(key))
        {
            GD.PrintErr($"Evaluator with key {key} not found!");
            return null;
        }

        if (_evaluatorTypes[key] != typeof(T))
        {
            GD.PrintErr($"Type mismatch for evaluator {key}. Expected {_evaluatorTypes[key]}, got {typeof(T)}");
            return null;
        }

        return (Evaluator<T>)_evaluators[key];
    }

    // 移除评估器
    public void RemoveEvaluator(string key)
    {
        if (_evaluators.ContainsKey(key))
        {
            _evaluators.Remove(key);
            _evaluatorTypes.Remove(key);
        }
    }

    // 清除所有评估器
    public void ClearEvaluators()
    {
        _evaluators.Clear();
        _evaluatorTypes.Clear();
    }

    // 检查评估器是否存在
    public bool HasEvaluator(string key)
    {
        return _evaluators.ContainsKey(key);
    }

    // 在EvaluatorManager类中添加
    public Dictionary<string, IEvaluator> GetAllEvaluators()
    {
        return new Dictionary<string, IEvaluator>(_evaluators);
    }
} 