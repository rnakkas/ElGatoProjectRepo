using Godot;

namespace ElGatoProject.UI.Menus.Scripts;

public partial class MainMenu : Control
{
	[Export] private Button _startGameButton, _exitGameButton;

	[Signal]
	public delegate void StartGameEventHandler();

	[Signal]
	public delegate void ExitGameEventHandler();

	public override void _Ready()
	{
		if (_startGameButton != null)
		{
			_startGameButton.GrabFocus();
			_startGameButton.MouseEntered += OnMouseHoverStartGameButton;
			_startGameButton.Pressed += OnStartGameButtonPressed;
		}

		if (_exitGameButton != null)
		{
			_exitGameButton.MouseEntered += OnMouseHoverExitGameButton;
			_exitGameButton.Pressed += OnExitGameButtonPressed;
		}
	}

	private void OnStartGameButtonPressed()
	{
		EmitSignal(SignalName.StartGame);
	}

	private void OnExitGameButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnMouseHoverStartGameButton()
	{
		_startGameButton?.GrabFocus();
	}

	private void OnMouseHoverExitGameButton()
	{
		_exitGameButton?.GrabFocus();
	}

public void MainMenuVisibility(bool isVisible)
	{
		SetVisible(isVisible);
		
		if (isVisible)
			_startGameButton?.GrabFocus();
		else
			_startGameButton?.ReleaseFocus();
	}
	
}
