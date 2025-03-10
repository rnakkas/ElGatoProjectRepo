using Godot;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Utilties;

namespace ElGatoProject.Projectiles.Scripts;

public partial class BulletProjectile : Node2D
{
	[Export] public Utility.PlayerOrEnemy PlayerOrEnemyBullet { get; set; }
	
	// Nodes
	private ProjectileHitboxComponent _hitbox;
	private AnimatedSprite2D _bulletSprite;
	private Timer _despawnTimer;

	private bool _hitStatus;
	public float BulletSpeed, Knockback, DespawnTime;
	public Vector2 Target;
	public int BulletDamage;
	public Utility.WeaponType BulletWeaponType;
	
	private Vector2 _velocity;
	
	public override void _Ready()
	{
		SetSprite();
		SetHitbox();
		SetTimerValues();
		ConnectToSignals();
		_bulletSprite?.Play(Utility.BulletFlyAnimation);
	}
	
	// Helper functions
	private void SetSprite()
	{
		_bulletSprite = ResourceLoader.Load<PackedScene>(Utility.BulletSpritePackedScenePaths[BulletWeaponType])
			.Instantiate<AnimatedSprite2D>();
		AddChild(_bulletSprite);
	}

	private void SetHitbox()
	{
		_hitbox = _bulletSprite.GetNodeOrNull<ProjectileHitboxComponent>("ProjectileHitboxComponent");
		
		if (_hitbox != null)
		{
			_hitbox.PlayerOrEnemyProjectile = PlayerOrEnemyBullet;
			_hitbox.Damage = BulletDamage;
			_hitbox.Knockback = Knockback;
		}
	}

	private void SetTimerValues()
	{
		_despawnTimer = GetNodeOrNull<Timer>("despawnTimer");
		_despawnTimer?.SetWaitTime(DespawnTime);
		_despawnTimer?.Start();
	}
	
	private void ConnectToSignals()
	{
		if (_despawnTimer != null) _despawnTimer.Timeout += OmBulletDespawnTimerTimedOut;

		if (_hitbox != null) _hitbox.HitboxCollided += OnHitBoxCollision;
	}

	// Hitting targets
	private async void OnHitBoxCollision()
	{
		_hitStatus = true;
		
		if (_bulletSprite != null)
		{
			_bulletSprite.Play(Utility.BulletHitAnimation);
			await ToSignal(_bulletSprite, "animation_finished");
		}
		
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

		if (_hitbox != null) _hitbox.Velocity = _velocity;
	}

	private void FlipSprite(float velocityX)
	{
		switch (velocityX)
		{
			case < 0:
				_bulletSprite.FlipH = true;
				break;
			case > 0:
				_bulletSprite.FlipH = false;
				break;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ApplyVelocity((float)delta);
		FlipSprite(_velocity.X);
	}
}
