using Godot;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerTheCat : CharacterBody2D
{
	[Export] private PlayerControllerComponent _playerController;
	[Export] private VelocityComponent _velocityComponent;
	
	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready()
	{
		_velocity = Velocity;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_playerController.UpdateState((float)delta);
		_velocity = _velocityComponent.EntityVelocity;
		Velocity = _velocity;
		
		MoveAndSlide();
	}
}
