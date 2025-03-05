using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;
public partial class GameOverMenu : Control
{
	[Export] public Label ScoreValueLabel;

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
