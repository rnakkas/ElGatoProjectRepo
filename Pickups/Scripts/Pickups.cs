using Godot;
using System;

namespace ElGatoProject.Pickups.Scripts;

[GlobalClass]
public partial class Pickups : Area2D
{
	private enum PickupType
	{
		Coffee,
		Catnip,
		WeaponMod
	}

	public enum WeaponModType
	{
		None,
		Shotgun,
		MachineGun,
		Railgun
	}
	
	[Export] private PickupType _pickupType;
	[Export] public int HealAmount;
	[Export] public int ScorePoints;
	[Export] public WeaponModType WeaponModifier;
	
	[Export] private AnimatedSprite2D _sprite;
	
	public override void _Ready()
	{
		_sprite?.Play("idle");

		AreaEntered += PlayerEnteredPickupArea;
	}

	private void PlayerEnteredPickupArea(Area2D area)
	{
		if (area.IsInGroup("Players"))
		{
			ItemGetsPickedUp();
		}
	}
	
	private void ItemGetsPickedUp()
	{
		// Turn collision layer off so player cannot quickly run inside layer to heal again during despawn animation
		CollisionLayer = 0;
        
		Tween tween1 = GetTree().CreateTween();
		Tween tween2 = GetTree().CreateTween();

		tween1.TweenProperty(_sprite, "modulate:a", 0, 0.4);
		tween2.TweenProperty(_sprite, "position", _sprite.Position - new Vector2(0, 75), 0.5);
		tween2.TweenCallback(Callable.From(QueueFree));
	}

}
