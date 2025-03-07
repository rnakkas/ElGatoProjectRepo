using ElGatoProject.Singletons;
using ElGatoProject.Utilties;
using Godot;

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
	[Export] private Timer _dashCooldownTimer, _dashTimer, _gameOverTimer, _invincibilityTimer, _blinkTimer;
	[Export] private HurtboxComponent _hurtboxComponent;
	[Export] private HealthComponent _healthComponent;

	private bool _isDashing, _onDashCooldown;
	private Vector2 _direction = Vector2.Zero;
	public Utility.CharacterState CurrentState;

	public override void _Ready()
	{
		ConnectSignals();
	}

	// Signals and connections methods
	private void ConnectSignals()
	{
		if (_dashCooldownTimer != null) _dashCooldownTimer.Timeout += OnDashCooldownTimerTimeout;
		
		if (_dashTimer != null) _dashTimer.Timeout += OnDashTimerTimeout;

		if (_gameOverTimer != null) _gameOverTimer.Timeout += OnGameOverTimerTimeout;

		if (_invincibilityTimer != null) _invincibilityTimer.Timeout += OnInvincibilityTimerTimedOut;

		if (_blinkTimer != null) _blinkTimer.Timeout += BlinkTimerTimedOut;
		
		if (_hurtboxComponent != null)
		{
			_hurtboxComponent.GotHit += OnHitByAttack;
			_hurtboxComponent.HurtStatusCleared += OnHurtStatusCleared; 
		}
		
		if (_healthComponent != null) _healthComponent.HealthDepleted += OnHealthDepleted;
	}

	private void OnDashCooldownTimerTimeout()
	{
		_onDashCooldown = false;
	}

	private void OnDashTimerTimeout()
	{
		_isDashing = false;
	}

	private static void OnGameOverTimerTimeout()
	{
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerDied));
	}

	private void OnInvincibilityTimerTimedOut()
	{
		_blinkTimer?.Stop();
		if (_playerCharacter != null) _playerCharacter.Visible = true;
		
		if (CurrentState != Utility.CharacterState.Death)
		{
			_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, true);
		}
	}

	private void BlinkTimerTimedOut()
	{
		_playerCharacter.Visible = !_playerCharacter.Visible;
	}
	
	private void OnHitByAttack()
	{
		_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, false);
		_invincibilityTimer?.Start();
		_blinkTimer?.Start();
		
		SetState(Utility.CharacterState.Hurt);
	}
	
	private void OnHurtStatusCleared()
	{
		if (_healthComponent is { CurrentHealth: > 0 } )
			SetState(Utility.CharacterState.Idle);
	}
	
	private void OnHealthDepleted()
	{
		SetState(Utility.CharacterState.Death);
	}
	
	
	// State machine
	private void SetState(Utility.CharacterState newState)
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
				break;
			case Utility.CharacterState.WallSlide:
				break;
			case Utility.CharacterState.WallJump:
				break;
			case Utility.CharacterState.Dash:
				_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, true); // Exit invincibility after dash finished
				if (_velocityComponent != null) _velocityComponent.EntityVelocity = Vector2.Zero;
				break;
		}
	}

	private void EnterState()
	{
		switch (CurrentState)
		{
			case Utility.CharacterState.Idle:
				if (_velocityComponent != null) _velocityComponent.EntityVelocity.Y = 0;
				_sprite?.Play(Utility.EntityIdleAnimation);
				break;
			
			case Utility.CharacterState.Run:
				_sprite?.Play(Utility.EntityRunAnimation);
				break;
			
			case Utility.CharacterState.Jump:
				_velocityComponent?.JumpeVelocity();
				_sprite?.Play(Utility.EntityJumpAnimation);
				break;
			
			case Utility.CharacterState.Fall:
				_sprite?.Play(Utility.EntityFallAnimation);
				break;
			
			case Utility.CharacterState.Hurt:
				_sprite?.Play(Utility.EntityHurtAnimation);
				break;
			
			case Utility.CharacterState.WallSlide:
				_sprite?.Play(Utility.EntityWallSlideAnimation);
				break;
			
			case Utility.CharacterState.WallJump:
				_velocityComponent?.WallJumpingVelocity(_direction);
				_sprite?.Play(Utility.EntityJumpAnimation);
				break;
			
			case Utility.CharacterState.Dash:
				_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, false); // Enter invincibility when dashing
				if (_sprite != null && !_sprite.IsFlippedH())
				{
					_velocityComponent?.DashVelocity(Vector2.Right);
				}
				else if (_sprite != null && _sprite.IsFlippedH())
				{
					_velocityComponent?.DashVelocity(Vector2.Left);
				}

				_isDashing = true;
				_dashTimer?.Start();
					
				_onDashCooldown = true;
				_dashCooldownTimer?.Start();
				
				_sprite?.Play(Utility.EntityDashAnimation);
				break;
			
			case Utility.CharacterState.Death:
				_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, false); // Don't get hit if dying
				if (_velocityComponent != null) _velocityComponent.EntityVelocity = Vector2.Zero;
				_sprite?.Play(Utility.EntityDeathAnimation);
				
				// Wait before transitioning to game over screen
				_gameOverTimer?.Start();
				Globals.Instance.IsPlayerDying = true;
				
				break;
		}
	}

	public void UpdateState(float delta)
	{
		_direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		FlipSprite(_direction);
		
		switch (CurrentState)
		{
			case Utility.CharacterState.Idle:
				_velocityComponent?.DecelerateToZeroVelocity(delta);
				
				if (_direction != Vector2.Zero) 
					SetState(Utility.CharacterState.Run);
				else if (_playerCharacter != null)
				{
					if ( !_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
						SetState(Utility.CharacterState.Fall);
					else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor()) 
						SetState(Utility.CharacterState.Jump);
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown) 
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Run:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
				
				if (_direction == Vector2.Zero)
					SetState(Utility.CharacterState.Idle);
				else if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
						SetState(Utility.CharacterState.Fall);
					else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor())
						SetState(Utility.CharacterState.Jump); 
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Jump:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);

				if (_playerCharacter != null && _leftWallDetect != null && _rightWallDetect != null)
				{
					if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
					{
						_velocityComponent?.FallVelocity(delta);

						if (_velocityComponent?.EntityVelocity.Y > 0)
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
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Fall:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
				_velocityComponent?.FallVelocity(delta);

				if (_playerCharacter != null && _leftWallDetect != null && _rightWallDetect != null)
				{
					if (_playerCharacter.IsOnFloor())
						SetState(Utility.CharacterState.Idle);
					else if (
						!_playerCharacter.IsOnFloor() &&
						(_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
					)
					{
						SetState(Utility.CharacterState.WallSlide);
					}
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				break;
			
			case Utility.CharacterState.Hurt:
				if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor())
					{
						_velocityComponent?.FallVelocity(delta);
					}
				}
				break;
			
			case Utility.CharacterState.WallSlide:
				_velocityComponent?.WallSlidingVelocity(delta);

				if (_playerCharacter != null && _leftWallDetect != null && _rightWallDetect != null)
				{
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
				}

				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.CharacterState.Dash);
				
				break;
			
			case Utility.CharacterState.WallJump:
				if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor())
					{
						_velocityComponent?.FallVelocity(delta);

						if (_velocityComponent?.EntityVelocity.Y > 0)
						{
							SetState(Utility.CharacterState.Fall);
						}
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
	}

	private void FlipSprite(Vector2 direction)
	{
		if (direction == Vector2.Right)
		{
			if (_sprite != null) _sprite.FlipH = false;
		}
		else if (direction == Vector2.Left)
		{
			if (_sprite != null) _sprite.FlipH = true;
		}
	}

	
}
