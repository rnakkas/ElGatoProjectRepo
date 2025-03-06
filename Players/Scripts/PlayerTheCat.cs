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
	
	// Will rework this soon to use collider instead
	private Area2D _targetBox;
	
	//TODO: Use this after reworking enemy's player detection logic
	private CollisionShape2D _collider;

	public override void _Ready()
	{
		_velocity = Velocity;

		ConnectSignals();
		
		// Will rework this soon to use collider instead
		_targetBox = GetNode<Area2D>("TargetBox");
		
		//TODO: Use this after reworking enemy's player detection logic
		_collider = GetNode<CollisionShape2D>("collider");
	}

	private void ConnectSignals()
	{
		if (_hurtboxComponent == null)
			return;
		_hurtboxComponent.GotHit += OnHitByAttack;
		_hurtboxComponent.HurtStatusCleared += OnHurtStatusCleared; 
		
		if (_healthComponent == null)
			return;
		_healthComponent.HealthDepleted += OnHealthDepleted;
	}

	private void OnHitByAttack()
	{
		_playerController.SetState(Utility.CharacterState.Hurt);
	}

	private void OnHurtStatusCleared()
	{
		if (_healthComponent is { CurrentHealth: > 0 } )
			_playerController.SetState(Utility.CharacterState.Idle);
	}

	private void OnHealthDepleted()
	{
		_playerController.SetState(Utility.CharacterState.Death);
		
		// Will rework this soon to use collider instead
		_targetBox.SetDeferred(Area2D.PropertyName.Monitorable, false);
		
		//TODO: Use this after reworking enemy's player detection logic
		_collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerController.UpdateState((float)delta);
		_velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}
}
