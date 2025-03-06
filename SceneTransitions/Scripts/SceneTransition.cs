using Godot;
using System;
using ElGatoProject.Singletons;

namespace ElGatoProject.SceneTransitions.Scripts;
public partial class SceneTransition : CanvasLayer
{
	private AnimationPlayer _animationPlayer;
	private ShaderMaterial _shaderMaterial;

	public override void _Ready()
	{
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_shaderMaterial = (ShaderMaterial)GetNodeOrNull<ColorRect>("ColorRect").Material;
	}
	
	public async void PlaySceneTransition()
	{
		SetWipeColor(new Color(34f / 255f, 35f / 255f, 35f / 255f)); // Hex: 222323;
		SetLayer(10); // Set layer high enough for the node to be on top
		SetVisible(true);
		
		_animationPlayer.Play(Utility.Instance.SceneTransitionWipeAnimation); // Play wipe animation

		await ToSignal(_animationPlayer, "animation_finished"); // Wait for animation to complete
		
		SetVisible(false);
	}
	
	private void SetWipeColor(Color newColor)
	{
		// Convert Color to Vector4 since Godot expects a vec4
		Vector4 colorVec4 = new Vector4(newColor.R, newColor.G, newColor.B, newColor.A);
		_shaderMaterial.SetShaderParameter(Utility.Instance.ShaderParameterWipeColor, colorVec4);
	}
}
