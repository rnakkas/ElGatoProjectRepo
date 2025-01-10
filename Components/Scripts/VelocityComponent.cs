using Godot;
using System;
using ElGatoProject.Resources;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

// This component sets velocity for entities
[GlobalClass]
public partial class VelocityComponent : Node2D
{
	[Export] private PlayerStats _playerStats;
	
	private Vector2 _velocity = Vector2.Zero;
	public float Direction;

	public Vector2 CalculatePlayerVelocity(
		Dictionary<string, bool> input, 
		float delta, 
		bool isOnFloor,
		bool isOnWall,
		RayCast2D leftWallDetect,
		RayCast2D rightWallDetect,
		bool hurtStatus
		)
	{
		// Set directions for velocity
		if (input["move_left"])
		{
			Direction = -1;
		}
		else if (input["move_right"])
		{
			Direction = 1;
		}
		
		// Running and stopping
		if ((input["move_left"] || input["move_right"]))
		{
			AccelerateToMaxSpeed(Direction, _playerStats.MaxSpeed, _playerStats.Acceleration);

			if (isOnFloor)
			{
				VerticalVelocityStoppedOnGround();
			}
		}
		else if (isOnFloor && (!input["move_left"] || !input["move_right"]))
		{
			SlowdownToZeroSpeed(_playerStats.Friction);
			VerticalVelocityStoppedOnGround();
		}

		// Jumping
		if (isOnFloor && input["jump"])
		{
			JumpVelocity(_playerStats.JumpVelocity);
		}

		if (!isOnFloor)
		{
			FallDueToGravity(delta, _playerStats.Gravity);
		}
		
		// Wall sliding
		if (!isOnFloor && (leftWallDetect.IsColliding() || rightWallDetect.IsColliding()))
		{
			WallSlide(_playerStats.WallSlideVelocity, _playerStats.WallSlideGravity);

			if (leftWallDetect.IsColliding())
			{
				Direction = 1;
			} 
			
			if (rightWallDetect.IsColliding())
			{
				Direction = -1;
			}
			
			// Wall Jump
			if (input["jump_justPressed"])
			{
				JumpVelocity(_playerStats.WallJumpVelocity);
				WallJumpHorizontalVelocity(Direction, _playerStats.MaxSpeed);
			}
		}
		
		// Hurt, don't allow movement
		if (hurtStatus)
		{ 
			_velocity = Vector2.Zero;
			return _velocity;
		}
		
		return _velocity;
	}
	
	private void AccelerateToMaxSpeed(float direction, float maxSpeed, float acceleration)
	{
		_velocity.X =  Mathf.MoveToward(_velocity.X, direction * maxSpeed, acceleration);
	}

	private void SlowdownToZeroSpeed(float friction)
	{
		_velocity.X = Mathf.MoveToward(_velocity.X, 0, friction);
	}

	private void JumpVelocity(float jumpVelocity)
	{
		_velocity.Y = jumpVelocity;
	}

	private void FallDueToGravity(float delta, float gravity)
	{
		_velocity.Y += gravity * delta;
	}

	private void VerticalVelocityStoppedOnGround()
	{
		_velocity.Y = 0;
	}

	private void WallSlide(float wallSlideVelocity, float wallSlideGravity)
	{
		_velocity.X = 0;
		_velocity.Y = Mathf.MoveToward(_velocity.Y, wallSlideVelocity, wallSlideGravity);
	}

	private void WallJumpHorizontalVelocity(float direction, float maxSpeed)
	{
		_velocity.X = direction * maxSpeed;
	}
	
}
