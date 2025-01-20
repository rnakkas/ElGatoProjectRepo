using Godot;
using System;

public partial class EnemyBullet : Area2D
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Timer _despawnTimer;

	public Vector2 Target;
	public float BulletSpeed, BulletKnockback, BulletDespawnTimeSeconds;
	public int BulletDamage;
	public Vector2 Velocity;
	
	public override void _Ready()
	{
		AddToGroup("PlayerProjectiles");
		
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
	
	public override void _Process(double delta)
	{
		Velocity.X = (float)delta * BulletSpeed * Target.X;
		Velocity.Y = (float)delta * BulletSpeed * Target.Y;

		MoveLocalX(Velocity.X, true);
	}
}
