using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class AnimationComponent : AnimatedSprite2D
{
	public void PlayAnimation(CharacterStatesComponent.State currentState, float direction)
	{
		switch (currentState)
		{
			case CharacterStatesComponent.State.Idle:
				Play("idle");
				break;
			
			case CharacterStatesComponent.State.Run:
				FlipSprite(direction);
				Play("run");
				break;
			
			case CharacterStatesComponent.State.Jump:
				FlipSprite(direction);
				Play("jump");
				break;
			
			case CharacterStatesComponent.State.Fall:
				FlipSprite(direction);
				
				if (Animation != "fall")
				{
					Stop();
					Play("fall");
				}
				break;
			
			case CharacterStatesComponent.State.WallSlide:
				FlipSprite(direction);
				Play("wall_slide");
				break;
			
			case CharacterStatesComponent.State.Hurt:
				FlipSprite(direction);
				Play("hurt");
				break;
		}
	}
	
	private void FlipSprite(float direction)
	{
		if (direction < 0)
		{
			FlipH = true;
		}
		else if (direction > 0)
		{
			FlipH = false;
		}
	}
	
}
