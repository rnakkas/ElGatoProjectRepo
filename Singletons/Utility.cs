using Godot;
using System;

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

    public override void _Ready()
    {
        Instance = this;
    }
}
