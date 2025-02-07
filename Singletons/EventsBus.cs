using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class EventsBus : Node
{
	public static EventsBus Instance { get; private set; } // Needed for c# singleton autoload

	public event Action<Area2D, Area2D, int, float, Vector2> OnAttackHit; // Event for when attack hits entity
	public event Action<Area2D, Area2D, bool> OnHealthPickupAttempt; // Event for health item pickup attempt
	public event Action<Area2D, int> OnHealthPickupSuccess; // Even for successful health item pickup
	
	
	// Emit the attack hit event
	public void EmitAttackHit(Area2D attackArea, Area2D entityArea, int attackDamage, float knockback, Vector2 attackVelocity)
	{
		OnAttackHit?.Invoke(attackArea, entityArea, attackDamage, knockback, attackVelocity);
	}
	
	// Emit the health item pickup attempt
	public void EmitHealthPickupAttempt(Area2D pickupArea, Area2D entityArea, bool canPickup)
	{
		OnHealthPickupAttempt?.Invoke(pickupArea, entityArea, canPickup);
	}
	
	// Emit the successful health item pickup
	public void EmitHealthPickupSuccess(Area2D entityArea, int healAmount)
	{
		OnHealthPickupSuccess?.Invoke(entityArea, healAmount);
	}
	
	public override void _Ready()
	{
		// Needed for c# singleton autoload
		Instance = this;
	}
}
