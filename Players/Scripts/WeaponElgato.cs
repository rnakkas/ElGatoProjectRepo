using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private ShootingComponent _shooting;
	[Export] private AnimationComponent _animation;
	
	[Export] private Label _debugWeaponLabel;
	
	public Vector2 Direction;
	public bool HurtStatus;
	private int _weaponAmmo;
	
	public override void _Ready()
	{
		ConnectToSignals();
	}

	private void ConnectToSignals()
	{
		_shooting.Shooting += OnShooting;
	}

	// Shooting signal connection
	private void OnShooting()
	{
		_animation.PlayWeaponAnimations(true);
		
		// Only reduce ammo for power-up weapons
		if (_shooting.WeaponType != Utility.WeaponType.PlayerPistol)
			_weaponAmmo--;
	}
	
	private void SetComponentProperties()
	{
		_shooting.TargetVector = Direction;
		_shooting.HurtStatus = HurtStatus;
	}

	public void SwitchWeapon(Utility.WeaponType weaponType)
	{
		_shooting.WeaponType = weaponType;
		
		switch (weaponType)
		{
			case Utility.WeaponType.None:
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyShotgun:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				return;
			
			case Utility.WeaponType.PlayerPistol:
				_shooting.ShootingProperties = Globals.Instance.PlayerPistolShootingProperties;
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				_shooting.ShootingProperties = Globals.Instance.PlayerShotgunShootingProperties;
				break;
			
			case Utility.WeaponType.PlayerMachineGun:
				_shooting.ShootingProperties = Globals.Instance.PlayerMachineGunShootingProperties;
				break;
			
			case Utility.WeaponType.PlayerRailGun:
				_shooting.ShootingProperties = Globals.Instance.PlayerRailGunShootingProperties;
				break;
			
			default:
				throw new ArgumentOutOfRangeException($"Weapon type does not exist");
		}
		
		_weaponAmmo = _shooting.ShootingProperties.MagazineSize;
		
		// Set the updated timer values for the new weapon type
		_shooting.SetTimerValues(); 
	}

	private void WeaponActions()
	{
		if (Input.IsActionPressed("shoot"))
		{
			_shooting.Shoot();
		}
		else
		{
			_animation.PlayWeaponAnimations(false);
		}
		
		// If weapon power-up ammo runs out, return to pistol
		if (_weaponAmmo <= 0)
		{
			SwitchWeapon(Utility.WeaponType.PlayerPistol);
		}
	}
	
	public override void _Process(double delta)
	{
		SetComponentProperties();
		WeaponActions();
		
		_animation.FlipSprite(Direction.X);
		
		_debugWeaponLabel.SetText(_shooting.WeaponType + ": " + _weaponAmmo);
	}
}
