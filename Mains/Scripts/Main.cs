using Godot;
using System;
using ElGatoProject.Menus.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Mains.Scripts;
public partial class Main : Node2D
{
	[Export] private MainMenu _mainMenu;
	[Export] private PauseMenu _pauseMenu;
	[Export] private LevelLoader _levelLoader;
	
	public override void _Ready()
	{
		_pauseMenu.SetVisible(false);
		
		ConnectToSignals();
	}

	private void ConnectToSignals()
	{
		_mainMenu.StartGame += OnStartGameButtonPressed;
		_pauseMenu.ReturnToMainMenu += OnReturnToMainMenuPressed;
	}

	private void OnStartGameButtonPressed()
	{
		_mainMenu.SetVisible(false);
		_levelLoader.LoadLevel("staging_level");
	}

	private void OnReturnToMainMenuPressed()
	{
		_levelLoader.UnloadCurrentLevel();
		_mainMenu.SetVisible(true);
	}

	private void PauseGame()
	{
		if (Input.IsActionPressed("pause") && !_mainMenu.IsVisible())
		{
			GetTree().Paused = true;
			_pauseMenu.SetVisible(true);
		}
	}
	
	public override void _Process(double delta)
	{
		PauseGame();
	}
}
