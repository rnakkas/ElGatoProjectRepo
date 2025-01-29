using Godot;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

// This component allows player control over entities
[GlobalClass]
public partial class PlayerControllerComponent : Node
{
	public Dictionary<string, bool> GetInputs()
	{
		return new Dictionary<string, bool>
		{
			{"move_left", Input.IsActionPressed("move_left")},
			{"move_right", Input.IsActionPressed("move_right")},
			{"move_up", Input.IsActionPressed("move_up")},
			{"move_down", Input.IsActionPressed("move_down")},
			{"jump", Input.IsActionPressed("jump")},
			{"jump_justPressed", Input.IsActionJustPressed("jump")},
			{"shoot", Input.IsActionPressed("shoot")},
		};
	}
}
