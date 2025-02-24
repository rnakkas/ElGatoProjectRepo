using Godot;
using System;
using System.Numerics;
using ElGatoProject.Resources;
using Godot.Collections;
using Vector2 = Godot.Vector2;

namespace ElGatoProject.Components.Scripts;

// This component sets and holds velocity for entities
[GlobalClass]
public partial class VelocityComponent : Node
{
	[Export] public CharacterVelocityProperties CharacterVelocityProperties { get; set; }

	public Vector2 EntityVelocity = Vector2.Zero;
	
	public void KnockbackFromAttack(Vector2 attackPosition, float knockback)
	{
		EntityVelocity = new Vector2(
			knockback * -attackPosition.X,
			-knockback * CharacterVelocityProperties.KnockbackMultiplier
			);
	}

	public void JumpOnJumpPad(float jumpMultiplier)
	{
		EntityVelocity.Y = jumpMultiplier * CharacterVelocityProperties.JumpVelocity;
	}

	public void AccelerateToMaxVelocity(float delta, Vector2 direction)
	{
		EntityVelocity.X = Mathf.MoveToward(
			EntityVelocity.X,
			direction.X * CharacterVelocityProperties.MaxSpeed,
			CharacterVelocityProperties.Acceleration * delta
		);
	}

	public void DecelerateToZeroVelocity(float delta)
	{
		EntityVelocity.X = Mathf.MoveToward(EntityVelocity.X, 0, CharacterVelocityProperties.Friction * delta);
	}

	public void JumpeVelocity()
	{
		EntityVelocity.Y = CharacterVelocityProperties.JumpVelocity;
		
	}

	public void FallVelocity(float delta)
	{
		EntityVelocity.Y += CharacterVelocityProperties.Gravity * delta;
	}

	public void WallSlidingVelocity(float delta)
	{
		EntityVelocity = new Vector2(
			EntityVelocity.X, 
			Mathf.MoveToward(
				EntityVelocity.Y, 
				CharacterVelocityProperties.WallSlideVelocity, 
				CharacterVelocityProperties.WallSlideGravity * delta)
			);
	}

	public void WallJumpingVelocity(Vector2 direction)
	{
		EntityVelocity = new Vector2(
			direction.X * CharacterVelocityProperties.MaxSpeed,
			CharacterVelocityProperties.WallJumpVelocity
		);
	}

	public void DashVelocity(Vector2 direction)
	{
		EntityVelocity = new Vector2(CharacterVelocityProperties.DashSpeed * direction.X, 0);
	}
}
