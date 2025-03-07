using Godot;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;

namespace ElGatoProject.Players.Scripts;
public partial class PlayerHud : Control
{
	[Export] private Label _scoreValue, _weaponAmmo;
	[Export] private TextureRect _weaponTypeIcon;
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
		EventsBus.Instance.PlayerAmmoUpdate += OnPlayerAmmoReceived;
	}

	private void OnPlayerMaxHealthReceived(int maxHealth)
	{
		_caffeineBar?.SetMax(maxHealth);
	}

	private void OnPlayerCurrentHealthReceived(int currentHealth)
	{
		_caffeineBar?.SetValue(currentHealth);
	}

	private void OnPlayerScoreChangeReceived(int score)
	{
		UpdatePlayerScore(score);
	}

	public void UpdatePlayerScore(int score)
	{
		_scoreValue?.SetText(score.ToString("D8"));
	}

	private void OnPlayerCurrentWeaponReceived(string weaponType)
	{
		// Only show weapon hud if weapon is not pistol
		if (weaponType == Utility.WeaponType.PlayerPistol.ToString())
		{
			_weaponTypeIcon?.SetVisible(false);
			_weaponAmmo?.SetVisible(false);
		}
		else
		{
			_weaponTypeIcon?.SetVisible(true);
			_weaponAmmo?.SetVisible(true);
		}

		if (_weaponTypeIcon != null) _weaponTypeIcon.Texture = LoadWeaponIconTexture(weaponType);
	}

	private void OnPlayerAmmoReceived(int ammo)
	{
		_weaponAmmo?.SetText(ammo.ToString());
	}
	
	// Helper functions
	private static Texture2D LoadWeaponIconTexture(string weaponType)
	{
		// Lazy load texture resource depending on weapon pickup
		Texture2D texture = null;
		if (weaponType == Utility.WeaponType.PlayerShotgun.ToString())
		{
			texture= ResourceLoader.Load<Texture2D>(Utility.ShotgunHudIconTexturePath);
		}
		else if (weaponType == Utility.WeaponType.PlayerMachineGun.ToString())
		{
			texture= ResourceLoader.Load<Texture2D>(Utility.MachineGunHudIconTexturePath);
		}
		else if (weaponType == Utility.WeaponType.PlayerRailGun.ToString())
		{
			texture= ResourceLoader.Load<Texture2D>(Utility.RailGunHudIconTexturePath);
		}
		
		return texture;
	}
}
