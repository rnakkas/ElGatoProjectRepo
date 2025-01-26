using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class PickupsComponent : Area2D
{
	[Signal]
	public delegate void PickupAttemptedEventHandler(Area2D pickupType, int healAmount);
	
}
