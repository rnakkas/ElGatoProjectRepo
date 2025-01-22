using Godot;
using System;
using ElGatoProject.Players.Scripts;
using ElGatoProject.Resources;

namespace ElGatoProject.Enemies.Scripts;

[GlobalClass]
public partial class RangedEnemyBase : Area2D
{
	[Export] private RangedEnemyStats _rangedEnemyStats;
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
	private RandomNumberGenerator _rng = new();
	
	public override void _Ready()
	{
		if (_hurtStaggerTimer != null)
		{
			_hurtStaggerTimer.OneShot = true;
			_hurtStaggerTimer.SetWaitTime(_rangedEnemyStats.HurtStaggerTime);
			_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;
		}

		if (_shotCooldownTimer != null)
		{
			_shotCooldownTimer.OneShot = true;
            _shotCooldownTimer.SetWaitTime(_rangedEnemyStats.AttackCooldownTime);
            _shotCooldownTimer.Timeout += ShotCooldownTimerTimedOut;
		}
		
		if (_rapidFireTimer != null && _rangedEnemyStats.RangedEnemyType == RangedEnemyStats.Type.RangedEnemyMachineGun)
		{
			_rapidFireTimer.OneShot = true;
			_rapidFireTimer.SetWaitTime(_rangedEnemyStats.RapidFireTime);
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
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyStats.Health);
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
		else
		{
			EmitSignal(_hurtStatus ? SignalName.Hurt : SignalName.Idle);
		}
	}
	
	private void OnShoot()
	{
		ShootingBehaviour();
	}

	private void ShootingBehaviour()
	{
		switch (_rangedEnemyStats.RangedEnemyType)
		{
			case RangedEnemyStats.Type.RangedEnemyLight:
				if (!_onCooldown)
				{
					_debugStateLabel.SetText("Shooting");
					SpawnBullets(false);
					_onCooldown = true;
					_shotCooldownTimer.Start();
				}
				break;
			
			case RangedEnemyStats.Type.RangedEnemyHeavy:
				if (!_onCooldown)
				{
					_debugStateLabel.SetText("Shooting");
					SpawnBullets(false);
					_onCooldown = true;
					_shotCooldownTimer.Start();
				}
				break;
			
			case RangedEnemyStats.Type.RangedEnemyMachineGun:
				if (!_rapidFireCooldown && !_onCooldown)
				{
					_debugStateLabel.SetText("Shooting");
					SpawnBullets(true);
					_rapidFireCooldown = true;
					_rapidFireTimer.Start();
					_bulletCount++;

					if (_bulletCount >= _rangedEnemyStats.BulletsPerShot)
					{
						_onCooldown = true;
						_shotCooldownTimer.Start();
						_bulletCount = 0;
					}
				}
				break;
		}
	}
	
	private void SpawnBullets(bool rapidFire)
	{
		if (!rapidFire)
		{
			for (int i = 0; i < _rangedEnemyStats.BulletsPerShot; i++)
			{
				InstantiateBullet();
			}
		}
		else
		{
			InstantiateBullet();
		}
	}
	
	private void InstantiateBullet()
	{
		var bulletInstance = (EnemyBullet)_bulletScene.Instantiate();
		
		// Set properties for the bullet
		bulletInstance.Target = GlobalPosition.DirectionTo(_player.GlobalPosition);
		bulletInstance.RotationDegrees = _rng.RandfRange(-_rangedEnemyStats.BulletAngle, _rangedEnemyStats.BulletAngle);
		bulletInstance.BulletSpeed = _rangedEnemyStats.BulletSpeed;
		bulletInstance.Knockback = _rangedEnemyStats.Knockback;
		bulletInstance.BulletDespawnTimeSeconds = _rangedEnemyStats.BulletDespawnTimeSeconds;
		bulletInstance.AttackDamage = _rangedEnemyStats.AttackDamage;
		bulletInstance.GlobalPosition = _eyeMarker.GlobalPosition;
		
		// Add bullet instance to scene
		GetTree().Root.AddChild(bulletInstance);
	}

	private void RapidFireTimerTimedOut()
	{
		_rapidFireCooldown = false;
	}

	private void ResetBulletCount()
	{
		if (_rangedEnemyStats.RangedEnemyType == RangedEnemyStats.Type.RangedEnemyMachineGun)
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
		_rangedEnemyStats.TakeDamage(bullet.BulletDamage);
		
		// Die if health reaches zero
		if (_rangedEnemyStats.Health <= 0)
		{
			EmitSignal(SignalName.Death);
		}
		
		_hurtStaggerTimer.Start();
		
		// For debug only, remove later
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyStats.Health);
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
