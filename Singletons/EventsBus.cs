using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class EventsBus : Node
{
	public static EventsBus Instance { get; private set; } // Needed for c# singleton autoload

	[Signal]
	public delegate void AttemptedHealthPickupEventHandler(int currentHealth, int maxHealth);
	
	[Signal]
	public delegate void HealedPlayerEventHandler(int healAmount);
	
	public override void _Ready()
	{
		// Needed for c# singleton autoload
		Instance = this;
	}
}
