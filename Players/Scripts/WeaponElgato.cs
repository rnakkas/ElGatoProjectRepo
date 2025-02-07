using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Resources;
using ElGatoProject.Singletons;
using Godot.Collections;


namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private ShootingComponent _shooting;
	[Export] private AnimationComponent _animation;
	
	public Vector2 Direction;
	public bool HurtStatus;
	private bool _cooldown, _isShooting;
	
	public override void _Ready()
	{
		ConnectToSignals();
	}

	private void ConnectToSignals()
	{
		
	}

	private void OnCooldownStateUpdated(bool cooldown)
	{
		_cooldown = cooldown;
	}
	
	private void SetComponentProperties()
	{
		_shooting.TargetVector = Direction;
		_shooting.HurtStatus = HurtStatus;
	}

	private void WeaponActions()
	{
		if (Input.IsActionPressed("shoot"))
		{
			_shooting.Shoot();
			_animation.PlayWeaponAnimations(true, Direction.X);

		}
		else
		{
			_isShooting = false;
			_animation.PlayWeaponAnimations(false, Direction.X);
		}
	}
	
	public override void _Process(double delta)
	{
		SetComponentProperties();
		WeaponActions();
		
	}
}
