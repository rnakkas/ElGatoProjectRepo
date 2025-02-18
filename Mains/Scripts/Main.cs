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

	// Allows pausing and resuming using the same input key 
	private void PauseAndResumeGame()
	{
		if (!Input.IsActionJustPressed("pause") || _mainMenu.IsVisible()) 
			return;
		
		GetTree().Paused = !GetTree().Paused;

		if (GetTree().Paused)
		{
			_pauseMenu.SetVisible(true);
		}
		else if (!GetTree().Paused)
		{
			_pauseMenu.SetVisible(false);
		}
	}
	
	public override void _Process(double delta)
	{
		PauseAndResumeGame();
	}
}
