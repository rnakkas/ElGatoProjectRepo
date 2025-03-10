using Godot;
using ElGatoProject.UI.Menus.Scripts;
using ElGatoProject.SceneTransitions.Scripts;
using ElGatoProject.Singletons;
using ElGatoProject.UI.HUD.Scripts;
using ElGatoProject.Utilties;

namespace ElGatoProject.Mains.Scripts;

public partial class Main : Node2D
{
	[Export] private SceneTransition _sceneTransition;
	[Export] private PlayerHud _playerHud;
	[Export] private GameOverMenu _gameOverMenu;
	[Export] private MainMenu _mainMenu;
	[Export] private PauseMenu _pauseMenu;
	[Export] private LevelLoader _levelLoader;
	
	public override void _Ready()
	{
		_playerHud?.SetVisible(false);
		_pauseMenu?.SetVisible(false);
		_gameOverMenu?.SetVisible(false);

		ConnectToSignals();
	}

	private void ConnectToSignals()
	{
		if (_mainMenu != null) _mainMenu.StartGame += OnStartGameButtonPressed;

		if (_pauseMenu != null) _pauseMenu.ReturnToMainMenu += OnReturnToMainMenuPressed;

		if (_gameOverMenu != null)
		{
			_gameOverMenu.ReturnToMainMenu += OnReturnToMainMenuPressed;
            _gameOverMenu.Retry += OnRetryButtonPressed;
		}

		EventsBus.Instance.PlayerDied += OnPlayerDeath;
	}

	private void OnStartGameButtonPressed()
	{
		Globals.Instance.IsPlayerDying = false;
		
		_mainMenu?.MainMenuVisibility(false);

		_levelLoader?.LoadLevel(Utility.LevelOne);

		_sceneTransition?.PlaySceneTransition();

		_playerHud?.SetVisible(true);

		// Set player score to 0 on new game start
		_playerHud?.UpdatePlayerScore(Globals.Instance.PlayerScore = 0);
	}

	private void OnReturnToMainMenuPressed()
	{
		_sceneTransition?.PlaySceneTransition();
		_levelLoader?.UnloadCurrentLevel();
		_mainMenu?.MainMenuVisibility(true);
		_playerHud?.SetVisible(false);

		// Reset player score on return to main menu
		_playerHud?.UpdatePlayerScore(Globals.Instance.PlayerScore = 0);
	}

	private void OnPlayerDeath()
	{
		_playerHud?.SetVisible(false);
		_gameOverMenu?.GameOverScreenVisibility(true);
	}

	private void OnRetryButtonPressed()
	{
		Globals.Instance.IsPlayerDying = false;
		_sceneTransition?.PlaySceneTransition();
		_gameOverMenu?.GameOverScreenVisibility(false);
		_levelLoader?.UnloadCurrentLevel();
		_levelLoader?.LoadLevel(Utility.LevelOne);
		
		_playerHud?.SetVisible(true);

		// Set player score to 0 on retry
		_playerHud?.UpdatePlayerScore(Globals.Instance.PlayerScore = 0);
		
	}

	// Allows pausing and resuming using the same input key 
	private void PauseAndResumeGame()
	{
		// Don't allow pausing if on main menu, during scene transition, during game over screen and while dying
		if (
			!Input.IsActionJustPressed("pause") ||
			(_mainMenu != null && _mainMenu.IsVisible()) ||
			(_sceneTransition != null && _sceneTransition.IsVisible()) ||
			(_gameOverMenu != null && _gameOverMenu.IsVisible()) ||
			Globals.Instance.IsPlayerDying
		)
		{
			return;
		}
		
		GetTree().Paused = !GetTree().Paused;

		if (GetTree().Paused)
		{
			_pauseMenu?.PauseMenuVisibility(true);
		}
		else if (!GetTree().Paused)
		{
			_pauseMenu?.PauseMenuVisibility(false);
		}
	}

	public override void _Input(InputEvent @event)
	{
		PauseAndResumeGame();
	}
}
