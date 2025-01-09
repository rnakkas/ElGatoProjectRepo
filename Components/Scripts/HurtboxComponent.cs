using Godot;
using System;
using ElGatoProject.TestingStuff;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
	[Signal]
	public delegate void DamagedByAttackEventHandler(
		int damage, 
		float knockback, 
		Vector2 velocity, 
		float direction
		);
	
	public override void _Ready()
	{
		AreaEntered += GotHitByEnemyAttack;
	}

	private void GotHitByEnemyAttack(Area2D area)
	{
		if (area is TestArea testArea)
		{
			EmitSignal(
				SignalName.DamagedByAttack,
				testArea.AttackDamage,
				testArea.Knockback,
				testArea.Velocity,
				testArea.Direction
				);
		}
	}

}
