using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class PlayerStats : Resource
{
    // Player stats variables
    [Export] public float MaxSpeed { get; set; }
    [Export] public float Acceleration { get; set; }
    [Export] public float Friction { get; set; }
    [Export] public int MaxHealth { get; set; }
    [Export] public int CurrentHealth{ get; set; }
    [Export] public float JumpVelocity{ get; set; }
    [Export] public float Gravity { get; set; }
    [Export] public float WallSlideGravity { get; set; }
    [Export] public float WallJumpVelocity { get; set; }
    [Export] public float WallSlideVelocity { get; set; }
    [Export] public float HurtStaggerTime { get; set; }

    public Utility.EntityState State;
    
    // Methods
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    public void Heal(int heal)
    {
        if (CurrentHealth + heal >= MaxHealth)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + heal, MaxHealth);
        }
        else
        {
            CurrentHealth += heal;
        }
    }
    
    
}
