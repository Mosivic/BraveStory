using System;
using System.Collections.Generic;
using Godot;

namespace GPC;

public interface IHubProvider
{
    public IHub Hub { get; }
}

public interface IModule
{
    void TryInit();
}

public interface IUtility
{
}

public interface ILocal{

}

public interface IGlobal{
    
}

public interface ICanInit : IModule
{
    bool Preload { get; }
    void PreInit(IHub hub);
    void DeInit();
}

public abstract class AbsModule : ICanInit, IHubProvider
{
    protected bool Inited;

    void ICanInit.PreInit(IHub hub)
    {
        Hub = hub;
        if (!Preload) return;
        OnInit();
        Inited = true;
    }

    void ICanInit.DeInit()
    {
        if (!Inited) return;
        OnDeInit();
        Inited = false;
    }

    void IModule.TryInit()
    {
        if (Inited) return;
        OnInit();
        Inited = true;
    }

    public virtual bool Preload => false;

    public IHub Hub { get; private set; }

    protected abstract void OnInit();

    protected virtual void OnDeInit()
    {
    }
}

public interface IHub
{
    M Module<M>() where M : class, IModule;
    U Utility<U>() where U : class, IUtility;
}

public abstract class Hub<H> : IHub where H : Hub<H>, new()
{
    private static H _hub;
    private readonly Dictionary<Type, IModule> _modules = new();
    private readonly Dictionary<Type, IUtility> _utilities = new();

    M IHub.Module<M>()
    {
        if (_modules.TryGetValue(typeof(M), out var module))
        {
            module.TryInit();
            return module as M;
        }
#if DEBUG
        GD.Print($"{typeof(M)}模块未被注册.");
#endif
        return null;
    }

    U IHub.Utility<U>()
    {
        if (_utilities.TryGetValue(typeof(U), out var utility)) return utility as U;

#if DEBUG
        GD.Print($"{typeof(U)}工具未被注册.");
#endif
        return null;
    }

    public static IHub GetIns()
    {
        if (_hub == null)
        {
            _hub = new H();
            _hub.Build();
        }

        return _hub;
    }

    protected abstract void Build();

    protected void AddModule<M>(M module) where M : IModule
    {
        if (_modules.TryAdd(typeof(M), module))
            (module as ICanInit).PreInit(this);
    }

    protected void AddUtility<U>(U utility) where U : IUtility
    {
        _utilities[typeof(U)] = utility;
    }

    protected void DeInit()
    {
        if (_modules.Count > 0)
        {
            foreach (var module in _modules.Values) (module as ICanInit).DeInit();
            _modules.Clear();
        }

        _utilities.Clear();
    }
}

public class GHub : Hub<GHub>
{
    protected override void Build()
    {
        throw new NotImplementedException();
    }
}