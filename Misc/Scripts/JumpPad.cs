using Godot;
using System;

namespace ElGatoProject.Misc.Scripts;

public partial class JumpPad : Area2D
{
	[Export] public float JumpMultiplier = 2.0f;
	
	private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite");
		_sprite.Play("idle");

		AreaEntered += PlayerEntered;
		AreaExited += PlayerExited;
		
	}

	private void PlayerEntered(Area2D area)
	{
		if (area.IsInGroup("Players"))
		{
			GD.Print("Play jump activation animation");
		}
	}
	
	private void PlayerExited(Area2D area)
	{
		if (area.IsInGroup("Players"))
		{
			GD.Print("Play idle animation");
		}
	}
	
	public override void _Process(double delta)
	{
	}
}
