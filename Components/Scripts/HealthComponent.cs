using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export] public int CurrentHealth { get; set; }
	[Export] public int MaxHealth { get; set; }
	
	[Signal]
	public delegate void HealthDepletedEventHandler();
	
	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;

		if (CurrentHealth <= 0)
		{
			EmitSignal(SignalName.HealthDepleted);
		}
	}

	public void Heal(int heal)
	{
		if (CurrentHealth >= MaxHealth)
			return;
		CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
	}
}
