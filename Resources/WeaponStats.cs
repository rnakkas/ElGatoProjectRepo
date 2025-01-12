using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class WeaponStats : Resource
{
    [Export] public float CooldownTime { get; set; }
    [Export] public int BulletsPerShot { get; set; }
    [Export] public float ShotAngleDegrees { get; set; }

    public enum WeaponState
    {
        Idle,
        Shooting
    }

    public WeaponState State;
}
