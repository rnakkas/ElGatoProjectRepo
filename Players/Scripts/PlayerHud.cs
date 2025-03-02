using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;
public partial class PlayerHud : Control
{
	[Export] private Label _scoreValue, _weaponType, _weaponAmmo;
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
		EventsBus.Instance.PlayerCurrentWeaponUpdate += OnPlayerCurrentWeaponReceived;
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
		UpdatePlayerScore(score);
	}

	public void UpdatePlayerScore(int score)
	{
		_scoreValue.SetText(score.ToString("D8"));
	}

	private void OnPlayerCurrentWeaponReceived(string weaponType, int ammo)
	{
		if (_weaponType.Text != weaponType)
		{
			_weaponType.SetText(weaponType);
		}

		_weaponAmmo.SetText(weaponType == Utility.WeaponType.PlayerPistol.ToString()
			? Utility.Instance.InfinitySymbol
			: ammo.ToString());
	}
}
