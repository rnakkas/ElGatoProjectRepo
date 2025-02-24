using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerTheCat : CharacterBody2D
{
	[Export] private PlayerControllerComponent _playerController;
	[Export] private HealthComponent _healthComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private VelocityComponent _velocityComponent;

	private bool _isDashing, _hurtStatus;
	private Vector2 _velocity, _direction = Vector2.Zero;

	public override void _Ready()
	{
		_velocity = Velocity;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_velocity = _playerController.BasicMovements((float)delta);
		_velocity = _playerController.Dash();
		_velocity = _playerController.WallSlideAndWallJump((float)delta);
		Velocity = _velocity;
		
		MoveAndSlide();
	}
}
