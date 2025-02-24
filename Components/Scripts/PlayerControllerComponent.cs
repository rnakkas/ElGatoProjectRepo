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
	[Export] private CharacterBody2D _playerCharacter;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private VelocityComponent _velocityComponent;
	[Export] private RayCast2D _leftWallDetect;
	[Export] private RayCast2D _rightWallDetect;
	[Export] private Area2D _miscBox;
	[Export] private Timer _dashCooldownTimer;
	[Export] private Timer _dashTimer;

	private bool _isDashing,_onDashCooldown, _hurtStatus, _wallSlideStatus;
	private Vector2 _velocity, _direction = Vector2.Zero;

	public override void _Ready()
	{
		ConnectSignals();
	}

	public void ConnectSignals()
	{
		// _miscBox.AreaEntered += EnteredJumpPad;

		_dashCooldownTimer.Timeout += OnDashCooldownTimerTimeout;
		
		_dashTimer.Timeout += OnDashTimerTimeout;
	}
	
	private void EnteredJumpPad(Area2D area)
	{
		if (area.IsInGroup("JumpPads"))
		{
			_velocity.Y = _velocityComponent.JumpOnJumpPad((float)area.Get("JumpMultiplier"));
		}
	}

	private void OnDashCooldownTimerTimeout()
	{
		_onDashCooldown = false;
	}

	private void OnDashTimerTimeout()
	{
		_isDashing = false;
		_velocity = Vector2.Zero;
	}
	
	public Vector2 BasicMovements(float delta)
	{
		if (!_playerCharacter.IsOnFloor() && !_isDashing && !_wallSlideStatus)
		{
			_velocity.Y += _velocityComponent.FallVelocity(delta);
		}
		else
		{
			_velocity.Y = _velocityComponent.OnFloorVelocity();
		}

		if (_playerCharacter.IsOnCeiling() && !_isDashing && !_wallSlideStatus)
		{
			_velocity.Y += _velocityComponent.FallVelocity(delta);
		}
		
		_direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		if (!_hurtStatus && !_isDashing && !_wallSlideStatus)
		{
			if (_direction != Vector2.Zero)
			{
				_velocity.X = _velocityComponent.AccelerateToMaxVelocity(delta, _direction, _velocity);
			}
			else if (_direction == Vector2.Zero)
			{
				_velocity.X = _velocityComponent.DecelerateToZeroVelocity(delta, _velocity);
			}
		
			if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor() )
			{
				_velocity.Y = _velocityComponent.JumpeVelocity();
			}
		}
		
		return _velocity;
	}

	public Vector2 Dash()
	{
		if (!Input.IsActionJustPressed("dashDodge") || _onDashCooldown || _hurtStatus) 
			return _velocity;
		
		if (!_sprite.IsFlippedH())
		{
			_velocity = _velocityComponent.DashVelocity(Vector2.Right);
		}
		else if (_sprite.IsFlippedH())
		{
			_velocity = _velocityComponent.DashVelocity(Vector2.Left);
		}

		_isDashing = true;
		_dashTimer.Start();
			
		_onDashCooldown = true;
		_dashCooldownTimer.Start();
		
		return _velocity;
	}

	public Vector2 WallSlideAndWallJump(float delta)
	{
		if (
			(_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding()) &&
			!_playerCharacter.IsOnFloor()
		)
		{
			_wallSlideStatus = true;
			
			_velocity.Y += _velocityComponent.WallSlidingVelocity(delta);
			
			

			if (_leftWallDetect.IsColliding() && Input.IsActionJustPressed("jump"))
			{
				_sprite.FlipH = false;
				_direction = new Vector2(1.0f, 0);
				_velocity = _velocityComponent.WallJumpingVelocity(_direction);
			}
			else if (_rightWallDetect.IsColliding() && Input.IsActionJustPressed("jump"))
			{
				_sprite.FlipH = true;
				_direction = new Vector2(-1.0f, 0);
				_velocity = _velocityComponent.WallJumpingVelocity(_direction);
			}
			
		}
		else
		{
			_wallSlideStatus = false;
		}
		
		return _velocity;
	}

	
}
