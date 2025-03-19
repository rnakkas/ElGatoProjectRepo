using ElGatoProject.Players.Scripts;
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
	[Export] private AnimationPlayer _animaationPlayer;
	[Export] private WeaponElgato _weapon;
	[Export] private ShootingComponent _shootingComponent;

	private bool _isDashing, _onDashCooldown;
	private Vector2 _direction = Vector2.Zero;
	public Utility.EntityState CurrentState;
	public bool IsShooting;

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
		
		if (CurrentState != Utility.EntityState.Death)
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
		
		SetState(Utility.EntityState.Hurt);
	}
	
	private void OnHurtStatusCleared()
	{
		if (_healthComponent is { CurrentHealth: > 0 } )
			SetState(Utility.EntityState.Idle);
	}
	
	private void OnHealthDepleted()
	{
		SetState(Utility.EntityState.Death);
	}
	
	
	// State machine
	public void SetState(Utility.EntityState newState)
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
			case Utility.EntityState.Idle:
				break;
			case Utility.EntityState.IdleShoot:
				IsShooting = false;
				break;
			case Utility.EntityState.Run:
				break;
			case Utility.EntityState.RunShoot:
				IsShooting = false;
				break;
			case Utility.EntityState.Jump:
				break;
			case Utility.EntityState.Fall:
				break;
			case Utility.EntityState.Hurt:
				break;
			case Utility.EntityState.WallSlide:
				break;
			case Utility.EntityState.WallJump:
				break;
			case Utility.EntityState.Dash:
				_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, true); // Exit invincibility after dash finished
				if (_velocityComponent != null) _velocityComponent.EntityVelocity = Vector2.Zero;
				break;
		}
	}

	private void EnterState()
	{
		// _sprite?.Play(Utility.EntityAnimations[CurrentState]);
		// _animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
		
		switch (CurrentState)
		{
			case Utility.EntityState.Idle:
				if (_velocityComponent != null) _velocityComponent.EntityVelocity.Y = 0;
				if (_animaationPlayer.CurrentAnimation != "idle_shoot")
					_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				else if (_animaationPlayer.CurrentAnimation == "idle_shoot" && !_animaationPlayer.IsPlaying())
				{
					_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				}
				break;
			
			case Utility.EntityState.IdleShoot:
				if (_velocityComponent != null) _velocityComponent.EntityVelocity.Y = 0;
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.Run:
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.RunShoot:
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.Jump:
				_velocityComponent?.JumpeVelocity();
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.Fall:
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.Hurt:
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.WallSlide:
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.WallJump:
				_velocityComponent?.WallJumpingVelocity(_direction);
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				break;
			
			case Utility.EntityState.Dash:
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
				
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				
				break;
			
			case Utility.EntityState.Death:
				_hurtboxComponent?.SetDeferred(Area2D.PropertyName.Monitorable, false); // Don't get hit if dying
				if (_velocityComponent != null) _velocityComponent.EntityVelocity = Vector2.Zero;
				
				// Wait before transitioning to game over screen
				_gameOverTimer?.Start();
				Globals.Instance.IsPlayerDying = true;
				
				_animaationPlayer?.Play(Utility.EntityAnimations[CurrentState]);
				
				break;
		}
	}

	public void UpdateState(float delta)
	{
		_direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		FlipSprite(_direction);
		
		switch (CurrentState)
		{
			case Utility.EntityState.Idle:
				_velocityComponent?.DecelerateToZeroVelocity(delta);
				
				if (_direction != Vector2.Zero) 
					SetState(Utility.EntityState.Run);
				else if (_playerCharacter != null)
				{
					if ( !_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
						SetState(Utility.EntityState.Fall);
					else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor()) 
						SetState(Utility.EntityState.Jump);
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown) 
					SetState(Utility.EntityState.Dash);

				if (Input.IsActionPressed("shoot"))
				{
					var directionFacing = SetDirectionBasedOnSprite();
					if (_shootingComponent.Shoot(directionFacing))
						SetState(Utility.EntityState.IdleShoot);
				}
				
				break;
			
			case Utility.EntityState.IdleShoot:
				if (Input.IsActionPressed("shoot"))
				{
					var directionFacing = SetDirectionBasedOnSprite();
					if (_shootingComponent.Shoot(directionFacing))
						SetState(Utility.EntityState.IdleShoot);
				}
				else if (!Input.IsActionPressed("shoot"))
					SetState(Utility.EntityState.Idle);
				
				break;
			
			case Utility.EntityState.Run:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
				
				if (_direction == Vector2.Zero)
					SetState(Utility.EntityState.Idle);
				else if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
						SetState(Utility.EntityState.Fall);
					else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor())
						SetState(Utility.EntityState.Jump); 
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.EntityState.Dash);
				
				break;
			
			case Utility.EntityState.RunShoot:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
				
				if (_direction == Vector2.Zero)
					SetState(Utility.EntityState.Idle);
				else if (_direction != Vector2.Zero)
					SetState(Utility.EntityState.Run);
				
				else if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
						SetState(Utility.EntityState.Fall);
					else if (Input.IsActionPressed("jump") && _playerCharacter.IsOnFloor())
						SetState(Utility.EntityState.Jump); 
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.EntityState.Dash);
				
				break;
			
			case Utility.EntityState.Jump:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);

				if (_playerCharacter != null && _leftWallDetect != null && _rightWallDetect != null)
				{
					if (!_playerCharacter.IsOnFloor() || _playerCharacter.IsOnCeiling())
					{
						_velocityComponent?.FallVelocity(delta);

						if (_velocityComponent?.EntityVelocity.Y > 0)
						{
							SetState(Utility.EntityState.Fall);
						}
					}
					else if (
						!_playerCharacter.IsOnFloor() &&
						(_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
					)
					{
						SetState(Utility.EntityState.WallSlide);
					}
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.EntityState.Dash);
				break;
			
			case Utility.EntityState.Fall:
				_velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
				_velocityComponent?.FallVelocity(delta);

				if (_playerCharacter != null && _leftWallDetect != null && _rightWallDetect != null)
				{
					if (_playerCharacter.IsOnFloor())
						SetState(Utility.EntityState.Idle);
					else if (
						!_playerCharacter.IsOnFloor() &&
						(_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
					)
					{
						SetState(Utility.EntityState.WallSlide);
					}
				}
				
				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.EntityState.Dash);
				break;
			
			case Utility.EntityState.Hurt:
				if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor())
					{
						_velocityComponent?.FallVelocity(delta);
					}
				}
				break;
			
			case Utility.EntityState.WallSlide:
				_velocityComponent?.WallSlidingVelocity(delta);

				if (_playerCharacter != null && _leftWallDetect != null && _rightWallDetect != null)
				{
					if (_playerCharacter.IsOnFloor())
					{
						SetState(Utility.EntityState.Idle);

						if (_direction != Vector2.Zero)
						{
							SetState(Utility.EntityState.Run);
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
						else
						{
							SetState(Utility.EntityState.Idle);
						}

						if (Input.IsActionJustPressed("jump"))
						{
							SetState(Utility.EntityState.WallJump);
						}
					}
				}

				if (Input.IsActionJustPressed("dashDodge") && !_onDashCooldown)
					SetState(Utility.EntityState.Dash);
				
				break;
			
			case Utility.EntityState.WallJump:
				if (_playerCharacter != null)
				{
					if (!_playerCharacter.IsOnFloor())
					{
						_velocityComponent?.FallVelocity(delta);

						if (_velocityComponent?.EntityVelocity.Y > 0)
						{
							SetState(Utility.EntityState.Fall);
						}
					}
				}
				break;
			
			case Utility.EntityState.Dash:
				if (!_isDashing)
				{
					SetState(Utility.EntityState.Idle);
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

	private Vector2 SetDirectionBasedOnSprite()
	{
		Vector2 directionFacing = Vector2.Zero;
		if (_sprite.IsFlippedH())
			directionFacing = Vector2.Left;
		else if (!_sprite.IsFlippedV())
			directionFacing = Vector2.Right;
		
		return directionFacing;
	}

	
}
