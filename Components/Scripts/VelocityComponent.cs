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
	
	// private Vector2 _velocity;
	// public bool IsDashing;
	
	public float KnockbackFromAttack(Vector2 attackPosition, float knockback, Vector2 attackVelocity)
	{
		return knockback * -attackPosition.X;

		// if (attackVelocity != Vector2.Zero)
		// {
		// 	_velocity.X = knockback * -attackPosition.X;	
		// } 
		//
		// // return _velocity.X;
	}

	public float JumpOnJumpPad(float jumpMultiplier)
	{
		return jumpMultiplier * CharacterVelocityProperties.JumpVelocity;
		
		// _velocity.Y = jumpMultiplier * CharacterVelocityProperties.JumpVelocity;
		// return _velocity.Y;
	}

	public float AccelerateToMaxVelocity(float delta, Vector2 direction, Vector2 currentVelocity)
	{
		return Mathf.MoveToward(currentVelocity.X, direction.X * CharacterVelocityProperties.MaxSpeed, CharacterVelocityProperties.Acceleration * delta);
		
		// _velocity.X = Mathf.MoveToward(
		// 	_velocity.X, 
		// 	direction.X * CharacterVelocityProperties.MaxSpeed, 
		// 	CharacterVelocityProperties.Acceleration * delta);
		//
		// return _velocity;
	}

	public float DecelerateToZeroVelocity(float delta, Vector2 currentVelocity)
	{
		return Mathf.MoveToward(currentVelocity.X, 0, CharacterVelocityProperties.Friction * delta);
		
		// _velocity.X = Mathf.MoveToward(_velocity.X, 0, CharacterVelocityProperties.Friction * delta);
		//
		// return _velocity;
	}

	public float JumpeVelocity()
	{
		return CharacterVelocityProperties.JumpVelocity;
		
		// _velocity.Y = CharacterVelocityProperties.JumpVelocity;
		// return _velocity;
	}

	public float FallVelocity(float delta)
	{
		return CharacterVelocityProperties.Gravity * delta;
		
		// _velocity.Y += CharacterVelocityProperties.Gravity * delta;
		// return _velocity;
	}

	public float WallSlidingVelocity(float delta)
	{
		
		return CharacterVelocityProperties.Gravity * delta;
		
		//
		// return new Vector2(
		// 	0, 
		// 	Mathf.MoveToward(
		// 		currentVelocity.Y, 
		// 		CharacterVelocityProperties.WallSlideVelocity, 
		// 		CharacterVelocityProperties.WallSlideGravity * delta)
		// 	);

		// _velocity.X = 0;
		// _velocity.Y = Mathf.MoveToward(
		// 	_velocity.Y, 
		// 	CharacterVelocityProperties.WallSlideVelocity, 
		// 	CharacterVelocityProperties.WallSlideGravity * delta);
		//
		// return _velocity;
	}

	public Vector2 WallJumpingVelocity(Vector2 direction)
	{
		return new Vector2(
			direction.X * CharacterVelocityProperties.MaxSpeed,
			CharacterVelocityProperties.WallJumpVelocity
		);

		//
		// _velocity.Y = CharacterVelocityProperties.WallJumpVelocity;
		// _velocity.X = direction.X * CharacterVelocityProperties.MaxSpeed;
		//
		// return _velocity;
	}

	public Vector2 DashVelocity(Vector2 direction)
	{
		return new Vector2(CharacterVelocityProperties.DashSpeed * direction.X, 0);

		//
		// _velocity = Vector2.Zero;
		// _velocity.X = CharacterVelocityProperties.DashSpeed * direction.X;
		//
		// return _velocity;
	}

	public float OnFloorVelocity()
	{
		return 0;
		
		// _velocity.Y = 0;
		// return _velocity;
	}
	
	
	
	//###################//
	
	// public Vector2 CalculateVelocity(float delta, Vector2 direction)
	// {
	// 	RunStopAndIdleCalculations(delta, direction);
	// 	JumpCalculations(direction);
	// 	FallCalculations(delta);
	// 	HittingCeilingsCalculations(delta);
	// 	WallSlideAndWallJumpCalculations(delta, direction);
	// 	DashingVelocityCalculations(direction);
	// 	
	// 	return _velocity;
	// }
	//
	// private void RunStopAndIdleCalculations(float delta, Vector2 direction)
	// {
	// 	// Running, stopping and idle
	// 	if (direction.X != 0)
	// 	{
	// 		_velocity.X = Mathf.MoveToward(_velocity.X, direction.X * MaxSpeed, Acceleration * delta);
	// 	
	// 		if (IsOnFloor)
	// 		{
	// 			_velocity.Y = 0;
	// 		}
	// 	}
	// 	else if (IsOnFloor && direction.X == 0)
	// 	{
	// 		_velocity.X = Mathf.MoveToward(_velocity.X, 0, Friction * delta);
	// 		_velocity.Y = 0;
	// 	}
	// }
	//
	// private void JumpCalculations(Vector2 direction)
	// {
	// 	// Jump
	// 	if (IsOnFloor && direction.Y < 0)
	// 	{
	// 		_velocity.Y = JumpVelocity;
	// 	}
	// }
	//
	// private void FallCalculations(float delta)
	// {
	// 	// Fall
	// 	if (!IsOnFloor && !IsDashing)
	// 	{
	// 		_velocity.Y += Gravity * delta;
	// 		
	// 	}
	// }
	//
	// private void HittingCeilingsCalculations(float delta)
	// {
	// 	// Hitting ceilings
	// 	if (IsOnCeiling)
	// 	{
	// 		_velocity.Y += Gravity * delta;
	// 	}
	// }
	//
	// private void WallSlideAndWallJumpCalculations(float delta, Vector2 direction)
	// {
	// 	// Wall slide and wall jump
	// 	if (!IsOnFloor && (IsLeftWallDetected || IsRightWallDetected))
	// 	{
	// 		_velocity.X = 0;
	// 		_velocity.Y = Mathf.MoveToward(_velocity.Y, WallSlideVelocity, WallSlideGravity * delta);
	//
	// 		if (IsLeftWallDetected)
	// 		{
	// 			direction.X = 1.0f;
	// 		} 
	// 		
	// 		if (IsRightWallDetected)
	// 		{
	// 			direction.X = -1.0f;
	// 		}
	// 		
	// 		// Wall Jump
	// 		if (direction.Y < 0)
	// 		{
	// 			_velocity.Y = WallJumpVelocity;
	// 			_velocity.X = direction.X * MaxSpeed;
	// 		}
	// 	}
	// }
	//
	// private void DashingVelocityCalculations(Vector2 direction)
	// {
	// 	if (!IsDashing)
	// 		return;
	// 	_velocity = Vector2.Zero;
	// 	_velocity.X = DashSpeed * direction.X;
	// }
}
