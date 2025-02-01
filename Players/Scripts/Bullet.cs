using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class Bullet : Area2D
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] private Timer _despawnTimer;

	public float Direction, BulletSpeed, Knockback, BulletDespawnTimeSeconds;
	public Vector2 Target;
	public int BulletDamage;
	private Vector2 _velocity;
	
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

	private void BulletHitEnemy(Area2D enemyArea)
	{
		if (!enemyArea.IsInGroup("Enemies"))
			return;
		if (enemyArea is not HurtboxComponent hurtboxComponent)
			return;
		
		hurtboxComponent.HitByAttack(this, BulletDamage, Knockback, _velocity);
		QueueFree();
	}

	private void BulletDespawnTimerTimedOut()
	{
		QueueFree();
	}
	
	public override void _Process(double delta)
	{
		_velocity.X = (float)delta * BulletSpeed * Target.X;

		MoveLocalX(_velocity.X, true);
	}
}
