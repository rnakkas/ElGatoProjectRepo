using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Projectiles.Scripts;

public partial class BulletProjectile : Node2D
{
	[Export] private ProjectileHitboxComponent _hitbox;
	[Export] private Timer _despawnTimer;
	
	public float BulletSpeed, Knockback, BulletLifeTime; 
	public Vector2 Target;
	public int BulletDamage;
	private Vector2 _velocity;
	
	public override void _Ready()
	{
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(BulletLifeTime);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();
	}

	private void BulletDespawnTimerTimedOut()
	{
		QueueFree();
	}

	private void SetComponentProperties()
	{
		_hitbox.BulletDamage = BulletDamage;
		_hitbox.Knockback = Knockback;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		SetComponentProperties();
	}
}
