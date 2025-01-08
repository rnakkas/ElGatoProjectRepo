using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class PlayerControllerComponent : Node2D
{
	private Vector2 _direction = Vector2.Zero;
	private Vector2 _velocity = Vector2.Zero;
	
	[Signal] public delegate void MovementInputDetectedEventHandler(Vector2 direction);
	[Signal] public delegate void MovementInputNotDetectedEventHandler();
	[Signal] public delegate void JumpInputDetectedEventHandler();
	
	public void Run()
	{
		_direction = Input.GetVector(
			"move_left", 
			"move_right", 
			"move_up", 
			"move_down");
	
		if (_direction.X != 0)
		{
			EmitSignal(SignalName.MovementInputDetected, _direction);
			// _velocity.X = _velocityComponent.AccelerateToMaxSpeed(_direction, _playerStats.MaxSpeed, _playerStats.Acceleration);
		}
	}

	public void Idle()
	{
		if (_direction.X == 0)
		{
			// _velocity.X = _velocityComponent.SlowdownToZeroSpeed(_playerStats.Friction);
			EmitSignal(SignalName.MovementInputNotDetected);
		}
	}

	public void Jump()
	{
		if (Input.IsActionPressed("jump"))
		{
			EmitSignal(SignalName.JumpInputDetected);
		}
	}

	public override void _Process(double delta)
	{
		Idle();
		Run();
		Jump();
	}
}
