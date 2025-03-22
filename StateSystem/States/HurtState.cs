using ElGatoProject.Components.Scripts;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class HurtState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private AnimationPlayer _animationPlayer;
    
    private StateMachine _stateMachine;
    
    public override void _Ready()
    {
        _character = GetOwnerOrNull<CharacterBody2D>();
        _velocityComponent = _character?.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _animationPlayer = _character?.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
    }
    
    public void Initialize(StateMachine stateMachine)
    {
       _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.HurtState]);
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
    }

    public void PhysicsUpdate(float delta)
    {
        if (_character == null) return;
        if (!_character.IsOnFloor())
        {
            _velocityComponent?.FallVelocity(delta);
        }
    }
}