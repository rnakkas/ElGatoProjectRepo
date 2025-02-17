using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;
public partial class PauseMenu : Control
{
	[Export] private Button _resumeButton, _mainMenuButton, _exitGameButton;
	
	public override void _Ready()
	{
		_resumeButton.ButtonDown += OnResumeButtonPressed;
		_mainMenuButton.ButtonDown += OnMainMenuButtonPressed;
		_exitGameButton.ButtonDown += OnExitGaneButtonPressed;
		
		// Pause game processing when pause menu is ready/pause button pressed
		GetTree().Paused = true;
	}

	private void OnResumeButtonPressed()
	{
		GetTree().Paused = false; // Unpause game on resume
	}

	private void OnMainMenuButtonPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(Globals.Instance.MainMenu);
	}

	private void OnExitGaneButtonPressed()
	{
		GetTree().Quit();
	}
}
