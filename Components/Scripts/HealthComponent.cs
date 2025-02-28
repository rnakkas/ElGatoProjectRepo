using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export] public int CurrentHealth { get; set; }
	[Export] public int MaxHealth { get; set; }
	
	[Signal]
	public delegate void HealthDepletedEventHandler();
	[Signal]
	public delegate void HealthChangedEventHandler();

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;

		EmitSignal(SignalName.HealthChanged);
		
		if (CurrentHealth <= 0)
		{
			EmitSignal(SignalName.HealthDepleted);
		}
	}

	public bool Heal(int heal)
	{
		if (CurrentHealth >= MaxHealth)
			return false;
		
		CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
		
		EmitSignal(SignalName.HealthChanged);
		
		return true;
	}
}
