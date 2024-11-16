// namespace Miros.Core;

// internal class TaskAll(CompoundState state) : AbsTaskBase(state)
// {


//     public override void Enter()
//     {
//         foreach (var childCfg in state.SubTasks)
//             _provider.Executor.Enter(childCfg);
//         _Enter();
//     }


//     public bool IsSucceed()
//     {
//         foreach (var childCfg in state.SubTasks)
//         {
//             if (childCfg.Status != TaskRunningStatus.Succeed) return false;
//             _provider.Executor.Enter(childCfg);
//         }

//         return true;
//     }

//     public bool IsFailed()
//     {
//         foreach (var childCfg in state.SubTasks)
//             if (childCfg.Status == TaskRunningStatus.Failed)
//                 return true;
//         return false;
//     }


//     public override void Update(double delta)
//     {
//         foreach (var childCfg in state.SubTasks)
//             _provider.Executor.Update(childCfg, delta);
//     }

//     public override void PhysicsUpdate(double delta)
//     {
//         foreach (var childCfg in state.SubTasks)
//             _provider.Executor.PhysicsUpdate(childCfg, delta);
//     }
// }

