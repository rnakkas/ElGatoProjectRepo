using Godot;
using System;


namespace ElGatoProject.Misc.Scripts;

[GlobalClass]
public partial class ElevatorBase : AnimatableBody2D
{
	[Export] private Area2D _passengerDetector;
	[Export] private Area2D _obstructionDetector;

	[Export] private Label _debugLabel;
	
	public override void _Ready()
	{
		if (_passengerDetector != null)
		{
			_passengerDetector.BodyEntered += PlayerEnteredElevatorArea;
		}

		if (_obstructionDetector != null)
		{
			_obstructionDetector.BodyEntered += PlayerEnteredObstructionArea;
		}
	}

	private void PlayerEnteredElevatorArea(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_debugLabel.SetText("Player entered elevator");
		}
	}

	private void PlayerEnteredObstructionArea(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_debugLabel.SetText("Player obstructing elevator");
		}
	}
	
	public override void _Process(double delta)
	{
	}
}
