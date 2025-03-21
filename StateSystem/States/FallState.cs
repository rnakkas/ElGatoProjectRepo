using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class FallState: Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private AnimationPlayer _animationPlayer;
    private RayCast2D _leftWallDetect, _rightWallDetect;
    private Timer _dashCooldownTimer;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;

    public override void _Ready()
    {
        _character = GetOwnerOrNull<CharacterBody2D>();
        _velocityComponent = _character.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _animationPlayer = _character.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _leftWallDetect = _character.GetNodeOrNull<RayCast2D>("LeftWallDetect");
        _rightWallDetect = _character.GetNodeOrNull<RayCast2D>("RightWallDetect");
        _dashCooldownTimer = _character.GetNodeOrNull<Timer>("Timers/DashCooldownTimer");
    }

    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animationPlayer?.Play("fall");
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
        
        if (_character != null && _leftWallDetect != null && _rightWallDetect != null)
        {
            if (_character.IsOnFloor())
                _stateMachine?.SetState("WallSlideState");
            else if (
                !_character.IsOnFloor() &&
                (_leftWallDetect.IsColliding() || _rightWallDetect.IsColliding())
            )
            {
                _stateMachine?.SetState("WallSlideState");
            }
        }
        
        if (Input.IsActionJustPressed("dashDodge") && _dashCooldownTimer?.TimeLeft == 0) 
            _stateMachine?.SetState("DashState");
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
        _velocityComponent?.FallVelocity(delta);
    }
}