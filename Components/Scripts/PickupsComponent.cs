using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Export] private HealthComponent _healthComponent;
	
	[Signal]
	public delegate void PickedUpHealthEventHandler(int healAmount);

	[Signal]
	public delegate void PickedUpWeaponPowerUpEventHandler(string modType);

	public bool PickupHealthItem(int healAmount)
	{
		return _healthComponent != null && _healthComponent.Heal(healAmount);
	}

	public void PickUpScoreItem(int scorePoints)
	{
		Globals.Instance.PlayerScore += scorePoints;
		EventsBus.Instance.EmitSignal(nameof(EventsBus.PlayerScoreUpdate), Globals.Instance.PlayerScore);
	}

	public void PickupWeaponMod(Utility.WeaponType weaponType)
	{
		EmitSignal(SignalName.PickedUpWeaponPowerUp, weaponType.ToString());
	}

}
