using ElGatoProject.Singletons;
using Godot;

namespace ElGatoProject.UI.Menus.Scripts;
public partial class GameOverMenu : Control
{
	[Export] private Label _scoreValueLabel;
	[Export] private Button _retryButton, _mainMenuButton, _exitGameButton;
	
	[Signal]
	public delegate void CloseGameOverScreenEventHandler();
	
	[Signal]
	public delegate void ReturnToMainMenuEventHandler();

	[Signal]
	public delegate void RetryEventHandler();

	public override void _Ready()
	{
		ConnectSignals();
	}

	private void ConnectSignals()
	{
		if (_retryButton != null)
		{
			_retryButton.MouseEntered += OnMouseHoverRetryButton;
			_retryButton.Pressed += OnRetryButtonPressed;
		}

		if (_mainMenuButton != null)
		{
			_mainMenuButton.MouseEntered += OnMouseHoverMainMenuButton;
			_mainMenuButton.Pressed += OnMainMenuButtonPressed;
		}

		if (_exitGameButton != null)
		{
			_exitGameButton.MouseEntered += OnMouseHoverExitGameButton;
			_exitGameButton.Pressed += OnExitGameButtonPressed;
		}
	}
	
	private void OnMouseHoverRetryButton()
	{
		_retryButton?.GrabFocus();
	}
	private void OnMouseHoverMainMenuButton()
	{
		_mainMenuButton?.GrabFocus();
	}
	private void OnMouseHoverExitGameButton()
	{
		_exitGameButton?.GrabFocus();
	}

	private void OnRetryButtonPressed()
	{
		SetVisible(false);
		EmitSignal(SignalName.Retry);
	}

	private void OnMainMenuButtonPressed()
	{
		SetVisible(false);
		EmitSignal(SignalName.ReturnToMainMenu);
	}

	private void OnExitGameButtonPressed()
	{
		GetTree().Quit();
	}

	public void GameOverScreenVisibility(bool isVisible)
	{
		SetVisible(isVisible);

		if (isVisible)
		{
			_retryButton?.GrabFocus();
			_scoreValueLabel?.SetText(Globals.Instance.PlayerScore.ToString("D8"));
		}
		else
			_retryButton?.ReleaseFocus();
	}
}
