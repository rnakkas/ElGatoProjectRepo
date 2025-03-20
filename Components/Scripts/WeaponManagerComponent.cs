using Godot;
using System;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class WeaponManagerComponent : Node
{
	[Export] private PickupsComponent _pickupsComponent;
	[Export] private ShootingComponent _shootingComponent;
	
	private int _weaponAmmo;

	public override void _Ready()
	{
		ConnectSignals();
	}
	
	private void ConnectSignals()
	{
		if (_shootingComponent != null) _shootingComponent.Shooting += OnShooting;
		
		if (_pickupsComponent != null) _pickupsComponent.PickedUpWeaponPowerUp += OnWeaponPowerUpPickedUp;
	}
	
	private void OnShooting()
	{
		// Only reduce ammo for power-up weapons
		if (_shootingComponent?.ShootingProperties.WeaponType == Utility.WeaponType.PlayerPistol) 
			return;
		_weaponAmmo--;
		
		if (_weaponAmmo <= 0)
			SwitchWeapon(Utility.WeaponType.PlayerPistol);
		
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerAmmoUpdate), _weaponAmmo);
	}
	
	private void OnWeaponPowerUpPickedUp(string weaponPowerUp)
	{
		if (Enum.TryParse(weaponPowerUp, out Utility.WeaponType weaponType))
		{
			SwitchWeapon(weaponType);
		}
	}
	
	private void SwitchWeapon(Utility.WeaponType weaponType)
	{
		if (_shootingComponent == null) 
			return;
		
		switch (weaponType)
		{
			case Utility.WeaponType.None:
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyShotgun:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				return;
			
			case Utility.WeaponType.PlayerPistol:
				if (_shootingComponent != null)
					_shootingComponent.ShootingProperties =
						ResourceLoader.Load<ShootingProperties>(Utility.PlayerPistolShootingProperties);
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				if (_shootingComponent != null)
					_shootingComponent.ShootingProperties =
						ResourceLoader.Load<ShootingProperties>(Utility.PlayerShotgunShootingProperties);
				break;
			
			case Utility.WeaponType.PlayerMachineGun:
				if (_shootingComponent != null)
					_shootingComponent.ShootingProperties =
						ResourceLoader.Load<ShootingProperties>(Utility.PlayerMachineGunShootingProperties);
				break;
			
			case Utility.WeaponType.PlayerRailGun:
				if (_shootingComponent != null)
					_shootingComponent.ShootingProperties =
						ResourceLoader.Load<ShootingProperties>(Utility.PlayerRailGunShootingProperties);
				break;
			
			default:
				throw new ArgumentOutOfRangeException($"Weapon type does not exist");
		}

		// _shootingComponent.WeaponType = weaponType;
		_weaponAmmo = _shootingComponent.ShootingProperties.MagazineSize;
		
		// Set the updated timer values for the new weapon type
		_shootingComponent.SetTimerValues(); 
		
		// Set the updated muzzle flash sprite for the new weapon type
		_shootingComponent.SetWeaponMuzzleFlash(weaponType);
		
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerCurrentWeaponUpdate), weaponType.ToString());
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerAmmoUpdate), _weaponAmmo);
	}
}
