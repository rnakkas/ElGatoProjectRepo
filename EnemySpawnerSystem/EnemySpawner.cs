using System;
using ElGatoProject.Singletons;
using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.EnemySpawnerSystem;

[GlobalClass]
public partial class EnemySpawner : Node
{
    public override void _Ready()
    {
        EventsBus.Instance.PlayerEnteredEnemySpawnTrigger += OnPlayerTriggeredEnemySpawn;
    }

    private void OnPlayerTriggeredEnemySpawn(string enemy, Vector2 playerPosition)
    {
        if (Enum.TryParse(enemy, out Utility.EnemyType enemyType))
            SpawnEnemy(enemyType, playerPosition);
    }

    private void SpawnEnemy(Utility.EnemyType enemyType, Vector2 playerPosition)
    {
        GD.Print("spawned enemy: " + enemyType);
        GD.Print("player pos: " + playerPosition);
    }
}