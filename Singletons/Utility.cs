using Godot;
using System;
using Godot.Collections;

namespace ElGatoProject.Singletons;
public partial class Utility : Node
{
    public static Utility Instance { get; private set; }

    public enum EntityState
    {
        Idle,
        Run,
        Jump,
        Fall,
        WallSlide,
        Shoot,
        Hurt,
        Death
    }
    
    public enum RangedEnemyType
    {
        RangedEnemyLight,
        RangedEnemyHeavy,
        RangedEnemyMachineGun
    }

    public enum PickupType
    {
        Coffee,
        Catnip,
        WeaponMod
    }
    
    public enum WeaponModType
    {
    	None,
    	Shotgun,
    	MachineGun,
        RailGun
    }

    public enum WeaponType
    {
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

    // Bullet animation names
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
