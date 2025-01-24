using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class EventsBus : Node
{
	public static EventsBus Instance { get; private set; } // Needed for c# singleton autoload

	public event Action<Area2D, Area2D, int, float, Vector2> OnAttackHit; // Event for when attack hits entity

	// Emit the attack hit event
	public void EmitAttackHit(Area2D attackArea, Area2D entityArea, int attackDamage, float knockback, Vector2 attackVelocity)
	{
		OnAttackHit?.Invoke(attackArea, entityArea, attackDamage, knockback, attackVelocity);
	}
	
	
	[Signal]
	public delegate void AttemptedHealthPickupEventHandler(int currentHealth, int maxHealth);
	[Signal]
	public delegate void HealedPlayerEventHandler(int healAmount);
	[Signal]
	public delegate void AttackHitEventHandler(Area2D area, int attackDamage, float knockback, Vector2 attackVelocity);
	
	public override void _Ready()
	{
		// Needed for c# singleton autoload
		Instance = this;
	}
}
