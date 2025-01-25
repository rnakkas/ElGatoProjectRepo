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

	private bool _hurtStatus;
	
	
	public override void _Ready()
	{
		_hurtStaggerTimer.Timeout += HurtStatusTimerTimedOut;
	}

	private void HurtStatusTimerTimedOut()
	{
		_hurtStatus = false;
	}
	
	public void HitByAttack(Area2D attackArea, int attackDamage, float knockback, Vector2 attackVelocity)
	{
		_hurtStatus = true;
		_hurtStaggerTimer.Start();
		
		Vector2 attackPosition = attackArea.GlobalPosition - GlobalPosition;
		
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
