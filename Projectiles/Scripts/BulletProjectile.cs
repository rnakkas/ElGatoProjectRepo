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
	
	public float BulletSpeed, Knockback, BulletLifeTime; 
	public Vector2 Target;
	public int BulletDamage;
	public Utility.WeaponType BulletWeaponType;
	
	private Vector2 _velocity;
	private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(BulletLifeTime);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();

		_hitbox.HitboxCollided += OnHitBoxCollision;
		
		SetComponentProperties();
		_animation.PlayProjectileAnimations(BulletWeaponType);
	}

	private void OnHitBoxCollision()
	{
		QueueFree();
	}
	
	private void BulletDespawnTimerTimedOut()
	{
		QueueFree();
	}

	private void SetComponentProperties()
	{
		switch (PlayerOrEnemyBullet)
		{
			case Utility.PlayerOrEnemy.Player:
				_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite_player");
				break;
			
			case Utility.PlayerOrEnemy.Enemy:
				_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite_enemy");
				break;
		}
		
		_hitbox.PlayerOrEnemyProjectile = PlayerOrEnemyBullet;
		_hitbox.BulletDamage = BulletDamage;
		_hitbox.Knockback = Knockback;
		
		_animation.Sprite = _sprite;
	}
	
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
