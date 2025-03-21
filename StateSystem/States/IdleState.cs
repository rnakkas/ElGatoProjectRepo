using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class IdleState : Node, IState
{
    // Generic character reference. This will work for Player, Enemy, etc.
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
    private Timer _dashCooldownTimer;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;

    public override void _Ready()
    {
        _character = GetOwner<CharacterBody2D>();
        _velocityComponent = _character.GetNodeOrNull<VelocityComponent>("VelocityComponent");
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
        _animationPlayer?.Play("idle");
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
        if (_character.IsInGroup(Utility.NodeGroupPlayers))
        {
             _direction = Input.GetVector(
                        "move_left", 
                        "move_right", 
                        "move_up", 
                        "move_down");
        }
        
        if (_characterSprite != null) _stateMachine.FlipSprite(_characterSprite, _direction);
        
        if (_direction == Vector2.Left || _direction == Vector2.Right)
        {
            _stateMachine?.SetState("RunState");
        }
        else if (_character != null)
        {
            if (!_character.IsOnFloor() || _character.IsOnCeiling())
                _stateMachine?.SetState("FallState");
            else if (Input.IsActionPressed("jump") && _character.IsOnFloor())
                _stateMachine?.SetState("JumpState");
        }
        
        if (Input.IsActionJustPressed("dashDodge") && _dashCooldownTimer?.TimeLeft == 0) 
            _stateMachine?.SetState("DashState");
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.DecelerateToZeroVelocity(delta);
    }
}