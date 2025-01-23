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

    public override void _Ready()
    {
        Instance = this;
    }
}
