using Godot;
using System;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class CharacterStatesComponent : Node2D
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        Fall,
        WallSlide
    }

    public State CurrentState;

    public void UpdateState(Vector2 velocity)
    {
        if (velocity.X != 0)
        {
            CurrentState = State.Run;
        }
        else if (velocity == Vector2.Zero)
        {
            CurrentState = State.Idle;
        }

        if (velocity.Y < 0)
        {
            CurrentState = State.Jump;
        }
        else if (velocity.Y >= 0)
        {
            CurrentState = State.Fall;
        }
    }
}
