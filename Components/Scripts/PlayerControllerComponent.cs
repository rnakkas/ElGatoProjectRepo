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

	public bool HurtStatus;
	
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _directionVector = Vector2.Zero;
	
	private bool _onDashCooldown, _isDashing;

	public override void _Ready()
	{
		ConnectSignals();
	}

	public void ConnectSignals()
	{
		_miscBox.AreaEntered += EnteredJumpPad;

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
		_playerCharacter.Velocity = Vector2.Zero;
	}

	public void BasicMovements(float delta)
	{
		if (!_playerCharacter.IsOnFloor() && !_isDashing)
		{
			_velocity = _velocityComponent.FallVelocity(delta);
		}
		else
		{
			_velocity = _velocityComponent.OnFloorVelocity();
		}

		if (_playerCharacter.IsOnCeiling())
		{
			_velocity = _velocityComponent.FallVelocity(delta);
		}
		
		_directionVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		if (_directionVector != Vector2.Zero && !_isDashing && !HurtStatus)
		{
			_velocity = _velocityComponent.AccelerateToMaxVelocity(delta, _directionVector);
		}
		else if (_directionVector == Vector2.Zero && !_isDashing && !HurtStatus)
		{
			_velocity = _velocityComponent.DecelerateToZeroVelocity(delta);
		}
		
		if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor() && !_isDashing && !HurtStatus)
		{
			_velocity = _velocityComponent.JumpeVelocity();
		}
		
	}

	private void Dash()
	{
		if (!Input.IsActionJustPressed("dashDodge") || _onDashCooldown || HurtStatus) 
			return;
		
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
	}

	public override void _PhysicsProcess(double delta)
	{
		_velocity = _playerCharacter.Velocity;
		BasicMovements((float)delta);
		Dash();
		
		_playerCharacter.Velocity = _velocity;
		
		_playerCharacter.MoveAndSlide();
	}
	
}
