using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using Godot.Collections;


namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private PlayerControllerComponent _playerController;
	[Export] private WeaponStats _weaponStats;
	
	private Dictionary<string, bool> _playerInputs;
	private AnimatedSprite2D _sprite;
	private Marker2D _muzzle;
	private Timer _shotCooldownTimer;

	public float Direction;
	
	public override void _Ready()
	{
		// Get child nodes
		_sprite = GetNodeOrNull<AnimatedSprite2D>("sprite");
		_muzzle = GetNodeOrNull<Marker2D>("muzzle");
		_shotCooldownTimer = GetNodeOrNull<Timer>("shotCooldownTimer");
		
		_sprite.Play("idle");
		
		
	}
	
	
	
	public override void _PhysicsProcess(double delta)
	{
		_playerInputs = _playerController.GetInputs();

		if (_playerInputs["shoot"])
		{
			GD.Print("shooting!!!");
		}

		if (Direction < 0)
		{
			_sprite.FlipH = true;
		}
		else if (Direction > 0)
		{
			_sprite.FlipH = false;
		}
	}
}
