using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	// Components
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
	[Export] private Label _debugHealthLabel;
	
	private Vector2 _velocity = Vector2.Zero;
	private float _direction;
	private bool _hurtStatus;
	
	public override void _Ready()
	{
		_velocity = Velocity;
		
		if (_pickupsBox == null)
			return;
		_pickupsBox.MaxHealth = _health.MaxHealth;
		_pickupsBox.CurrentHealth = _health.CurrentHealth;
		_pickupsBox.PickedUpHealth += OnHealthPickedUp;
		
		if (_hurtbox == null)
			return;
		_hurtbox.GotHit += OnHitByAttack;
		_hurtbox.HurtStatusCleared += OnHurtStatusCleared; 
		
		_miscBox.AreaEntered += EnteredJumpPad;
		
		_debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
	}
	
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
	
	// Jumping on jump pad
	private void EnteredJumpPad(Area2D area)
	{
		if (area.IsInGroup("JumpPads"))
		{
			_velocity.Y = _velocityComponent.JumpOnJumpPad((float)area.Get("JumpMultiplier"));
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
		if (_health == null)
			return;
		if (_pickupsBox == null)
			return;
		if (_velocityComponent == null)
			return;
		if (_animation == null)
			return;
		
		_hurtStatus = hurtStatus;
		
		_health.TakeDamage(attackDamage);
		_pickupsBox.CurrentHealth = _health.CurrentHealth;

		_velocity.X = _velocityComponent.KnockbackFromAttack(attackPosition, knockback, attackVelocity);
		
		_animation.FlipSpriteToFaceHitDirection(attackPosition);
	}

	private void OnHurtStatusCleared(bool hurtStatus)
	{
		_hurtStatus = hurtStatus;
	}
	
	// Healing items
	private void OnHealthPickedUp(int healAmount)
	{
		if (_health == null)
			return;
		if (_pickupsBox == null)
			return;
		
		_health.Heal(healAmount);
		_pickupsBox.CurrentHealth = _health.CurrentHealth;
	}
	
	// Setting data for components
	private void SetComponentProperties()
	{
		if (_animation == null)
			return;
		if (_velocityComponent == null)
			return;

		_animation.Direction = _direction;
		_animation.Velocity = _velocity;
		_animation.IsOnFloor = IsOnFloor();
		_animation.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_animation.IsRightWallDetected = _rightWallDetect.IsColliding();
		_animation.HurtStatus = _hurtStatus;
		
		_velocityComponent.IsOnFloor = IsOnFloor();
		_velocityComponent.IsOnCeiling = IsOnCeiling();
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
			_weapon.Direction = -1.0f;
		}
		else if (!_sprite.IsFlippedH())
		{
			_weapon.Direction = 1.0f;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		SetDirections();
		SetComponentProperties();
		SetWeaponProperties();

		_velocity = _velocityComponent.CalculateVelocity((float)delta, _direction);
		Velocity = _velocity;

		_animation.PlayCharacterAnimations();
		
		MoveAndSlide();
		
		_debugHealthLabel.SetText("HP:" + _health.CurrentHealth);
	}
}
