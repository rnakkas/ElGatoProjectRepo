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
	
	private Vector2 _velocity = Vector2.Zero;
	private Dictionary<string, bool> _playerInputs;
	private float _direction;
	
	
	
	private bool _hurtStatus;
	private float _knockback;
	private Vector2 _attackVelocity;
	
	public override void _Ready()
	{
		_velocity = Velocity;
		// _hurtboxComponent.HitByAttack += PlayerHitByAttack;
	}

	// private void PlayerHitByAttack(
	// 	int damage, 
	// 	float knockback, 
	// 	Vector2 attackVelocity, 
	// 	float attackDirection)
	// {
	// 	_healthComponent.TakeDamage(damage);
	// 	_knockback = knockback;
	// 	_attackVelocity = attackVelocity;
	// }

	
	// Set direction
	private void SetDirection()
	{
		if (_playerInputs["move_left"])
			_direction = -1.0f;
		else if (_playerInputs["move_right"])
			_direction = 1.0f;
		else if (!_playerInputs["move_left"] || !_playerInputs["move_right"])
		{
			_direction = 0;
		}
	}
	private void PlayerMovement(float delta)
	{
		SetDirection();
		
		// Run and idle
		RunAndIdle();
		
		// Jumping and falling
		JumpingAndFalling(delta);
		
		// Wall Sliding and wall jumping
		WallSlideAndWallJump();
		
	}

	private void RunAndIdle()
	{
		if (_direction != 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, _direction * _playerStats.MaxSpeed, _playerStats.Acceleration);

			if (IsOnFloor())
			{
				_sprite.Play("run");
			}
			
			if (IsOnFloor())
			{
				_velocity.Y = 0;
			}
		}
		else if (IsOnFloor() && _direction == 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, _playerStats.Friction);
			_velocity.Y = 0;
			_sprite.Play("idle");
		}
	}

	private void JumpingAndFalling(float delta)
	{
		if (IsOnFloor() && _playerInputs["jump"])
		{
			_velocity.Y = _playerStats.JumpVelocity;
			_sprite.Play("jump");
		}

		if (!IsOnFloor())
		{
			_velocity.Y += _playerStats.Gravity * delta;

			if (_velocity.Y > 0)
			{
				_sprite.Play("fall");
			}
		}
	}

	private void WallSlideAndWallJump()
	{
		if (!IsOnFloor() && (_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding()))
		{
			_sprite.Play("wall_slide");
			_velocity.X = 0;
			_velocity.Y = Mathf.MoveToward(_velocity.Y, _playerStats.WallSlideVelocity, _playerStats.WallSlideGravity);

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
				_velocity.Y = _playerStats.WallJumpVelocity;;
				_velocity.X = _direction * _playerStats.MaxSpeed;
				_sprite.Play("jump");
			}
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
	
	public override void _PhysicsProcess(double delta)
	{
		_playerInputs = _playerController.GetInputs();
		
		PlayerMovement((float)delta);
		FlipSprite();
		
		Velocity = _velocity;
		MoveAndSlide();
	}
}
