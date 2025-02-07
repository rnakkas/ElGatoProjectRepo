using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	// Components
	[Export] private PlayerControllerComponent _playerController;
	
	public override void _Ready()
	{
		_playerController.Velocity = Velocity;
		_playerController.ConnectSignals();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerController.IsOnFloor = IsOnFloor();
		_playerController.IsOnCeiling = IsOnCeiling();
		
		_playerController.PlayerControllerActions((float)delta);
		
		Velocity = _playerController.Velocity;
		
		MoveAndSlide();
	}
}
