using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Players.Scripts;

public partial class Bullet : Area2D
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Timer _despawnTimer;

	public float Direction, BulletSpeed, BulletKnockback, BulletDespawnTimeSeconds;
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
		AreaEntered += BulletHitEnemy;
		
		_sprite.Play("fly");

	}
	
	private void BulletHitWallOrFloor(Node body)
	{
		if (body is TileMapLayer)
		{
			QueueFree();
		}
	}

	private void BulletHitEnemy(Area2D area)
	{
		if (area.IsInGroup("Enemies"))
		{
			QueueFree();
		}
	}

	private void BulletDespawnTimerTimedOut()
	{
		QueueFree();
	}
	
	public override void _Process(double delta)
	{
		Velocity.X = (float)delta * BulletSpeed * Direction;

		MoveLocalX(Velocity.X, true);
	}
}
