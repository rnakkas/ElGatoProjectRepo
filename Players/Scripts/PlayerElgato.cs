using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using Godot.Collections;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	// Resource
	[Export] private PlayerStats _playerStats;
	
	// Components
	[Export] private HealthComponent _health;
	[Export] private HurtboxComponent _hurtbox;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private PlayerControllerComponent _playerController;
	[Export] private AnimationComponent _animation;
	[Export] private PickupsComponent _pickupsBox;
	
	[Export] private AnimatedSprite2D _sprite;
	[Export] private RayCast2D _leftWallDetect;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private Area2D _miscBox;
	[Export] private WeaponElgato _weapon;
	[Export] private Label _debugHealthLabel;
	
	private Vector2 _velocity = Vector2.Zero;
	private Dictionary<string, bool> _playerInputs;
	private float _direction;
	private bool _hurtStatus;
	
	public override void _Ready()
	{
		_velocity = Velocity;

		_health.CurrentHealth = _playerStats.CurrentHealth;
		_health.MaxHealth = _playerStats.MaxHealth;
		
		_hurtbox.GotHit += OnHitByAttack;
		_hurtbox.HurtStatusCleared += OnHurtStatusCleared; 

		_pickupsBox.MaxHealth = _health.MaxHealth;
		// _pickupsBox.AreaEntered += PlayerEnteredPickupArea;
		_pickupsBox.PickedUpHealth += OnHealthPickedUp;
		
		_miscBox.AreaEntered += EnteredJumpPad;
		
		_debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
	}
	
	private void SetDirection()
	{
		if (_playerInputs["move_left"])
		{
			_direction = -1.0f;
		}
		else if (_playerInputs["move_right"])
			_direction = 1.0f;
		else if (!_playerInputs["move_left"] || !_playerInputs["move_right"])
		{
			_direction = 0;
		}
	}
	
	// Jumping on jump pad
	private void EnteredJumpPad(Area2D area)
	{
		if (area.IsInGroup("JumpPads"))
		{
			_velocity.Y = _velocityComponent.JumpOnJumpPad(
				(float)area.Get("JumpMultiplier"), 
				_playerStats.JumpVelocity);
		}
	}

	// Getting hit by enemy attacks
	private void OnHitByAttack(
		bool hurtStatus, 
		Vector2 attackPosition, 
		int attackDamage,
		float knockback, 
		Vector2 attackVelocity
		)
	{
		_hurtStatus = hurtStatus;
		
		_health.TakeDamage(attackDamage);

		_velocity.X = _velocityComponent.KnockbackFromAttack(attackPosition, knockback, attackVelocity);
		
		_animation.FlipSpriteToFaceHitDirection(attackPosition);
	}

	private void OnHurtStatusCleared(bool hurtStatus)
	{
		_hurtStatus = hurtStatus;
	}
	
	// Picking up items - make pickups component handle it
	// private void PlayerEnteredPickupArea(Area2D pickupArea)
	// {
	// 	if (pickupArea.IsInGroup("HealthPickups"))
	// 	{
	// 		_pickupsBox.CurrentHealth = _health.CurrentHealth;
	// 	}
	// 	
	// 	// if (!pickupArea.IsInGroup("HealthPickups"))
	// 	// 	return;
	// 	// if (_playerStats.CurrentHealth < _playerStats.MaxHealth)
	// 	// {
	// 	// 	EventsBus.Instance.EmitHealthPickupAttempt(pickupArea, _pickupsBox, true);
	// 	// }
	// }
	
	// Healing items - make health component handle it
	
	private void OnHealthPickedUp(int healAmount)
	{
		_health.Heal(healAmount);
	}
	
	// Setting data for velocity calculations
	private Vector2 CalculateVelocity(float delta)
	{
		if (_velocityComponent == null) 
			return Vector2.Zero;
		
		_velocityComponent.PlayerInputs = _playerController.GetInputs();
		_velocityComponent.MaxSpeed = _playerStats.MaxSpeed;
		_velocityComponent.Acceleration = _playerStats.Acceleration;
		_velocityComponent.Friction = _playerStats.Friction;
		_velocityComponent.JumpVelocity = _playerStats.JumpVelocity;
		_velocityComponent.Gravity = _playerStats.Gravity;
		_velocityComponent.WallSlideGravity = _playerStats.WallSlideGravity;
		_velocityComponent.WallJumpVelocity = _playerStats.WallJumpVelocity;
		_velocityComponent.WallSlideVelocity = _playerStats.WallSlideVelocity;
		_velocityComponent.IsOnFloor = IsOnFloor();
		_velocityComponent.IsOnCeiling = IsOnCeiling();
		_velocityComponent.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_velocityComponent.IsRightWallDetected = _rightWallDetect.IsColliding();
		
		return _velocityComponent.CalculateVelocity(delta, _direction);
	}

	// Setting data for animations
	private void PlayerAnimations()
	{
		if (_animation == null)
			return;

		_animation.Direction = _direction;
		_animation.Velocity = _velocity;
		_animation.IsOnFloor = IsOnFloor();
		_animation.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_animation.IsRightWallDetected = _rightWallDetect.IsColliding();
		_animation.HurtStatus = _hurtStatus;
		
		_animation.PlayAnimations();
	}
	
	private void SetWeaponProperties()
	{
		_weapon.HurtStatus = _hurtStatus;
		
		if (_sprite.IsFlippedH())
		{
			_weapon.Direction = -1.0f;
		}
		else if (!_sprite.IsFlippedH())
		{
			_weapon.Direction = 1.0f;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerInputs = _playerController.GetInputs();
		SetDirection();

		_velocity = CalculateVelocity((float)delta);

		_pickupsBox.CurrentHealth = _health.CurrentHealth;

		PlayerAnimations();
		
		SetWeaponProperties();
		
		Velocity = _velocity;
		MoveAndSlide();
		
		_debugHealthLabel.SetText("HP:" + _health.CurrentHealth);
	}
}
