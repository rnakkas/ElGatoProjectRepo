using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class RunShootState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private ShootingComponent _shootingComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
    private Timer _dashCooldownTimer;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;
    
    public override void _Ready()
    {
        _character = GetOwnerOrNull<CharacterBody2D>();
        _velocityComponent = _character?.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _shootingComponent = _character?.GetNodeOrNull<ShootingComponent>("ShootingComponent");
        _animationPlayer = _character?.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _characterSprite = _character?.GetNodeOrNull<AnimatedSprite2D>("sprite");
        _dashCooldownTimer = _character?.GetNodeOrNull<Timer>("Timers/DashCooldownTimer");
    }
    
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        switch (_shootingComponent.ShootingProperties.WeaponType)
        {
            case Utility.WeaponType.PlayerPistol:
            case Utility.WeaponType.PlayerShotgun:
            case Utility.WeaponType.PlayerRailGun:
                _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.RunShootState]);
                break;
            case Utility.WeaponType.PlayerMachineGun:
                _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.RunShootState], -1, 1.5f);
                break;
        }
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
        _direction = Input.GetVector(
            "move_left", 
            "move_right", 
            "move_up", 
            "move_down");
        
        if (_characterSprite != null) Utility.FlipSprite(_characterSprite, _direction);
        
        if (_shootingComponent != null)
        {
            if (!Input.IsActionPressed("shoot") ||
                !_shootingComponent.Shoot(Utility.SetShootingDirection(_characterSprite)))
            {
                _stateMachine?.SetState(Utility.EntityState.RunState.ToString());
            }
        }
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
    }
}