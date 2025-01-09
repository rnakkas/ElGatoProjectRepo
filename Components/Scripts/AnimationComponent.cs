using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class AnimationComponent : AnimatedSprite2D
{
	public void FlipSprite(Vector2 direction)
	{
		if (direction.X < 0)
		{
			FlipH = true;
		}
		else if (direction.X > 0)
		{
			FlipH = false;
		}
	}
	
	public void PlayIdleAnimation()
	{
		Play("idle");
	}

	public void PlayRunAnimation()
	{
		Play("run");
	}

	public void PlayJumpAnimation()
	{
		Play("jump");
	}

	public void PlayFallAnimation()
	{
		Play("fall");
	}

	public void PlayWallSlideAnimation()
	{
		Play("wall_slide");
	}
}
