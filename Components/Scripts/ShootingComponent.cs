using Godot;
using System;
using ElGatoProject.Enemies.Scripts;
using ElGatoProject.Players.Scripts;
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

	private void ShootingLogic()
	{
		switch (_weaponType)
		{
			case Utility.WeaponType.EnemyShotgun:
				break;
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				var bulletInstance = (EnemyBullet)Globals.Instance.EnemyBullet.Instantiate();
		
				// Set properties for the bullet
				bulletInstance.Target = GlobalPosition.DirectionTo(TargetVector);
				bulletInstance.RotationDegrees = Globals.Instance.Rng.RandfRange(-_bulletSwayAngle, _bulletSwayAngle);
				bulletInstance.BulletSpeed = _bulletSpeed;
				bulletInstance.Knockback = _bulletKnockback;
				bulletInstance.BulletDespawnTimeSeconds = _bulletLifeTime;
				bulletInstance.BulletDamage = _bulletDamage;
				bulletInstance.GlobalPosition = _muzzle.GlobalPosition;
		
				// Add bullet instance to scene
				GetTree().Root.AddChild(bulletInstance);
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				break;
			case Utility.WeaponType.PlayerPistol:
			case Utility.WeaponType.PlayerMachineGun:
			case Utility.WeaponType.PlayerRailGun:	
				var playerBulletInstance = (Bullet)Globals.Instance.EnemyBullet.Instantiate();
		
				// Set properties for the bullet
				playerBulletInstance.Target = GlobalPosition.DirectionTo(TargetVector);
				playerBulletInstance.RotationDegrees = Globals.Instance.Rng.RandfRange(-_bulletSwayAngle, _bulletSwayAngle);
				playerBulletInstance.BulletSpeed = _bulletSpeed;
				playerBulletInstance.Knockback = _bulletKnockback;
				playerBulletInstance.BulletDespawnTimeSeconds = _bulletLifeTime;
				playerBulletInstance.BulletDamage = _bulletDamage;
				playerBulletInstance.GlobalPosition = _muzzle.GlobalPosition;
		
				// Add bullet instance to scene
				GetTree().Root.AddChild(playerBulletInstance);
				break;
		}
	}
}

/*
 * NOTES:
 * Make the bullets a generic scene with multiple sprite sheets in the AnimatedSprite2D node
 * This way can Instantiate that generic bullet scene and pass the properties to it
 * In the bullet scene, change out the sprite based on the bullet/weapon type,
 *	for example shotgun bullets will play different fly and hit animations to pistol bullets
 *
 */
