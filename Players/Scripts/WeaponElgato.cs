using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private ShootingComponent _shooting;
	[Export] private AnimatedSprite2D _weaponSprite, _flashSprite, _characterSprite;
	[Export] private PlayerControllerComponent _playerController;
	[Export] private PickupsComponent _pickupsComponent;
	
	[Export] private Label _debugWeaponLabel;

	private Vector2 _direction;
	private int _weaponAmmo;
	private Vector2 _weaponPosition;
	
	public override void _Ready()
	{
		ConnectSignals();
		
		_weaponPosition = Position;
		
		_weaponSprite.Play(Utility.Instance.EntityIdleAnimation);
		_flashSprite.Play(Utility.Instance.EntityIdleAnimation);

		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerCurrentWeaponUpdate),
			_shooting.WeaponType.ToString());
	}

	private void ConnectSignals()
	{
		if (_shooting == null)
			return;
		_shooting.Shooting += OnShooting;
		
		if (_pickupsComponent == null)
			return;
		_pickupsComponent.PickedUpWeaponPowerUp += OnWeaponPowerUpPickedUp;
	}

	private void OnWeaponPowerUpPickedUp(string weaponPowerUp)
	{
		if (Enum.TryParse(weaponPowerUp, out Utility.WeaponType weaponType))
		{
			SwitchWeapon(weaponType);
		}
	}
	
	// Shooting signal connection
	private void OnShooting()
	{
		// Change speed scale of animations based on weapon type from shooting properties
		_weaponSprite.SetSpeedScale(_shooting.ShootingProperties.AnimationSpeed);
		_flashSprite.SetSpeedScale(_shooting.ShootingProperties.AnimationSpeed);
		
		_weaponSprite.Play(Utility.Instance.EntityShootAnimation);
		_flashSprite.Play(Utility.Instance.EntityShootAnimation);
	
		
		// Only reduce ammo for power-up weapons
		if (_shooting.WeaponType == Utility.WeaponType.PlayerPistol) 
			return;
		_weaponAmmo--;
		
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerAmmoUpdate), _weaponAmmo);
	}

	private void SwitchWeapon(Utility.WeaponType weaponType)
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
		
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerCurrentWeaponUpdate), weaponType.ToString());
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerAmmoUpdate), _weaponAmmo);
	}

	private void WeaponActions()
	{
		if (
			Input.IsActionPressed("shoot") && 
			_playerController?.CurrentState != Utility.CharacterState.Dash &&
			_playerController?.CurrentState != Utility.CharacterState.Hurt
			)
		{
			_shooting.Shoot(_direction);
		}
		else
		{
			// Set speed scale back to 1 for idle animations
			_weaponSprite.SetSpeedScale(1);
			_flashSprite.SetSpeedScale(1);
			
			if (!_weaponSprite.IsPlaying())
				_weaponSprite.Play(Utility.Instance.EntityIdleAnimation);
			if (!_flashSprite.IsPlaying())
				_flashSprite.Play(Utility.Instance.EntityIdleAnimation);
			
		}
		
		// If weapon power-up ammo runs out, return to pistol
		if (_weaponAmmo <= 0)
		{
			SwitchWeapon(Utility.WeaponType.PlayerPistol);
		}
	}

	private void SetWeaponDirection()
	{
		if (_characterSprite.IsFlippedH())
		{
			_direction = Vector2.Left;
		}
		else if (!_characterSprite.IsFlippedH())
		{
			_direction = Vector2.Right;
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
		SetWeaponDirection();
		FlipSprite();
		WeaponActions();
		
		_debugWeaponLabel.SetText(_shooting.WeaponType + ": " + _weaponAmmo);
	}
}
