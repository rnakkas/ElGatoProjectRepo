using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public RandomNumberGenerator Rng = new();
    
    public PackedScene BulletProjectile = ResourceLoader.Load<PackedScene>("res://Projectiles/Scenes/bullet_projectile.tscn");
    
    public override void _Ready()
    {
        Instance = this;
    }
}
