using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.Enemies.Scripts;

public partial class InjectorScientist : CharacterBody2D
{
	[Export] private VelocityComponent _velocityComponent;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private HealthComponent _healthComponent;
	[Export] private StateMachine _stateMachine;
	[Export] private Timer _despawnTimer;
	[Export] private float _despawnTimeSeconds;
	
	private Vector2 _velocity = Vector2.Zero;
	
	public override void _Ready()
	{
		_velocity = Velocity;
		SetTimerValues();
		ConnectSignals();
	}
	
	private void SetTimerValues()
	{
		if (_despawnTimer == null) return;
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(_despawnTimeSeconds);
		_despawnTimer.Start();
	}
	
	/** Signals and connections */
	private void ConnectSignals()
	{
		if (_healthComponent != null) _healthComponent.HealthDepleted += OnHealthDepleted;
		if (_despawnTimer != null) _despawnTimer.Timeout += OnDespawnTimerTimedOut;
	}

	private void OnDespawnTimerTimedOut()
	{
		QueueFree(); // Despawn on timer timeout
	}
	
	private void OnHealthDepleted()
	{
		_stateMachine?.SetState(Utility.EntityState.DeathState.ToString());
	}
}