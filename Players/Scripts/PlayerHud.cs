using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;
public partial class PlayerHud : Control
{
	[Export] private Label _scoreValue;
	[Export] private ProgressBar _caffeineBar;
	
	public override void _Ready()
	{
		if (!IsVisible())
			return;
		
		_scoreValue.SetText(Globals.Instance.PlayerScore.ToString());
		_caffeineBar.SetMax(Globals.Instance.PlayerMaxHealth);
		_caffeineBar.SetValue(Globals.Instance.PlayerCurrentHealth);
		
		GD.Print("Max HP: " + Globals.Instance.PlayerMaxHealth + ", Current HP: " + Globals.Instance.PlayerCurrentHealth);
	}
	
}
