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
	public Dictionary<string, bool> PlayerInputs;
	public Dictionary<string, float> EntityVelocityFields;
	public Dictionary<string, bool> SurfaceDetectionFields;
	
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
	
	public Vector2 CalculateVelocity(float delta, float direction)
	{
		if (direction != 0)
		{
			Velocity.X = Mathf.MoveToward(
				Velocity.X, direction * EntityVelocityFields["MaxSpeed"], 
				EntityVelocityFields["Acceleration"] * delta);
		
			if (SurfaceDetectionFields["IsOnFloor"])
			{
				Velocity.Y = 0;
			}
		}
		else if (SurfaceDetectionFields["IsOnFloor"] && direction == 0)
		{
			Velocity.X = Mathf.MoveToward(
				Velocity.X, 
				0, 
				EntityVelocityFields["Friction"] * delta);
			Velocity.Y = 0;
		}
		
		if (SurfaceDetectionFields["IsOnFloor"] && PlayerInputs["jump"])
		{
			Velocity.Y = EntityVelocityFields["JumpVelocity"];
		}

		if (!SurfaceDetectionFields["IsOnFloor"])
		{
			Velocity.Y += EntityVelocityFields["Gravity"] * delta;
			
		}

		if (SurfaceDetectionFields["IsOnCeiling"])
		{
			Velocity.Y += EntityVelocityFields["Gravity"] * delta;
		}
		
		if (!SurfaceDetectionFields["IsOnFloor"] && 
		    (SurfaceDetectionFields["IsLeftWallDetected"] || SurfaceDetectionFields["IsRightWallDetected"])
		    )
		{
			Velocity.X = 0;
			Velocity.Y = Mathf.MoveToward(Velocity.Y, 
				EntityVelocityFields["WallSlideVelocity"], 
				EntityVelocityFields["WallSlideGravity"] * delta);

			if (SurfaceDetectionFields["IsLeftWallDetected"])
			{
				direction = 1.0f;
			} 
			
			if (SurfaceDetectionFields["IsRightWallDetected"])
			{
				direction = -1.0f;
			}
			
			// Wall Jump
			if (PlayerInputs["jump_justPressed"])
			{
				Velocity.Y = EntityVelocityFields["WallJumpVelocity"];
				Velocity.X = direction * EntityVelocityFields["MaxSpeed"];
			}
		}
		
		return Velocity;
	}
	
}
