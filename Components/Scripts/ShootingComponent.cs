using Godot;
using System;
using ElGatoProject.Projectiles.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class ShootingComponent : Node2D
{
	[Export] private Utility.WeaponType _weaponType;
	[Export] private float _shootingCooldownTime;
	[Export] private float _reloadTime;
	[Export] private int _magazineSize;
	[Export] private int _bulletDamage;
	[Export] private float _bulletKnockback;
	[Export] private int _bulletsPerShot;
	[Export] private float _bulletSwayAngle;
	[Export] private float _bulletSpeed;
	[Export] private Marker2D _muzzle;
	[Export] private Timer _shotCooldownTimer;
	[Export] private Timer _reloadTimer;
	
	public bool HurtStatus, OnCooldown;
	public Vector2 TargetVector;
	private bool _reloading;
	private int _bulletCount;
	private Vector2 _muzzlePosition;
	
	public override void _Ready()
	{
		SetTimerValues();
		ConnectToSignals();
		
		_muzzlePosition = _muzzle.Position;
	}
	
	// Public method
	public void Shoot()
	{
		if (!HurtStatus && !OnCooldown)
		{
			ShootingLogic();
			OnCooldown = true;
			_shotCooldownTimer.Start();
		}
	}

	// Helper functions
	private void SetTimerValues()
	{
		if (_shotCooldownTimer == null)
			return;
		_shotCooldownTimer.OneShot = true;
		_shotCooldownTimer.WaitTime = _shootingCooldownTime;
		
		if (_reloadTimer == null)
			return;
		_reloadTimer.OneShot = true;
		_reloadTimer.WaitTime = _reloadTime;
	}

	private void ConnectToSignals()
	{
		if (_shotCooldownTimer == null)
			return;
		_shotCooldownTimer.Timeout += OnShotCoolDownTimerTimeout;
		
		if (_reloadTimer == null)
			return;
		_reloadTimer.Timeout += OnReloadTimerTimeout;
	}

	private void OnShotCoolDownTimerTimeout()
	{
		OnCooldown = false;
	}

	private void OnReloadTimerTimeout()
	{
		_reloading = false;
		_bulletCount = 0;
	}

	private void FlipMuzzle()
	{
		if (TargetVector.X < 0)
		{
			_muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
		}
		else if (TargetVector.X > 0)
		{
			_muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
		}
	}

	private void ShootingLogic()
	{
		switch (_weaponType)
		{
			case Utility.WeaponType.EnemyShotgun:
				for (int i = 0; i < _bulletsPerShot; i++)
				{
					CreateAndSetBulletProperties(
						Utility.PlayerOrEnemy.Enemy, 
						_weaponType, 
						GlobalPosition.DirectionTo(TargetVector)
						);
				}
				break;
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				if (!_reloading)
				{
					CreateAndSetBulletProperties(
						Utility.PlayerOrEnemy.Enemy, 
						_weaponType, 
						GlobalPosition.DirectionTo(TargetVector)
						);
					
					_bulletCount++;
					
					if (_bulletCount >= _magazineSize)
					{
						_reloading = true;
						_reloadTimer.Start();
					}
				}
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				FlipMuzzle();
				for (int i = 0; i < _bulletsPerShot; i++)
				{
					CreateAndSetBulletProperties(
						Utility.PlayerOrEnemy.Player, 
						_weaponType, 
						new Vector2(TargetVector.X, TargetVector.Y)
						);
				}
				break;
			case Utility.WeaponType.PlayerPistol:
			case Utility.WeaponType.PlayerMachineGun:
			case Utility.WeaponType.PlayerRailGun:	
				FlipMuzzle();
				CreateAndSetBulletProperties(
					Utility.PlayerOrEnemy.Player, 
					_weaponType,
					new Vector2(TargetVector.X, TargetVector.Y)
					);
				break;
		}
	}

	private void CreateAndSetBulletProperties(
		Utility.PlayerOrEnemy playerOrEnemy, 
		Utility.WeaponType projectileWeaponType,
		Vector2 directionToTarget
		)
	{
		var projectileInstance = Globals.Instance.BulletProjectile.Instantiate<BulletProjectile>();
		
		projectileInstance.PlayerOrEnemyBullet = playerOrEnemy;
		projectileInstance.BulletWeaponType = projectileWeaponType;
		
		projectileInstance.Target = directionToTarget;
		projectileInstance.RotationDegrees = Globals.Instance.Rng.RandfRange(-_bulletSwayAngle, _bulletSwayAngle);
		projectileInstance.BulletSpeed = _bulletSpeed;
		projectileInstance.Knockback = _bulletKnockback;
		projectileInstance.BulletDamage = _bulletDamage;
		projectileInstance.GlobalPosition = _muzzle.GlobalPosition;
		
		GetTree().Root.AddChild(projectileInstance);
	}
}
