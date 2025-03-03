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

    public enum CharacterState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Hurt,
        Death,
        WallSlide,
        WallJump,
        Dash,
        Shoot
    }

    // Character animation string names
    public string EntityIdleAnimation = "idle";
    public string EntityRunAnimation = "run";
    public string EntityJumpAnimation = "jump";
    public string EntityFallAnimation = "fall";
    public string EntityWallSlideAnimation = "wall_slide";
    public string EntityHurtAnimation = "hurt";
    public string EntityShootAnimation = "shoot";
    public string EntityDashAnimation = "dash";
    
    // Projectile animation string names
    public string BulletFlyAnimation = "fly";
    public string BulletHitAnimation = "hit";
    
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

    // Scene transition and shader string names
    public string SceneTransitionWipeAnimation = "screen_wipe_animation";
    public string ShaderParameterWipeColor = "wipe_color";
    
    // Node group string names
    public string NodeGroupPlayers = "Players";
    
    // Misc strings
    public string InfinitySymbol = "\u221E";
    
    // Packed scene path strings
    // HUD Icons
    public string ShotgunHudIconTexturePath = "res://Assets/HUD/shotgun-Icon-001.png";
    public string MachineGunHudIconTexturePath = "res://Assets/HUD/machineGun-Icon-001.png";
    public string RailGunHudIconTexturePath = "res://Assets/HUD/railGun-Icon-001.png";
    
    public override void _Ready()
    {
        Instance = this;
    }
}
