using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;

public partial class MainMenu : Control
{
	[Export] private Button _startGameButton, _exitGameButton;
	public override void _Ready()
	{
		_startGameButton.ButtonDown += OnStartGameButtonPressed;
		_exitGameButton.ButtonDown += OnExitGameButtonPressed;
	}

	private void OnStartGameButtonPressed()
	{
		GetTree().ChangeSceneToPacked(Globals.Instance.StagingLevel);
	}

	private void OnExitGameButtonPressed()
	{
		GetTree().Quit();
	}
	
}
