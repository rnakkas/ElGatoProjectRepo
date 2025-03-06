using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;
public partial class GameOverMenu : Control
{
	[Export] public Label ScoreValueLabel;

	//TODO: Have a timer that then takes player to main menu or high scores screen
	
	// public override void _Ready()
	// {
	// 	ConnectSignals();
	// }

	// private void ConnectSignals()
	// {
	// 	EventsBus.Instance.PlayerHealthDepleted += OnPlayerHealthDepleted;
	// }

	// private void OnPlayerHealthDepleted()
	// {
	// 	_scoreValueLabel.SetText(Globals.Instance.PlayerScore.ToString("D8"));
	// }
}
