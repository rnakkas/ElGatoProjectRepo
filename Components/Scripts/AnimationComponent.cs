using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class AnimationComponent : Node
{
	[Export] public AnimatedSprite2D Sprite;
	[Export] public float Direction { get; set; }
	[Export] public Vector2 Velocity {get; set;}
	[Export] public bool IsOnFloor {get; set;}
	[Export] public bool IsLeftWallDetected {get; set;}
	[Export] public bool IsRightWallDetected {get; set;}
	[Export] public bool HurtStatus { get; set; }
	
	public void FlipSprite(float direction)
	{
		if (Sprite == null) 
			return;
		
		if (direction < 0)
		{
			Sprite.FlipH = true;
		}
		else if (direction > 0)
		{
			Sprite.FlipH = false;
		}
	}
	
	public void FlipSpriteToFaceHitDirection(Vector2 attackPosition)
	{
		if (Sprite == null)
			return;
		
		// Flip sprite if hit from behind
		if (attackPosition.X > 0 && Sprite.IsFlippedH())
		{
			Sprite.FlipH = false;
		}
		else if (attackPosition.X < 0 && !Sprite.IsFlippedH())
		{
			Sprite.FlipH = true;
		}
	}

	public void PlayAnimations()
	{
		if (Sprite == null)
			return;
		
		FlipSprite(Direction);

		if (!HurtStatus)
		{
			if (Velocity.X == 0 && IsOnFloor)
			{
				Sprite.Play("idle");
			}
			else if (Velocity.X != 0 && IsOnFloor)
			{
				Sprite.Play("run");
			}
			else if (Velocity.Y < 0 && !IsOnFloor)
			{
				Sprite.Play("jump");
			}
			else if (Velocity.Y > 0 && !IsOnFloor)
			{
				Sprite.Play("fall");
			}
		
			if (!IsOnFloor && IsLeftWallDetected)
			{
				FlipSprite(1.0f);
				Sprite.Play("wall_slide");
			}
			else if (!IsOnFloor && IsRightWallDetected)
			{
				FlipSprite(-1.0f);
				Sprite.Play("wall_slide");
			}
		}
		else
		{
			Sprite.Play("hurt");
		}
	}

	public void PlayProjectileAnimations(Utility.WeaponType bulletWeaponType)
	{
		Sprite.SetVisible(true);
		
		switch (bulletWeaponType)
		{
			case Utility.WeaponType.EnemyPistol:
			case Utility.WeaponType.PlayerPistol:
				Sprite.Play("pistol_fly");
				GD.Print("pistol_fly animation");
				break;
			case Utility.WeaponType.EnemyShotgun:
			case Utility.WeaponType.PlayerShotgun:
				Sprite.Play("shotgun_fly");
				break;
			case Utility.WeaponType.EnemyMachineGun:
			case Utility.WeaponType.PlayerMachineGun:
				Sprite.Play("machinegun_fly");
				break;
			case Utility.WeaponType.EnemyRailGun:
			case Utility.WeaponType.PlayerRailGun:
				Sprite.Play("railgun_fly");
				break;
		}
	}
}
