using Godot;

namespace ElGatoProject.Menus.Scripts;
public partial class PauseMenu : Control
{
	[Export] private Button _resumeButton, _mainMenuButton, _exitGameButton;
	
	[Signal]
	public delegate void ReturnToMainMenuEventHandler();
	
	public override void _Ready()
	{
		_resumeButton.MouseEntered += OnMouseHoverResumeButton;
		_mainMenuButton.MouseEntered += OnMouseHoverMainMenuButtoon;
		_exitGameButton.MouseEntered += OnMouseHoverExitGameButton;
		
		_resumeButton.Pressed += OnResumeButtonPressed;
		_mainMenuButton.Pressed += OnMainMenuButtonPressed;
		_exitGameButton.Pressed += OnExitGaneButtonPressed;
	}

	private void OnMouseHoverResumeButton()
	{
		_resumeButton.GrabFocus();
	}
	private void OnMouseHoverMainMenuButtoon()
	{
		_mainMenuButton.GrabFocus();
	}
	private void OnMouseHoverExitGameButton()
	{
		_exitGameButton.GrabFocus();
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

	private void OnExitGaneButtonPressed()
	{
		GetTree().Quit();
	}

	public void PauseMenuVisibility(bool isVisible)
	{
		SetVisible(isVisible);
		
		if (isVisible)
			_resumeButton.GrabFocus();
		else
			_resumeButton.ReleaseFocus();
	}
}
