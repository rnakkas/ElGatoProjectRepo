using Godot;
using System;
using ElGatoProject.Singletons;

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


	public override void _Ready()
	{
		
	}
	
	private bool CanPlayerPickupHealth()
	{
		return CurrentHealth < MaxHealth;
	}

	private void PickupHealthItem(int healAmount)
	{
		EmitSignal(SignalName.PickedUpHealth, healAmount);
	}

}
