using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Projectiles.Scripts;

public partial class BulletProjectile : Node2D
{
	[Export] public Utility.PlayerOrEnemy PlayerOrEnemyBullet { get; set; }
	
	// Components
	[Export] private ProjectileHitboxComponent _hitbox;
	[Export] private AnimationComponent _animation;
	
	[Export] private Timer _despawnTimer;

	public float BulletSpeed, Knockback;
	public Vector2 Target;
	public int BulletDamage;
	public Utility.WeaponType BulletWeaponType;
	
	private Vector2 _velocity;
	
	public override void _Ready()
	{
		ConnectToSignals();
		SetComponentProperties();
		
		_animation.PlayProjectileAnimations(BulletWeaponType, false);
		_despawnTimer.Start();
	}
	
	// Helper functions
	private void ConnectToSignals()
	{
		_despawnTimer.Timeout += OmBulletDespawnTimerTimedOut;
		_hitbox.HitboxCollided += OnHitBoxCollision;
	}
	
	private void SetComponentProperties()
	{
		_hitbox.PlayerOrEnemyProjectile = PlayerOrEnemyBullet;
		_hitbox.Damage = BulletDamage;
		_hitbox.Knockback = Knockback;
	}

	// Hitting targets
	private void OnHitBoxCollision()
	{
		_animation.PlayProjectileAnimations(BulletWeaponType, true);
		QueueFree();
	}
	
	private void OmBulletDespawnTimerTimedOut()
	{
		QueueFree();
	}
	
	// Velocity calculations
	private void ApplyVelocity(float delta)
	{
		_velocity = new Vector2(
			delta * BulletSpeed * Target.X,
			delta * BulletSpeed * Target.Y
		);
		
		MoveLocalX(_velocity.X, true);
		MoveLocalY(_velocity.Y, true);
		
		_hitbox.Velocity = _velocity;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ApplyVelocity((float)delta);
	}
}
