using Godot;
using System;
using ElGatoProject.Players.Scripts;
using ElGatoProject.Resources;

namespace ElGatoProject.TestingStuff;

[GlobalClass]
public partial class TestArea : Area2D
{
	public enum Type
	{
		HealthPickup,
		EnemyProjectile,
		EnemyAttack,
		Enemy
	}

	[Export] public Type AreaType = Type.HealthPickup;
	[Export] public int HealAmount = 20;
	[Export] public int AttackDamage = 15;
	[Export] public float Knockback = 70.0f;
	[Export] public float MovementSpeed = 80.0f;
	[Export] public float EnemyHealth = 40.0f;
	[Export] public float HurtStaggerTime = 0.2f;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private RayCast2D _leftWallDetect;
	[Export] private Timer _hurtStaggerTimer;

	public float Direction;
	public Vector2 Velocity;
	private bool _hurtStatus;
	private Area2D _playerProjectile;
	
	public override void _Ready()
	{
		SetAreaProperties();

		if (_hurtStaggerTimer != null)
		{
			_hurtStaggerTimer.OneShot = true;
			_hurtStaggerTimer.SetWaitTime(HurtStaggerTime);
			_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;
		}
		
		AreaEntered += HitByPlayerBullets;
	}

	private void SetAreaProperties()
	{
		switch (AreaType)
		{
			case Type.HealthPickup:
				Direction = 0.0f;
				AddToGroup("Pickups");
				break;
			case Type.EnemyProjectile:
				Direction = 1.0f;
				AddToGroup("EnemyProjectiles");
				break;
			case Type.EnemyAttack:
				Direction = 0.0f;
				AddToGroup("EnemyAttacks");
				break;
			case Type.Enemy:
				Direction = 0.0f;
				AddToGroup("Enemies");
				break;
		}
	}

	private void HitByPlayerBullets(Area2D area)
	{
		if (!area.IsInGroup("PlayerProjectiles")) 
			return;

		_playerProjectile = area;
		TakeDamageFromPlayerProjectile();

	}

	private void HurtStaggerTimerTimedOut()
	{
		_hurtStatus = false;
	}

	private void TakeDamageFromPlayerProjectile()
	{
		// Pattern matching to check that BulletStats is not null, otherwise assign to variable bulletStats
		if (_playerProjectile is not Bullet { BulletStats: { } bulletStats }) 
			return;
		
		_hurtStatus = true;
		EnemyHealth -= bulletStats.BulletDamage;
		GD.Print("enemy health: "+ EnemyHealth);
		
		_hurtStaggerTimer.Start();
	}

	private void CalculateVelocity(float delta)
	{
		if (!_hurtStatus)
		{
			Velocity.X = delta * MovementSpeed * Direction;
		}
		else if (_hurtStatus)
		{
			Velocity = Vector2.Zero;
			if (IsInstanceValid(_playerProjectile))
			{
				if (_playerProjectile is not Bullet { BulletStats: { } bulletStats }) 
                	return;
                float bulletKnockback = bulletStats.BulletKnockback;
                Vector2 bulletVelocity = (Vector2) _playerProjectile.Get("Velocity");
                Velocity.X = delta * bulletKnockback * bulletVelocity.X;
			}
			
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

		CalculateVelocity((float)delta);

		MoveLocalX(Velocity.X, true);

	}
}
