using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using Godot.Collections;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	[Export] private PlayerStats _playerStats;
	[Export] private PlayerControllerComponent _playerController;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private RayCast2D _leftWallDetect;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private Area2D _hurtbox;
	[Export] private Area2D _pickupsBox;
	[Export] private Area2D _miscBox;
	[Export] private Timer _hurtStaggerTimer;
	[Export] private WeaponElgato _weapon;
	[Export] private Label _debugHealthLabel;
	
	private Vector2 _velocity = Vector2.Zero;
	private Dictionary<string, bool> _playerInputs;
	private float _direction;
	private bool _hurtStatus;
	private float _knockback;
	private Vector2 _attackVelocity;
	private Area2D _enemyAttackArea;
	
	public override void _Ready()
	{
		_velocity = Velocity;
		
		EventsBus.Instance.AttackHit += HitByAttack;
		_hurtStaggerTimer.OneShot = true;
		_hurtStaggerTimer.SetWaitTime(_playerStats.HurtStaggerTime);
		_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;

		EventsBus.Instance.HealedPlayer += HealthRestored;
		_pickupsBox.AreaEntered += PlayerEnteredPickupArea;

		_miscBox.AreaEntered += EnteredJumpPad;
		
		_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
	}

	
	// Jumping on jump pad
	private void EnteredJumpPad(Area2D area)
	{
		if (area.IsInGroup("JumpPads"))
		{
			float jumpMultiplier = (float)area.Get("JumpMultiplier");
			_velocity.Y = jumpMultiplier * _playerStats.JumpVelocity;
		}
	}

	private void HitByAttack(Area2D area, int attackDamage, float knockback, Vector2 attackVelocity)
	{
		if (!area.IsInGroup("EnemyAttacks") || !area.IsInGroup("EnemyProjectiles"))
			return;
		
		_enemyAttackArea = area;
		
		_playerStats.TakeDamage(attackDamage);
		KnockbackFromAttack(area, knockback, attackVelocity);
		_hurtStatus = true;
		_hurtStaggerTimer.Start();
		
		_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
	}

	private void HurtStaggerTimerTimedOut()
	{
		_hurtStatus = false;
	}
	
	// Picking up items
	private void PlayerEnteredPickupArea(Area2D area)
	{
		if (area.IsInGroup("HealthPickups"))
		{
			EventsBus.Instance.EmitSignal(
				nameof(EventsBus.AttemptedHealthPickup),
				_playerStats.CurrentHealth, 
				_playerStats.MaxHealth
			);
		}
	}
	
	// Healing items
	private void HealthRestored(int healAmount)
	{
		_playerStats.Heal(healAmount);
		_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
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
			
			// if (!IsInstanceValid(_enemyAttackArea)) 
			// 	return;
			//
			// // Knockback from attack
			// float knockback = (float)_enemyAttackArea.Get("Knockback");
			// Vector2 attackVelocity = (Vector2)_enemyAttackArea.Get("Velocity");
			// KnockbackFromAttack(knockback, attackVelocity);
		}
	}

	private void KnockbackFromAttack(Area2D enemyAttackArea, float knockback, Vector2 attackVelocity)
	{
		Vector2 attackPosition = enemyAttackArea.GlobalPosition - GlobalPosition;
		
		if (attackVelocity != Vector2.Zero)
		{
			_velocity.X = knockback * attackVelocity.X;	
		} 
		else if (attackVelocity == Vector2.Zero && attackPosition.X < 0)
		{
			_velocity.X = knockback;
		}
		else if (attackVelocity == Vector2.Zero && attackPosition.X > 0)
		{
			_velocity.X = -knockback;
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
		
		// Flip sprite if hit from behind
		if (_enemyAttackArea == null)
			return;
		if (!IsInstanceValid(_enemyAttackArea))
			return;
		
		if ((GlobalPosition - _enemyAttackArea.GlobalPosition).Normalized().X < 0 && _sprite.IsFlippedH())
		{
			_sprite.FlipH = false;
		}
		else if ((GlobalPosition - _enemyAttackArea.GlobalPosition).Normalized().X > 0 && !_sprite.IsFlippedH())
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
		PlayerMovements((float)delta);
		PlayerAnimations();
		FlipSprite();
		
		SetWeaponProperties();
		
		Velocity = _velocity;
		MoveAndSlide();
	}
}
