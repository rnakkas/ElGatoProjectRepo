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
	[Export] private Timer _dashCooldownTimer;
	[Export] private Timer _dashTimer;

	private bool _isDashing, _onDashCooldown;
	private Vector2 _velocity, _direction = Vector2.Zero;
	public Utility.CharacterState CurrentState;
	public Vector2 KnocbackVelocity;

	public override void _Ready()
	{
		ConnectSignals();
	}

	private void ConnectSignals()
	{
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
		GD.Print("dash ready");
	}

	private void OnDashTimerTimeout()
	{
		_isDashing = false;
	}
	
	// State machine

	public void SetState(Utility.CharacterState newState)
	{
		if (newState == CurrentState)
			return;

		ExitState();
		CurrentState = newState;
		EnterState();
	}

	private void ExitState()
	{
		switch (CurrentState)
		{
			case Utility.CharacterState.Idle:
				break;
			case Utility.CharacterState.Run:
				break;
			case Utility.CharacterState.Jump:
				break;
			case Utility.CharacterState.Fall:
				break;
			case Utility.CharacterState.Hurt:
				_velocity = Vector2.Zero;
				break;
			case Utility.CharacterState.WallSlide:
				break;
			case Utility.CharacterState.WallJump:
				break;
			case Utility.CharacterState.Dash:
				_velocity = Vector2.Zero;
				break;
		}
	}

	private void EnterState()
	{
		switch (CurrentState)
		{
			case Utility.CharacterState.Idle:
				_velocity.Y = 0;
				_sprite.Play(Utility.Instance.EntityIdleAnimation);
				break;
			case Utility.CharacterState.Run:
				_sprite.Play(Utility.Instance.EntityRunAnimation);
				break;
			case Utility.CharacterState.Jump:
				_velocity.Y = _velocityComponent.JumpeVelocity();
				_sprite.Play(Utility.Instance.EntityJumpAnimation);
				break;
			case Utility.CharacterState.Fall:
				_sprite.Play(Utility.Instance.EntityFallAnimation);
				break;
			case Utility.CharacterState.Hurt:
				_velocity = KnocbackVelocity;
				_sprite.Play(Utility.Instance.EntityHurtAnimation);
				break;
			case Utility.CharacterState.WallSlide:
				_sprite.Play(Utility.Instance.EntityWallSlideAnimation);
				break;
			case Utility.CharacterState.WallJump:
				_velocity = _velocityComponent.WallJumpingVelocity(_direction);
				_sprite.Play(Utility.Instance.EntityJumpAnimation);
				break;
			case Utility.CharacterState.Dash:
				GD.Print("Dash");
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
				
				_sprite.Play(Utility.Instance.EntityDashAnimation);
				break;
		}
	}

	public Vector2 UpdateState(float delta)
	{
		_direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		FlipSprite(_direction);
		
		switch (CurrentState)
		{
			case Utility.CharacterState.Idle:
				_velocity.X = _velocityComponent.DecelerateToZeroVelocity(delta, _velocity);
				
				if (_direction != Vector2.Zero) 
					SetState(Utility.CharacterState.Run);
				else if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
					SetState(Utility.CharacterState.Fall);
				else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor()) 
					SetState(Utility.CharacterState.Jump);
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown) 
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Run:
				_velocity.X = _velocityComponent.AccelerateToMaxVelocity(delta, _direction, _velocity);
				
				if (_direction == Vector2.Zero)
					SetState(Utility.CharacterState.Idle);
				else if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
					SetState(Utility.CharacterState.Fall);
				else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor())
					SetState(Utility.CharacterState.Jump); 
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Jump:
				_velocity.X = _velocityComponent.AccelerateToMaxVelocity(delta, _direction, _velocity);

				if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
				{
					_velocity.Y += _velocityComponent.FallVelocity(delta);

					if (_velocity.Y > 0)
					{
						SetState(Utility.CharacterState.Fall);
					}
				}
				else if (
					!_playerCharacter.IsOnFloor() &&
					(_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
				)
				{
					SetState(Utility.CharacterState.WallSlide);
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Fall:
				_velocity.X = _velocityComponent.AccelerateToMaxVelocity(delta, _direction, _velocity);
				_velocity.Y += _velocityComponent.FallVelocity(delta);
				
				if (_playerCharacter.IsOnFloor())
					SetState(Utility.CharacterState.Idle);
				else if (
					!_playerCharacter.IsOnFloor() &&
					(_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
				)
				{
					SetState(Utility.CharacterState.WallSlide);
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				
				break;
			case Utility.CharacterState.Hurt:
				if (!_playerCharacter.IsOnFloor())
				{
					_velocity.Y += _velocityComponent.FallVelocity(delta);
				}
				break;
			case Utility.CharacterState.WallSlide:
				_velocity = _velocityComponent.WallSlidingVelocity(delta, _velocity);

				if (_playerCharacter.IsOnFloor())
				{
					SetState(Utility.CharacterState.Idle);

					if (_direction != Vector2.Zero)
					{
						SetState(Utility.CharacterState.Run);
					}
				}
				else if (!_playerCharacter.IsOnFloor())
				{
					if (_leftWallDetect.IsColliding())
					{
						_sprite.FlipH = false;
						_direction = Vector2.Right;
					}
					else if (_rightWallDetect.IsColliding())
					{
						_sprite.FlipH = true;
						_direction = Vector2.Left;
					}

					if (Input.IsActionJustPressed("jump"))
					{
						SetState(Utility.CharacterState.WallJump);
					}
				}

				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				
				break;
			case Utility.CharacterState.WallJump:
				if (!_playerCharacter.IsOnFloor())
				{
					_velocity.Y += _velocityComponent.FallVelocity(delta);

					if (_velocity.Y > 0)
					{
						SetState(Utility.CharacterState.Fall);
					}
				}
				break;
			
			case Utility.CharacterState.Dash:
				if (!_isDashing)
				{
					SetState(Utility.CharacterState.Idle);
				}
				
				break;
		}
		
		return _velocity;
	}

	private void FlipSprite(Vector2 direction)
	{
		if (direction == Vector2.Right)
		{
			_sprite.FlipH = false;
		}
		else if (direction == Vector2.Left)
		{
			_sprite.FlipH = true;
		}
	}

	
}
