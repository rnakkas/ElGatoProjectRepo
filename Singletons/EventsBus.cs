using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class EventsBus : Node
{
	public static EventsBus Instance { get; private set; } // Needed for c# singleton autoload

	[Signal] 
	public delegate void PlayerMaxHealthUpdateEventHandler(int maxHealth);
	[Signal] 
	public delegate void PlayerCurrentHealthUpdateEventHandler(int currentHealth);
	[Signal]
	public delegate void PlayerHealthDepletedEventHandler();
	[Signal]
	public delegate void PlayerScoreUpdateEventHandler(int score);
	[Signal]
	public delegate void PlayerCurrentWeaponUpdateEventHandler(string weaponType);
	[Signal]
	public delegate void PlayerAmmoUpdateEventHandler(int ammo);
	
	public override void _Ready()
	{
		// Needed for c# singleton autoload
		Instance = this;
	}
}
