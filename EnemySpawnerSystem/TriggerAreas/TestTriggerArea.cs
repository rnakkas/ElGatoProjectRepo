using System.Collections.Generic;
using System.Linq;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;
using Godot;
using Godot.Collections;

namespace ElGatoProject.EnemySpawnerSystem.TriggerAreas;

[GlobalClass]
public partial class TestTriggerArea : Area2D
{
    [Export] private Utility.EnemyType _enemyType;

    private IEnumerable<Marker2D> _spawnPoints;

    public override void _Ready()
    {
        BodyEntered += OnPlayerEntered;
    }

    private void OnPlayerEntered(Node2D body)
    {
        if (body is CharacterBody2D && body.IsInGroup(Utility.NodeGroupPlayers))
        {
            _spawnPoints = GetChildren().OfType<Marker2D>();
        
            foreach (var spawnPoint in _spawnPoints)
            {
                GD.Print("Spawn point: " + spawnPoint.Name + " Pos: " + spawnPoint.Position);
            }
            
            // Turn off monitoring and remove the area
            SetDeferred("monitoring", false);
            CallDeferred("queue_free");

            EventsBus.Instance.EmitSignal(nameof(EventsBus.Instance.PlayerEnteredEnemySpawnTrigger),
                _enemyType.ToString(), body.GlobalPosition);
        }
    }
}