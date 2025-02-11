using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Singletons;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public RandomNumberGenerator Rng = new();
    
    public PackedScene BulletProjectile = 
        ResourceLoader.Load<PackedScene>("res://Projectiles/Scenes/bullet_projectile.tscn");

    public ShootingProperties PlayerPistolShootingProperties = 
        ResourceLoader.Load<ShootingProperties>("res://Resources/PlayerPistolShootingProperties.tres");

    public ShootingProperties PlayerShotgunShootingProperties =
        ResourceLoader.Load<ShootingProperties>("res://Resources/PlayerShotgunShootingProperties.tres");
    
    public ShootingProperties PlayerMachineGunShootingProperties =
        ResourceLoader.Load<ShootingProperties>("res://Resources/PlayerMachineGunShootingProperties.tres");
    
    public ShootingProperties PlayerRailGunShootingProperties =
        ResourceLoader.Load<ShootingProperties>("res://Resources/PlayerRailGunShootingProperties.tres");
    
    public override void _Ready()
    {
        Instance = this;
    }
}
