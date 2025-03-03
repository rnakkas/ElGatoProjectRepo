using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerTheCat : CharacterBody2D
{
	[Export] private PlayerControllerComponent _playerController;
	[Export] private HealthComponent _healthComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private PickupsComponent _pickupsComponent;

	private bool _isDashing, _hurtStatus;
	private Vector2 _velocity, _direction = Vector2.Zero;

	public override void _Ready()
	{
		_velocity = Velocity;

		ConnectSignals();
	}

	private void ConnectSignals()
	{
		if (_hurtboxComponent == null)
			return;
		_hurtboxComponent.GotHit += OnHitByAttack;
		_hurtboxComponent.HurtStatusCleared += OnHurtStatusCleared; 
	}

	private void OnHitByAttack()
	{
		_playerController.SetState(Utility.CharacterState.Hurt);
	}

	private void OnHurtStatusCleared()
	{
		_playerController.SetState(Utility.CharacterState.Idle);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerController.UpdateState((float)delta);
		_velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}
}
