using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public RandomNumberGenerator Rng = new();
    
    public PackedScene EnemyPistolBullet = ResourceLoader.Load<PackedScene>("res://Enemies/Scenes/enemy_bullet.tscn");
    
    public PackedScene PlayerPistolBullet = ResourceLoader.Load<PackedScene>("res://Players/Scenes/bullet.tscn");
    
    public override void _Ready()
    {
        Instance = this;
    }
}
