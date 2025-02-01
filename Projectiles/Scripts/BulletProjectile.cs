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
	private Vector2 _velocity;
	private Utility.WeaponType _weaponType;
	private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(BulletLifeTime);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();
		
		switch (PlayerOrEnemyBullet)
		{
			case Utility.PlayerOrEnemy.Player:
				_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite_player");
				break;
			
			case Utility.PlayerOrEnemy.Enemy:
				_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite_enemy");
				break;
		}
		
		_animation.Sprite = _sprite;
		_animation.PlayProjectileAnimations(_weaponType);
		
	}

	private void BulletDespawnTimerTimedOut()
	{
		QueueFree();
	}

	private void SetComponentProperties()
	{
		_hitbox.PlayerOrEnemyBullet = PlayerOrEnemyBullet;
		_hitbox.BulletDamage = BulletDamage;
		_hitbox.Knockback = Knockback;
		_hitbox.Velocity = _velocity;
	}
	
	private void ApplyVelocity(float delta)
	{
		_velocity = new Vector2(
			delta * BulletSpeed * Target.X,
			delta * BulletSpeed * Target.Y
		);
		
		MoveLocalX(_velocity.X, true);
		MoveLocalY(_velocity.Y, true);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		SetComponentProperties();
		
		ApplyVelocity((float)delta);
	}
}
