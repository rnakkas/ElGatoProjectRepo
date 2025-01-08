using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	[Export] private PlayerStats PlayerStats { get; set; }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += PlayerStats.Gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = PlayerStats.JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X =  Mathf.MoveToward(Velocity.X, direction.X * PlayerStats.MaxSpeed, PlayerStats.Acceleration*(float)delta );
			// _velocityComponent.RunVelocity(direction, (float)delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, PlayerStats.Friction*(float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
