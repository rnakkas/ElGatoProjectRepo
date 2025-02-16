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
	[Export] private Timer _dashCooldownTimer;
	[Export] private Timer _dashTimer;
	
	// Debug labels
	[Export] private Label _debugHealthLabel;
	[Export] private Label _debugScoreLabel;
	
	public Vector2 Velocity = Vector2.Zero;
	private Vector2 _directionVector = Vector2.Zero;
	private bool _hurtStatus, _onDashCooldown, _isDashing;
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

		_dashCooldownTimer.Timeout += OnDashCooldownTimerTimeout;
		
		_dashTimer.Timeout += OnDashTimerTimeout;
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

		_animation?.FlipSprite(attackPosition);

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
			_weapon.SwitchWeapon(weaponType);
		}
	}

	private void OnDashCooldownTimerTimeout()
	{
		_onDashCooldown = false;
	}

	private void OnDashTimerTimeout()
	{
		_isDashing = false;
		
		// Set velocity x to 0 once dashing has finished
		// Velocity.X = _velocityComponent.DashingVelocityCalculations(_directionVector);
	}
	
	// Helper functions
	private void SetComponentProperties()
	{
		if (_animation == null)
			return;
		if (_velocityComponent == null)
			return;

		_animation.Direction = _directionVector;
		_animation.Velocity = Velocity;
		_animation.IsOnFloor = IsOnFloor;
		_animation.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_animation.IsRightWallDetected = _rightWallDetect.IsColliding();
		_animation.HurtStatus = _hurtStatus;
		_animation.IsDashing = _isDashing;
		
		_velocityComponent.IsOnFloor = IsOnFloor;
		_velocityComponent.IsOnCeiling = IsOnCeiling;
		_velocityComponent.IsLeftWallDetected = _leftWallDetect.IsColliding();
		_velocityComponent.IsRightWallDetected = _rightWallDetect.IsColliding();
		_velocityComponent.IsDashing = _isDashing;
	}
	
	private void SetWeaponProperties()
	{
		if (_weapon == null)
			return;
		
		_weapon.HurtStatus = _hurtStatus;
		_weapon.IsDashing = _isDashing;
		
		if (_animation.Sprite.IsFlippedH())
		{
			_weapon.Direction = new Vector2(-1.0f, 0f);
		}
		else if (!_animation.Sprite.IsFlippedH())
		{
			_weapon.Direction = new Vector2(1.0f, 0f);
		}
	}

	// Player controls
	private void MovementLogic(float delta)
	{
		BasicMovements(delta);
		Dash();
		Velocity = _velocityComponent.CalculateVelocity(delta, _directionVector);
	}

	private void BasicMovements(float delta)
	{
		if (Input.IsActionPressed("move_left") && !_isDashing)
		{
			_directionVector = Vector2.Left;
		}
		else if (Input.IsActionPressed("move_right") && !_isDashing)
		{
			_directionVector = Vector2.Right;
		}
		else if (
			(!Input.IsActionPressed("move_left") || !Input.IsActionPressed("move_right")) && 
			!_isDashing
		)
		{
			_directionVector = Vector2.Zero;
		}
		
		if (Input.IsActionPressed("jump"))
		{
			_directionVector = Vector2.Up;
		}
		
		// Velocity = _velocityComponent.CalculateVelocity(delta, _directionVector);

	}

	//TODO: Logic on being able to move through bullets and enemies and not take damage when dashing
	//TODO: When dashing, gravity will not affect player - DONE
	//TODO: When dashing, player will not be able to shoot/attack
	//TODO: WHen dashing player will not be able to jump - DONE
	private void Dash()
	{
		if (!Input.IsActionJustPressed("dashDodge") || _onDashCooldown || _hurtStatus) 
			return;
		
		if (!_animation.Sprite.IsFlippedH())
		{
			_directionVector = Vector2.Right;
		}
		else if (_animation.Sprite.IsFlippedH())
		{
			_directionVector = Vector2.Left;
		}

		_isDashing = true;
		_dashTimer.Start();
			
		_onDashCooldown = true;
		_dashCooldownTimer.Start();

		// Velocity.X = _velocityComponent.DashingVelocityCalculations(_directionVector);
	}

	public void PlayerControllerActions(float delta)
	{
		SetComponentProperties();
		SetWeaponProperties();
		MovementLogic(delta);

		_animation.PlayCharacterAnimations();
		
		_debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
		_debugScoreLabel.SetText("Score: " + _score);
	}
}
