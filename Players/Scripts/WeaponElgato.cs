using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Players.Scripts;

/*
 * NOTES:
 * This could be turned into a weapon manager class which handles weapon ammo, switching weapons on pickup etc
 */
public partial class WeaponElgato : Node2D
{
	[Export] private AnimatedSprite2D _characterSprite;
	[Export] private PlayerControllerComponent _playerController;
	[Export] private PickupsComponent _pickupsComponent;

	private ShootingComponent _shootingComponent;
	private AnimatedSprite2D _weaponSprite;
	
	private Vector2 _direction;
	private int _weaponAmmo;
	private Vector2 _weaponPosition;
	private AnimatedSprite2D _flashSprite;
	private Marker2D _muzzle;
	
	public override void _Ready()
	{
		GetChildNodes();
		ConnectSignals();
		
		_weaponPosition = Position;
		
		SetWeaponMuzzleFlash(Utility.WeaponType.PlayerPistol);
		
		_weaponSprite?.Play(Utility.EntityAnimations[Utility.EntityState.Idle]);
		_flashSprite?.Play(Utility.EntityAnimations[Utility.EntityState.Idle]);

		_shootingComponent.PlayerOrEnemy = Utility.PlayerOrEnemy.Player;
	}

	private void GetChildNodes()
	{
		_weaponSprite = GetNodeOrNull<AnimatedSprite2D>("weaponSprite");
		_muzzle = GetNodeOrNull<Marker2D>("muzzle");
		_shootingComponent = GetNodeOrNull<ShootingComponent>("ShootingComponent");
	}

	private void ConnectSignals()
	{
		if (_shootingComponent != null) _shootingComponent.Shooting += OnShooting;
		
		if (_pickupsComponent != null) _pickupsComponent.PickedUpWeaponPowerUp += OnWeaponPowerUpPickedUp;
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
		_weaponSprite?.SetSpeedScale(_shootingComponent.ShootingProperties.AnimationSpeed);
		
		_weaponSprite?.Play(Utility.EntityAnimations[Utility.EntityState.Shoot]);
		_flashSprite?.Play(Utility.EntityAnimations[Utility.EntityState.Shoot]);

		_playerController?.SetState(Utility.EntityState.IdleShoot);
		
		// Only reduce ammo for power-up weapons
		if (_shootingComponent?.WeaponType == Utility.WeaponType.PlayerPistol) 
			return;
		_weaponAmmo--;
		
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerAmmoUpdate), _weaponAmmo);
	}

	private void SwitchWeapon(Utility.WeaponType weaponType)
	{
		if (_shootingComponent != null) _shootingComponent.WeaponType = weaponType;
		SetWeaponMuzzleFlash(weaponType);

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
		
		if (_shootingComponent != null) _weaponAmmo = _shootingComponent.ShootingProperties.MagazineSize;
		
		// Set the updated timer values for the new weapon type
		_shootingComponent?.SetTimerValues(); 
		
		// For HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerCurrentWeaponUpdate), weaponType.ToString());
		EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerAmmoUpdate), _weaponAmmo);
	}
	
	public bool HandleShooting(Utility.EntityState currentState)
	{
		var shooting = false;
		if (!CanShoot(currentState))
		{
			WeaponIdleAnimation();
		}

		switch (_shootingComponent.ShootingProperties.TriggerType)
		{
			case Utility.WeaponTriggerType.Automatic:
			case Utility.WeaponTriggerType.SingleShot:
				if (Input.IsActionPressed("shoot"))
				{
					_shootingComponent?.Shoot(_direction);
					shooting = true;
				}
				else
					// shooting = false;
					WeaponIdleAnimation();
				break;
				// TODO: Keeping this here just in case I want to use it later
				// if (Input.IsActionJustPressed("shoot"))
				// 	_shootingComponent?.Shoot(_direction);
				// else
				// 	WeaponIdleAnimation();
				// break;
    
			case Utility.WeaponTriggerType.None:
				break;
    
			default:
				throw new ArgumentOutOfRangeException($"TriggerType does not exist");
		}
		
		HandleWeaponState();
		return shooting;
	}
	
	private bool CanShoot(Utility.EntityState currentState)
	{
		return currentState != Utility.EntityState.Dash && 
		       currentState != Utility.EntityState.Hurt;
	}
	
	private void WeaponIdleAnimation()
	{
		// Set speed scale back to 1 on idle
		_weaponSprite?.SetSpeedScale(1);

		// Sync to character's idle animation after shooting
		if (_weaponSprite != null && !_weaponSprite.IsPlaying())
		{
			_weaponSprite.Play(Utility.EntityAnimations[Utility.EntityState.Idle]);
			_weaponSprite.Frame = _characterSprite.Frame;
		}

		// // Sync to character's idle animation
		// if (
		// 	_characterSprite != null &&
		// 	_weaponSprite != null &&
		// 	!_weaponSprite.IsPlaying() &&
		// 	_characterSprite.IsPlaying() &&
		// 	_characterSprite.Animation == Utility.EntityAnimations[Utility.EntityState.Idle]
		// )
		// {
		// 	_weaponSprite.Play(Utility.EntityAnimations[Utility.EntityState.Idle]);
		// 	_weaponSprite.Frame = _characterSprite.Frame;
		// 	GD.Print("llllll");
		// }

		// Idle muzzle flash animation, i.e. no muzzle flash
		if (_flashSprite != null && !_flashSprite.IsPlaying())
			_flashSprite.Play(Utility.EntityAnimations[Utility.EntityState.Idle]);
	}
	
	private void HandleWeaponState()
	{
		// If weapon power-up runs out of ammo, switch back to pistol
		if (_weaponAmmo <= 0)
		{
			SwitchWeapon(Utility.WeaponType.PlayerPistol);
		}
		
		// If player dies, despawn weapon
		if (_playerController?.CurrentState == Utility.EntityState.Death)
		{
			QueueFree();
		}
	}
	
	private void SetWeaponDirection()
	{
		if (_characterSprite == null) 
			return;
		
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

	private void SetWeaponMuzzleFlash(Utility.WeaponType weaponType)
	{
		// Remove current muzzle flash sprite
		if (_flashSprite != null)
			_muzzle.RemoveChild(_flashSprite); 
				
		// Add new muzzle flash sprite
		_flashSprite = ResourceLoader
			.Load<PackedScene>(Utility.MuzzleFlashPackedScenePaths[weaponType])
			.Instantiate<AnimatedSprite2D>();
		
		_muzzle?.AddChild(_flashSprite);
	}
	
	public override void _Process(double delta)
	{
		SetWeaponDirection();
		FlipSprite();
		// HandleShooting();
	}
}
