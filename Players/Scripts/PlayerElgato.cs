using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
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
		
		_hurtbox.AreaEntered += PlayerHitByAttack;
		_hurtStaggerTimer.OneShot = true;
		_hurtStaggerTimer.SetWaitTime(_playerStats.HurtStaggerTime);
		_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;

		_pickupsBox.AreaEntered += PlayerPickedUpItem;

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
	
	
	// Getting hit by attacks
	private void PlayerHitByAttack(Area2D area)
	{
		if (area.IsInGroup("EnemyProjectiles") || area.IsInGroup("EnemyAttacks"))
		{
			_enemyAttackArea = area;
			_playerStats.TakeDamage((int)area.Get("AttackDamage"));
			_hurtStatus = true;
			_hurtStaggerTimer.Start();
			
			_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
		}
	}

	private void HurtStaggerTimerTimedOut()
	{
		_hurtStatus = false;
	}
	
	// Picking up items
	private void PlayerPickedUpItem(Area2D area)
	{
		if (area.IsInGroup("Pickups"))
		{
			_playerStats.Heal((int)area.Get("HealAmount"));
			
			_debugHealthLabel.SetText("HP: " + _playerStats.CurrentHealth);
		}
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
				_playerStats.State = PlayerStats.PlayerState.Run;
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
			_playerStats.State = PlayerStats.PlayerState.Idle;
		}
	}

	private void JumpingAndFalling(float delta)
	{
		if (IsOnFloor() && _playerInputs["jump"])
		{
			_velocity.Y = _playerStats.JumpVelocity;
			_playerStats.State = PlayerStats.PlayerState.Jump;
		}

		if (!IsOnFloor())
		{
			_velocity.Y += _playerStats.Gravity * delta;

			if (_velocity.Y > 0)
			{
				_playerStats.State = PlayerStats.PlayerState.Fall;
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
			_playerStats.State = PlayerStats.PlayerState.WallSlide;
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
				_playerStats.State = PlayerStats.PlayerState.Jump;
			}
		}
	}
	
	private void PlayerHurt()
	{
		if (_hurtStatus)
		{ 
			_playerStats.State = PlayerStats.PlayerState.Hurt;
			
			if (!IsInstanceValid(_enemyAttackArea)) 
				return;
			
			// Knockback from attack
			float knockback = (float)_enemyAttackArea.Get("Knockback");
			Vector2 attackVelocity = (Vector2)_enemyAttackArea.Get("Velocity");
			KnockbackFromAttack(knockback, attackVelocity);
		}
	}

	private void KnockbackFromAttack(float knockback, Vector2 attackVelocity)
	{
		Vector2 attackPosition = _enemyAttackArea.GlobalPosition - GlobalPosition;
		
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
	}

	private void PlayerAnimations()
	{
		switch (_playerStats.State)
		{
			case PlayerStats.PlayerState.Run:
				_sprite.Play("run");
				break;
			case PlayerStats.PlayerState.Idle:
				_sprite.Play("idle");
				break;
			case PlayerStats.PlayerState.Jump:
				_sprite.Play("jump");
				break;
			case PlayerStats.PlayerState.Fall:
				_sprite.Play("fall");
				break;
			case PlayerStats.PlayerState.WallSlide:
				_sprite.Play("wall_slide");
				break;
			case PlayerStats.PlayerState.Hurt:
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
		
		_weapon.Direction = _direction;
		_weapon.HurtStatus = _hurtStatus;
		
		Velocity = _velocity;
		MoveAndSlide();
	}
}
