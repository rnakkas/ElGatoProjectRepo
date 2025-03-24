using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Enemies.Scripts;
public partial class EnemyHiredGoon : CharacterBody2D
{
	[Export] private EnemyControllerComponent _enemyController;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private HealthComponent _healthComponent;
	[Export] private PlayerDetectionComponent _playerDetectionComponent;

	private ProgressBar _healthBar;
	
	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready()
	{
		_velocity = Velocity;
		
		_healthBar = GetNodeOrNull<ProgressBar>("healthBar");
		_healthBar?.SetMax(_healthComponent?.MaxHealth ?? 100);
		_healthBar?.SetVisible(false);

		if (_healthComponent != null)
		{
			_healthComponent.HealthDamaged += OnHealthDamaged;
			_healthComponent.HealthDepleted += OnHealthDepleted;
		}
		
	}
	
	private void OnHealthDamaged(int currentHealth)
	{
		_healthBar?.SetVisible(true);
		_healthBar?.SetValue(currentHealth);
	}

	private void OnHealthDepleted()
	{
		QueueFree();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// _enemyController?.UpdateState((float)delta);
		// if (_velocityComponent != null) _velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}
}
