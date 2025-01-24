using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Enemies.Scripts;
public partial class EnemyBullet : Area2D
{
	// Nodes
	private AnimatedSprite2D _sprite;
	private Timer _despawnTimer;

	public Vector2 Target;
	public float BulletSpeed, Knockback, BulletDespawnTimeSeconds, WeaponSwayAmount;
	public int AttackDamage;
	public Vector2 Velocity;
	
	public override void _Ready()
	{
		// Get nodes
		_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite");
		_despawnTimer = GetNodeOrNull<Timer>("despawnTimer");
		
		_despawnTimer.OneShot = true;
		_despawnTimer.SetWaitTime(BulletDespawnTimeSeconds);
		_despawnTimer.Timeout += BulletDespawnTimerTimedOut;
		_despawnTimer.Start();

		BodyEntered += BulletHitWallOrFloor;
		AreaEntered += BulletHitPlayer;
		
		_sprite.Play("fly");

	}
	
	private void BulletHitWallOrFloor(Node body)
	{
		if (body is TileMapLayer)
		{
			QueueFree();
		}
	}

	private void BulletHitPlayer(Area2D area)
	{
		if (!area.IsInGroup("PlayersHurtBox")) 
			return;
		GD.Print("hit player");
		EventsBus.Instance.EmitSignal(
			nameof(EventsBus.AttackHit), 
			this,
			AttackDamage, 
			Knockback, 
			Velocity
		);
			
		QueueFree();
	}

	private void BulletDespawnTimerTimedOut()
	{
		QueueFree();
	}

	private void CalculateVelocity(float delta)
	{
		Velocity = new Vector2(
			delta * BulletSpeed * Target.X,
			delta * BulletSpeed * Target.Y
		);
		
		MoveLocalX(Velocity.X, true);
		MoveLocalY(Velocity.Y, true);
		
		// Position += Velocity;
	}
	
	public override void _Process(double delta)
	{
		
		CalculateVelocity((float)delta);
	}
}
