using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private ShootingComponent _shooting;
	[Export] private AnimatedSprite2D _weaponSprite, _flashSprite, _characterSprite;
	[Export] private PlayerControllerComponent _playerController;
	
	[Export] private Label _debugWeaponLabel;

	private Vector2 _direction;
	// public bool HurtStatus, IsDashing;
	private int _weaponAmmo;
	private Vector2 _weaponPosition;
	
	public override void _Ready()
	{
		ConnectToSignals();
		
		_weaponPosition = Position;
		
		_weaponSprite.Play(Utility.Instance.EntityIdleAnimation);
		_flashSprite.Play(Utility.Instance.EntityIdleAnimation);
	}

	private void ConnectToSignals()
	{
		_shooting.Shooting += OnShooting;
	}

	// Shooting signal connection
	private void OnShooting()
	{
		// _animation.PlayWeaponAnimations(true, _shooting.WeaponType);
		
		_weaponSprite.Play(Utility.Instance.PistolShootAnimation);
		_flashSprite.Play(Utility.Instance.EntityShootAnimation);
		
		
		// Only reduce ammo for power-up weapons
		if (_shooting.WeaponType != Utility.WeaponType.PlayerPistol)
			_weaponAmmo--;
	}
	
	// private void SetComponentProperties()
	// {
	// 	_shooting.TargetVector = Direction;
	// 	_shooting.HurtStatus = HurtStatus;
	// }

	//TODO: set the animation speed in the weapon properties
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
		if (_characterSprite.IsFlippedH())
		{
			_direction = Vector2.Left;
		}
		else if (!_characterSprite.IsFlippedH())
		{
			_direction = Vector2.Right;
		}
		
		if (
			Input.IsActionPressed("shoot") && 
			_playerController.CurrentState != Utility.CharacterState.Dash &&
			_playerController.CurrentState != Utility.CharacterState.Hurt
			)
		{
			_shooting.Shoot(_direction);
		}
		else
		{
			// _animation.PlayWeaponAnimations(false, _shooting.WeaponType);
			
			_weaponSprite.Play(Utility.Instance.EntityIdleAnimation);
			_flashSprite.Play(Utility.Instance.EntityIdleAnimation);
			
		}
		
		// If weapon power-up ammo runs out, return to pistol
		if (_weaponAmmo <= 0)
		{
			SwitchWeapon(Utility.WeaponType.PlayerPistol);
		}
	}
	
	private void FlipSprite()
	{
		if (_weaponSprite == null || _flashSprite == null) 
			return;

		switch (_direction.X)
		{
			case < 0:
				_weaponSprite.FlipH = true;
				_flashSprite.FlipH = true;
				Position = new Vector2(-_weaponPosition.X, _weaponPosition.Y);
				break;
			case > 0:
				_weaponSprite.FlipH = false;
				_flashSprite.FlipH = false;
				Position = new Vector2(_weaponPosition.X, _weaponPosition.Y);
				break;
		}
	}
	
	public override void _Process(double delta)
	{
		// SetComponentProperties();
		
		WeaponActions();
		
		FlipSprite();

		// if (Direction.X < 0)
		// {
		// 	Position = new Vector2(-_weaponPosition.X, _weaponPosition.Y);
		// }
		// else if (Direction.X > 0)
		// {
		// 	Position = new Vector2(_weaponPosition.X, _weaponPosition.Y);
		// }
		
		
		_debugWeaponLabel.SetText(_shooting.WeaponType + ": " + _weaponAmmo);
	}
}
