using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Menus.Scripts;
public partial class GameOverMenu : Control
{
	[Export] private Label _scoreValueLabel;
	[Export] private Button _retryButton, _mainMenuButton, _exitGameButton;
	
	[Signal]
	public delegate void CloseGameOverScreenEventHandler();
	[Signal]
	public delegate void ReturnToMainMenuEventHandler();

	// private Timer _gameOverScreenTimer;

	public override void _Ready()
	{
		// _gameOverScreenTimer = GetNode<Timer>("gameOverScreenTimer");
		ConnectSignals();
	}

	private void ConnectSignals()
	{
		_retryButton.MouseEntered += OnMouseHoverRetryButton;
		_mainMenuButton.MouseEntered += OnMouseHoverMainMenuButton;
		_exitGameButton.MouseEntered += OnMouseHoverExitGameButton;
		
		_retryButton.Pressed += OnRetryButtonPressed;
		_mainMenuButton.Pressed += OnMainMenuButtonPressed;
		_exitGameButton.Pressed += OnExitGameButtonPressed;
		
		// _gameOverScreenTimer.Timeout += OnGameOverScreenTimedOut;
	}
	
	private void OnMouseHoverRetryButton()
	{
		_retryButton.GrabFocus();
	}
	private void OnMouseHoverMainMenuButton()
	{
		_mainMenuButton.GrabFocus();
	}
	private void OnMouseHoverExitGameButton()
	{
		_exitGameButton.GrabFocus();
	}

	private void OnRetryButtonPressed()
	{
		SetVisible(false);
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	private void OnMainMenuButtonPressed()
	{
		SetVisible(false);
		GetTree().Paused = false;
		EmitSignal(SignalName.ReturnToMainMenu);
	}

	private void OnExitGameButtonPressed()
	{
		GetTree().Quit();
	}

	// private void OnGameOverScreenTimedOut()
	// {
	// 	EmitSignal(SignalName.CloseGameOverScreen);
	// }

	public void GameOverScreenVisibility(bool isVisible)
	{
		SetVisible(isVisible);

		if (isVisible)
		{
			_retryButton.GrabFocus();
			_scoreValueLabel.SetText(Globals.Instance.PlayerScore.ToString("D8"));
		}
		else
			_retryButton.ReleaseFocus();
	}
}
