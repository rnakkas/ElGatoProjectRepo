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
	
	[Export] private AnimatedSprite2D _sprite;
	[Export] private RayCast2D _leftWallDetect;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private Area2D _pickupsBox;
	[Export] private Area2D _miscBox;
	[Export] private Timer _hurtStaggerTimer;
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
		
		_hurtbox.GotHit += GotHitByAttack;

		_pickupsBox.AreaEntered += PlayerEnteredPickupArea;
		
		_miscBox.AreaEntered += EnteredJumpPad;
		
		_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
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

	private void GotHitByAttack(Dictionary attackData)
	{
		_health.TakeDamage((int)attackData["AttackDamage"]);

		_velocity.X = _velocityComponent.KnockbackFromAttack(
			(Vector2)attackData["AttackPosition"],
			(float)attackData["Knockback"],
			(Vector2)attackData["AttackVelocity"]
			);
	}

	// private void OnHitByAttack(Area2D attackArea)
	// {
	// 	if (!IsInstanceValid(attackArea))
	// 		return;
	// 	if (!attackArea.IsInGroup("EnemyProjectiles") && !attackArea.IsInGroup("EnemyAttacks")) 
	// 		return;
	// 	
	// 	int attackDamage = (int)attackArea.Get("BulletDamage");
	// 	
	// 	_playerStats.TakeDamage(attackDamage);
 //            		
	// 	KnockbackFromAttack(attackArea, knockback, attackVelocity);
 //            
	// 	FlipSpriteToFaceHitDirection(attackArea);
 //            
	// 	_hurtStatus = true;
	// 	_hurtStaggerTimer.Start();
 //            
	// 	_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
	// }
	
	
	// Picking up items - make pickups component handle it
	private void PlayerEnteredPickupArea(Area2D pickupArea)
	{
		if (!pickupArea.IsInGroup("HealthPickups"))
			return;
		if (_playerStats.CurrentHealth < _playerStats.MaxHealth)
		{
			EventsBus.Instance.EmitHealthPickupAttempt(pickupArea, _pickupsBox, true);
		}
	}
	
	// Healing items - make health component handle it
	private void HealthRestored(Area2D entityArea, int healAmount)
	{
		if (entityArea != _pickupsBox)
			return;
		
		_playerStats.Heal(healAmount);
		_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
	}
	
	// Sending data for velocity calculations
	private void SetVelocityComponentValues()
	{
		_velocityComponent.PlayerInputs = _playerController.GetInputs();
		_velocityComponent.EntityMovementData = new Dictionary<string, float>
		{
			{"MaxSpeed", _playerStats.MaxSpeed},
			{"Acceleration", _playerStats.Acceleration},
			{"Friction", _playerStats.Friction},
			{"JumpVelocity", _playerStats.JumpVelocity},
			{"Gravity", _playerStats.Gravity},
			{"WallSlideGravity", _playerStats.WallSlideGravity},
			{"WallJumpVelocity", _playerStats.WallJumpVelocity},
			{"WallSlideVelocity", _playerStats.WallSlideVelocity}
		};
		_velocityComponent.EntityBools = new Dictionary<string, bool>
		{
			{"IsOnFloor", IsOnFloor()},
			{"IsOnCeiling", IsOnCeiling()},
			{"IsLeftWallDetected", _leftWallDetect.IsColliding()},
			{"IsRightWallDetected", _rightWallDetect.IsColliding()}
		};
	}
	
	// Set direction
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
	private void PlayerMovements(float delta)
	{
		SetDirection();
		
		// Run and idle
		RunAndIdle(delta);
		
		// Jumping and falling
		JumpingAndFalling(delta);
		
		// Wall Sliding and wall jumping
		WallSlideAndWallJump(delta);
		
		// Hurt
		PlayerHurt();

	}

	private void RunAndIdle(float delta)
	{
		if (_direction != 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, _direction * _playerStats.MaxSpeed, _playerStats.Acceleration * delta);

			if (IsOnFloor())
			{
				_playerStats.State = Utility.EntityState.Run;
			}
			
			if (IsOnFloor())
			{
				_velocity.Y = 0;
			}
		}
		else if (IsOnFloor() && _direction == 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, _playerStats.Friction * delta);
			_velocity.Y = 0;
			_playerStats.State = Utility.EntityState.Idle;
		}
	}

	private void JumpingAndFalling(float delta)
	{
		if (IsOnFloor() && _playerInputs["jump"])
		{
			_velocity.Y = _playerStats.JumpVelocity;
			_playerStats.State = Utility.EntityState.Jump;
		}

		if (!IsOnFloor())
		{
			_velocity.Y += _playerStats.Gravity * delta;

			if (_velocity.Y > 0)
			{
				_playerStats.State = Utility.EntityState.Fall;
			}
		}

		if (IsOnCeiling())
		{
			_velocity.Y += _playerStats.Gravity * delta;
		}
	}

	private void WallSlideAndWallJump(float delta)
	{
		if (!IsOnFloor() && (_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding()))
		{
			_playerStats.State = Utility.EntityState.WallSlide;
			_velocity.X = 0;
			_velocity.Y = Mathf.MoveToward(_velocity.Y, _playerStats.WallSlideVelocity, _playerStats.WallSlideGravity * delta);

			if (_leftWallDetect.IsColliding())
			{
				_direction = 1.0f;
			} 
			
			if (_rightWallDetect.IsColliding())
			{
				_direction = -1.0f;
			}
			
			// Wall Jump
			if (_playerInputs["jump_justPressed"])
			{
				_velocity.Y = _playerStats.WallJumpVelocity;
				_velocity.X = _direction * _playerStats.MaxSpeed;
				_playerStats.State = Utility.EntityState.Jump;
			}
		}
	}
	
	private void PlayerHurt()
	{
		if (_hurtStatus)
		{ 
			_playerStats.State = Utility.EntityState.Hurt;
		}
	}

	private void FlipSprite()
	{
		if (_direction < 0)
		{
			_sprite.FlipH = true;
		}
		else if (_direction > 0)
		{
			_sprite.FlipH = false;
		}
	}

	private void FlipSpriteToFaceHitDirection(Area2D attackArea)
	{
		// Flip sprite if hit from behind
		if ((GlobalPosition - attackArea.GlobalPosition).Normalized().X < 0 && _sprite.IsFlippedH())
		{
			_sprite.FlipH = false;
		}
		else if ((GlobalPosition - attackArea.GlobalPosition).Normalized().X > 0 && !_sprite.IsFlippedH())
		{
			_sprite.FlipH = true;
		}
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

	private void PlayerAnimations()
	{
		switch (_playerStats.State)
		{
			case Utility.EntityState.Run:
				_sprite.Play("run");
				break;
			case Utility.EntityState.Idle:
				_sprite.Play("idle");
				break;
			case Utility.EntityState.Jump:
				_sprite.Play("jump");
				break;
			case Utility.EntityState.Fall:
				_sprite.Play("fall");
				break;
			case Utility.EntityState.WallSlide:
				_sprite.Play("wall_slide");
				break;
			case Utility.EntityState.Hurt:
				_sprite.Play("hurt");
				break;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerInputs = _playerController.GetInputs();

		SetVelocityComponentValues();
		_velocity = _velocityComponent.CalculateVelocity((float)delta);
		
		
		
		// PlayerMovements((float)delta);
		PlayerAnimations();
		FlipSprite();
		
		SetWeaponProperties();
		
		Velocity = _velocity;
		MoveAndSlide();
	}

	// Unsubscribe from events
	public override void _ExitTree()
	{
		
	}
}
