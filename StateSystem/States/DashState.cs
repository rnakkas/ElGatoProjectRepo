using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class DashState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
    private Timer _dashTimer, _dashCooldownTimer;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;
    
    public override void _Ready()
    {
        _character = GetOwner<CharacterBody2D>();
        _velocityComponent = _character.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _animationPlayer = _character.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _characterSprite = _character.GetNodeOrNull<AnimatedSprite2D>("sprite");
        
        _dashTimer = _character.GetNodeOrNull<Timer>("Timers/DashTimer");
        _dashCooldownTimer = _character.GetNodeOrNull<Timer>("Timers/DashCooldownTimer");

        if (_dashTimer != null)
        {
            _dashTimer.OneShot = true;
            if (_velocityComponent != null)
                _dashTimer.SetWaitTime(_velocityComponent.CharacterVelocityProperties.DashTime);
            _dashTimer.Timeout += OnDashTimerTimedOut;
        }

        if (_dashCooldownTimer != null)
        {
            _dashCooldownTimer.OneShot = true;
            if (_velocityComponent != null)
                _dashCooldownTimer.SetWaitTime(_velocityComponent.CharacterVelocityProperties.DashCooldownTime);
        }
    }

    private void OnDashTimerTimedOut()
    {
        _stateMachine.SetState(Utility.EntityState.IdleState.ToString());
    }
    
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.DashState]);
        
        if (_characterSprite != null && !_characterSprite.IsFlippedH())
        {
            _velocityComponent?.DashVelocity(Vector2.Right);
        }
        else if (_characterSprite != null && _characterSprite.IsFlippedH())
        {
            _velocityComponent?.DashVelocity(Vector2.Left);
        }
        
        _dashTimer?.Start();
        _dashCooldownTimer?.Start();
    }

    public void Exit()
    {
        _velocityComponent?.ResetHorizontalVelocity();
        _velocityComponent?.ResetVerticalVelocity();
    }

    public void Update(float delta)
    {
    }

    public void PhysicsUpdate(float delta)
    {
        
    }
}