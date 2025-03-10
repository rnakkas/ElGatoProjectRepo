using Godot;
using ElGatoProject.Projectiles.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class ShootingComponent : Node2D
{
	[Export] public Utility.WeaponType WeaponType;
	[Export] public ShootingProperties ShootingProperties;
	[Export] private Marker2D _muzzle;
	[Export] private Timer _shotCooldownTimer, _reloadTimer;

	[Signal]
	public delegate void ShootingEventHandler();

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
	public void Shoot(Vector2 targetVector)
	{
		if (!_onCooldown)
		{
			EmitSignal(SignalName.Shooting);
			ShootingLogic(targetVector);
			_onCooldown = true;
			_shotCooldownTimer?.Start();
		}
	}

	// Helper functions
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
					CreateAndSetBulletProperties(
						Utility.PlayerOrEnemy.Enemy, 
						WeaponType, 
						GlobalPosition.DirectionTo(targetVector)
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
						WeaponType, 
						GlobalPosition.DirectionTo(targetVector)
						);
					
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
					CreateAndSetBulletProperties(
						Utility.PlayerOrEnemy.Player, 
						WeaponType, 
						new Vector2(targetVector.X, targetVector.Y)
						);
				}
				break;
			case Utility.WeaponType.PlayerPistol:
			case Utility.WeaponType.PlayerMachineGun:
			case Utility.WeaponType.PlayerRailGun:
				CreateAndSetBulletProperties(
					Utility.PlayerOrEnemy.Player, 
					WeaponType,
					new Vector2(targetVector.X, targetVector.Y)
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
		// var projectileInstance = Globals.Instance.BulletProjectile.Instantiate<BulletProjectile>();
		var projectileInstance = ResourceLoader.Load<PackedScene>(Utility.BulletPackedScenePath)
			.Instantiate<BulletProjectile>();
		
		projectileInstance.PlayerOrEnemyBullet = playerOrEnemy;
		projectileInstance.BulletWeaponType = projectileWeaponType;
		
		projectileInstance.Target = directionToTarget;
		projectileInstance.RotationDegrees = 
			Globals.Instance.Rng.RandfRange(-ShootingProperties.BulletSwayAngle, ShootingProperties.BulletSwayAngle);
		
		projectileInstance.BulletSpeed = ShootingProperties.BulletSpeed;
		projectileInstance.Knockback = ShootingProperties.BulletKnockback; 
		projectileInstance.BulletDamage = ShootingProperties.BulletDamage;
		if (_muzzle != null) projectileInstance.GlobalPosition = _muzzle.GlobalPosition;
		
		// Add the projectiles to the level instead of root, ensure level is the currently visible one
		var levelsArray = GetTree().GetNodesInGroup("Levels");
		foreach (var level in levelsArray)
		{
			if (level is Node2D levelNode && levelNode.IsVisible())
			{
				levelNode.AddChild(projectileInstance);
			}
		}
	}
}
