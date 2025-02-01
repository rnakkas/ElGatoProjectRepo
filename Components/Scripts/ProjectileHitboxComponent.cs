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
	[Export] public Utility.PlayerOrEnemy PlayerOrEnemyBullet { get; set; }
	[Export] public int BulletDamage { get; set; }
	[Export] public float Knockback { get; set; }
	[Export] public Vector2 Velocity { get; set; }

	public override void _Ready()
	{
		BodyEntered += BulletHitWallOrFloor;
		AreaEntered += BulletHitEntity;
	}
	
	private void BulletHitWallOrFloor(Node body)
	{
		if (body is TileMapLayer)
		{
			QueueFree();
		}
	}
	
	private void BulletHitEntity(Area2D entityArea)
	{
		if (
			entityArea.IsInGroup("PlayersHurtBox") && PlayerOrEnemyBullet == Utility.PlayerOrEnemy.Enemy ||
			entityArea.IsInGroup("Enemies") && PlayerOrEnemyBullet == Utility.PlayerOrEnemy.Player
			)
		{
			if (entityArea is not HurtboxComponent hurtboxComponent)
				return;
			hurtboxComponent.HitByAttack(this, BulletDamage, Knockback, Velocity);
			QueueFree();
		}
	}
}


