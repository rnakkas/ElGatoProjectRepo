using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class CharacterVelocityProperties : Resource
{
    [Export] public float MaxSpeed { get; set; }
    [Export] public float DashSpeed { get; set; }
    [Export] public float Acceleration { get; set; }
    [Export] public float Friction { get; set; }
    [Export] public float JumpVelocity{ get; set; }
    [Export] public float Gravity { get; set; }
    [Export] public float WallSlideGravity { get; set; }
    [Export] public float WallJumpVelocity { get; set; }
    [Export] public float WallSlideVelocity { get; set; }
}
