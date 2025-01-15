using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class WeaponStats : Resource
{
    [Export] public float CooldownTime { get; set; }
    [Export] public int BulletsPerShot { get; set; }
    [Export] public float WeaponSwayDegrees { get; set; }
    [Export] public float BulletSpeed { get; set; }
    [Export] public int BulletDamage { get; set; }
    [Export] public float BulletDespawnTimeSeconds { get; set; }
    [Export] public float BulletKnockback { get; set; }

}
