using Godot;

namespace Miros.Core;

public class EffectPeriodTicker
{
	private readonly Effect _effect;
	private double _durationRemaining;
	private double _periodRemaining;

	public EffectPeriodTicker(Effect effect)
	{
		_effect = effect;
		_durationRemaining = effect.Duration;
		_periodRemaining = _effect.Period;
	}
	
	/// <summary>
	///     注意: Period 小于 0.01f 可能出现误差, 基本够用了
	/// </summary>
	private void UpdatePeriod(double delta)
	{
		// 前提: Period不会动态修改
		if (_effect.Period <= 0) return;

		if (_periodRemaining < Mathf.Epsilon)
			// 第一次执行
			return;

		_periodRemaining -= delta;

		while (_periodRemaining < 0)
		{
			// 不能直接将_periodRemaining置为0, 这将累计误差
			_periodRemaining += _effect.Period;
			//_effect.PeriodExecution?.TriggerOnExecute();
			_effect.Owner.ApplyModFromInstantEffect(_effect);
		}
	}

	private void UpdateDuration(double delta)
	{
		if (_effect.DurationPolicy == DurationPolicy.Infinite)
			return;

		_durationRemaining -= delta;

		// 当 Effect 的 DurationPolicy 为 Duration 时，且 Duration 持续时间结束时
		if (_effect.DurationPolicy == DurationPolicy.Duration && _effect.Stacking!=null && _durationRemaining <= 0)
		{
			// 处理STACKING
			if (_effect.Stacking.StackingType == StackingType.None)
			{
				_effect.Status = RunningStatus.Succeed;
			}
			else
			{
				if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.ClearEntireStack)
				{
					_effect.Status = RunningStatus.Succeed;
				}
				else if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
				{
					if (_effect.Stacking.StackCount > 1)
						RefreshStack(_effect.Stacking.StackCount - 1);
					else
						_effect.Status = RunningStatus.Succeed;
				}
				else if (_effect.Stacking.ExpirationPolicy == ExpirationPolicy.RefreshDuration)
				{
					//持续时间结束时,再次刷新Duration，这相当于无限Duration，
					_durationRemaining = _effect.Duration;
				}
			}
		}
	}


	public void Tick(double delta)
	{
		//_effect.PeriodExecution?.TriggerOnTick();
		UpdateDuration(delta);
		UpdatePeriod(delta);
	}


	private void RefreshStack(int stackCount)
	{
		if (stackCount <= _effect.Stacking.LimitCount)
		{
			// 更新栈数
			_effect.Stacking.StackCount = Mathf.Max(1, stackCount); // 最小层数为1
			// 是否刷新Duration
			if (_effect.Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
				_durationRemaining = _effect.Duration;
			// 是否重置Period
			if (_effect.Stacking.PeriodResetPolicy == PeriodResetPolicy.ResetOnSuccessfulApplication)
				_periodRemaining = _effect.Period;
		}
		else
		{
			// 溢出GE生效
			foreach (var overflowEffect in _effect.Stacking.OverflowEffects)
				_effect.Owner.AddState(ExecutorType.EffectExecutor, overflowEffect);

			if (_effect.Stacking.DurationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
			{
				if (_effect.Stacking.DenyOverflowApplication)
				{
					//当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
					if (_effect.Stacking.ClearStackOnOverflow) _effect.Status = RunningStatus.Succeed;
				}
				else
				{
					_durationRemaining = _effect.Duration;
				}
			}
		}
	}
}
