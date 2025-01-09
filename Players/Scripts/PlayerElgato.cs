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
	[Export] private RayCast2D _leftWallDetect;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private HealthComponent _healthComponent;

	private Vector2 _velocity = Vector2.Zero;
	private Dictionary<string, bool> _playerInputs;
	private bool _hurtStatus;
	
	public override void _Ready()
	{
		_velocity = Velocity;

		_hurtboxComponent.DamagedByAttack += PlayerDamagedByAttack;
	}

	private void PlayerDamagedByAttack(int damage, float knockback, Vector2 velocity, float direction)
	{
		_healthComponent.TakeDamage(damage);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
		_playerInputs = _playerController.GetInputs();
		
		_velocity = _velocityComponent.CalculatePlayerVelocity(
			_playerInputs, 
			(float)delta, 
			IsOnFloor(), 
			IsOnWall(),
			_leftWallDetect,
			_rightWallDetect
			);
		
		_playerStates.UpdateState(_velocity, _leftWallDetect, _rightWallDetect, IsOnFloor(), _hurtStatus);
		
		_animationComponent.PlayAnimation(
			_playerStates.CurrentState, 
			_velocityComponent.Direction);
		
		Velocity = _velocity;
		MoveAndSlide();
	}
}
