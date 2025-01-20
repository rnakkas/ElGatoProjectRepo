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
	[Export] private RayCast2D _playerDetectionRay;
	[Export] private Timer _hurtStaggerTimer;
	[Export] private Label _debugStateLabel;
	[Export] private Label _debugHealthLabel;
	
	[Signal] public delegate void HealthDepletedEventHandler();

	private bool _playerInRange;
		
	public float Direction;
	public Vector2 Velocity;
	private bool _hurtStatus;
	private Area2D _playerProjectile;
	
	public override void _Ready()
	{
		if (_hurtStaggerTimer != null)
		{
			_hurtStaggerTimer.OneShot = true;
			_hurtStaggerTimer.SetWaitTime(_rangedEnemyOneStats.HurtStaggerTime);
			_hurtStaggerTimer.Timeout += HurtStaggerTimerTimedOut;
		}
		
		AreaEntered += HitByPlayerBullets;

		_playerDetectionArea.BodyEntered += PlayerEnteredDetectionRange;
		_playerDetectionArea.BodyExited += PlayerExitedDetectionRange;

		HealthDepleted += OnHealthDepleted;
		
		// For debug only, remove later
		_debugStateLabel.SetText("idle");
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyOneStats.EnemyHealth);
	}
	
	// Enemy behaviour
	private void EnemyBehaviour()
	{
		if (_rangedEnemyOneStats.EnemyHealth <= 0)
		{
			EmitSignal(SignalName.HealthDepleted);
		}
	}
	
	
	// Getting hit by player bullets
	private void HurtStaggerTimerTimedOut()
	{
		_hurtStatus = false;
	}
	
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
		
		// For debug only, remove later
		_debugHealthLabel.SetText("HP: "+ _rangedEnemyOneStats.EnemyHealth);
		
		_hurtStaggerTimer.Start();
	}
	
	// Dying
	private void OnHealthDepleted()
	{
		QueueFree();
	}
	
	// Detecting when player enters attack range
	private void PlayerEnteredDetectionRange(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_playerInRange = true;
			
			_debugStateLabel.SetText("PlayerInRange");
		}
	}

	private void PlayerExitedDetectionRange(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_playerInRange = false;
			
			_debugStateLabel.SetText("Idle");
		}
	}
	
	public override void _Process(double delta)
	{
		EnemyBehaviour();
	}
}
