using Godot;
using System;
using Godot.Collections;

namespace ElGatoProject.Singletons;
public partial class Utility : Node
{
    public static Utility Instance { get; private set; }
    
    public enum EnemyType
    {
        Melee,
        Ranged
    }

    public enum PickupType
    {
        Coffee,
        Catnip,
        WeaponTypeModifier
    }

    public enum WeaponType
    {
        None,
        EnemyPistol,
        EnemyShotgun,
        EnemyMachineGun,
        EnemyRailGun,
        PlayerPistol,
        PlayerShotgun,
        PlayerMachineGun,
        PlayerRailGun
    }

    public enum PlayerOrEnemy
    {
        Player,
        Enemy
    }

    // Character animation string names
    public string EntityIdleAnimation = "idle";
    public string EntityRunAnimation = "run";
    public string EntityJumpAnimation = "jump";
    public string EntityFallAnimation = "fall";
    public string EntityWallSlideAnimation = "wall_slide";
    public string EntityHurtAnimation = "hurt";
    public string EntityShootAnimation = "shoot";
    
    // Projectile animation string names
    public string EnemyMachineGunFly = "enemy_machinegun_fly";
    public string EnemyMachineGunHit = "enemy_machinegun_hit";
    public string EnemyPistolFly = "enemy_pistol_fly";
    public string EnemyPistolHit = "enemy_pistol_hit";
    public string EnemyRailGunFly = "enemy_railgun_fly";
    public string EnemyRailGunHit = "enemy_railgun_hit";
    public string EnemyShotgunFly = "enemy_shotgun_fly";
    public string EnemyShotgunHit = "enemy_shotgun_hit";
    public string PlayerMachineGunFly = "player_machinegun_fly";
    public string PlayerMachineGunHit = "player_machinegun_hit";
    public string PlayerPistolFly = "player_pistol_fly";
    public string PlayerPistolHit = "player_pistol_hit";
    public string PlayerRailGunFly = "player_railgun_fly";
    public string PlayerRailGunHit = "player_railgun_hit";
    public string PlayerShotgunFly = "player_shotgun_fly";
    public string PlayerShotgunHit = "player_shotgun_hit";

    public override void _Ready()
    {
        Instance = this;
    }
}
