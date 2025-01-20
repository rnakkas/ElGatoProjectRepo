using Godot;
using System;
using ElGatoProject.Players.Scripts;
using ElGatoProject.Resources;

namespace ElGatoProject.Enemies.Scripts;

/*
TODO: 
Dying: DONE
If health <= 0
	Die - QueueFree()

Shooting player:
If player is in range
	Rotate the raycast to the player's position
If the raycast collides with player 
	Shoot at the player's position
Else if the raycast collides with wall or floor
	Don't shoot
*/


public partial class RangedEnemyOne : Area2D
{
	[Export] private EnemyStats _rangedEnemyOneStats;
	[Export] private AnimatedSprite2D _spriteBody, _spriteEye;
	[Export] private Marker2D _eyeMarker;
	[Export] private Area2D _playerDetectionArea;
	[Export] private RayCast2D _wallDetectionRay;
	[Export] private Timer _hurtStaggerTimer;
	[Export] private Timer _shotCooldownTimer;
	[Export] private Timer _alertTimer;
	[Export] private Label _debugStateLabel;
	[Export] private Label _debugHealthLabel;
	
	[Signal]
	public delegate void ShootEventHandler();
	[Signal]
	public delegate void IdleEventHandler();
	[Signal]
	public delegate void AlertEventHandler();
	
	private bool _hurtStatus, _playerInRange, _onCooldown, _onAlert;
	private Area2D _playerProjectile;
	private Node2D _player;
	
	public float Direction;
	public Vector2 Velocity;
	
	public override void _Ready()
	{
		if (_hurtStaggerTimer != null)
		{
			_hurtStaggerTimer.OneShot = true;
			_hurtStaggerTimer.SetWaitTime(_rangedEnemyOneStats.HurtStaggerTime);
			_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;
		}
		
		_shotCooldownTimer.OneShot = true;
		_shotCooldownTimer.SetWaitTime(_rangedEnemyOneStats.AttackCooldownTime);
		_shotCooldownTimer.Timeout += ShotCooldownTimerTimedOut;
		
		_alertTimer.OneShot = true;
		_alertTimer.SetWaitTime(_rangedEnemyOneStats.ChaseTime);
		_alertTimer.Timeout += AlertTimerTimedOut;
		
		AreaEntered += HitByPlayerBullets;

		_playerDetectionArea.BodyEntered += PlayerEnteredDetectionRange;
		_playerDetectionArea.BodyExited += PlayerExitedDetectionRange;

		_wallDetectionRay.Enabled = false;

		Shoot += OnShoot;
		Idle += OnIdle;
		Alert += OnAlert;
		
		// For debug only, remove later
		_debugStateLabel.SetText("idle");
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyOneStats.EnemyHealth);
	}
	
	// Enemy behaviour
	private void EnemyBehaviour()
	{
		// Use raycast to detect if line of sight to player is blocked by wall
		if (_playerInRange)
		{
			_wallDetectionRay.TargetPosition = ToLocal(_player.GlobalPosition);
		}

		if (_playerInRange && !_wallDetectionRay.IsColliding())
		{
			if (!_onAlert)
			{
				EmitSignal(SignalName.Alert);
	            _onAlert = true;
	            _alertTimer.Start();
			}
			else if (!_onCooldown)
			{
				EmitSignal(SignalName.Shoot);
				_onCooldown = true;
				_shotCooldownTimer.Start();
			}
		}
		else if (!_playerInRange)
		{
			EmitSignal(SignalName.Idle);
		}
	}

	private void OnShoot()
	{
		GD.Print("shooting");
		_debugStateLabel.SetText("Shooting");
	}

	private void OnIdle()
	{
		_debugStateLabel.SetText("Idle");
	}

	private void OnAlert()
	{
		GD.Print("alert");
		_debugStateLabel.SetText("Alert");
	}

	private void ShotCooldownTimerTimedOut()
	{
		_onCooldown = false;
	}

	private void AlertTimerTimedOut()
	{
		_onAlert = false;
	}
	
	// Getting hit by player bullets
	private void HitByPlayerBullets(Area2D area)
	{
		if (!area.IsInGroup("PlayerProjectiles")) 
			return;

		_playerProjectile = area;
		TakeDamageFromPlayerProjectile();
	}
	
	private void TakeDamageFromPlayerProjectile()
	{
		if (_playerProjectile is not Bullet bullet)
			return;
		
		_hurtStatus = true;
		_rangedEnemyOneStats.TakeDamage(bullet.BulletDamage);
		
		// Die if health reaches zero
		if (_rangedEnemyOneStats.EnemyHealth <= 0)
		{
			QueueFree();
		}
		
		_hurtStaggerTimer.Start();
		
		// For debug only, remove later
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyOneStats.EnemyHealth);
	}
	
	private void HurtStaggerTimerTimedOut()
	{
		_hurtStatus = false;
	}
	
	// Detecting when player enters or exits detection range
	private void PlayerEnteredDetectionRange(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_player = body;
			_playerInRange = true;
			_wallDetectionRay.Enabled = true;
		}
	}

	private void PlayerExitedDetectionRange(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_playerInRange = false;
			_wallDetectionRay.Enabled = false;
			
			_debugStateLabel.SetText("Idle");
		}
	}
	
	public override void _Process(double delta)
	{
		EnemyBehaviour();
	}
}
