using Godot;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;
using ElGatoProject.StateSystem;
using ElGatoProject.Utilties;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerTheCat : CharacterBody2D
{
	[Export] private VelocityComponent _velocityComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private HealthComponent _healthComponent;
	[Export] private StateMachine _stateMachine;
	
	private Vector2 _velocity = Vector2.Zero;
	private Timer _gameOverTimer, _invincibilityTimer, _blinkTimer;

	public override void _Ready()
	{
		_velocity = Velocity;
		GetChildNodes();
		ConnectSignals();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_velocityComponent != null) _velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		MoveAndSlide();
	}
	
	/** Get child nodes */
	private void GetChildNodes()
	{
		_invincibilityTimer = GetNodeOrNull<Timer>("Timers/InvincibilityTimer");
		_blinkTimer = GetNodeOrNull<Timer>("Timers/BlinkTimer");
		_gameOverTimer = GetNodeOrNull<Timer>("Timers/GameOverTimer");
	}
	
	/** Signals and connections */
	private void ConnectSignals()
	{
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
	
	private static void OnGameOverTimerTimeout()
	{
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerDied));
	}
	
	private void OnInvincibilityTimerTimedOut()
	{
		_blinkTimer?.Stop();
		Visible = true;

		if (_healthComponent is { CurrentHealth: > 0 })
		{
			_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, true);
		}
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
		
		_stateMachine?.SetState(Utility.EntityState.HurtState.ToString());
	}
	
	private void OnHurtStatusCleared()
	{
		if (_healthComponent is { CurrentHealth: > 0 } )
			_stateMachine?.SetState(Utility.EntityState.IdleState.ToString());
	}
	
	private void OnHealthDepleted()
	{
		_stateMachine?.SetState(Utility.EntityState.DeathState.ToString());
	}
}
