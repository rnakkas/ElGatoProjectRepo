using Godot;
using System;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class AnimationComponent : Node2D
{
	[Export] private AnimatedSprite2D _sprite;
	[Export] public float Direction { get; set; }
	[Export] public Vector2 Velocity {get; set;}
	[Export] public bool IsOnFloor {get; set;}
	[Export] public bool IsLeftWallDetected {get; set;}
	[Export] public bool IsRightWallDetected {get; set;}
	[Export] public bool HurtStatus { get; set; }
	
	public void FlipSprite(float direction)
	{
		if (_sprite == null) 
			return;
		
		if (direction < 0)
		{
			_sprite.FlipH = true;
		}
		else if (direction > 0)
		{
			_sprite.FlipH = false;
		}
	}
	
	public void FlipSpriteToFaceHitDirection(Vector2 attackPosition)
	{
		if (_sprite == null)
			return;
		
		// Flip sprite if hit from behind
		if (attackPosition.X > 0 && _sprite.IsFlippedH())
		{
			_sprite.FlipH = false;
		}
		else if (attackPosition.X < 0 && !_sprite.IsFlippedH())
		{
			_sprite.FlipH = true;
		}
	}

	public void PlayAnimations()
	{
		if (_sprite == null)
			return;
		
		FlipSprite(Direction);

		if (!HurtStatus)
		{
			if (Velocity.X == 0 && IsOnFloor)
			{
				_sprite.Play("idle");
			}
			else if (Velocity.X != 0 && IsOnFloor)
			{
				_sprite.Play("run");
			}
			else if (Velocity.Y < 0 && !IsOnFloor)
			{
				_sprite.Play("jump");
			}
			else if (Velocity.Y > 0 && !IsOnFloor)
			{
				_sprite.Play("fall");
			}
		
			if (!IsOnFloor && IsLeftWallDetected)
			{
				FlipSprite(1.0f);
				_sprite.Play("wall_slide");
			}
			else if (!IsOnFloor && IsRightWallDetected)
			{
				FlipSprite(-1.0f);
				_sprite.Play("wall_slide");
			}
		}
		else
		{
			_sprite.Play("hurt");
		}
		
	}
}
