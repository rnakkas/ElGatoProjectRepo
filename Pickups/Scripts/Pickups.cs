using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Pickups.Scripts;

[GlobalClass]
public partial class Pickups : Area2D
{
	[Export] private Utility.PickupType _pickupType;
	[Export] private int _healAmount;
	[Export] private int _scorePoints;
	[Export] private Utility.WeaponModType _weaponModifier;
	[Export] private AnimatedSprite2D _sprite;

	private bool _canPickup;
	
	public override void _Ready()
	{
		SubscribeToEvents();
			
		_sprite?.Play("idle");
	}

	private void SubscribeToEvents()
	{
		if (_pickupType == Utility.PickupType.Coffee)
		{
			EventsBus.Instance.OnHealthPickupAttempt += PlayerAttemptedHealthPickup;
		}
	}

	private void UnsubscribeFromEvents()
	{
		EventsBus.Instance.OnHealthPickupAttempt -= PlayerAttemptedHealthPickup;
	}
	
	private void PlayerAttemptedHealthPickup(Area2D pickupArea, Area2D entityArea, bool canPickup)
	{
		if (pickupArea != this)
			return;

		if (canPickup)
		{
			EventsBus.Instance.EmitHealthPickupSuccess(entityArea, _healAmount);
			ItemGetsPickedUp();
		}
	}
	
	private void ItemGetsPickedUp()
	{
		// Turn collision layer and mask off so player cannot quickly run inside layer to heal again during despawn animation
		CollisionLayer = 0;
		CollisionMask = 0;
        
		Tween tween1 = GetTree().CreateTween();
		Tween tween2 = GetTree().CreateTween();

		tween1.TweenProperty(_sprite, "modulate:a", 0, 0.4);
		tween2.TweenProperty(_sprite, "position", _sprite.Position - new Vector2(0, 75), 0.5);
		tween2.TweenCallback(Callable.From(QueueFree));
	}
	
	public override void _ExitTree()
	{
		UnsubscribeFromEvents();
	}
}
