using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Players.Scripts;

public partial class Bullet : Area2D
{
	[Export] private BulletStats _bulletStats;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Timer _despawnTimer;

	public float Direction;
	public Vector2 Velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(_bulletStats.BulletDespawnTime);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();
		
	}

	private void BulletDespawnTimerTimedOut()
	{
		GD.Print("despawn");
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Velocity.X = (float)delta * _bulletStats.BulletSpeed * Direction;

		MoveLocalX(Velocity.X, true);
	}
}
