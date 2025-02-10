using System;
using ElGatoProject.Players.Scripts;
using ElGatoProject.Singletons;
using Godot;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

// This component allows player control over entities
[GlobalClass]
public partial class PlayerControllerComponent : Node
{
	[Export] private HealthComponent _health;
	[Export] private HurtboxComponent _hurtbox;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private AnimationComponent _animation;
	[Export] private PickupsComponent _pickupsBox;
	
	[Export] private AnimatedSprite2D _sprite;
	[Export] private RayCast2D _leftWallDetect;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private Area2D _miscBox;
	[Export] private WeaponElgato _weapon;
	
	// Debug labels
	[Export] private Label _debugHealthLabel;
	[Export] private Label _debugScoreLabel;
	[Export] private Label _debugWeaponLabel;
	
	public Vector2 Velocity = Vector2.Zero;
	private float _direction;
	private bool _hurtStatus;
	public bool IsOnFloor, IsOnCeiling;
	private int _score;
	
	public void ConnectSignals()
	{
		if (_pickupsBox == null)
			return;
		_pickupsBox.CheckCurrentHealth += OnHealthCheck;
		_pickupsBox.PickedUpHealth += OnHealthPickedUp;
		_pickupsBox.PickedUpScoreItem += OnScoreItemPickup;
		_pickupsBox.PickedUpWeaponMod += OnWeaponModPickup;
		
		if (_hurtbox == null)
			return;
		_hurtbox.GotHit += OnHitByAttack;
		_hurtbox.HurtStatusCleared += OnHurtStatusCleared; 
		
		_miscBox.AreaEntered += EnteredJumpPad;
	}
	
	// Connected signal methods
	private void OnHealthCheck()
	{
		_pickupsBox.MaxHealth = _health.MaxHealth;
		_pickupsBox.CurrentHealth = _health.CurrentHealth;
	}
	
	private void OnHealthPickedUp(int healAmount)
	{
		_health?.Heal(healAmount);
	}
	
	private void OnHitByAttack(
		bool hurtStatus, 
		Vector2 attackPosition, 
		int attackDamage,
		float knockback, 
		Vector2 attackVelocity
	)
	{
		_hurtStatus = hurtStatus;
		
		_health?.TakeDamage(attackDamage);
		
		_animation?.FlipSpriteToFaceHitDirection(attackPosition);

		if (_velocityComponent == null)
			return;
		Velocity.X = _velocityComponent.KnockbackFromAttack(attackPosition, knockback, attackVelocity);
	}
	
	private void OnHurtStatusCleared(bool hurtStatus)
	{
		_hurtStatus = hurtStatus;
	}
	
	private void EnteredJumpPad(Area2D area)
	{
		if (area.IsInGroup("JumpPads"))
		{
			Velocity.Y = _velocityComponent.JumpOnJumpPad((float)area.Get("JumpMultiplier"));
		}
	}

	private void OnScoreItemPickup(int scorePoints)
	{
		_score += scorePoints;
	}

	private void OnWeaponModPickup(string modType)
	{
		if (Enum.TryParse(modType, out Utility.WeaponType weaponType))
		{
			_weapon.WeaponType = weaponType;
		}
	}
	
	// Helper functions
	private void SetDirections()
	{
		if (Input.IsActionPressed("move_left"))
		{
			_direction = -1.0f;
		}
		else if (Input.IsActionPressed("move_right"))
			_direction = 1.0f;
		else if (!Input.IsActionPressed("move_left") || !Input.IsActionPressed("move_right"))
		{
			_direction = 0;
		}
	}
	
	private void SetComponentProperties()
	{
		if (_animation == null)
			return;
		if (_velocityComponent == null)
			return;

		_animation.Direction = _direction;
		_animation.Velocity = Velocity;
		_animation.IsOnFloor = IsOnFloor;
		_animation.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_animation.IsRightWallDetected = _rightWallDetect.IsColliding();
		_animation.HurtStatus = _hurtStatus;
		
		_velocityComponent.IsOnFloor = IsOnFloor;
		_velocityComponent.IsOnCeiling = IsOnCeiling;
		_velocityComponent.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_velocityComponent.IsRightWallDetected = _rightWallDetect.IsColliding();
	}
	
	private void SetWeaponProperties()
	{
		if (_weapon == null)
			return;
		
		_weapon.HurtStatus = _hurtStatus;
		
		if (_sprite.IsFlippedH())
		{
			_weapon.Direction.X = -1.0f;
		}
		else if (!_sprite.IsFlippedH())
		{
			_weapon.Direction.X = 1.0f;
		}
	}

	public void PlayerControllerActions(float delta)
	{
		SetDirections();
		SetComponentProperties();
		SetWeaponProperties();

		Velocity = _velocityComponent.CalculateVelocity(delta, _direction);

		_animation.PlayCharacterAnimations();
		
		_debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
		_debugScoreLabel.SetText("Score: " + _score);
		_debugWeaponLabel.SetText(_weapon.WeaponType + ": " + _weapon.WeaponAmmo);
	}
}
