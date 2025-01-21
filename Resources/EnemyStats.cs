using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class EnemyStats : Resource
{
    public enum Type
    {
        MeleeEnemyStationary,
        MeleeEnemyMoving,
        RangedEnemyLight,
        RangedEnemyHeavy,
        RangedEnemyMachineGun
    }
    
    [Export] public Type EnemyType { get; set; }
    [Export] public int EnemyHealth { get; set; }
    [Export] public float AttackCooldownTime { get; set; }
    [Export] public int AttackDamage { get; set; }
    [Export] public float Knockback { get; set; }
    [Export] public float PatrolSpeed { get; set; }
    [Export] public float ChaseSpeed { get; set; }
    [Export] public float ChaseTime { get; set; }
    [Export] public float HurtStaggerTime { get; set; }
    [Export] public int BulletsPerShot { get; set; }
    [Export] public float BulletAngle { get; set; }
    [Export] public float BulletSpeed { get; set; }
    [Export] public float BulletDespawnTimeSeconds { get; set; }
    
    // Methods
    public void TakeDamage(int damage)
    {
        EnemyHealth -= damage;
    }
}
