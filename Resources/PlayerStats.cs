using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export] public float MaxSpeed { get; set; }
    [Export] public float Acceleration { get; set; }
    [Export] public float Friction { get; set; }
    [Export] public float MaxHealth { get; set; }
    [Export] public float CurrentHealth{ get; set; }
    [Export] public float JumpVelocity{ get; set; }
    [Export] public float Gravity { get; set; }
    [Export] public float WallSlideGravity { get; set; }
    [Export] public float WallJumpVelocity { get; set; }
    [Export] public float WallSlideVelocity { get; set; }
    
    [Export] public float HurtStaggerTime { get; set; }

    public enum State
    {
        Idle,
        Run,
        Jump,
        Fall,
        WallSlide,
        Hurt,
        Death
    }
    
    [Export] public State PlayerState { get; set; }
    
    
}
