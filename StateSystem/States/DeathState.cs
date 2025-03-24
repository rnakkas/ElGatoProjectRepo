using ElGatoProject.Components.Scripts;
using ElGatoProject.Singletons;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem.States;

[GlobalClass]
public partial class DeathState : Node, IState
{
    private CharacterBody2D _character;
    private VelocityComponent _velocityComponent;
    private AnimationPlayer _animationPlayer;
    
    private StateMachine _stateMachine;
    private Timer _gameOverTimer;
    
    public override void _Ready()
    {
        _character = GetOwnerOrNull<CharacterBody2D>();
        _velocityComponent = _character?.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        _animationPlayer = _character?.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _gameOverTimer = _character?.GetNodeOrNull<Timer>("Timers/GameOverTimer");
    }
    
    public void Initialize(StateMachine stateMachine)
    {
       _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _animationPlayer?.Play(Utility.EntityAnimations[Utility.EntityState.DeathState]);

        if (_character.IsInGroup(Utility.NodeGroupPlayers))
        {
            if (_velocityComponent != null) _velocityComponent.EntityVelocity = Vector2.Zero;
            _gameOverTimer?.Start();
            Globals.Instance.IsPlayerDying = true;
        }
    }

    public void Exit()
    {
    }

    public void Update(float delta)
    {
    }

    public void PhysicsUpdate(float delta)
    {
    }
}