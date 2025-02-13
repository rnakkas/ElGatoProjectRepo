using Godot;
using System;
using Godot.Collections;

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
	public bool HurtStatus;
	public bool OnDashCooldown;
	
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
	
	public Vector2 CalculateVelocity(float delta, float direction, bool hurtStatus)
	{
		RunStopAndIdleCalculations(delta, direction);
		JumpCalculations();
		FallCalculations(delta);
		HittingCeilingsCalculations(delta);
		WallSlideAndWallJumpCalculations(delta, direction);
		DashCalculations(direction);
		
		return _velocity;
	}

	private void RunStopAndIdleCalculations(float delta, float direction)
	{
		// Running, stopping and idle
		if (direction != 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, direction * MaxSpeed, Acceleration * delta);
		
			if (IsOnFloor)
			{
				_velocity.Y = 0;
			}
		}
		else if (IsOnFloor && direction == 0)
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, Friction * delta);
			_velocity.Y = 0;
		}
	}

	private void JumpCalculations()
	{
		// Jump
		if (IsOnFloor && Input.IsActionPressed("jump"))
		{
			_velocity.Y = JumpVelocity;
		}
	}

	private void FallCalculations(float delta)
	{
		// Fall
		if (!IsOnFloor)
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

	private void WallSlideAndWallJumpCalculations(float delta, float direction)
	{
		// Wall slide and wall jump
		if (!IsOnFloor && (IsLeftWallDetected || IsRightWallDetected))
		{
			_velocity.X = 0;
			_velocity.Y = Mathf.MoveToward(_velocity.Y, WallSlideVelocity, WallSlideGravity * delta);

			if (IsLeftWallDetected)
			{
				direction = 1.0f;
			} 
			
			if (IsRightWallDetected)
			{
				direction = -1.0f;
			}
			
			// Wall Jump
			if (Input.IsActionJustPressed("jump"))
			{
				_velocity.Y = WallJumpVelocity;
				_velocity.X = direction * MaxSpeed;
			}
		}
	}

	private void DashCalculations(float direction)
	{
		if (HurtStatus || OnDashCooldown)
			return;
		
		if (Input.IsActionJustPressed("dashDodge"))
		{
			_velocity.X = DashSpeed * direction;
		}
	}
}
