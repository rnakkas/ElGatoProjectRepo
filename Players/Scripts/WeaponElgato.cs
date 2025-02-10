using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private ShootingComponent _shooting;
	[Export] private AnimationComponent _animation;
	
	public Vector2 Direction;
	public bool HurtStatus;
	public Utility.WeaponType WeaponType = Utility.WeaponType.PlayerPistol;
	public int WeaponAmmo;
	
	public override void _Ready()
	{
		
	}
	
	private void SetComponentProperties()
	{
		_shooting.TargetVector = Direction;
		_shooting.HurtStatus = HurtStatus;
		
		switch (WeaponType)
		{
			case Utility.WeaponType.None:
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.EnemyShotgun:
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.EnemyRailGun:
				return;
			
			case Utility.WeaponType.PlayerPistol:
				_shooting.WeaponType = WeaponType;
				_shooting.ShootingProperties = Globals.Instance.PlayerPistolShootingProperties;
				break;
			
			case Utility.WeaponType.PlayerShotgun:
				_shooting.WeaponType = WeaponType;
				_shooting.ShootingProperties = Globals.Instance.PlayerShotgunShootingProperties;
				
				WeaponAmmo = _shooting.ShootingProperties.MagazineSize;
				break;
			
			case Utility.WeaponType.PlayerMachineGun:
				_shooting.WeaponType = WeaponType;
				_shooting.ShootingProperties = Globals.Instance.PlayerMachineGunShootingProperties;
				break;
			
			case Utility.WeaponType.PlayerRailGun:
				_shooting.WeaponType = WeaponType;
				_shooting.ShootingProperties = Globals.Instance.PlayerRailGunShootingProperties;
				break;
			
			default:
				throw new ArgumentOutOfRangeException($"Weapon type does not exist");
		}
	}

	private void WeaponActions()
	{
		if (Input.IsActionPressed("shoot") && !_shooting.OnCooldown)
		{
			_shooting.Shoot();
			WeaponAmmo--;
			_animation.PlayWeaponAnimations(!HurtStatus, Direction.X);
		}
		else
		{
			_animation.PlayWeaponAnimations(false, Direction.X);
		}
	}
	
	public override void _Process(double delta)
	{
		SetComponentProperties();
		WeaponActions();
		
		if (WeaponAmmo <= 0)
		{
			WeaponType = Utility.WeaponType.PlayerPistol;
		}
	}
}
