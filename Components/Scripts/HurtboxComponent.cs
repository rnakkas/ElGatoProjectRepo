using Godot;
using System;
using ElGatoProject.Resources;
using ElGatoProject.TestingStuff;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
	[Export] private PlayerStats _playerStats;
	[Export] private Timer _hurtStatusTimer;
	
	[Signal]
	public delegate void HitByAttackEventHandler(
		int damage, 
		float knockback, 
		Vector2 attackVelocity, 
		float attackDirection
		);

	private bool _hurtStatus;
	
	public override void _Ready()
	{
		_hurtStatusTimer.OneShot = true;
		_hurtStatusTimer.SetWaitTime(_playerStats.HurtStaggerTime);
		_hurtStatusTimer.Timeout += HurtStatusTimerTimedOut;
		
		AreaEntered += GotHitByEnemyAttack;
	}

	private void HurtStatusTimerTimedOut()
	{
		_hurtStatus = false;
	}

	private void GotHitByEnemyAttack(Area2D area)
	{
		if (area is TestArea testArea)
		{
			_hurtStatus = true;
			_hurtStatusTimer.Start();
			
			EmitSignal(
				SignalName.HitByAttack,
				testArea.AttackDamage,
				testArea.Knockback,
				testArea.Velocity,
				testArea.Direction
				);
		}
	}

	public bool GetHurtStatus()
	{
		return _hurtStatus;
	}

}
