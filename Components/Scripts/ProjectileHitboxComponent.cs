using Godot;
using ElGatoProject.Utilties;

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
	public int Damage { get; set; }
	public float Knockback { get; set; }
	public Vector2 Velocity { get; set; }
	
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
			SetDeferred(Area2D.PropertyName.Monitoring, false);
			SetDeferred(Area2D.PropertyName.Monitorable, false);
			EmitSignal(SignalName.HitboxCollided);
		}
	}
	
	private void OnEntityHit(Area2D entityArea)
	{
		if (
			(entityArea.IsInGroup(Utility.NodeGroupPlayersHurtbox) && PlayerOrEnemyProjectile == Utility.PlayerOrEnemy.Enemy) ||
			(entityArea.IsInGroup(Utility.NodeGroupEnemies) && PlayerOrEnemyProjectile == Utility.PlayerOrEnemy.Player)
			)
		{
			if (entityArea is not HurtboxComponent hurtboxComponent)
				return;
			SetDeferred(Area2D.PropertyName.Monitoring, false);
			SetDeferred(Area2D.PropertyName.Monitorable, false);
			hurtboxComponent.HitByAttack(this, Damage, Knockback);
			EmitSignal(SignalName.HitboxCollided);
		}
	}
}


