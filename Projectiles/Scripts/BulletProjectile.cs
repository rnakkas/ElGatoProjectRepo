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
	[Export] private AnimatedSprite2D _bulletSprite;
	
	[Export] private Timer _despawnTimer;

	private bool _hitStatus;
	
	public float BulletSpeed, Knockback;
	public Vector2 Target;
	public int BulletDamage;
	public Utility.WeaponType BulletWeaponType;
	
	private Vector2 _velocity;
	
	public override void _Ready()
	{
		ConnectToSignals();
		SetComponentProperties();
		
		_bulletSprite.Play(Utility.Instance.BulletFlyAnimation);
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
	private async void OnHitBoxCollision()
	{
		if (_bulletSprite == null)
			return;
		
		_hitStatus = true;
		
		_bulletSprite.Play(Utility.Instance.BulletHitAnimation);
		await ToSignal(_bulletSprite, "animation_finished");
		
		QueueFree();
	}
	
	private void OmBulletDespawnTimerTimedOut()
	{
		QueueFree();
	}
	
	// Velocity calculations
	private void ApplyVelocity(float delta)
	{
		if (_hitStatus)
			return;
		
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
