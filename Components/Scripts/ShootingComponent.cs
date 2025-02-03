using Godot;
using System;
using ElGatoProject.Enemies.Scripts;
using ElGatoProject.Players.Scripts;
using ElGatoProject.Projectiles.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class ShootingComponent : Node2D
{
	[Export] private Utility.WeaponType _weaponType;
	[Export] private float _shootingCooldownTime;
	[Export] private float _reloadTime;
	[Export] private int _bulletDamage;
	[Export] private float _bulletKnockback;
	[Export] private float _bulletsPerShot;
	[Export] private float _bulletSwayAngle;
	[Export] private float _bulletSpeed;
	[Export] private float _bulletLifeTime;
	[Export] private Marker2D _muzzle;
	[Export] private Timer _shotCooldownTimer;
	[Export] private Timer _reloadTimer;

	public bool HurtStatus, CanSeePlayer;
	public Vector2 TargetVector;
	private bool _onCooldown, _reloading;
	private int _bulletCount;

	public override void _Ready()
	{
		if (_shotCooldownTimer == null)
			return;
		_shotCooldownTimer.OneShot = true;
		_shotCooldownTimer.WaitTime = _shootingCooldownTime;
		_shotCooldownTimer.Timeout += OnShotCoolDownTimerTimeout;
		
		if (_reloadTimer == null)
			return;
		_reloadTimer.OneShot = true;
		_reloadTimer.WaitTime = _reloadTime;
		_reloadTimer.Timeout += OnReloadTimerTimeout;
	}

	private void OnShotCoolDownTimerTimeout()
	{
		_onCooldown = false;
	}

	private void OnReloadTimerTimeout()
	{
		_reloading = false;
		_bulletCount = 0;
	}

	public void Shoot()
	{
		if (CanSeePlayer && !HurtStatus && !_onCooldown)
		{
			ShootingLogic();
			_onCooldown = true;
			_shotCooldownTimer.Start();
		}
	}
	
	private void ShootingLogic()
	{
		switch (_weaponType)
		{
			case Utility.WeaponType.EnemyShotgun:
				for (int i = 0; i < _bulletsPerShot; i++)
				{
					CreateAndSetBulletProperties(Utility.PlayerOrEnemy.Enemy, _weaponType);
				}
				break;
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				if (!_reloading)
				{
					CreateAndSetBulletProperties(Utility.PlayerOrEnemy.Enemy, _weaponType);
					_bulletCount++;
					
					if (_bulletCount >= _bulletsPerShot)
					{
						_reloading = true;
						_reloadTimer.Start();
					}
				}
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				for (int i = 0; i < _bulletsPerShot; i++)
				{
					CreateAndSetBulletProperties(Utility.PlayerOrEnemy.Player, _weaponType);
				}
				break;
			case Utility.WeaponType.PlayerPistol:
			case Utility.WeaponType.PlayerMachineGun:
			case Utility.WeaponType.PlayerRailGun:	
				CreateAndSetBulletProperties(Utility.PlayerOrEnemy.Player, _weaponType);
				break;
		}
	}

	private void CreateAndSetBulletProperties(Utility.PlayerOrEnemy playerOrEnemy, Utility.WeaponType projectileWeaponType)
	{
		var projectileInstance = Globals.Instance.BulletProjectile.Instantiate<BulletProjectile>();
		
		projectileInstance.PlayerOrEnemyBullet = playerOrEnemy;
		projectileInstance.BulletWeaponType = projectileWeaponType;
		
		projectileInstance.Target = GlobalPosition.DirectionTo(TargetVector);
		projectileInstance.RotationDegrees = Globals.Instance.Rng.RandfRange(-_bulletSwayAngle, _bulletSwayAngle);
		projectileInstance.BulletSpeed = _bulletSpeed;
		projectileInstance.Knockback = _bulletKnockback;
		projectileInstance.BulletLifeTime = _bulletLifeTime;
		projectileInstance.BulletDamage = _bulletDamage;
		projectileInstance.GlobalPosition = _muzzle.GlobalPosition;
		
		GetTree().Root.AddChild(projectileInstance);
	}
}
