using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Mains.Scripts;
public partial class Main : Node2D
{
	public override void _Ready()
	{
		GetTree().ChangeSceneToPacked(Globals.Instance.MainMenu);
	}

}
