using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class JumpState : Node, IState
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
        _velocityComponent?.JumpeVelocity();
        _animationPlayer?.Play("jump");
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
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.AccelerateToMaxVelocity(delta, _direction);

        if (!_character.IsOnFloor() || _character.IsOnCeiling())
        {
            _velocityComponent?.FallVelocity(delta);

            if (_velocityComponent?.EntityVelocity.Y > 0)
            {
                _stateMachine?.SetState("FallState");
            }
        }
    }
}