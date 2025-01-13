using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class BulletStats : Resource
{
    [Export] public float BulletSpeed { get; set; }
    [Export] public int BulletDamage { get; set; }
    [Export] public float BulletDespawnTime { get; set; }
    [Export] public float BulletKnockback { get; set; }
}
