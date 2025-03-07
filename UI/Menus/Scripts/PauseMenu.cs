using Godot;

namespace ElGatoProject.UI.Menus.Scripts;
public partial class PauseMenu : Control
{
	[Export] private Button _resumeButton, _mainMenuButton, _exitGameButton;
	
	[Signal]
	public delegate void ReturnToMainMenuEventHandler();
	
	public override void _Ready()
	{
		ConnectSignals();
	}

	private void ConnectSignals()
	{
		if (_resumeButton != null)
		{
			_resumeButton.MouseEntered += OnMouseHoverResumeButton;
			_resumeButton.Pressed += OnResumeButtonPressed;
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

	private void OnMouseHoverResumeButton()
	{
		_resumeButton?.GrabFocus();
	}
	private void OnMouseHoverMainMenuButton()
	{
		_mainMenuButton?.GrabFocus();
	}
	private void OnMouseHoverExitGameButton()
	{
		_exitGameButton?.GrabFocus();
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

	private void OnExitGameButtonPressed()
	{
		GetTree().Quit();
	}

	public void PauseMenuVisibility(bool isVisible)
	{
		SetVisible(isVisible);
		
		if (isVisible)
			_resumeButton?.GrabFocus();
		else
			_resumeButton?.ReleaseFocus();
	}
}
