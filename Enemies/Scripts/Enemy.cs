using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Enemies.Scripts;

[GlobalClass]
public partial class Enemy : Area2D
{
	[Export] private EnemyStats _enemyStats;
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
	}
}
