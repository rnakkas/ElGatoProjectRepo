using Godot;
using ElGatoProject.Menus.Scripts;
using ElGatoProject.SceneTransitions.Scripts;

namespace ElGatoProject.Mains.Scripts;
public partial class Main : Node2D
{
	[Export] private MainMenu _mainMenu;
	[Export] private PauseMenu _pauseMenu;
	[Export] private LevelLoader _levelLoader;
	[Export] private SceneTransition _sceneTransition;
	
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
		_mainMenu.MainMenuVisibility(false);

		_levelLoader.LoadLevel("staging_level");
		
		_sceneTransition?.TransitionToScene();
	}

	private void OnReturnToMainMenuPressed()
	{
		_sceneTransition?.TransitionToScene();
		_levelLoader.UnloadCurrentLevel();
		_mainMenu.MainMenuVisibility(true);
	}

	// Allows pausing and resuming using the same input key 
	private void PauseAndResumeGame()
	{
		if (!Input.IsActionJustPressed("pause") || _mainMenu.IsVisible() || _sceneTransition.IsVisible()) 
			return;
		
		GetTree().Paused = !GetTree().Paused;

		if (GetTree().Paused)
		{
			_pauseMenu.PauseMenuVisibility(true);
		}
		else if (!GetTree().Paused)
		{
			_pauseMenu.PauseMenuVisibility(false);
		}
	}

	public override void _Input(InputEvent @event)
	{
		PauseAndResumeGame();
	}
}
