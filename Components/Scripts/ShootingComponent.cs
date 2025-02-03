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
	[Export] private int _bulletDamage;
	[Export] private float _bulletKnockback;
	[Export] private float _bulletsPerShot;
	[Export] private float _bulletSwayAngle;
	[Export] private float _bulletSpeed;
	[Export] private float _bulletLifeTime;
	[Export] private Marker2D _muzzle;
	[Export] private Timer _shotCooldownTimer;

	public bool HurtStatus, CanSeePlayer;
	public Vector2 TargetVector;
	private bool _onCooldown;

	public override void _Ready()
	{
		if (_shotCooldownTimer == null)
			return;
		_shotCooldownTimer.OneShot = true;
		_shotCooldownTimer.WaitTime = _shootingCooldownTime;
		_shotCooldownTimer.Timeout += ShotCoolDownTimerTimeout;
	}

	private void ShotCoolDownTimerTimeout()
	{
		_onCooldown = false;
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

	//TODO: Create shooting logic for machine gun
	/*
	 * Option 1:
	 * Use an int maxBullets variable
	 * After each shot/cooldown timeout increment the bullet count
	 * If bulletCount == maxBullets start a reload cooldown timer
	 * When reload cooldown timer timesout, reset bulletCount to 0 to be able to start shooting again
	 *
	 * Option 2:
	 * Use a shooting timer
	 * Enemy can only shoot while the timer is ruuning
	 * When timer times out, stop shooting and start reload timer
	 * When reload timer runs out, start shooting timer and start shooting again
	 */
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
				CreateAndSetBulletProperties(Utility.PlayerOrEnemy.Enemy, _weaponType);
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
