using Godot;
using System;


namespace ElGatoProject.Misc.Scripts;

[GlobalClass]
public partial class ElevatorBase : AnimatableBody2D
{
	[Export] private Area2D _passengerDetector;
	[Export] private Area2D _obstructionDetector;
	[Export] private Timer _elevatorActivationTimer;
	[Export] private float _elevatorActivationTimeSeconds;
	[Export] private float _elevatorSpeed;

	[Export] private Label _debugLabel;

	private bool _passengerDetected;
	private bool _obstructionDetected;
	private bool _isActive;
	private Vector2 _velocity;
	
	public override void _Ready()
	{
		if (_passengerDetector != null)
		{
			_passengerDetector.BodyEntered += PlayerEnteredElevatorArea;
		}

		if (_obstructionDetector != null)
		{
			_obstructionDetector.BodyEntered += BodyEnteredObstructionArea;
			_obstructionDetector.BodyExited += BodyExitedObstructionArea;
		}

		if (_elevatorActivationTimer != null)
		{
			_elevatorActivationTimer.OneShot = true;
			_elevatorActivationTimer.SetWaitTime(_elevatorActivationTimeSeconds);
			_elevatorActivationTimer.Timeout += ElevatorActivationTimerTimedOut;
		}
		
	}

	private void PlayerEnteredElevatorArea(Node2D body)
	{
		if (body.IsInGroup("Players"))
		{
			_passengerDetected = true;
			
			_elevatorActivationTimer.Start();
			
			_debugLabel.SetText("Player entered elevator");
		}
	}

	private void BodyEnteredObstructionArea(Node2D body)
	{
		if (body.IsInGroup("Players") || body.IsInGroup("Enemies"))
		{
			_obstructionDetected = true;
			
			_debugLabel.SetText("Player obstructing elevator");
		}
		else if (body is TileMapLayer)
		{
			_debugLabel.SetText("on floor");
		}
	}

	private void BodyExitedObstructionArea(Node2D body)
	{
		if (body.IsInGroup("Players") || body.IsInGroup("Enemies"))
		{
			_obstructionDetected = false;
			
			_debugLabel.SetText("obsctructions cleared");
		}
	}

	private void ElevatorActivationTimerTimedOut()
	{
		_isActive = true;
	}

	private void CalculateVelocity(float delta)
	{
		if (_isActive)
		{
			_velocity.Y = delta * -_elevatorSpeed;
		}
		
		MoveLocalY(_velocity.Y, true);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		CalculateVelocity((float)delta);
	}
}
