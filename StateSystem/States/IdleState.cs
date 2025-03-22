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
    private ShootingComponent _shootingComponent;
    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _characterSprite;
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
        _dashCooldownTimer = _character?.GetNodeOrNull<Timer>("Timers/DashCooldownTimer");
    }
    
    public void Initialize(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public async void Enter()
    {
        _velocityComponent?.ResetVerticalVelocity();

        if (_animationPlayer != null)
        {
            if (_animationPlayer.CurrentAnimation == Utility.EntityAnimations[Utility.EntityState.IdleShootState])
            {
                await ToSignal(_animationPlayer, "animation_finished");
            }
        }

        _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.IdleState]);
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
        if (_character != null)
        {
            if (_character.IsInGroup(Utility.NodeGroupPlayers))
            {
                _direction = Input.GetVector(
                    "move_left",
                    "move_right",
                    "move_up",
                    "move_down");
            }
        }

        if (_characterSprite != null) _stateMachine.FlipSprite(_characterSprite, _direction);
        
        if (_direction == Vector2.Left || _direction == Vector2.Right)
        {
            _stateMachine?.SetState(Utility.EntityState.RunState.ToString());
        }
        else if (_character != null)
        {
            if (!_character.IsOnFloor() || _character.IsOnCeiling())
                _stateMachine?.SetState(Utility.EntityState.FallState.ToString());
            else if (Input.IsActionPressed("jump") && _character.IsOnFloor())
                _stateMachine?.SetState(Utility.EntityState.JumpState.ToString());
        }
        
        if (Input.IsActionJustPressed("dashDodge") && _dashCooldownTimer?.TimeLeft == 0) 
            _stateMachine?.SetState(Utility.EntityState.DashState.ToString());

        if (_shootingComponent == null) 
            return;
        if (Input.IsActionPressed("shoot"))
        {
            if (_shootingComponent.Shoot(SetDirectionBasedOnSprite()))
                _stateMachine?.SetState(Utility.EntityState.IdleShootState.ToString());
        }
    }

    public void PhysicsUpdate(float delta)
    {
        _velocityComponent?.DecelerateToZeroVelocity(delta);
    }
    
    private Vector2 SetDirectionBasedOnSprite()
    {
        var directionFacing = Vector2.Zero;

        if (_characterSprite == null) 
            return directionFacing;
        
        if (_characterSprite.IsFlippedH())
            directionFacing = Vector2.Left;
        else if (!_characterSprite.IsFlippedV())
            directionFacing = Vector2.Right;

        return directionFacing;
    }
}