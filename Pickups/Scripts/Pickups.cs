using Godot;
using System;
using ElGatoProject.Singletons;

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

	private bool _canPickup;
	
	public override void _Ready()
	{
		_sprite?.Play("idle");

		if (_pickupType == PickupType.Coffee)
		{
			EventsBus.Instance.AttemptedHealthPickup += PlayerAttemptedHealthPickup;
		}
		
		AreaEntered += PlayerPickedUpItem;
	}

	private void PlayerAttemptedHealthPickup(int currentHealth, int maxHealth)
	{
		if (currentHealth < maxHealth)
		{
			_canPickup = true;
		}
		else
		{
			_canPickup = false;
		}
	}

	private void PlayerPickedUpItem(Area2D area)
	{
		if (!area.IsInGroup("PlayersPickupsBox"))
			return;

		switch (_pickupType)
		{
			case PickupType.Coffee when _canPickup:
				EventsBus.Instance.EmitSignal(nameof(EventsBus.HealedPlayer), HealAmount);
				ItemGetsPickedUp();
				break;
			
			case PickupType.Catnip:
			case PickupType.WeaponMod:
				ItemGetsPickedUp();
				break;
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

}
