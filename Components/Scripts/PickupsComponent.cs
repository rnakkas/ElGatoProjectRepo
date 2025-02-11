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
	
	[Signal]
	public delegate void PickedUpScoreItemEventHandler(int scoreAmount);

	[Signal]
	public delegate void PickedUpWeaponModEventHandler(string modType);
	
	public int CurrentHealth { get; set; }
	public int MaxHealth { get; set; }

	public bool PickupHealthItem(int healAmount)
	{
		EmitSignal(SignalName.CheckCurrentHealth);
		
		if (CurrentHealth >= MaxHealth) 
			return false;
		
		EmitSignal(SignalName.PickedUpHealth, healAmount);
		return true;
	}

	public void PickUpScoreItem(int scorePoints)
	{
		EmitSignal(SignalName.PickedUpScoreItem, scorePoints);
	}

	public void PickupWeaponMod(Utility.WeaponType weaponType)
	{
		EmitSignal(SignalName.PickedUpWeaponMod, weaponType.ToString());
	}

}
