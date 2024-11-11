using System;

namespace Miros.Core;
// 对自定义回调函数的处理

public abstract class AbsJob
{

    public event Action OnEnter;
    public event Action OnExit;
    public event Action<double> OnUpdate;
    public event Action<double> OnPhysicsUpdate;
    public event Action OnSucceed;
    public event Action OnFailed;
    public event Action OnPause;
    public event Action OnResume;


    protected NativeJob State { get; }

    protected AbsJob(NativeJob state)
    {
        State = state;
        RegisterHandlers();
    }

    protected virtual void RegisterHandlers()
    {
        State.RegisterHandler(new StandardDelegateHandler<NativeJob>(State));
    }

}