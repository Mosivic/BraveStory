using System;
using System.Collections.Generic;

namespace Miros.Core;

public interface IPersona
{
    void AddState<TState>(TState state) where TState : State;

    void AddStateTo(State state, IPersona target);

    void RemoveState<TState>(TState state) where TState : State;

    void Update(double delta);

    void PhysicsUpdate(double delta);

    JobBase[] GetAllJobs();

    bool HasTag(Tag tag);

    bool HasAllTags(TagSet tags);

    bool HasAnyTags(TagSet tags);

    void AddFixedTags(TagSet tags);

    void AddFixedTag(Tag tag);

    void RemoveFixedTags(TagSet tags);

    void RemoveFixedTag(Tag tag);
    
    void ApplyModFromInstantEffect(Effect effect);

    Dictionary<string, float> DataSnapshot();

    float? GetAttributeCurrentValue(string setName, string attributeShortName);

    float? GetAttributeBaseValue(string setName, string attributeShortName);

    T AttrSet<T>() where T : AttributeSet;

}   

