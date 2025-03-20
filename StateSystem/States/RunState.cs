using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class RunState: Node, IState
{
    [Export] private CharacterBody2D _character;
    [Export] private VelocityComponent _velocityComponent;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private AnimatedSprite2D _characterSprite;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;
    
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animationPlayer?.Play("run");
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
        
        _stateMachine.FlipSprite(_characterSprite, _direction);
        
        if (_direction == Vector2.Zero)
        {
            _stateMachine?.SetState("IdleState");
        }
        else if (_character != null)
        {
            if (!_character.IsOnFloor() || _character.IsOnCeiling())
                _stateMachine?.SetState("FallState");
            else if (Input.IsActionPressed("jump") && _character.IsOnFloor())
                _stateMachine?.SetState("JumpState");
        }
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
    }
}