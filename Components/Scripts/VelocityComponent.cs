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
	private float _direction;

	public Vector2 CalculateVelocity(
		Dictionary<string, bool> input, 
		CharacterStatesComponent.State currentState, 
		float delta, 
		bool isOnFloor)
	{
		if (input["move_left"])
		{
			_direction = -1;
		}
		else if (input["move_right"])
		{
			_direction = 1;
		}

		if (input["move_left"] || input["move_right"])
		{
			AccelerateToMaxSpeed(_direction, _playerStats.MaxSpeed, _playerStats.Acceleration);
		}
		else if (isOnFloor && (!input["move_left"] || !input["move_right"]))
		{
			SlowdownToZeroSpeed(_playerStats.Friction);
			VerticalVelocityStoppedOnGround();
		}

		if (isOnFloor && input["jump"])
		{
			JumpVelocity(_playerStats.JumpVelocity);
		}

		if (!isOnFloor)
		{
			FallDueToGravity(delta, _playerStats.Gravity);
		}
		
		return _velocity;
	}
	
	public void AccelerateToMaxSpeed(float direction, float maxSpeed, float acceleration)
	{
		_velocity.X =  Mathf.MoveToward(_velocity.X, direction * maxSpeed, acceleration);
	}

	public void SlowdownToZeroSpeed(float friction)
	{
		_velocity.X = Mathf.MoveToward(_velocity.X, 0, friction);
	}

	public void JumpVelocity(float jumpVelocity)
	{
		_velocity.Y = jumpVelocity;
	}

	public void FallDueToGravity(float delta, float gravity)
	{
		_velocity.Y += gravity * delta;
	}

	public void VerticalVelocityStoppedOnGround()
	{
		_velocity.Y = 0;
	}
	
}
