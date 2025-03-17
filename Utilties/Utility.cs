using Godot.Collections;

namespace ElGatoProject.Utilties;
public static class Utility
{
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

    public enum WeaponTriggerType
    {
        None,
        SingleShot,
        Automatic
    }
    
    public enum PlayerOrEnemy
    {
        Player,
        Enemy
    }

    public enum EntityState
    {
        Idle,
        IdleShoot,
        Run,
        RunShoot,
        Jump,
        JumpShoot,
        Fall,
        FallShoot,
        Hurt,
        Death,
        WallSlide,
        WallSlideShoot,
        WallJump,
        WallJumpShoot,
        Dash,
        Shoot
    }

    public static readonly Dictionary<EntityState, string> EntityAnimations = new()
    {
        [EntityState.Idle] = "idle",
        [EntityState.IdleShoot] = "idle_shoot",
        [EntityState.Run] = "run",
        [EntityState.RunShoot] = "run_shoot",
        [EntityState.Jump] = "jump",
        [EntityState.JumpShoot] = "jump_shoot",
        [EntityState.Fall] = "fall",
        [EntityState.FallShoot] = "fall_shoot",
        [EntityState.Hurt] = "hurt",
        [EntityState.Death] = "death",
        [EntityState.WallSlide] = "wall_slide",
        [EntityState.WallSlideShoot] = "wall_slide_shoot",
        [EntityState.WallJump] = "jump",
        [EntityState.WallJumpShoot] = "jump_shoot",
        [EntityState.Dash] = "dash",
        [EntityState.Shoot] = "shoot"
    };
    
    // Projectile animation string names
    public const string BulletFlyAnimation = "fly";
    public const string BulletHitAnimation = "hit";
    
    // Scene transition and shader string names
    public const string SceneTransitionWipeAnimation = "screen_wipe_animation";
    public const string ShaderParameterWipeColor = "wipe_color";
    
    // Node group string names
    public const string NodeGroupPlayers = "Players";
    public const string NodeGroupEnemies = "Enemies";
    public const string NodeGroupPlayersHurtbox = "PlayersHurtBox";
    public const string NodeGroupLevels = "Levels";
    
    // Level names
    public const string LevelOne = "level_one";
    
    /*** Path strings for packed scenes and resources **/
    // HUD Icons
    public const string ShotgunHudIconTexturePath = "res://Assets/HUD/shotgun-Icon-002.png";
    public const string MachineGunHudIconTexturePath = "res://Assets/HUD/machineGun-Icon-002.png";
    public const string RailGunHudIconTexturePath = "res://Assets/HUD/railGun-Icon-002.png";
    
    // Bullet
    public const string BulletPackedScenePath = "res://Projectiles/Scenes/bullet_projectile.tscn";
    
    // Resources
    public const string PlayerPistolShootingProperties = "res://Resources/PlayerPistolShootingProperties.tres";
    public const string PlayerShotgunShootingProperties = "res://Resources/PlayerShotgunShootingProperties.tres";
    public const string PlayerMachineGunShootingProperties = "res://Resources/PlayerMachineGunShootingProperties.tres";
    public const string PlayerRailGunShootingProperties = "res://Resources/PlayerRailGunShootingProperties.tres";
    
    // Muzzle flashes
    public static readonly Dictionary<WeaponType, string> MuzzleFlashPackedScenePaths = new()
    {
        [WeaponType.PlayerPistol] = "res://Players/Scenes/PackedScenes/weapon_muzzle_flash_pistol.tscn",
        [WeaponType.PlayerShotgun] = "res://Players/Scenes/PackedScenes/weapon_muzzle_flash_shotgun.tscn",
        [WeaponType.PlayerMachineGun] = "res://Players/Scenes/PackedScenes/weapon_muzzle_flash_machinegun.tscn",
        [WeaponType.PlayerRailGun] = "res://Players/Scenes/PackedScenes/weapon_muzzle_flash_railgun.tscn"
    };
    
    // Bullet sprites
    public static readonly Dictionary<WeaponType, string> BulletSpritePackedScenePaths = new()
    {
        [WeaponType.PlayerPistol] = "res://Projectiles/PackedScenes/pistol_bullet_projectile.tscn",
        [WeaponType.PlayerShotgun] = "res://Projectiles/PackedScenes/shotgun_bullet_projectile.tscn",
        [WeaponType.PlayerMachineGun] = "res://Projectiles/PackedScenes/machinegun_bullet_projectile.tscn",
        [WeaponType.PlayerRailGun] = "res://Projectiles/PackedScenes/railgun_bullet_projectile.tscn"
    };

}
