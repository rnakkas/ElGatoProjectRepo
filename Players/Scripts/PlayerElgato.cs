using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using Godot.Collections;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	[Export] private PlayerStats _playerStats;
	[Export] private PlayerControllerComponent _playerController;
	[Export] private CharacterStatesComponent _playerStates;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private AnimationComponent _animationComponent;

	private Vector2 _velocity = Vector2.Zero;
	private Dictionary<string, bool> _playerInputs;
	
	public override void _Ready()
	{
		_velocity = Velocity;

		// _playerController.MovementInputDetected += PlayerMovementStarted;
		// _playerController.MovementInputNotDetected += PlayerMovementStopped;
		// _playerController.JumpInputDetected += PlayerJumped;
	}

	// private void PlayerMovementStarted(Vector2 direction)
	// {
	// 	_velocity.X = _velocityComponent.AccelerateToMaxSpeed(direction, _playerStats.MaxSpeed, _playerStats.Acceleration);
	// 	_animationComponent.FlipSprite(direction);
	//
	// 	if (IsOnFloor())
	// 	{
	// 		_velocity.Y = _velocityComponent.VerticalVelocityStoppedOnGround();
	// 		_animationComponent.PlayRunAnimation();
	// 	}
	// }
	//
	// private void PlayerMovementStopped()
	// {
	// 	_velocity.X = _velocityComponent.SlowdownToZeroSpeed(_playerStats.Friction);
	//
	// 	if (IsOnFloor())
	// 	{
	// 		_animationComponent.PlayIdleAnimation();
	// 		_velocity.Y = _velocityComponent.VerticalVelocityStoppedOnGround();
	// 	}
	// }
	//
	// private void PlayerJumped()
	// {
	// 	if (IsOnFloor())
	// 	{
	// 		_velocity.Y = _velocityComponent.JumpVelocity(_playerStats.JumpVelocity);
	// 		_animationComponent.PlayJumpAnimation();
	// 	}
	// }
	
	public override void _PhysicsProcess(double delta)
	{
		
		_playerInputs = _playerController.GetInputs();
		_velocity = _velocityComponent.CalculateVelocity(
			_playerInputs, 
			_playerStates.CurrentState, 
			(float)delta, 
			IsOnFloor()
		);
		_playerStates.UpdateState(_velocity);
		
		
		Velocity = _velocity;
		MoveAndSlide();
	}
}
