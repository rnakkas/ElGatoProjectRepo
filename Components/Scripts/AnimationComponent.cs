using Godot;
using System;
using Godot.Collections;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class AnimationComponent : Node2D
{
	[Export] private AnimatedSprite2D _sprite;
	
	public void FlipSprite(float direction)
	{
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
}
