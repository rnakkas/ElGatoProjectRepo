using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Enemies.Scripts;

public partial class RangedEnemyLight : Node2D
{
	// Get components
	[Export] private HealthComponent _health;
	[Export] private HurtboxComponent _hurtbox;
	[Export] private PlayerDetectionComponent _playerDetection;
	[Export] private ShootingComponent _shooting;
	[Export] private AnimationComponent _animation;
	
	[Export] private Label _debugHealthLabel;

	private bool _hurtStatus, _canSeePlayer;
	
	public override void _Ready()
	{
		if (_hurtbox == null)
			return;
		_hurtbox.GotHit += OnHitByAttack;
		_hurtbox.HurtStatusCleared += OnHurtStatusCleared; 
		
		if (_health == null)
			return;
		_health.HealthDepleted += OnHealthDepleted;
		
		_debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
	}
	
	// Getting hit by attacks
	private void OnHitByAttack(
		bool hurtStatus, 
		Vector2 attackPosition, 
		int attackDamage,
		float knockback, 
		Vector2 attackVelocity
	)
	{
		if (_health == null)
			return;
		if (_animation == null)
			return;
		
		_hurtStatus = hurtStatus;
		
		_health.TakeDamage(attackDamage);
		
		_animation.FlipSpriteToFaceHitDirection(attackPosition);
	}
	
	private void OnHurtStatusCleared(bool hurtStatus)
	{
		_hurtStatus = hurtStatus;
	}

	// Dying
	private void OnHealthDepleted()
	{
		QueueFree();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_canSeePlayer = _playerDetection.PlayerDetectionBehaviour();
		
		_debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
	}
}
