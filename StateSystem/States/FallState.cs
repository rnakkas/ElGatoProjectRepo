using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class FallState: Node, IState
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
        
        if (_character != null && _character.IsOnFloor())
            _stateMachine?.SetState("IdleState");
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.AccelerateToMaxVelocity(delta, _direction);
        _velocityComponent?.FallVelocity(delta);
    }
}