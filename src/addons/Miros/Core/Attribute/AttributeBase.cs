using System;
using System.Collections.Generic;
using System.Linq;

namespace Miros.Core
{
    public class AttributeBase
    {
        public readonly string Name;
        public readonly string SetName;
        public readonly string ShortName;
        
        protected event Action<AttributeBase, float> OnPreCurrentValueChange; // 当前值变化前的事件
        protected event Action<AttributeBase, float, float> OnPostCurrentValueChange; // 当前值变化后的事件
        protected event Func<AttributeBase, float, float> OnPreBaseValueChange; // 基础值变化前的事件
        protected event Action<AttributeBase, float, float> OnPostBaseValueChange; // 基础值变化后的事件
        protected IEnumerable<Func<AttributeBase, float, float>> PreBaseValueChangeListeners; // 基础值变化前的事件监听器

        private AttributeValue _value;

        public Persona Owner { get; private set; }

        public AttributeBase(string attrSetName, string attrName, float value = 0,
            CalculateMode calculateMode = CalculateMode.Stacking,
            SupportedOperation supportedOperation = SupportedOperation.All,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            SetName = attrSetName;
            Name = $"{attrSetName}.{attrName}";
            ShortName = attrName;
            _value = new AttributeValue(value, calculateMode, supportedOperation, minValue, maxValue);
        }

        public AttributeValue Value => _value;
        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;
        public float MinValue => _value.MinValue;
        public float MaxValue => _value.MaxValue;
        public CalculateMode CalculateMode => _value.CalculateMode;
        public SupportedOperation SupportedOperation => _value.SupportedOperation;

        public void SetOwner(Persona owner)
        {
            Owner = owner;
        }

        public void SetMinValue(float min)
        {
            _value.SetMinValue(min);
        }

        public void SetMaxValue(float max)
        {
            _value.SetMaxValue(max);
        }

        public void SetMinMaxValue(float min, float max)
        {
            _value.SetMinValue(min);
            _value.SetMaxValue(max);
        }

        public bool IsSupportOperation(ModifierOperation operation)
        {
            return _value.IsSupportOperation(operation);
        }

        public void SetCurrentValue(float value)
        {
            value = Math.Clamp(value, _value.MinValue, _value.MaxValue);
            OnPreCurrentValueChange?.Invoke(this, value);

            var oldValue = CurrentValue;
            _value.SetCurrentValue(value);

            if (Math.Abs(oldValue - value) > float.Epsilon)
                OnPostCurrentValueChange?.Invoke(this, oldValue, value);
        }

        public void SetBaseValue(float value)
        {
            if (OnPreBaseValueChange != null)
            {
                value = InvokePreBaseValueChangeListeners(value);
            }

            var oldValue = _value.BaseValue;
            _value.SetBaseValue(value);

            if (Math.Abs(oldValue - value) > float.Epsilon)
                OnPostBaseValueChange?.Invoke(this, oldValue, value);
        }

        public void SetCurrentValueWithoutEvent(float value)
        {
            _value.SetCurrentValue(value);
        }

        public void SetBaseValueWithoutEvent(float value)
        {
            _value.SetBaseValue(value);
        }

        public void RegisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            OnPreBaseValueChange += func;
            PreBaseValueChangeListeners = OnPreBaseValueChange?.GetInvocationList()
                .Cast<Func<AttributeBase, float, float>>();
        }

        public void RegisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostBaseValueChange += action;
        }

        public void RegisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            OnPreCurrentValueChange += action;
        }

        public void RegisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostCurrentValueChange += action;
        }

        public void UnregisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            OnPreBaseValueChange -= func;
            PreBaseValueChangeListeners = OnPreBaseValueChange?.GetInvocationList()
                .Cast<Func<AttributeBase, float, float>>();
        }

        public void UnregisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostBaseValueChange -= action;
        }

        public void UnregisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            OnPreCurrentValueChange -= action;
        }

        public void UnregisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostCurrentValueChange -= action;
        }

        public virtual void Dispose()
        {
            OnPreBaseValueChange = null;
            OnPostBaseValueChange = null;
            OnPreCurrentValueChange = null;
            OnPostCurrentValueChange = null;
        }

        private float InvokePreBaseValueChangeListeners(float value)
        {
            if (PreBaseValueChangeListeners == null) return value;

            return PreBaseValueChangeListeners.Aggregate(value, (current, t) => t.Invoke(this, current));
        }
    }
} 