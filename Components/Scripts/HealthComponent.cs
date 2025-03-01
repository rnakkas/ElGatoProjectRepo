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

	public override void _Ready()
	{
		if (!Owner.IsInGroup("Players"))
			return;
		
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerMaxHealthUpdate), MaxHealth);
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;

		if (Owner.IsInGroup("Players"))
			EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);
		
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
		
		if (Owner.IsInGroup("Players"))
			EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);
		
		return true;
	}
}
