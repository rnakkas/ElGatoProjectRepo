using Godot;
using System;
using System.Numerics;
using Godot.Collections;
using Vector2 = Godot.Vector2;

namespace ElGatoProject.Components.Scripts;

// This component sets velocity for entities
[GlobalClass]
public partial class VelocityComponent : Node
{
	[Export] public float MaxSpeed { get; set; }
	[Export] public float DashSpeed { get; set; }
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

	private Vector2 _velocity;
	public bool IsDashing;
	
	public float KnockbackFromAttack(Vector2 attackPosition, float knockback, Vector2 attackVelocity)
	{
		if (attackVelocity != Vector2.Zero)
		{
			_velocity.X = knockback * attackVelocity.X;	
		} 
		else if (attackVelocity == Vector2.Zero && attackPosition.X < 0)
		{
			_velocity.X = knockback;
		}
		else if (attackVelocity == Vector2.Zero && attackPosition.X > 0)
		{
			_velocity.X = -knockback;
		}

		return _velocity.X;
	}

	public float JumpOnJumpPad(float jumpMultiplier)
	{
		_velocity.Y = jumpMultiplier * JumpVelocity;
		return _velocity.Y;
	}
	
	public Vector2 CalculateVelocity(float delta, Vector2 direction)
	{
		RunStopAndIdleCalculations(delta, direction);
		JumpCalculations(direction);
		FallCalculations(delta);
		HittingCeilingsCalculations(delta);
		WallSlideAndWallJumpCalculations(delta, direction);
		
		return _velocity;
	}

	private void RunStopAndIdleCalculations(float delta, Vector2 direction)
	{
		// Running, stopping and idle
		if (direction.X != 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, direction.X * MaxSpeed, Acceleration * delta);
		
			if (IsOnFloor)
			{
				_velocity.Y = 0;
			}
		}
		else if (IsOnFloor && direction.X == 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, Friction * delta);
			_velocity.Y = 0;
		}
	}

	private void JumpCalculations(Vector2 direction)
	{
		// Jump
		if (IsOnFloor && direction.Y < 0 && !IsDashing)
		{
			_velocity.Y = JumpVelocity;
		}
	}

	private void FallCalculations(float delta)
	{
		// Fall
		if (!IsOnFloor && !IsDashing)
		{
			_velocity.Y += Gravity * delta;
			
		}
	}

	private void HittingCeilingsCalculations(float delta)
	{
		// Hitting ceilings
		if (IsOnCeiling)
		{
			_velocity.Y += Gravity * delta;
		}
	}

	private void WallSlideAndWallJumpCalculations(float delta, Vector2 direction)
	{
		// Wall slide and wall jump
		if (!IsOnFloor && (IsLeftWallDetected || IsRightWallDetected))
		{
			_velocity.X = 0;
			_velocity.Y = Mathf.MoveToward(_velocity.Y, WallSlideVelocity, WallSlideGravity * delta);

			if (IsLeftWallDetected)
			{
				direction.X = 1.0f;
			} 
			
			if (IsRightWallDetected)
			{
				direction.X = -1.0f;
			}
			
			// Wall Jump
			if (direction.Y < 0)
			{
				_velocity.Y = WallJumpVelocity;
				_velocity.X = direction.X * MaxSpeed;
			}
		}
	}

	public float DashingVelocityCalculations(Vector2 direction, bool isDashing)
	{
		IsDashing = isDashing;
		
		if (!isDashing)
			return _velocity.X;
		
		_velocity = Vector2.Zero;
		_velocity.X = DashSpeed * direction.X;
		
		return _velocity.X;
	}
}
