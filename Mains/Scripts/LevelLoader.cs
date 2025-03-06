using Godot;
using System;

namespace ElGatoProject.Mains.Scripts;

[GlobalClass]
public partial class LevelLoader : Node
{
    private string _currentLevel;

    public void LoadLevel(string levelName)
    {
        if (levelName == _currentLevel)
            return;
        _currentLevel = levelName;
        var level = ResourceLoader.Load<PackedScene>($"res://Levels/Scenes/{levelName}.tscn").Instantiate();
        GetOwner().AddChild(level);
    }

    public void UnloadCurrentLevel()
    {
       GetOwner().GetNode(_currentLevel).QueueFree();
       _currentLevel = "";
    }
    
}
