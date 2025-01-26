using Godot;
using System;
using ElGatoProject.Resources;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

// This component sets velocity for entities
[GlobalClass]
public partial class VelocityComponent : Node2D
{
	[Export] public float MaxSpeed { get; set; }
	[Export] public float Acceleration { get; set; }
	[Export] public float Friction { get; set; }
	[Export] public float JumpVelocity{ get; set; }
	[Export] public float Gravity { get; set; }
	[Export] public float WallSlideGravity { get; set; }
	[Export] public float WallJumpVelocity { get; set; }
	[Export] public float WallSlideVelocity { get; set; }
	[Export] public bool IsOnFloor { get; set; }
	[Export] public bool IsOnCeiling { get; set; }
	[Export] public bool IsLeftWallDetected {get; set;}
	[Export] public bool IsRightWallDetected {get; set;}
	
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
			Velocity.X = Mathf.MoveToward(Velocity.X, direction * MaxSpeed, Acceleration * delta);
		
			if (IsOnFloor)
			{
				Velocity.Y = 0;
			}
		}
		else if (IsOnFloor && direction == 0)
		{
			Velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction * delta);
			Velocity.Y = 0;
		}
		
		if (IsOnFloor && PlayerInputs["jump"])
		{
			Velocity.Y = JumpVelocity;
		}

		if (!IsOnFloor)
		{
			Velocity.Y += Gravity * delta;
			
		}

		if (IsOnCeiling)
		{
			Velocity.Y += Gravity * delta;
		}
		
		if (!IsOnFloor && (IsLeftWallDetected || IsRightWallDetected))
		{
			Velocity.X = 0;
			Velocity.Y = Mathf.MoveToward(Velocity.Y, WallSlideVelocity, WallSlideGravity * delta);

			if (IsLeftWallDetected)
			{
				direction = 1.0f;
			} 
			
			if (IsRightWallDetected)
			{
				direction = -1.0f;
			}
			
			// Wall Jump
			if (PlayerInputs["jump_justPressed"])
			{
				Velocity.Y = WallJumpVelocity;
				Velocity.X = direction * MaxSpeed;
			}
		}
		
		return Velocity;
	}
	
}
