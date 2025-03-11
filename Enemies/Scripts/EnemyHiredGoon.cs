using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Enemies.Scripts;
public partial class EnemyHiredGoon : CharacterBody2D
{
	[Export] private EnemyControllerComponent _enemyController;
	[Export] private VelocityComponent _velocityComponent;
	
	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready()
	{
		_velocity = Velocity;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// _enemyController?.UpdateState((float)delta);
		_velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}
}
