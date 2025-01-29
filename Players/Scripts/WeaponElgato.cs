using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using Godot.Collections;


namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private WeaponStats _weaponStats;
	[Export] private PackedScene _bulletScene;

	[Signal]
	public delegate void ShootEventHandler();
	[Signal]
	public delegate void IdleEventHandler();
	
	private AnimatedSprite2D _sprite;
	private Marker2D _muzzle;
	private Timer _shotCooldownTimer;

	public float Direction;
	public bool HurtStatus;
	private bool _onCooldown;
	private Vector2 _muzzlePosition;
	
	public override void _Ready()
	{
		// Get child nodes
		_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite");
		_muzzle = GetNodeOrNull<Marker2D>("muzzle");
		_shotCooldownTimer = GetNodeOrNull<Timer>("shotCooldownTimer");
		
		_shotCooldownTimer.OneShot = true;
		_shotCooldownTimer.WaitTime = _weaponStats.CooldownTime;
		_shotCooldownTimer.Timeout += ShotCooldownTimedOut;
		
		_sprite.Play("idle");

		Shoot += OnShoot;
		Idle += OnIdle;
		
		_muzzlePosition = _muzzle.Position;
	}
	
	private void ShotCooldownTimedOut()
	{
		_onCooldown = false;
	}

	private void WeaponBehaviour()
	{
		// player can only shoot if not on cooldown and not hurt
		if (Input.IsActionPressed("shoot") && !_onCooldown && !HurtStatus)
		{
			EmitSignal(SignalName.Shoot);
			_onCooldown = true;
			_shotCooldownTimer.Start();
		}
		else
		{
			EmitSignal(SignalName.Idle);
		}
	}

	private void OnShoot()
	{
		_sprite.Play("shoot");
		SpawnBullets();
		
	}
	
	private void SpawnBullets()
	{
		var bulletInstance = (Bullet)_bulletScene.Instantiate();

		// Set direction for bullet
		if (_sprite.IsFlippedH())
		{
			Direction = -1.0f;
		}
		else if (!_sprite.IsFlippedH())
		{
			Direction = 1.0f;
		}
		
		// Set properties for the bullet
		bulletInstance.Direction = Direction;
		bulletInstance.RotationDegrees = Globals.Instance.Rng.RandfRange(-_weaponStats.WeaponSwayDegrees, _weaponStats.WeaponSwayDegrees);
		bulletInstance.BulletSpeed = _weaponStats.BulletSpeed;
		bulletInstance.BulletKnockback = _weaponStats.BulletKnockback;
		bulletInstance.BulletDespawnTimeSeconds = _weaponStats.BulletDespawnTimeSeconds;
		bulletInstance.BulletDamage = _weaponStats.BulletDamage;
		bulletInstance.GlobalPosition = _muzzle.GlobalPosition;
		GetTree().Root.AddChild(bulletInstance);
	}

	private void OnIdle()
	{
		if (!_sprite.IsPlaying())
		{
			_sprite.Play("idle");
		}
	}
	
	// Flip sprite based on direction
	private void FlipSprite()
	{
		if (Direction < 0)
		{
			_sprite.FlipH = true;
			_muzzle.Position = new Vector2(-_muzzlePosition.X, _muzzlePosition.Y);
		}
		else if (Direction > 0)
		{
			_sprite.FlipH = false;
			_muzzle.Position = new Vector2(_muzzlePosition.X, _muzzlePosition.Y);
		}
	}
	
	public override void _Process(double delta)
	{
		WeaponBehaviour();
		FlipSprite();
	}
}
