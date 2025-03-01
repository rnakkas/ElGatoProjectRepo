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
		EventsBus.Instance.PlayerCurrentWeaponAmmoUpdate += OnPlayerCurrentWeaponAmmoReceived;
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

	private void OnPlayerCurrentWeaponReceived(Utility.WeaponType weaponType)
	{
		_weaponType.SetText(weaponType.ToString());
	}

	private void OnPlayerCurrentWeaponAmmoReceived(int ammo)
	{
		_weaponAmmo.SetText(ammo.ToString());
	}
}
