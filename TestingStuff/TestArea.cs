using Godot;
using System;

namespace ElGatoProject.TestingStuff;

[GlobalClass]
public partial class TestArea : Area2D
{
	public enum Type
	{
		HealthPickup,
		EnemyProjectile,
		EnemyAttack
	}

	[Export] public Type AreaType = Type.HealthPickup;
	[Export] public int HealAmount = 20;
	[Export] public int AttackDamage = 15;
	[Export] public float Knockback = 70.0f;
	[Export] public float MovementSpeed = 80.0f;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private RayCast2D _leftWallDetect;

	public float Direction;
	public Vector2 Velocity;
	
	public override void _Ready()
	{
		if (AreaType == Type.HealthPickup)
		{
			Direction = 0.0f;
			AddToGroup("Pickups");
		}
		else if (AreaType == Type.EnemyProjectile)
		{
			Direction = 1.0f;
			AddToGroup("EnemyProjectiles");
		}
		else if (AreaType == Type.EnemyAttack)
		{
			Direction = 0.0f;
			AddToGroup("EnemyAttacks");
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_rightWallDetect.IsColliding())
		{
			Direction = -1.0f;
		}

		if (_leftWallDetect.IsColliding())
		{
			Direction = 1.0f;
		}
		
		Velocity.X = (float)delta * MovementSpeed * Direction;

		MoveLocalX(Velocity.X, true);

	}
}
