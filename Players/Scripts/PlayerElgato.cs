using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	[Export] private PlayerStats _playerStats;
	[Export] private PlayerControllerComponent _playerController;
	[Export] private VelocityComponent _velocityComponent;

	private Vector2 _velocity = Vector2.Zero;
	
	public override void _Ready()
	{
		_velocity = Velocity;

		_playerController.MovementInputDetected += PlayerMovementStarted;
		_playerController.MovementInputNotDetected += PlayerMovementStopped;
		_playerController.JumpInputDetected += PlayerJumped;
	}

	private void PlayerMovementStarted(Vector2 direction)
	{
		_velocity.X = _velocityComponent.AccelerateToMaxSpeed(direction, _playerStats.MaxSpeed, _playerStats.Acceleration);
	}

	private void PlayerMovementStopped()
	{
		_velocity.X = _velocityComponent.SlowdownToZeroSpeed(_playerStats.Friction);
	}

	private void PlayerJumped()
	{
		_velocity.Y = _velocityComponent.JumpVelocity(_playerStats.JumpVelocity);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// Add the gravity.
		if (!IsOnFloor())
		{
			_velocity.Y += _playerStats.Gravity * (float)delta;
		}

		Velocity = _velocity;
		MoveAndSlide();
	}
}
