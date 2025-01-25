using Godot;
using System;


[GlobalClass]
public partial class PlayerDetectionComponent : Area2D
{
	[Signal] public delegate void PlayerDetectedEventHandler();
	
	[Export] private RayCast2D _detectionRaycast;

	private Area2D _playerArea;

	public override void _Ready()
	{
		AreaEntered += PlayerEnteredDetectionRange;
		AreaExited += PlayerExitedDetectionRange;
	}

	private void PlayerEnteredDetectionRange(Area2D player)
	{
		if (player.IsInGroup("PlayerTargetBox"))
		{
			GD.Print("Player entered detection range");
			_detectionRaycast.Enabled = false;
			_playerArea = player;
		}
	}

	private void PlayerExitedDetectionRange(Area2D area)
	{
		if (area.IsInGroup("PlayerTargetBox"))
		{
			GD.Print("Player exited detection range");
			_detectionRaycast.Enabled = false;
		}
	}

	private void TargetPlayer()
	{
		if (_detectionRaycast.Enabled)
		{
			GD.Print("sfasdfasdf");
			_detectionRaycast.TargetPosition = ToLocal(_playerArea.GlobalPosition);
		}
	}

	public override void _Process(double delta)
	{
		TargetPlayer();
	}
}
