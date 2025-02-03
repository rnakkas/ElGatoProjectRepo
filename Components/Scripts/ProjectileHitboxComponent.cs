using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

/*
 * Needs to know if bullet is from a player or enemy
 * If from player, monitor enemy group for hit
 * If from enemy, monitor player group for hit
 * Regardless, queue free if wall or floor tilemap hit
 */

[GlobalClass]
public partial class ProjectileHitboxComponent : Area2D
{
	[Export] public int Damage { get; set; }
	[Export] public float Knockback { get; set; }
	[Export] public Vector2 Velocity { get; set; }
	
	[Signal] public delegate void HitboxCollidedEventHandler();
	
	public Utility.PlayerOrEnemy PlayerOrEnemyProjectile { get; set; }

	public override void _Ready()
	{
		BodyEntered += OnWallOrFloorHit;
		AreaEntered += OnEntityHit;
	}
	
	private void OnWallOrFloorHit(Node body)
	{
		if (body is TileMapLayer)
		{
			EmitSignal(SignalName.HitboxCollided);
		}
	}
	
	private void OnEntityHit(Area2D entityArea)
	{
		if (
			(entityArea.IsInGroup("PlayersHurtBox") && PlayerOrEnemyProjectile == Utility.PlayerOrEnemy.Enemy) ||
			(entityArea.IsInGroup("Enemies") && PlayerOrEnemyProjectile == Utility.PlayerOrEnemy.Player)
			)
		{
			if (entityArea is not HurtboxComponent hurtboxComponent)
				return;
			GD.Print(Velocity);
			hurtboxComponent.HitByAttack(this, Damage, Knockback, Velocity);
			EmitSignal(SignalName.HitboxCollided);
		}
	}
}


