using Godot;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Utilties;

namespace ElGatoProject.Pickups.Scripts;

[GlobalClass]
public partial class Pickups : Area2D
{
	[Export] private Utility.PickupType _pickupType;
	[Export] private int _healAmount;
	[Export] private int _scorePoints;
	[Export] private Utility.WeaponType _weaponType;
	[Export] private AnimatedSprite2D _sprite;

	private bool _canPickup;
	
	public override void _Ready()
	{
		_sprite?.Play(Utility.EntityIdleAnimation);
		
		AreaEntered += OnPlayerEntered;
	}

	private void OnPlayerEntered(Area2D playerArea)
	{
		if (!playerArea.IsInGroup("PlayersPickupsBox"))
			return;
		if (playerArea is not PickupsComponent pickupsComponent)
			return;
		
		switch (_pickupType)
		{
			case Utility.PickupType.Coffee:
				_canPickup = pickupsComponent.PickupHealthItem(_healAmount);
				break;
			
			case Utility.PickupType.Catnip:
				_canPickup = true;
				pickupsComponent.PickUpScoreItem(_scorePoints);
				break;
			
			case Utility.PickupType.WeaponTypeModifier:
				_canPickup = true;
				pickupsComponent.PickupWeaponMod(_weaponType);
				pickupsComponent.PickUpScoreItem(_scorePoints);
				break;
		}
		
		if (_canPickup)
		{
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

		tween1.TweenProperty(this, "modulate:a", 0, 0.4);
		tween2.TweenProperty(this, "position", Position - new Vector2(0, 45), 0.5);
		tween2.TweenCallback(Callable.From(QueueFree));
	}
	
}
