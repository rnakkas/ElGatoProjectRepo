using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
	[Export] private Timer _hurtStaggerTimer;
	[Export] private HealthComponent _healthComponent;
	[Export] private AnimatedSprite2D _bodySprite;
	[Export] private VelocityComponent _velocityComponent;
	
	[Signal]
	public delegate void GotHitEventHandler();
	[Signal]
	public delegate void HurtStatusClearedEventHandler();
	
	public override void _Ready()
	{
		if (_hurtStaggerTimer != null) _hurtStaggerTimer.Timeout += HurtStatusTimerTimedOut;
	}

	private void HurtStatusTimerTimedOut()
	{
		EmitSignal(SignalName.HurtStatusCleared);
	}
	
	// Called by the attacking area, for example attacking bullet calls this method to pass the attack data
	public void HitByAttack(Area2D attackArea, int attackDamage, float knockback)
	{
		_hurtStaggerTimer?.Start();
		
		var attackPosition = (attackArea.GlobalPosition - GlobalPosition).Normalized();
		
		_healthComponent?.TakeDamage(attackDamage);
		
		if (_velocityComponent != null) _velocityComponent.KnockbackFromAttack(attackPosition, knockback);

		if (_bodySprite != null)
		{
			switch (attackPosition.X)
			{
				case < 0:
					_bodySprite.FlipH = true;
					break;
				case > 0:
					_bodySprite.FlipH = false;
					break;
			}
		}
		
		
		EmitSignal(SignalName.GotHit);
	}


}
