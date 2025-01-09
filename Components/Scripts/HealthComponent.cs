using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HealthComponent : Node2D
{
	[Export] private PlayerStats _playerStats;
	
	public void TakeDamage(int damage)
	{
		_playerStats.CurrentHealth -= damage;
		GD.Print("health: " +_playerStats.CurrentHealth);
	}

	public void Heal(int heal)
	{
		if (_playerStats.CurrentHealth >= _playerStats.MaxHealth)
			return;
		Mathf.Min(_playerStats.CurrentHealth + heal, _playerStats.MaxHealth);
	}
}
