using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class EnemyControllerComponent : Node
{
    // Get components
    [Export] private Utility.EnemyType _enemyType;
    [Export] private HealthComponent _health;
    [Export] private HurtboxComponent _hurtbox;
    [Export] private PlayerDetectionComponent _playerDetection;
    [Export] private ShootingComponent _shooting;
    [Export] private AnimationComponent _animation;
    
    [Export] private Label _debugHealthLabel;

    private bool _hurtStatus, _canSeePlayer;
    
    public void ConnectToSignals()
    {
    	if (_hurtbox == null)
    		return;
    	_hurtbox.GotHit += OnHitByAttack;
    	_hurtbox.HurtStatusCleared += OnHurtStatusCleared; 
    	
    	if (_health == null)
    		return;
    	_health.HealthDepleted += OnHealthDepleted;
    }
    
    private void SetComponentProperties()
    {
    	_shooting.TargetVector = _playerDetection.PlayerPosition;
    	_shooting.HurtStatus = _hurtStatus;
    	_shooting.CanSeePlayer = _canSeePlayer;
    }

    private void SetAttackType()
    {
	    switch (_enemyType)
	    {
		    case Utility.EnemyType.Ranged:
			    _shooting.Shoot();
			    break;
		    
		    case Utility.EnemyType.Melee:
			    break;
	    }
    }
    
    // Getting hit by attacks
    private void OnHitByAttack(
    	bool hurtStatus, 
    	Vector2 attackPosition, 
    	int attackDamage,
    	float knockback, 
    	Vector2 attackVelocity
    )
    {
    	if (_health == null)
    		return;
    	if (_animation == null)
    		return;
    	
    	_hurtStatus = hurtStatus;
    	
    	_health.TakeDamage(attackDamage);
    	
    	_animation.FlipSpriteToFaceHitDirection(attackPosition);
    }
    
    private void OnHurtStatusCleared(bool hurtStatus)
    {
    	_hurtStatus = hurtStatus;
    }

    // Dying
    private void OnHealthDepleted()
    {
    	GetParent().QueueFree();
    }

    public void EnemyControllerActions(float delta)
    {
	    _canSeePlayer = _playerDetection.PlayerDetectionBehaviour();
	    
	    SetComponentProperties();
	    SetAttackType();
	    
	    _debugHealthLabel.SetText("HP: " + _health.CurrentHealth);
    }
}
