using System;
using System.Collections.Generic;
using Godot;
using GPC.AI;
using GPC.AI.StateMachine;
using GPC.Job;
using GPC.Job.Config;
using GPC.Test;

namespace GPC
{
    public partial class GHub : Hub<GHub>
    {
        protected override void Build()
        {
            AddModule(new TestModule());
        }
    }
}

namespace GPC.Test
{

    public interface ITestModule : IModule
    {
        void say(string message);
    }

    public class TestModule : AbsModule, ITestModule
    {
        public void say(string message)
        {
            GD.Print(message);
        }

        public void TryInit()
        {
        }

        protected override void OnInit()
        {
        }
    }


    public partial class Fsm : Node2D
    {
        private ConditionMachine _conditionMachine;
        private StateMachine _stateMachine;

        public override void _Ready()
        {
            PredicateCondition hasMoveInput = new PredicateCondition((state => Input.IsActionJustPressed("move")));
            PredicateCondition noMoveInput = new PredicateCondition((state => !Input.IsActionJustPressed("move")));
            
            var saySomething = new State
            {
                Type = typeof(Say),
                Id = "2",
                Preconditions = new Dictionary<object, bool> { { hasMoveInput, true } },
                FailedConditions = new Dictionary<object, bool> { { noMoveInput, false } }
            };
            var root = new State()
            {
                Type = typeof(JobSingle),
                Id = "1",
            };
            
            _conditionMachine = new ConditionMachine([saySomething]);
            //_stateMachine = new StateMachine([saySomething,root]);
            //_stateMachine.AddTransition(root, saySomething, new PredicateCondition(
                //(root) => true));
            //_stateMachine.SetState(root);



            //saySomething.Hub.Module<TestModule>().say("我是王八蛋");


            //         float hp = 100;
            //         System.Threading.Timer timer = new System.Threading.Timer((object s) =>
            //{
            //	hp -= 10;
            //	GD.Print("1：" + hp.ToString());
            //}, ".",0,2000);
            //         //_Run();
            //         System.Threading.Timer timer1 = new System.Threading.Timer((object s) =>
            //         {
            //             hp += 10;
            //             GD.Print("2：" + hp.ToString());
            //         }, ".", 0, 2000);
            //         System.Threading.Timer timer2 = new System.Threading.Timer((object s) =>
            //         {
            //             hp += 2;
            //             GD.Print("3:"+hp.ToString());
            //         }, ".", 0, 2000);
        }

        public override void _Process(double delta)
        {
            _conditionMachine.Update(delta);
            //_stateMachine.Update(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            _conditionMachine.PhysicsUpdate(delta);
            //_stateMachine.PhysicsUpdate(delta);
        }
    }
}