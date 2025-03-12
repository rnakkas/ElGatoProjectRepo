using Godot;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node
{
	[Export] public int CurrentHealth { get; set; }
	[Export] private int _maxHealth;
	[Export] private Timer _caffeineDrainTimer;
	
	[Signal]
	public delegate void HealthDepletedEventHandler();
	
	[Signal]
	public delegate void HealthDamagedEventHandler(int currentHealth);
	
	public int MaxHealth => _maxHealth;

	public override void _Ready()
	{
		// Logic only for player character
		if (!Owner.IsInGroup(Utility.NodeGroupPlayers))
			return;
		
		// To display health(caffeine) on HUD
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerMaxHealthUpdate), _maxHealth);
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);

		// To continuously drain health(caffeine) meter
		if (_caffeineDrainTimer != null) _caffeineDrainTimer.Timeout += OnCaffeineDrainTimerTimeout;
	}

	private void OnCaffeineDrainTimerTimeout()
	{
		TakeDamage(1);
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;

		switch (CurrentHealth)
		{
			case > 0:
			{
				if (Owner.IsInGroup(Utility.NodeGroupPlayers))
					EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);
				break;
			}
			case <= 0:
				EmitSignal(SignalName.HealthDepleted);
				break;
		}
		
		if (Owner.IsInGroup(Utility.NodeGroupEnemies))
			EmitSignal(SignalName.HealthDamaged, CurrentHealth);
	}

	public bool Heal(int heal)
	{
		if (CurrentHealth >= _maxHealth)
			return false;
		
		CurrentHealth = Mathf.Min(CurrentHealth + heal, _maxHealth);
		
		if (Owner.IsInGroup(Utility.NodeGroupPlayers))
			EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerCurrentHealthUpdate), CurrentHealth);
		
		return true;
	}
}
