using Godot;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

// This component allows player control over entities
[GlobalClass]
public partial class PlayerControllerComponent : Node2D
{
	private Vector2 _direction = Vector2.Zero;
	private Vector2 _velocity = Vector2.Zero;
	
	[Signal] public delegate void MovementInputDetectedEventHandler(Vector2 direction);
	[Signal] public delegate void MovementInputNotDetectedEventHandler();
	[Signal] public delegate void JumpInputDetectedEventHandler();

	public Dictionary<string, bool> GetInputs()
	{
		return new Dictionary<string, bool>
		{
			{"move_left", Input.IsActionPressed("move_left")},
			{"move_right", Input.IsActionPressed("move_right")},
			{"move_up", Input.IsActionPressed("move_up")},
			{"move_down", Input.IsActionPressed("move_down")},
			{"jump", Input.IsActionPressed("jump")},
			{"shoot", Input.IsActionPressed("shoot")},
		};
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	public void Run()
	{
		_direction = Input.GetVector(
			"move_left", 
			"move_right", 
			"move_up", 
			"move_down");
	
		if (_direction.X != 0)
		{
			EmitSignal(SignalName.MovementInputDetected, _direction);
		}
	}

	public void Idle()
	{
		if (_direction.X == 0)
		{
			EmitSignal(SignalName.MovementInputNotDetected);
		}
	}

	public void Jump()
	{
		if (Input.IsActionPressed("jump"))
		{
			EmitSignal(SignalName.JumpInputDetected);
		}
	}

	public override void _Process(double delta)
	{
		Idle();
		Run();
		Jump();
	}
}
