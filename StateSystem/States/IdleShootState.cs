using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class IdleShootState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private ShootingComponent _shootingComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
    private Timer _dashCooldownTimer;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;
    private bool _isAnimationFinished;
    
    public override void _Ready()
    {
        _character = GetOwner<CharacterBody2D>();
        _velocityComponent = _character.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _shootingComponent = _character.GetNodeOrNull<ShootingComponent>("ShootingComponent");
        _animationPlayer = _character.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _characterSprite = _character.GetNodeOrNull<AnimatedSprite2D>("sprite");
        _dashCooldownTimer = _character.GetNodeOrNull<Timer>("Timers/DashCooldownTimer");
    }
    
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _velocityComponent?.ResetVerticalVelocity();

        _animationPlayer?.Play("idle_shoot");
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
        
        if (_characterSprite != null) _stateMachine.FlipSprite(_characterSprite, _direction);
        
        if (Input.IsActionPressed("shoot"))
        {
            _stateMachine?.SetState(_shootingComponent.Shoot(SetDirectionBasedOnSprite())
                ? "IdleShootState"
                : "IdleState");
        }
        else if (!Input.IsActionPressed("shoot"))
            _stateMachine?.SetState("IdleState");
        else if (Input.IsActionPressed("shoot") && 
                 (_direction == Vector2.Left || _direction == Vector2.Right)
                 )
        {
            _stateMachine?.SetState(_shootingComponent.Shoot(SetDirectionBasedOnSprite())
                ? "RunShootState"
                : "RunState");
        }
       
        if (Input.IsActionJustPressed("dashDodge") && _dashCooldownTimer?.TimeLeft == 0) 
            _stateMachine?.SetState("DashState");
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.DecelerateToZeroVelocity(delta);
    }
    
    private Vector2 SetDirectionBasedOnSprite()
    {
        Vector2 directionFacing = Vector2.Zero;
        if (_characterSprite.IsFlippedH())
            directionFacing = Vector2.Left;
        else if (!_characterSprite.IsFlippedV())
            directionFacing = Vector2.Right;
		
        return directionFacing;
    }
}