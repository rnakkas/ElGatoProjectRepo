using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	// Components
	[Export] private PlayerControllerComponent _playerController;
	[Export] private VelocityComponent _velocityComponent;
	
	public override void _Ready()
	{
		// _playerController.Velocity = Velocity;
		// _playerController.ConnectSignals();
		_velocityComponent.Velocity = Velocity;

	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!IsOnFloor())
		{
			Velocity = _velocityComponent.FallVelocity((float)delta);
		}
		// else if (IsOnFloor())
		// {
		// 	Velocity = _velocityComponent.OnFloorVelocity();
		// }

		
		
		MoveAndSlide();

		// _playerController.IsOnFloor = IsOnFloor();
		// _playerController.IsOnCeiling = IsOnCeiling();

		// _playerController.PlayerControllerActions((float)delta);

		// Velocity = _playerController.Velocity;
		//
		// MoveAndSlide();
	}
}
