using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class VelocityComponent : Node2D
{
	private Vector2 _velocity = Vector2.Zero;
	
	public float AccelerateToMaxSpeed(Vector2 direction, float maxSpeed, float acceleration)
	{
		_velocity.X =  Mathf.MoveToward(_velocity.X, direction.X * maxSpeed, acceleration);
		return _velocity.X;
	}

	public float SlowdownToZeroSpeed(float friction)
	{
		_velocity.X = Mathf.MoveToward(_velocity.X, 0, friction);
		return _velocity.X;
	}

	public float JumpVelocity(float jumpVelocity)
	{
		_velocity.Y = jumpVelocity;
		return _velocity.Y;
	}

	public void FallDueToGravity(float delta, float gravity)
	{
		_velocity.Y += gravity * delta;
	}
	
}
