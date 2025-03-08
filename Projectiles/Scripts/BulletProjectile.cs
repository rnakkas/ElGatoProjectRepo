using Godot;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Utilties;

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
		
		_bulletSprite?.Play(Utility.BulletFlyAnimation);
		_despawnTimer?.Start();
	}
	
	// Helper functions
	private void ConnectToSignals()
	{
		if (_despawnTimer != null) _despawnTimer.Timeout += OmBulletDespawnTimerTimedOut;

		if (_hitbox != null) _hitbox.HitboxCollided += OnHitBoxCollision;
	}

	private void SetComponentProperties()
	{
		if (_hitbox != null)
		{
			_hitbox.PlayerOrEnemyProjectile = PlayerOrEnemyBullet;
			_hitbox.Damage = BulletDamage;
			_hitbox.Knockback = Knockback;
		}
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
	
	public override void _PhysicsProcess(double delta)
	{
		ApplyVelocity((float)delta);
	}
}
