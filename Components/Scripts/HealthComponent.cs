using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export] public int CurrentHealth { get; set; }
	[Export] public int MaxHealth { get; set; }
	
	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
	}

	public void Heal(int heal)
	{
		if (CurrentHealth >= MaxHealth)
			return;
		CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
	}

	public void Die()
	{
		
	}
}
