﻿// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Godot;

// namespace Miros.Core;

// public class GoalOrientedJPlan : AbsExecutor
// {
//     private readonly List<AbsState> _goalQueue;
//     private readonly List<AbsState> _goals;
//     private readonly Planner _planner;

//     public GoalOrientedJPlan(List<AbsState> states, List<AbsState> goals) : base(states)
//     {
//         _goals = goals;
//         _planner = new Planner(TaskWrapper);
//         _goalQueue = new List<AbsState>();
//     }


//     public override void Update(double delta)
//     {
//         _UpdateValidGoalCfgQueue();
//         _RemoveInvalidGoalCfgQueue();

//         if (_goalQueue.Count > 0) TaskWrapper.Update(_goalQueue[-1], delta);
//     }

//     public override void PhysicsUpdate(double delta)
//     {
//         if (_goalQueue.Count > 0) TaskWrapper.PhysicsUpdate(_goalQueue[-1], delta);
//     }

//     private void _UpdateValidGoalCfgQueue()
//     {
//         var bestGoalCfg = _planner.GetBestGoalCfg(_goals);
//         if (bestGoalCfg == null)
//         {
//             GD.Print("GPC.AI.OldGOAP.__UpdateValidGoalCfgQueue(): Can't find cound used cfg.");
//             return;
//         }

//         // 队列为空 -> 规划最佳目标方案加入队列
//         if (_goalQueue.Count == 0)
//         {
//             var bestPlan = _planner.GetBestGoalPlan(bestGoalCfg, States);
//             if (bestPlan == null || bestPlan.TasksCfg.Count == 0)
//             {
//                 _planner.AddToBlackList(bestGoalCfg);
//                 return;
//             }

//             _goalQueue.Add(bestGoalCfg);

//             bestGoalCfg.Subtasks = bestPlan.TasksCfg;
//             TaskWrapper.Enter(bestGoalCfg);
//         }
//         // 队列非空 & 最佳目标已存在队列中 -> if 队列顶端已是当前最佳目标 return else 暂停队列顶端目标,将最佳目标设为队列顶端并恢复
//         else if (_goalQueue.Contains(bestGoalCfg))
//         {
//             if (_goalQueue[-1] == bestGoalCfg) return;

//             var lastGoalCfg = _goalQueue.Last();
//             TaskWrapper.Pause(lastGoalCfg);
//             _goalQueue.Remove(lastGoalCfg);

//             _goalQueue.Add(bestGoalCfg);
//             TaskWrapper.Resume(bestGoalCfg);
//         }
//         // 队列非空 & 最佳目标不存在队列中 -> 暂停队列顶端目标, 规划最佳目标方案加入队列顶端
//         else
//         {
//             var bestPlan = _planner.GetBestGoalPlan(bestGoalCfg, States);
//             if (bestPlan == null || bestPlan.TasksCfg.Count == 0)
//             {
//                 _planner.AddToBlackList(bestGoalCfg);
//                 return;
//             }

//             var lastGoalCfg = _goalQueue.Last();
//             TaskWrapper.Pause(lastGoalCfg);
//             _goalQueue.Add(lastGoalCfg);

//             bestGoalCfg.Subtasks = bestPlan.TasksCfg;
//             TaskWrapper.Enter(bestGoalCfg);
//         }
//     }

//     private void _RemoveInvalidGoalCfgQueue()
//     {
//         if (_goalQueue.Count == 0) return;

//         foreach (var cfg in _goalQueue)
//         {
//             var state = cfg.Status;

//             if (state == Status.Successed || state == Status.Failed)
//             {
//                 TaskWrapper.Exit(cfg);
//                 _goalQueue.Remove(cfg);
//             }
//             else if (state == Status.Pause && !TaskWrapper.CanExecute(cfg))
//             {
//                 TaskWrapper.Resume(cfg);
//                 TaskWrapper.Exit(cfg);
//                 _goalQueue.Remove(cfg);
//             }
//             else if (state == Status.Running && !TaskWrapper.CanExecute(cfg))
//             {
//                 TaskWrapper.Exit(cfg);
//                 _goalQueue.Remove(cfg);
//             }
//         }
//     }
// }

// internal class Plan
// {
//     public List<State> TasksCfg { get; set; }
//     public int Cost { get; set; }
// }

// internal class PlanNode
// {
//     public State Cfg { get; set; }
//     public Dictionary<object, object> Desired { get; set; }
//     public List<PlanNode> Children { get; set; }
// }

// internal class Planner
// {
//     private readonly List<object> _CfgBlackList;
//     private readonly TaskWrapper _taskTaskWrapper;

//     public Planner(TaskWrapper taskWrapper)
//     {
//         _taskTaskWrapper = taskWrapper;
//         _CfgBlackList = new List<object>();
//     }

//     public void AddToBlackList(State cfg)
//     {
//         _CfgBlackList.Add(cfg);
//     }

//     public Goal GetBestGoalCfg(List<Goal> goals)
//     {
//         Goal bestCfg = null;

//         foreach (var goal in goals)
//         {
//             if (_CfgBlackList.Contains(goal)) continue;

//             if (_taskTaskWrapper.CanExecute(goal))
//             {
//                 if (bestCfg == null) bestCfg = goal;
//                 else if (goal.Priority > bestCfg.Priority) bestCfg = goal;
//             }
//         }

//         return bestCfg;
//     }


//     public Plan GetBestGoalPlan(State goalCfg, List<State> tasksCfg)
//     {
//         var _DesiredTemp = goalCfg.Desired;
//         if (_DesiredTemp.Count == 0) return null;

//         var root = new PlanNode
//         {
//             Cfg = goalCfg,
//             Desired = _DesiredTemp,
//             Children = new List<PlanNode>()
//         };


//         if (_BuildPlans(root, tasksCfg)) return _GetCheapestPlanInPlans(_TransformTreeIntoArray(root));

//         return null;
//     }

//     private bool _BuildPlans(PlanNode node, List<State> tasksCfg) //Need Fix
//     {
//         var hasFollup = false;
//         var desired = new Dictionary<object, object>(node.Desired);

//         foreach (var key in desired.Keys)
//             if (key is string) desired.Remove(key);
//             else if (key is Delegate)
//                 if ((key as Delegate).DynamicInvoke(node.Cfg) == desired[key])
//                     desired.Remove(key);

//         if (desired.Count == 0) return true;

//         foreach (var cfg in tasksCfg)
//         {
//             var shouldUseAction = false;
//             var effect = cfg.SuccessedEffects;
//             var desiredCopy = new Dictionary<object, object>(desired);

//             foreach (var key in desiredCopy.Keys)
//                 if (effect.ContainsKey(key))
//                 {
//                 }

//             if (shouldUseAction)
//             {
//             }
//         }

//         return hasFollup;
//     }


//     private List<Plan> _TransformTreeIntoArray(PlanNode node)
//     {
//         var _Plans = new List<Plan>();

//         if (node.Children.Count == 0)
//         {
//             var plan = new Plan
//             {
//                 TasksCfg = new List<State> { node.Cfg },
//                 Cost = node.Cfg.Cost
//             };
//             _Plans.Add(plan);

//             return _Plans;
//         }

//         foreach (var childNode in node.Children)
//         foreach (var childPlan in _TransformTreeIntoArray(childNode))
//         {
//             childPlan.TasksCfg.Add(node.Cfg);
//             childPlan.Cost += node.Cfg.Cost;
//             _Plans.Add(childPlan);
//         }

//         return _Plans;
//     }

//     private Plan _GetCheapestPlanInPlans(List<Plan> plans)
//     {
//         //_CheapestPlan["States"].pop_buck
//         return plans[plans.Min(x => x.Cost)];
//     }
// }

