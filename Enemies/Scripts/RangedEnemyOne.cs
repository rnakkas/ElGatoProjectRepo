using Godot;
using System;
using ElGatoProject.Resources;

namespace ElGatoProject.Enemies.Scripts;

public partial class RangedEnemyOne : Area2D
{
	[Export] private EnemyStats _rangedEnemyOneStats;
	[Export] private AnimatedSprite2D _spriteBody, _spriteEye;
	[Export] private Marker2D _eyeMarker;
	[Export] private Area2D _playerDetectionArea;
	[Export] private RayCast2D _playerDetectionRay;
	[Export] private Label _testingLabel;
	
	public override void _Ready()
	{
		_testingLabel.SetText("idle");
	}
	
	public override void _Process(double delta)
	{
	}
}
