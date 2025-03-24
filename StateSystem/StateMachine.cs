using System.Collections.Generic;
using ElGatoProject.StateSystem.Interfaces;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.StateSystem;

[GlobalClass]
public partial class StateMachine : Node
{
    private IState _currentState;
    private Dictionary<string, IState> _states = new();

    public override void _Ready()
    {
        // Automatically detect child states that implement IState.
        foreach (var child in GetChildren())
        {
            if (child is IState state)
            {
                // Initialize the state with a ref to this stateMachine
                state.Initialize(this);
                
                // The node name is used as the state key.
                _states[child.Name] = state;
            }
        }

        // Optionally set an initial state (e.g., "IdleState")
        if (_states.ContainsKey(Utility.EntityState.IdleState.ToString()))
        {
            SetState(Utility.EntityState.IdleState.ToString());
        }
        else if (_states.Count > 0)
        {
            // Fallback: set the first state in the dictionary
            foreach (var key in _states.Keys)
            {
                SetState(key);
                break;
            }
        }
    }

    // Change the state by name.
    public void SetState(string stateName)
    {
        if (!_states.TryGetValue(stateName, out var state))
        {
            GD.PrintErr($"StateMachine: State '{stateName}' not found.");
            return;
        }

        if (_currentState == state)
            return;

        _currentState?.Exit();
        _currentState = _states[stateName];
        _currentState?.Enter();
    }

    public override void _Process(double delta)
    {
        _currentState?.Update((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState?.PhysicsUpdate((float)delta);
    }
}