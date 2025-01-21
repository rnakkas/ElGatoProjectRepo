using Godot;
using System;
using ElGatoProject.Players.Scripts;
using ElGatoProject.Resources;

namespace ElGatoProject.Enemies.Scripts;
public partial class RangedEnemyOne : Area2D
{
	[Export] private EnemyStats _rangedEnemyOneStats;
	[Export] private AnimatedSprite2D _spriteBody, _spriteEye;
	[Export] private Marker2D _eyeMarker;
	[Export] private Area2D _playerDetectionArea;
	[Export] private RayCast2D _wallDetectionRay;
	[Export] private Timer _hurtStaggerTimer;
	[Export] private Timer _shotCooldownTimer;
	[Export] private Timer _rapidFireTimer;
	[Export] private PackedScene _bulletScene;
	[Export] private Label _debugStateLabel;
	[Export] private Label _debugHealthLabel;
	
	[Signal]
	public delegate void ShootEventHandler();
	[Signal]
	public delegate void IdleEventHandler();
	[Signal]
	public delegate void HurtEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	private bool _hurtStatus, _playerInRange, _onCooldown, _rapidFireCooldown;
	private Area2D _playerProjectile;
	private Node2D _player;
	private int _bulletCount;
	
	public float Direction;
	public Vector2 Velocity;
	
	public override void _Ready()
	{
		if (_hurtStaggerTimer != null)
		{
			_hurtStaggerTimer.OneShot = true;
			_hurtStaggerTimer.SetWaitTime(_rangedEnemyOneStats.HurtStaggerTime);
			_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;
		}
		
		_shotCooldownTimer.OneShot = true;
		_shotCooldownTimer.SetWaitTime(_rangedEnemyOneStats.AttackCooldownTime);
		_shotCooldownTimer.Timeout += ShotCooldownTimerTimedOut;

		if (_rapidFireTimer != null && _rangedEnemyOneStats.EnemyType == EnemyStats.Type.RangedEnemyMachineGun)
		{
			_rapidFireTimer.OneShot = true;
			_rapidFireTimer.SetWaitTime(_rangedEnemyOneStats.RapidFireTime);
			_rapidFireTimer.Timeout += RapidFireTimerTimedOut;
		}
		
		AreaEntered += HitByPlayerBullets;

		_playerDetectionArea.AreaEntered += PlayerEnteredDetectionRange;
		_playerDetectionArea.AreaExited += PlayerExitedDetectionRange;

		_wallDetectionRay.Enabled = false;

		Shoot += OnShoot;
		Idle += OnIdle;
		Hurt += OnHurt;
		Death += OnDeath;
		
		// For debug only, remove later
		_debugStateLabel.SetText("idle");
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyOneStats.EnemyHealth);
	}
	
	// Detecting when player enters or exits detection range
	private void PlayerEnteredDetectionRange(Area2D area)
	{
		if (area.IsInGroup("Players"))
		{
			_player = area;
			_playerInRange = true;
			_wallDetectionRay.Enabled = true;
			_wallDetectionRay.TargetPosition = ToLocal(_player.GlobalPosition);

			ResetBulletCount();
		}
	}

	private void PlayerExitedDetectionRange(Area2D area)
	{
		if (area.IsInGroup("Players"))
		{
			_playerInRange = false;
			_wallDetectionRay.Enabled = false;
			
			_debugStateLabel.SetText("Idle");
		}
	}
	
	// Enemy behaviour
	private void EnemyBehaviour()
	{
		// Use raycast to detect if line of sight to player is blocked by wall
		if (_playerInRange)
		{
			_wallDetectionRay.TargetPosition = ToLocal(_player.GlobalPosition);

			if (!_wallDetectionRay.IsColliding() && !_hurtStatus)
			{
				EmitSignal(SignalName.Shoot);
				// _onCooldown = true;
				// _shotCooldownTimer.Start();
			}
			else if (_hurtStatus)
			{
				EmitSignal(SignalName.Hurt);
			}
			else
			{
				EmitSignal(SignalName.Idle);
			}
		}

		// if (_hurtStatus)
		// {
		// 	EmitSignal(SignalName.Hurt);
		// }
		// else
		// {
		// 	EmitSignal(SignalName.Idle);
		// }
	}
	
	private void OnShoot()
	{
		switch (_rangedEnemyOneStats.EnemyType)
		{
			case EnemyStats.Type.RangedEnemyHeavy:
				if (!_onCooldown)
				{
					_debugStateLabel.SetText("Shooting");
					SpawnShotgunShells();
					_onCooldown = true;
					_shotCooldownTimer.Start();
				}
				break;
			
			case EnemyStats.Type.RangedEnemyMachineGun:
				if (!_rapidFireCooldown && !_onCooldown)
				{
					_debugStateLabel.SetText("Shooting");
					SpawnMachineGunBullets();
					_rapidFireCooldown = true;
					_rapidFireTimer.Start();
					_bulletCount++;

					if (_bulletCount >= _rangedEnemyOneStats.BulletsPerShot)
					{
						_onCooldown = true;
						_shotCooldownTimer.Start();
						_bulletCount = 0;
					}
				}
				break;
		}
		
	}
	
	private void SpawnShotgunShells()
	{
		var rng = new RandomNumberGenerator();
		
		for (int i = 0; i < _rangedEnemyOneStats.BulletsPerShot; i++)
		{
			var bulletInstance = (EnemyBullet)_bulletScene.Instantiate();
            		
            // Set properties for the bullet
            bulletInstance.Target = GlobalPosition.DirectionTo(_player.GlobalPosition);
            bulletInstance.RotationDegrees = rng.RandfRange(-_rangedEnemyOneStats.BulletAngle, _rangedEnemyOneStats.BulletAngle);
            bulletInstance.BulletSpeed = _rangedEnemyOneStats.BulletSpeed;
            bulletInstance.Knockback = _rangedEnemyOneStats.Knockback;
            bulletInstance.BulletDespawnTimeSeconds = _rangedEnemyOneStats.BulletDespawnTimeSeconds;
            bulletInstance.AttackDamage = _rangedEnemyOneStats.AttackDamage;
            bulletInstance.GlobalPosition = _eyeMarker.GlobalPosition;
            GetTree().Root.AddChild(bulletInstance);
		}
	}

	private void SpawnMachineGunBullets()
	{
		var rng = new RandomNumberGenerator();
		
		var bulletInstance = (EnemyBullet)_bulletScene.Instantiate();
		
		// Set properties for the bullet
		bulletInstance.Target = GlobalPosition.DirectionTo(_player.GlobalPosition);
		bulletInstance.RotationDegrees = rng.RandfRange(-_rangedEnemyOneStats.BulletAngle, _rangedEnemyOneStats.BulletAngle);
		bulletInstance.BulletSpeed = _rangedEnemyOneStats.BulletSpeed;
		bulletInstance.Knockback = _rangedEnemyOneStats.Knockback;
		bulletInstance.BulletDespawnTimeSeconds = _rangedEnemyOneStats.BulletDespawnTimeSeconds;
		bulletInstance.AttackDamage = _rangedEnemyOneStats.AttackDamage;
		bulletInstance.GlobalPosition = _eyeMarker.GlobalPosition;
		GetTree().Root.AddChild(bulletInstance);
	}

	private void RapidFireTimerTimedOut()
	{
		_rapidFireCooldown = false;
	}

	private void ResetBulletCount()
	{
		if (_rangedEnemyOneStats.EnemyType == EnemyStats.Type.RangedEnemyMachineGun)
		{
			_bulletCount = 0;
		}
	}

	private void OnIdle()
	{
		_debugStateLabel.SetText("Idle");
	}

	private void ShotCooldownTimerTimedOut()
	{
		_onCooldown = false;
	}
	
	// Getting hit by player bullets
	private void HitByPlayerBullets(Area2D area)
	{
		if (!area.IsInGroup("PlayerProjectiles")) 
			return;

		_playerProjectile = area;

		TakeDamageFromPlayerProjectile();
	}
	
	private void TakeDamageFromPlayerProjectile()
	{
		if (_playerProjectile is not Bullet bullet)
			return;
		
		_hurtStatus = true;
		_rangedEnemyOneStats.TakeDamage(bullet.BulletDamage);
		
		// Die if health reaches zero
		if (_rangedEnemyOneStats.EnemyHealth <= 0)
		{
			EmitSignal(SignalName.Death);
		}
		
		_hurtStaggerTimer.Start();
		
		// For debug only, remove later
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyOneStats.EnemyHealth);
	}

	private void OnHurt()
	{
		_debugStateLabel.SetText("Hurt");
	}
	
	private void HurtStaggerTimerTimedOut()
	{
		_hurtStatus = false;
	}

	private void OnDeath()
	{
		_debugStateLabel.SetText("Death");
		QueueFree();
	}
	
	public override void _Process(double delta)
	{
		EnemyBehaviour();
	}
}
