using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;
public partial class PlayerHud : Control
{
	[Export] private Label _scoreValue;
	[Export] private ProgressBar _caffeineBar;
	
	public override void _Ready()
	{
		if (!IsVisible())
			return;
		ConnectSignals();
	}

	private void ConnectSignals()
	{
		EventsBus.Instance.PlayerMaxHealthUpdate += OnPlayerMaxHealthReceived;
		EventsBus.Instance.PlayerCurrentHealthUpdate += OnPlayerCurrentHealthReceived;
		EventsBus.Instance.PlayerScoreUpdate += OnPlayerScoreChangeReceived;
	}

	private void OnPlayerMaxHealthReceived(int maxHealth)
	{
		_caffeineBar.SetMax(maxHealth);
	}

	private void OnPlayerCurrentHealthReceived(int currentHealth)
	{
		_caffeineBar.SetValue(currentHealth);
	}

	private void OnPlayerScoreChangeReceived(int score)
	{
		_scoreValue.SetText(score.ToString("D8"));
	}
	
}
