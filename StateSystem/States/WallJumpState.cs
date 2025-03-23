using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class WallJumpState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private ShootingComponent _shootingComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
    private RayCast2D _leftWallDetect, _rightWallDetect;
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
        _leftWallDetect = _character?.GetNodeOrNull<RayCast2D>("LeftWallDetect");
        _rightWallDetect = _character?.GetNodeOrNull<RayCast2D>("RightWallDetect");
        _dashCooldownTimer = _character?.GetNodeOrNull<Timer>("Timers/DashCooldownTimer");
    }
    
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        if (_leftWallDetect != null && _rightWallDetect != null)
        {
            if (_leftWallDetect.IsColliding())
                _direction = Vector2.Right;
            else if (_rightWallDetect.IsColliding())
                _direction = Vector2.Left;
        }
        _velocityComponent?.WallJumpingVelocity(_direction);
        _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.JumpState]);
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
        if (Input.IsActionJustPressed("dashDodge") && _dashCooldownTimer?.TimeLeft == 0) 
            _stateMachine?.SetState(Utility.EntityState.DashState.ToString());
    }

    public void PhysicsUpdate(float delta)
    {
        if (_character == null) 
            return;

        if (_character.IsOnFloor()) 
            return;
        
        _velocityComponent?.FallVelocity(delta);
	
        if (_velocityComponent?.EntityVelocity.Y > 0)
        {
            _stateMachine?.SetState(Utility.EntityState.FallState.ToString());
        }
    }
}