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

	[Export] private Label _debugHealthLabel;

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

	private void OnHitByAttack(float knockbackVelocity)
	{
		_playerController.SetState(Utility.CharacterState.Hurt);
	}

	private void OnHurtStatusCleared(bool hurtStatus)
	{
		_playerController.SetState(Utility.CharacterState.Idle);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_velocity = _playerController.UpdateState((float)delta);
		Velocity = _velocity;
		
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		_debugHealthLabel.SetText("HP: " + _healthComponent.CurrentHealth);
	}
}
