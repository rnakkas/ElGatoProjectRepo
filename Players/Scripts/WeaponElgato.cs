using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Players.Scripts;

public partial class WeaponElgato : Node2D
{
	[Export] private ShootingComponent _shooting;
	[Export] private AnimationComponent _animation;
	
	public Vector2 Direction;
	public bool HurtStatus;
	
	public override void _Ready()
	{
		
	}
	
	private void SetComponentProperties()
	{
		_shooting.TargetVector = Direction;
		_shooting.HurtStatus = HurtStatus;
	}

	private void WeaponActions()
	{
		if (Input.IsActionPressed("shoot") && !_shooting.OnCooldown)
		{
			_shooting.Shoot();
			_animation.PlayWeaponAnimations(true, Direction.X);
		}
		else
		{
			_animation.PlayWeaponAnimations(false, Direction.X);
		}
	}
	
	public override void _Process(double delta)
	{
		SetComponentProperties();
		WeaponActions();
		
	}
}
