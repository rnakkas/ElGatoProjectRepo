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
    public const string EntityIdleAnimation = "idle";
    public const string EntityRunAnimation = "run";
    public const string EntityJumpAnimation = "jump";
    public const string EntityFallAnimation = "fall";
    public const string EntityWallSlideAnimation = "wall_slide";
    public const string EntityHurtAnimation = "hurt";
    public const string EntityShootAnimation = "shoot";
    public const string EntityDashAnimation = "dash";
    public const string EntityDeathAnimation = "death";
    
    // Projectile animation string names
    public const string BulletFlyAnimation = "fly";
    public const string BulletHitAnimation = "hit";
    
    public static string EnemyMachineGunFly = "enemy_machinegun_fly";
    public static string EnemyMachineGunHit = "enemy_machinegun_hit";
    public static string EnemyPistolFly = "enemy_pistol_fly";
    public static string EnemyPistolHit = "enemy_pistol_hit";
    public static string EnemyRailGunFly = "enemy_railgun_fly";
    public static string EnemyRailGunHit = "enemy_railgun_hit";
    public static string EnemyShotgunFly = "enemy_shotgun_fly";
    public static string EnemyShotgunHit = "enemy_shotgun_hit";
    public static string PlayerMachineGunFly = "player_machinegun_fly";
    public static string PlayerMachineGunHit = "player_machinegun_hit";
    public static string PlayerPistolFly = "player_pistol_fly";
    public static string PlayerPistolHit = "player_pistol_hit";
    public static string PlayerRailGunFly = "player_railgun_fly";
    public static string PlayerRailGunHit = "player_railgun_hit";
    public static string PlayerShotgunFly = "player_shotgun_fly";
    public static string PlayerShotgunHit = "player_shotgun_hit";

    // Scene transition and shader string names
    public const string SceneTransitionWipeAnimation = "screen_wipe_animation";
    public const string ShaderParameterWipeColor = "wipe_color";
    
    // Node group string names
    public const string NodeGroupPlayers = "Players";
    
    // Packed scene path strings
    // HUD Icons
    public const string ShotgunHudIconTexturePath = "res://Assets/HUD/shotgun-Icon-002.png";
    public const string MachineGunHudIconTexturePath = "res://Assets/HUD/machineGun-Icon-002.png";
    public const string RailGunHudIconTexturePath = "res://Assets/HUD/railGun-Icon-002.png";
    
}
