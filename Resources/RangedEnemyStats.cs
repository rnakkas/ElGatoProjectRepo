using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class RangedEnemyStats : Resource
{
    public enum Type
    {
        RangedEnemyLight,
        RangedEnemyHeavy,
        RangedEnemyMachineGun
    }
    
    [Export] public Type RangedEnemyType { get; set; }
    [Export] public int Health { get; set; }
    [Export] public float AttackCooldownTime { get; set; }
    [Export] public int AttackDamage { get; set; }
    [Export] public float Knockback { get; set; }
    [Export] public float HurtStaggerTime { get; set; }
    [Export] public int BulletsPerShot { get; set; }
    [Export] public float BulletAngle { get; set; }
    [Export] public float BulletSpeed { get; set; }
    [Export] public float BulletDespawnTimeSeconds { get; set; }
    [Export] public float RapidFireTime { get; set; }
    
    // Methods
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}
