using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
	[Export] private Timer _hurtStaggerTimer;
	[Export] private HealthComponent _healthComponent;
	[Export] private AnimatedSprite2D _bodySprite;
	
	[Signal]
	public delegate void GotHitEventHandler(bool hurtStatus, Vector2 attackPosition);
	[Signal]
	public delegate void HurtStatusClearedEventHandler(bool hurtStatus);

	public bool HurtStatus;
	
	public override void _Ready()
	{
		_hurtStaggerTimer.Timeout += HurtStatusTimerTimedOut;
	}

	private void HurtStatusTimerTimedOut()
	{
		HurtStatus = false;
		_bodySprite?.Play("idle");
		EmitSignal(SignalName.HurtStatusCleared, HurtStatus);
	}
	
	// Called by the attacking area, for example attacking bullet calls this method to pass the attack data
	public void HitByAttack(Area2D attackArea, int attackDamage, float knockback, Vector2 attackVelocity)
	{
		HurtStatus = true;
		_hurtStaggerTimer.Start();
		
		Vector2 attackPosition = (attackArea.GlobalPosition - GlobalPosition).Normalized();
		
		_healthComponent?.TakeDamage(attackDamage);
		
		_bodySprite?.Play("hurt");
		if (_bodySprite != null)
		{
			if (attackPosition.X < 0)
			{
				_bodySprite.FlipH = true;
			}
			else if (attackPosition.X > 0)
			{
				_bodySprite.FlipH = false;
			}
		}
		
		EmitSignal(SignalName.GotHit, HurtStatus, attackPosition);
	}


}
