using Godot;
using ElGatoProject.Projectiles.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class ShootingComponent : Node2D
{
	[Export] public Utility.PlayerOrEnemy PlayerOrEnemy;
	[Export] public Utility.WeaponType WeaponType;
	[Export] public ShootingProperties ShootingProperties;
	[Export] private Marker2D _muzzle;
	[Export] private Timer _shotCooldownTimer, _reloadTimer;

	[Signal]
	public delegate void ShootingEventHandler();

	[Signal]
	public delegate void CannotShootEventHandler();

	public bool HurtStatus;
	private bool _onCooldown, _reloading;
	private int _bulletCount;
	private Vector2 _muzzlePosition;
	
	public override void _Ready()
	{
		SetTimerValues();
		ConnectToSignals();
		
		if (_muzzle != null) _muzzlePosition = _muzzle.Position;
	}
	
	// Public method
	public bool Shoot(Vector2 targetVector)
	{
		bool shootStatus;
		if (!_onCooldown)
		{
			EmitSignal(SignalName.Shooting);
			ShootingLogic(targetVector);
			_onCooldown = true;
			_shotCooldownTimer?.Start();
			shootStatus = true;
		}
		else
		{
			EmitSignal(SignalName.CannotShoot);
			shootStatus = false;
		}

		return shootStatus;
	}

	// Helper methods
	public void SetTimerValues()
	{
		if (_shotCooldownTimer != null)
		{
			_shotCooldownTimer.OneShot = true;
			if (ShootingProperties != null) _shotCooldownTimer.WaitTime = ShootingProperties.ShootingCooldownTime;
		}

		if (_reloadTimer != null)
		{
			_reloadTimer.OneShot = true;
			if (ShootingProperties != null) _reloadTimer.WaitTime = ShootingProperties.ReloadTime;
		}
	}

	private void ConnectToSignals()
	{
		if (_shotCooldownTimer != null) _shotCooldownTimer.Timeout += OnShotCoolDownTimerTimeout;
		
		if (_reloadTimer != null) _reloadTimer.Timeout += OnReloadTimerTimeout;
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

	private void FlipMuzzle(Vector2 targetVector)
	{
		switch (targetVector.X)
		{
			case < 0:
			{
				if (_muzzle != null) _muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
				break;
			}
			case > 0:
			{
				if (_muzzle != null) _muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
				break;
			}
		}
	}

	private void ShootingLogic(Vector2 targetVector)
	{
		FlipMuzzle(targetVector);
		
		switch (WeaponType)
		{
			case Utility.WeaponType.None:
				break;
			
			case Utility.WeaponType.EnemyShotgun:
				for (int i = 0; i < ShootingProperties?.BulletsPerShot; i++)
				{
					CreateAndSetBulletProperties(WeaponType, GlobalPosition.DirectionTo(targetVector), i);
				}
				break;
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				if (!_reloading)
				{
					CreateAndSetBulletProperties(WeaponType, GlobalPosition.DirectionTo(targetVector));
					
					_bulletCount++;
					
					if (_bulletCount >= ShootingProperties?.MagazineSize)
					{
						_reloading = true;
						_reloadTimer?.Start();
					}
				}
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				for (int i = 0; i < ShootingProperties?.BulletsPerShot; i++)
				{
					CreateAndSetBulletProperties(WeaponType, new Vector2(targetVector.X, targetVector.Y), i);
				}
				break;
			
			case Utility.WeaponType.PlayerPistol:
			case Utility.WeaponType.PlayerMachineGun:
			case Utility.WeaponType.PlayerRailGun:
				CreateAndSetBulletProperties(WeaponType, new Vector2(targetVector.X, targetVector.Y));
				break;
		}
	}

	private void CreateAndSetBulletProperties(
		Utility.WeaponType projectileWeaponType,
		Vector2 directionToTarget,
		int projectileIndex = 0
		)
	{
		// var projectileInstance = Globals.Instance.BulletProjectile.Instantiate<BulletProjectile>();
		var projectileInstance = ResourceLoader.Load<PackedScene>(Utility.BulletPackedScenePath)
			.Instantiate<BulletProjectile>();
		
		projectileInstance.PlayerOrEnemyBullet = PlayerOrEnemy;
		projectileInstance.BulletWeaponType = projectileWeaponType;
		
		projectileInstance.Target = directionToTarget;

		if (ShootingProperties.BulletsPerShot > 1)
		{
			// Spread bullets equally over the angle range, for shotguns
			float angle = Mathf.Lerp(-ShootingProperties.BulletSwayAngle, ShootingProperties.BulletSwayAngle,
				projectileIndex / (float)(ShootingProperties.BulletsPerShot - 1));
			projectileInstance.RotationDegrees = angle;
		}
		else
		{
			// For other guns
			projectileInstance.RotationDegrees = Globals.Instance.Rng.RandfRange(-ShootingProperties.BulletSwayAngle,
				ShootingProperties.BulletSwayAngle);
		}

		projectileInstance.DespawnTime = ShootingProperties.BulletDespawnTimeSeconds;
		projectileInstance.BulletSpeed = ShootingProperties.BulletSpeed;
		projectileInstance.Knockback = ShootingProperties.BulletKnockback; 
		projectileInstance.BulletDamage = ShootingProperties.BulletDamage;
		if (_muzzle != null) projectileInstance.GlobalPosition = _muzzle.GlobalPosition;
		
		// Add the projectiles to the level instead of root, ensure level is the currently visible one
		var levelsArray = GetTree().GetNodesInGroup(Utility.NodeGroupLevels);
		foreach (var level in levelsArray)
		{
			if (level is Node2D levelNode && levelNode.IsVisible())
			{
				levelNode.AddChild(projectileInstance);
			}
		}
	}
}
