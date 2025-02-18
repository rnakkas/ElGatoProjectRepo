using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;
public partial class PauseMenu : Control
{
	[Export] private Button _resumeButton, _mainMenuButton, _exitGameButton;
	
	[Signal]
	public delegate void ReturnToMainMenuEventHandler();
	
	public override void _Ready()
	{
		_resumeButton.ButtonDown += OnResumeButtonPressed;
		_mainMenuButton.ButtonDown += OnMainMenuButtonPressed;
		_exitGameButton.ButtonDown += OnExitGaneButtonPressed;
	}

	private void OnResumeButtonPressed()
	{
		SetVisible(false);
		GetTree().Paused = false;
	}

	private void OnMainMenuButtonPressed()
	{
		SetVisible(false);
		GetTree().Paused = false;
		EmitSignal(SignalName.ReturnToMainMenu);
	}

	private void OnExitGaneButtonPressed()
	{
		GetTree().Quit();
	}
}
