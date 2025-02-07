using Godot;
using System;
using ElGatoProject.Components.Scripts;

namespace ElGatoProject.Enemies.Scripts;
public partial class RangedEnemyLightPistol : Node2D
{
	// Get components
	[Export] private EnemyControllerComponent _enemyController;
	
	public override void _Ready()
	{
		_enemyController.ConnectToSignals();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_enemyController.EnemyControllerActions((float)delta);
	}
}
