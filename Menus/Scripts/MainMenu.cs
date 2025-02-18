using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;

public partial class MainMenu : Control
{
	[Export] private Button _startGameButton, _exitGameButton;
	
	[Signal]
	public delegate void StartGameEventHandler();
	
	[Signal]
	public delegate void ExitGameEventHandler();
	
	public override void _Ready()
	{
		_startGameButton.ButtonDown += OnStartGameButtonPressed;
		_exitGameButton.ButtonDown += OnExitGameButtonPressed;
	}

	private void OnStartGameButtonPressed()
	{
		EmitSignal(SignalName.StartGame);
	}

	private void OnExitGameButtonPressed()
	{
		GetTree().Quit();
	}
	
}
