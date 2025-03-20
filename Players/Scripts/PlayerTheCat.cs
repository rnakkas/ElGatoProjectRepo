using Godot;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerTheCat : CharacterBody2D
{
	// [Export] private PlayerControllerComponent _playerController;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private HealthComponent _healthComponent;
	
	private Vector2 _velocity = Vector2.Zero;
	private Timer _dashCooldownTimer, _dashTimer, _gameOverTimer, _invincibilityTimer, _blinkTimer;
	private bool _isDashing, _onDashCooldown;

	public override void _Ready()
	{
		_velocity = Velocity;
		ConnectSignals();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// _playerController?.UpdateState((float)delta);
		if (_velocityComponent != null) _velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}
	
	// Signals and connections methods
	private void ConnectSignals()
	{
		if (_dashCooldownTimer != null) _dashCooldownTimer.Timeout += OnDashCooldownTimerTimeout;
		
		if (_dashTimer != null) _dashTimer.Timeout += OnDashTimerTimeout;
	
		if (_gameOverTimer != null) _gameOverTimer.Timeout += OnGameOverTimerTimeout;
	
		if (_invincibilityTimer != null) _invincibilityTimer.Timeout += OnInvincibilityTimerTimedOut;
	
		if (_blinkTimer != null) _blinkTimer.Timeout += OnBlinkTimerTimedOut;
		
		if (_hurtboxComponent != null)
		{
			_hurtboxComponent.GotHit += OnHitByAttack;
			_hurtboxComponent.HurtStatusCleared += OnHurtStatusCleared; 
		}
		
		if (_healthComponent != null) _healthComponent.HealthDepleted += OnHealthDepleted;
	}
	
	private void OnDashCooldownTimerTimeout()
	{
		_onDashCooldown = false;
	}
	
	private void OnDashTimerTimeout()
	{
		_isDashing = false;
	}
	
	private static void OnGameOverTimerTimeout()
	{
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerDied));
	}
	
	private void OnInvincibilityTimerTimedOut()
	{
		_blinkTimer?.Stop();
		Visible = true;
		
		// if (_currentState != Utility.EntityState.Death)
		// {
		// 	_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, true);
		// }
	}
	
	private void OnBlinkTimerTimedOut()
	{
		Visible = !Visible; // Toggle visibility when blinking
	}
	
	private void OnHitByAttack()
	{
		_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, false);
		_invincibilityTimer?.Start();
		_blinkTimer?.Start();
		
		// SetState(Utility.EntityState.Hurt);
	}
	
	private void OnHurtStatusCleared()
	{
		if (_healthComponent is { CurrentHealth: > 0 } )
			GD.Print("idle");
			// SetState(Utility.EntityState.Idle);
	}
	
	private void OnHealthDepleted()
	{
		// SetState(Utility.EntityState.Death);
	}
}
