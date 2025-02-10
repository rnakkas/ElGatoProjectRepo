using Godot;
using System;

namespace ElGatoProject.Resources;

[GlobalClass]
public partial class ShootingProperties : Resource
{
    [Export] public float ShootingCooldownTime;
    [Export] public float ReloadTime;
    [Export] public int MagazineSize;
    [Export] public int BulletDamage;
    [Export] public float BulletKnockback;
    [Export] public int BulletsPerShot;
    [Export] public float BulletSwayAngle;
    [Export] public float BulletSpeed;
}
