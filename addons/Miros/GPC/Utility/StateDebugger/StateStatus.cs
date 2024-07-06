using Godot;
using System;
using GPC;
using GPC.States;

public partial class StateStatus : HBoxContainer
{
	private AbsState State;
	private ColorRect _isActiveRect;
	private Label _nameLabel;
	private Label _timeLabel;

	private Color _ActiveColor = Colors.Green;
	private Color _WaitColor = Colors.Yellow;
	
	private double _elapsedTime = 0.0f;
	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("NameLabel");
		_timeLabel = GetNode<Label>("TimeLabel");
		_isActiveRect = GetNode<ColorRect>("IsActive/ColorRect");
	}

	public override void _Process(double delta)
	{
		if (State.Status == Status.Running)
		{
			_elapsedTime += delta;
			_timeLabel.SetText(_elapsedTime.ToString("F2"));		
		}
	}

	public void Init(AbsState state)
	{
		State = state;
		_timeLabel.SetText("0.00");	
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		_nameLabel.SetText(State.Name);
		
		if (State.Status == Status.Running)
		{
			_isActiveRect.SetColor(_ActiveColor);
		}
		else
		{
			_elapsedTime = 0.0f;
			_timeLabel.SetText(_elapsedTime.ToString("F2"));	
			_isActiveRect.SetColor(_WaitColor);
		}
		
	}

	public void OnStateChanged(AbsState state)
	{
		if (state.Equals(State))
		{
			UpdateDisplay();
		}
	}
}
