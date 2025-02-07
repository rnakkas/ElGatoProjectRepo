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

	public void PlayCharacterAnimations()
	{
		if (Sprite == null)
			return;
		
		FlipSprite(Direction);

		if (!HurtStatus)
		{
			if (Velocity.X == 0 && IsOnFloor)
			{
				Sprite.Play(Utility.Instance.EntityIdleAnimation);
			}
			else if (Velocity.X != 0 && IsOnFloor)
			{
				Sprite.Play(Utility.Instance.EntityRunAnimation);
			}
			else if (Velocity.Y < 0 && !IsOnFloor)
			{
				Sprite.Play(Utility.Instance.EntityJumpAnimation);
			}
			else if (Velocity.Y > 0 && !IsOnFloor)
			{
				Sprite.Play(Utility.Instance.EntityFallAnimation);
			}
		
			if (!IsOnFloor && IsLeftWallDetected)
			{
				FlipSprite(1.0f);
				Sprite.Play(Utility.Instance.EntityWallSlideAnimation);
			}
			else if (!IsOnFloor && IsRightWallDetected)
			{
				FlipSprite(-1.0f);
				Sprite.Play(Utility.Instance.EntityWallSlideAnimation);
			}
		}
		else
		{
			Sprite.Play(Utility.Instance.EntityHurtAnimation);
		}
	}

	public void PlayProjectileAnimations(Utility.WeaponType bulletWeaponType, bool hitStatus)
	{
		switch (bulletWeaponType)
		{
			case Utility.WeaponType.EnemyPistol:
				Sprite.Play(!hitStatus ? Utility.Instance.EnemyPistolFly : Utility.Instance.EnemyPistolHit);
				break;
			case Utility.WeaponType.PlayerPistol:
				Sprite.Play(!hitStatus ?  Utility.Instance.PlayerPistolFly : Utility.Instance.PlayerPistolHit);
				break;
			case Utility.WeaponType.EnemyShotgun:
				Sprite.Play(!hitStatus ? Utility.Instance.EnemyShotgunFly : Utility.Instance.EnemyShotgunHit);
				break;
			case Utility.WeaponType.PlayerShotgun:
				Sprite.Play(!hitStatus ? Utility.Instance.PlayerShotgunFly : Utility.Instance.PlayerShotgunHit);
				break;
			case Utility.WeaponType.EnemyMachineGun:
				Sprite.Play(!hitStatus ? Utility.Instance.EnemyMachineGunFly : Utility.Instance.EnemyMachineGunHit);
				break;
			case Utility.WeaponType.PlayerMachineGun:
				Sprite.Play(!hitStatus ? Utility.Instance.PlayerMachineGunFly : Utility.Instance.PlayerMachineGunHit);
				break;
			case Utility.WeaponType.EnemyRailGun:
				Sprite.Play(!hitStatus ? Utility.Instance.EnemyRailGunFly : Utility.Instance.EnemyRailGunHit);
				break;
			case Utility.WeaponType.PlayerRailGun:
				Sprite.Play(!hitStatus ? Utility.Instance.PlayerRailGunFly : Utility.Instance.PlayerRailGunHit);
				break;
		}
	}

	public void PlayWeaponAnimations(bool isShooting, float direction)
	{
		FlipSprite(direction);

		if (isShooting)
		{
			Sprite.Play(Utility.Instance.EntityShootAnimation);
		}
		else
		{
			if (!Sprite.IsPlaying())
			{
				Sprite.Play(Utility.Instance.EntityIdleAnimation);
			}
		}
	}
}
