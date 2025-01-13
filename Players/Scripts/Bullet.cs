using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Players.Scripts;

public partial class Bullet : Area2D
{
	[Export] public BulletStats BulletStats;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Timer _despawnTimer;

	public float Direction;
	public Vector2 Velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("PlayerProjectiles");
		
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(BulletStats.BulletDespawnTime);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();

		BodyEntered += BulletHitWallOrFloor;
		AreaEntered += BulletHitEnemy;

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
		GD.Print("despawn");
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Velocity.X = (float)delta * BulletStats.BulletSpeed * Direction;

		MoveLocalX(Velocity.X, true);
	}
}
