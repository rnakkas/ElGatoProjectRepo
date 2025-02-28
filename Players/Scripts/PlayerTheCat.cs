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
	
	private int _score;

	public override void _Ready()
	{
		_velocity = Velocity;

		EmitSignals();
		ConnectSignals();
	}

	private void EmitSignals()
	{
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerMaxHealthUpdate), _healthComponent.MaxHealth);
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), _healthComponent.CurrentHealth);
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerScoreUpdate), Globals.Instance.PlayerScore);
	}

	private void ConnectSignals()
	{
		if (_hurtboxComponent == null)
			return;
		_hurtboxComponent.GotHit += OnHitByAttack;
		_hurtboxComponent.HurtStatusCleared += OnHurtStatusCleared; 
		
		if (_healthComponent == null)
			return;
		_healthComponent.HealthChanged += OnPlayerHealthChanged;

		if (_pickupsComponent == null)
			return;
		_pickupsComponent.PickedUpScoreItem += OnScoreItemPickedUp;
	}

	private void OnHitByAttack()
	{
		_playerController.SetState(Utility.CharacterState.Hurt);
	}

	private void OnHurtStatusCleared()
	{
		_playerController.SetState(Utility.CharacterState.Idle);
	}

	// To display health on HUD
	private void OnPlayerHealthChanged()
	{
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), _healthComponent.CurrentHealth);
	}

	// To display score on HUD
	private void OnScoreItemPickedUp()
	{
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerScoreUpdate), Globals.Instance.PlayerScore);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerController.UpdateState((float)delta);
		_velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}

}
