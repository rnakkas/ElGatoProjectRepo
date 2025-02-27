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

	[Export] private Label _debugHealthLabel;
	[Export] private Label _debugScoreLabel;

	private bool _isDashing, _hurtStatus;
	private Vector2 _velocity, _direction = Vector2.Zero;
	
	private int _score;

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

	private void OnScoreItemPickedUp(int scoreAmount)
	{
		_score += scoreAmount;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerController.UpdateState((float)delta);
		_velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		_debugHealthLabel.SetText("HP: " + _healthComponent.CurrentHealth);
		_debugScoreLabel.SetText("SCORE: " + _score);
	}
}
