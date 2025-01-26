using Godot;
using System;
using ElGatoProject.Resources;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

// This component sets velocity for entities
[GlobalClass]
public partial class VelocityComponent : Node2D
{
	public Vector2 Velocity;
	
	public float KnockbackFromAttack(Vector2 attackPosition, float knockback, Vector2 attackVelocity)
	{
		if (attackVelocity != Vector2.Zero)
		{
			Velocity.X = knockback * attackVelocity.X;	
		} 
		else if (attackVelocity == Vector2.Zero && attackPosition.X < 0)
		{
			Velocity.X = knockback;
		}
		else if (attackVelocity == Vector2.Zero && attackPosition.X > 0)
		{
			Velocity.X = -knockback;
		}

		return Velocity.X;
	}

	public float JumpOnJumpPad(float jumpMultiplier, float jumpVelocity)
	{
		Velocity.Y = jumpMultiplier * jumpVelocity;
		return Velocity.Y;
	}
	
}
