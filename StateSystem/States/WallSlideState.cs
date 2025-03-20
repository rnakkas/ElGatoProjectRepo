using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class WallSlideState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
    private RayCast2D _leftWallDetect, _rightWallDetect;
    
    private StateMachine _stateMachine;
    private Vector2 _direction = Vector2.Zero;

    public override void _Ready()
    {
        _character = GetOwnerOrNull<CharacterBody2D>();
        _velocityComponent = _character.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _animationPlayer = _character.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _characterSprite = _character.GetNodeOrNull<AnimatedSprite2D>("sprite");
        _leftWallDetect = _character.GetNodeOrNull<RayCast2D>("LeftWallDetect");
        _rightWallDetect = _character.GetNodeOrNull<RayCast2D>("RightWallDetect");
    }

    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animationPlayer?.Play("wall_slide");
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
        if (_character != null && _leftWallDetect != null && _rightWallDetect != null)
        {
            if (_character.IsOnFloor())
            {
                _stateMachine?.SetState("IdleState");
	
                if (_direction != Vector2.Zero)
                {
                    _stateMachine?.SetState("RunState");
                }
            }
            else if (!_character.IsOnFloor())
            {
                if (_leftWallDetect.IsColliding())
                {
                    if (_characterSprite != null) _characterSprite.FlipH = false;

                    _direction = Vector2.Right;
                }
                else if (_rightWallDetect.IsColliding())
                {
                    if (_characterSprite != null) _characterSprite.FlipH = true;
                    _direction = Vector2.Left;
                }
                else
                {
                    _stateMachine?.SetState("IdleState");
                }
	
                if (Input.IsActionJustPressed("jump"))
                {
                    _stateMachine?.SetState("WallJumpState");
                }
            }
        }
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.WallSlidingVelocity(delta);
    }
}