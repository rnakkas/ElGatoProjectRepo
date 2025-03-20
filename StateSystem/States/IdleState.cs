using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class IdleState : Node, IState
{
    // Generic character reference. This will work for Player, Enemy, etc.
    [Export] private CharacterBody2D _character;
    [Export] private VelocityComponent _velocityComponent;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private AnimatedSprite2D _characterSprite;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;

    public override void _Ready()
    {
        // Use GetOwner() to obtain the character that owns this state machine.
        // _character = GetOwnerOrNull<CharacterBody2D>();
        // _velocityComponent = _character.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        // _animationPlayer = _character.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
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
        _direction = Input.GetVector(
            "move_left", 
            "move_right", 
            "move_up", 
            "move_down");

        _stateMachine.FlipSprite(_characterSprite, _direction);
        
        if (_direction == Vector2.Left || _direction == Vector2.Right)
        {
            // Get the state machine (assumes IdleState is a child of StateMachine)
            _stateMachine?.SetState("RunState");
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
        _velocityComponent?.DecelerateToZeroVelocity(delta);
    }
}