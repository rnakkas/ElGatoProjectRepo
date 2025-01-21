using Godot;
using System;

namespace ElGatoProject.Enemies.Scripts;
public partial class EnemyBullet : Area2D
{
	// Nodes
	private AnimatedSprite2D _sprite;
	private Timer _despawnTimer;

	public Vector2 Target;
	public float BulletSpeed, BulletKnockback, BulletDespawnTimeSeconds, WeaponSwayAmount;
	public int BulletDamage;
	public Vector2 Velocity;
	
	public override void _Ready()
	{
		// Get nodes
		_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite");
		_despawnTimer = GetNodeOrNull<Timer>("despawnTimer");
		
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(BulletDespawnTimeSeconds);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();

		BodyEntered += BulletHitWallOrFloor;
		BodyEntered += BulletHitPlayer;
		
		_sprite.Play("fly");

	}
	
	private void BulletHitWallOrFloor(Node body)
	{
		if (body is TileMapLayer)
		{
			QueueFree();
		}
	}

	private void BulletHitPlayer(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			QueueFree();
		}
	}

	private void BulletDespawnTimerTimedOut()
	{
		GD.Print("despawn");
		QueueFree();
	}

	private void CalculateVelocity(float delta)
	{
		Velocity = new Vector2(
			delta * BulletSpeed * Target.X,
			delta * BulletSpeed * (Target.Y + WeaponSwayAmount)
		);
		
		Position += Velocity;
	}
	
	public override void _Process(double delta)
	{
		CalculateVelocity((float)delta);
	}
}
