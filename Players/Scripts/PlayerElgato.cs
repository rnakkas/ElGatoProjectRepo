using Godot;
using System;
using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;

namespace ElGatoProject.Players.Scripts;

public partial class PlayerElgato : CharacterBody2D
{
	// Components
	[Export] private PlayerControllerComponent _playerController;
	[Export] private HurtboxComponent _hurtbox;
	[Export] private PickupsComponent _pickupsBox;
	[Export] private WeaponElgato _weapon;
	
	// Debug labels
	[Export] private HealthComponent _healthComponent;
	[Export] private Label _debugHealthLabel;
	[Export] private Label _debugScoreLabel;
	
	private int _score;
	
	public override void _Ready()
	{
		ConnectSignals();
		
		SetSlideOnCeilingEnabled(false);
		
		// _playerController.Velocity = Velocity;
		// _playerController.ConnectSignals();

	}
	
	public void ConnectSignals()
	{
		if (_pickupsBox == null)
			return;
		_pickupsBox.PickedUpScoreItem += OnScoreItemPickup;
		_pickupsBox.PickedUpWeaponMod += OnWeaponModPickup;
		
		if (_hurtbox == null)
			return;
		// _hurtbox.GotHit += OnHitByAttack;
		_hurtbox.HurtStatusCleared += OnHurtStatusCleared; 
	}
	
	private void OnScoreItemPickup(int scorePoints)
	{
		_score += scorePoints;
	}

	private void OnWeaponModPickup(string modType)
	{
		if (Enum.TryParse(modType, out Utility.WeaponType weaponType))
		{
			_weapon.SwitchWeapon(weaponType);
		}
	}
	
	private void OnHitByAttack(bool hurtStatus, Vector2 attackPosition)
	{
		_weapon.HurtStatus = hurtStatus;
		// _playerController.HurtStatus = hurtStatus;
	}
	
	private void OnHurtStatusCleared(bool hurtStatus)
	{
		_weapon.HurtStatus = hurtStatus;
		// _playerController.HurtStatus = hurtStatus;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
	}

	public override void _Process(double delta)
	{
		_debugHealthLabel.SetText("HP: " + _healthComponent.CurrentHealth);
		_debugScoreLabel.SetText("Score: " + _score);
	}
}
