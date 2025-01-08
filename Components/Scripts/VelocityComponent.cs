using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class VelocityComponent : Node2D
{
	[Export] private PlayerStats _playerStats;
	[Export] private CharacterBody2D _characterBody;
	
	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready()
	{
		_velocity = _characterBody.Velocity;
	}

	public void AccelerateToMaxSpeed(Vector2 direction, float delta)
	{
		_velocity.X =  Mathf.MoveToward(_characterBody.Velocity.X, direction.X * _playerStats.MaxSpeed, _playerStats.Acceleration*delta );
	}

	public void SlowdownToZeroSpeed(Vector2 direction, float delta)
	{
		_velocity.X = Mathf.MoveToward(_characterBody.Velocity.X, 0, _playerStats.Friction*delta);
	}

	public void JumpVelocity()
	{
		_velocity.Y = _playerStats.JumpVelocity;
	}

	public void FallDueToGravity(float delta)
	{
		_velocity.Y += _playerStats.Gravity * delta;
	}
	
}
