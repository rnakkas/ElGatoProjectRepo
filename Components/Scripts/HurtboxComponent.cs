using Godot;
using System;
using ElGatoProject.Resources;
using ElGatoProject.TestingStuff;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
	[Export] private Timer _hurtStaggerTimer;
	
	[Signal]
	public delegate void GotHitEventHandler(Dictionary attackData);
	[Signal]
	public delegate void HurtStatusClearedEventHandler(bool hurtStatus);

	private bool _hurtStatus;
	
	
	public override void _Ready()
	{
		_hurtStaggerTimer.Timeout += HurtStatusTimerTimedOut;
	}

	private void HurtStatusTimerTimedOut()
	{
		_hurtStatus = false;
		EmitSignal(SignalName.HurtStatusCleared, _hurtStatus);
	}
	
	// Called by the attacking area, for example enemy bullet calls this method to pass the attack data
	public void HitByAttack(Area2D attackArea, int attackDamage, float knockback, Vector2 attackVelocity)
	{
		_hurtStatus = true;
		_hurtStaggerTimer.Start();
		
		Vector2 attackPosition = (attackArea.GlobalPosition - GlobalPosition).Normalized();
		
		Dictionary attackData = new Dictionary()
		{
			{ "HurtStatus", _hurtStatus },
			{ "AttackPosition", attackPosition },
			{ "AttackDamage", attackDamage },
			{ "Knockback", knockback },
			{ "AttackVelocity", attackVelocity }
		};

		EmitSignal(SignalName.GotHit, attackData);
	}


}
