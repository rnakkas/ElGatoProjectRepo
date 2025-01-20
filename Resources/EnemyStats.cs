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
        RangedEnemyHeavy
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
    
    // Methods
    public void TakeDamage(int damage)
    {
        EnemyHealth -= damage;
    }
}
