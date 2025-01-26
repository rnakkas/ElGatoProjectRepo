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
	public float Direction;
	public Dictionary<string, bool> PlayerInputs;
	public Dictionary<string, float> EntityMovementData;
	public Dictionary<string, bool> EntityBools;
	
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
	
	private void SetDirection()
	{
		if (PlayerInputs["move_left"])
		{
			Direction = -1.0f;
		}
		else if (PlayerInputs["move_right"])
			Direction = 1.0f;
		else if (!PlayerInputs["move_left"] || !PlayerInputs["move_right"])
		{
			Direction = 0;
		}
	}
	
	public Vector2 CalculateVelocity(float delta)
	{
		SetDirection();
		
		if (Direction != 0)
		{
			Velocity.X = Mathf.MoveToward(
				Velocity.X, Direction * EntityMovementData["MaxSpeed"], 
				EntityMovementData["Acceleration"] * delta);
		
			if (EntityBools["IsOnFloor"])
			{
				Velocity.Y = 0;
			}
		}
		else if (EntityBools["IsOnFloor"] && Direction == 0)
		{
			Velocity.X = Mathf.MoveToward(
				Velocity.X, 
				0, 
				EntityMovementData["Friction"] * delta);
			Velocity.Y = 0;
		}
		
		if (EntityBools["IsOnFloor"] && PlayerInputs["jump"])
		{
			Velocity.Y = EntityMovementData["JumpVelocity"];
		}

		if (!EntityBools["IsOnFloor"])
		{
			Velocity.Y += EntityMovementData["Gravity"] * delta;
			
		}

		if (EntityBools["IsOnCeiling"])
		{
			Velocity.Y += EntityMovementData["Gravity"] * delta;
		}
		
		if (!EntityBools["IsOnFloor"] && EntityBools["IsLeftWallDetected"] || EntityBools["IsRightWallDetected"])
		{
			Velocity.X = 0;
			Velocity.Y = Mathf.MoveToward(Velocity.Y, 
				EntityMovementData["WallSlideVelocity"], 
				EntityMovementData["WallSlideGravity"] * delta);

			if (EntityBools["IsLeftWallDetected"])
			{
				Direction = 1.0f;
			} 
			
			if (EntityBools["IsRightWallDetected"])
			{
				Direction = -1.0f;
			}
			
			// Wall Jump
			if (PlayerInputs["jump_justPressed"])
			{
				Velocity.Y = EntityMovementData["WallJumpVelocity"];
				Velocity.X = Direction * EntityMovementData["MaxSpeed"];
			}
		}
		
		return Velocity;
	}
	
}
