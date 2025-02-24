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
	public delegate void GotHitEventHandler(Vector2 knockbackVelocity);
	[Signal]
	public delegate void HurtStatusClearedEventHandler();
	
	public override void _Ready()
	{
		_hurtStaggerTimer.Timeout += HurtStatusTimerTimedOut;
	}

	private void HurtStatusTimerTimedOut()
	{
		EmitSignal(SignalName.HurtStatusCleared);
	}
	
	// Called by the attacking area, for example attacking bullet calls this method to pass the attack data
	public void HitByAttack(Area2D attackArea, int attackDamage, float knockback)
	{
		Vector2 knockbackVelocity;
		
		_hurtStaggerTimer.Start();
		
		Vector2 attackPosition = (attackArea.GlobalPosition - GlobalPosition).Normalized();
		
		_healthComponent?.TakeDamage(attackDamage);
		
		if (_velocityComponent == null)
			return;
		knockbackVelocity = _velocityComponent.KnockbackFromAttack(attackPosition, knockback);
		
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
		
		EmitSignal(SignalName.GotHit, knockbackVelocity);
	}


}
