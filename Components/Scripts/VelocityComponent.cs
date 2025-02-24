using Godot;
using System;
using System.Numerics;
using ElGatoProject.Resources;
using Godot.Collections;
using Vector2 = Godot.Vector2;

namespace ElGatoProject.Components.Scripts;

// This component sets velocity for entities
[GlobalClass]
public partial class VelocityComponent : Node
{
	[Export] public CharacterVelocityProperties CharacterVelocityProperties { get; set; }
	
	public Vector2 KnockbackFromAttack(Vector2 attackPosition, float knockback)
	{
		return new Vector2(knockback * -attackPosition.X, -knockback * CharacterVelocityProperties.KnockbackMultiplier);
	}

	public float JumpOnJumpPad(float jumpMultiplier)
	{
		return jumpMultiplier * CharacterVelocityProperties.JumpVelocity;
	}

	public float AccelerateToMaxVelocity(float delta, Vector2 direction, Vector2 currentVelocity)
	{
		return Mathf.MoveToward(currentVelocity.X, direction.X * CharacterVelocityProperties.MaxSpeed, CharacterVelocityProperties.Acceleration * delta);
	}

	public float DecelerateToZeroVelocity(float delta, Vector2 currentVelocity)
	{
		return Mathf.MoveToward(currentVelocity.X, 0, CharacterVelocityProperties.Friction * delta);
	}

	public float JumpeVelocity()
	{
		return CharacterVelocityProperties.JumpVelocity;
		
	}

	public float FallVelocity(float delta)
	{
		return CharacterVelocityProperties.Gravity * delta;
	}

	public Vector2 WallSlidingVelocity(float delta, Vector2 currentVelocity)
	{
		
		return new Vector2(
			currentVelocity.X, 
			Mathf.MoveToward(
				currentVelocity.Y, 
				CharacterVelocityProperties.WallSlideVelocity, 
				CharacterVelocityProperties.WallSlideGravity * delta)
			);
	}

	public Vector2 WallJumpingVelocity(Vector2 direction)
	{
		return new Vector2(
			direction.X * CharacterVelocityProperties.MaxSpeed,
			CharacterVelocityProperties.WallJumpVelocity
		);
	}

	public Vector2 DashVelocity(Vector2 direction)
	{
		return new Vector2(CharacterVelocityProperties.DashSpeed * direction.X, 0);
	}
}
