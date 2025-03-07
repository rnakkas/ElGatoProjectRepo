using Godot;
using ElGatoProject.Utilties;

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
    	// _shooting.TargetVector = _playerDetection.PlayerPosition;
    	_shooting.HurtStatus = _hurtStatus;
    }

    private void SetAttackType()
    {
	    switch (_enemyType)
	    {
		    case Utility.EnemyType.Ranged:
			    if (_canSeePlayer)
					_shooting.Shoot(_playerDetection.PlayerPosition);
			    break;
		    
		    case Utility.EnemyType.Melee:
			    break;
	    }
    }
    
    // Getting hit by attacks
    private void OnHitByAttack()
    {
    	_hurtStatus = true;
    }
    
    private void OnHurtStatusCleared()
    {
    	_hurtStatus = false;
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
