using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export] public int CurrentHealth { get; set; }
	[Export] private int _maxHealth;
	[Export] private Timer _caffeineDrainTimer;
	
	[Signal]
	public delegate void HealthDepletedEventHandler();

	public override void _Ready()
	{
		// Logic only for player character
		if (!Owner.IsInGroup(Utility.Instance.NodeGroupPlayers))
			return;
		
		// To display health(caffeine) on HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerMaxHealthUpdate), _maxHealth);
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);

		// To continuously drain health(caffeine) meter
		if (_caffeineDrainTimer == null)
			return;
		_caffeineDrainTimer.Timeout += OnCaffeineDrainTimerTimeout;
	}

	private void OnCaffeineDrainTimerTimeout()
	{
		TakeDamage(1);
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;

		if (Owner.IsInGroup(Utility.Instance.NodeGroupPlayers))
			EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);

		if (CurrentHealth <= 0)
		{
			EmitSignal(SignalName.HealthDepleted);
		}
		
		// if (CurrentHealth > 0) 
		// 	return;
		// if (Owner.IsInGroup(Utility.Instance.NodeGroupPlayers))
		// {
		// 	EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerHealthDepleted));
		// }
		// else
		// {
		// 	EmitSignal(SignalName.HealthDepleted);
		// }
	}

	public bool Heal(int heal)
	{
		if (CurrentHealth >= _maxHealth)
			return false;
		
		CurrentHealth = Mathf.Min(CurrentHealth + heal, _maxHealth);
		
		if (Owner.IsInGroup(Utility.Instance.NodeGroupPlayers))
			EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);
		
		return true;
	}
}
