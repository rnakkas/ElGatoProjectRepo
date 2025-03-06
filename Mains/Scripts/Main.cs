using Godot;
using ElGatoProject.Menus.Scripts;
using ElGatoProject.Players.Scripts;
using ElGatoProject.SceneTransitions.Scripts;
using ElGatoProject.Singletons;

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
		_playerHud.SetVisible(false);
		_pauseMenu.SetVisible(false);
		_gameOverMenu.SetVisible(false);
		
		ConnectToSignals();
	}

	private void ConnectToSignals()
	{
		_mainMenu.StartGame += OnStartGameButtonPressed;
		_pauseMenu.ReturnToMainMenu += OnReturnToMainMenuPressed;
		
		EventsBus.Instance.PlayerHealthDepleted += OnPlayerHealthDepleted;
	}

	private void OnStartGameButtonPressed()
	{
		_mainMenu.MainMenuVisibility(false);

		_levelLoader.LoadLevel("staging_level");
		
		_sceneTransition?.PlaySceneTransition();
		
		_playerHud.SetVisible(true);
		
		// Set player score to 0 on new game start
		_playerHud.UpdatePlayerScore(Globals.Instance.PlayerScore = 0);
	}

	private void OnReturnToMainMenuPressed()
	{
		_sceneTransition?.PlaySceneTransition();
		_levelLoader.UnloadCurrentLevel();
		_mainMenu.MainMenuVisibility(true);
		_playerHud.SetVisible(false);
		
		// Reset player score on return to main menu
		_playerHud.UpdatePlayerScore(Globals.Instance.PlayerScore = 0);
	}

	private void OnPlayerHealthDepleted()
	{
		//TODO: Fix this
		_playerHud.SetVisible(false);
		_levelLoader.UnloadCurrentLevel();
		_sceneTransition?.PlaySceneTransition();
		_gameOverMenu.SetVisible(true);
		_gameOverMenu.ScoreValueLabel.SetText(Globals.Instance.PlayerScore.ToString("D8"));
	}

	// Allows pausing and resuming using the same input key 
	private void PauseAndResumeGame()
	{
		if (
			!Input.IsActionJustPressed("pause") ||
			_mainMenu.IsVisible() ||
			_sceneTransition.IsVisible() ||
			_gameOverMenu.IsVisible()
		)
		{
			return;
		}
		
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
