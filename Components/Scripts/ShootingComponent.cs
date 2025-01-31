using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class ShootingComponent : Node2D
{
	[Export] private float _shootingCooldownTime;
	[Export] private int _bulletDamage;
	[Export] private float _bulletKnockback;
	[Export] private float _bulletsPerShot;
	[Export] private float _bulletSwayAngle;
	[Export] private float _bulletSpeed;
	[Export] private float _bulletLifeTime;
	[Export] private float _rapitFireTime;
	[Export] private PackedScene _bulletScene;
	[Export] private Marker2D _muzzle;
}

/*
 * Shooting compoent
 * Sets the bullet properties
 * Can choose what type of bullet to shoot
 */
