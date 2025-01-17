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
    [Export] public int AttackDamage;
    [Export] public float Knockback;
    [Export] public float PatrolSpeed;
    [Export] public float ChaseSpeed;
    [Export] public float ChaseTime;
    [Export] public float HurtStaggerTime;
    
    // Methods
    public void TakeDamage(int damage)
    {
        EnemyHealth -= damage;
    }
}
