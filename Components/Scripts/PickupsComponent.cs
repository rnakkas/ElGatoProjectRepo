using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Signal]
	public delegate void CheckCurrentHealthEventHandler();
	[Signal]
	public delegate void PickedUpHealthEventHandler(int healAmount);
	
	public int CurrentHealth { get; set; }
	public int MaxHealth { get; set; }

	public bool PickupHealthItem(int healAmount)
	{
		if (CurrentHealth >= MaxHealth) 
			return false;
		
		EmitSignal(SignalName.PickedUpHealth, healAmount);
		return true;
	}

}
