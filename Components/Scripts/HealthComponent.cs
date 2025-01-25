using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node2D
{
	[Export] private Label _debugHealthLabel;
	
	public int CurrentHealth { get; set; }
	public int MaxHealth { get; set; }
	
	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
		GD.Print("health: " + CurrentHealth);
		
		_debugHealthLabel.SetText("HP:" + CurrentHealth);
	}

	public void Heal(int heal)
	{
		if (CurrentHealth >= MaxHealth)
			return;
		Mathf.Min(CurrentHealth + heal, MaxHealth);
	}
}
